using Microsoft.AspNetCore.Mvc;
using TinyToDo.Services;
using TinyToDo.Filters;
using TinyToDo.Models;

namespace TinyToDo.Controllers;

[RequireAuthentication]
public class TodoController : Controller
{
    private readonly ILogger<TodoController> _logger;
    private readonly TodoChangeNotifier _notifier;

    // コンストラクタでILoggerとTodoChangeNotifierを注入
    public TodoController(ILogger<TodoController> logger, TodoChangeNotifier notifier)
    {
        _logger = logger;
        _notifier = notifier;
    }

    // セッション情報をHttpContext.Itemsから取得するヘルパーメソッド
    private HttpSession GetSession()
    {
        return (HttpSession)HttpContext.Items["Session"]!;
    }

    // X-Tinytodo-SourceidヘッダからソースIDを取得するヘルパーメソッド
    private string GetSourceId()
    {
        return Request.Headers["X-Tinytodo-Sourceid"].ToString();
    }

    // /エンドポイント（HTMLページを返す）
    [Route("/")]
    [HttpGet]
    public IActionResult Index()
    {
        // セッション情報を取得
        var session = GetSession();

        // ユーザー情報をViewBagに設定
        ViewBag.UserId = session.UserAccount?.Id ?? "Guest";
        ViewBag.Expires = session.UserAccount?.ExpiresText() ?? "";

        // Todo.cshtmlビューを返す（データは空）
        return View("Todo");
    }

    // GET /todos
    [Route("/todos")]
    [HttpGet]
    public IActionResult GetTodos()
    {
        // セッション情報を取得
        var session = GetSession();
        // ユーザーアカウントに紐付くToDoリストを取得
        var todos = TodoService.GetAll(session.UserAccount);

        _logger.LogInformation(
            "ToDoリストを取得しました。ユーザーID={UserId} セッションID={SessionId} 件数={Count}",
                session.UserAccount?.Id,
                session.SessionId,
                todos.Count
        );
        
        // TodoModelをTodoItemResponseに変換してJSON形式で返す
        var items = todos.Select(t => new TodoItemResponse
        {
            Id = t.Id,
            Todo = t.Content
        }).ToList();
        
        // JSON形式で返す
        return Json(new { items = items });
    }

    // GET /todos/{id}
    [Route("/todos/{id}")]
    [HttpGet]
    public IActionResult GetTodo(string id)
    {
        // セッション情報を取得
        var session = GetSession();
        // ToDoを取得
        var todo = TodoService.Get(session.UserAccount, id);

        if (todo is null)
        {
            _logger.LogWarning(
                "ToDo項目の取得に失敗しました。ユーザーID={UserId} セッションID={SessionId} ToDoID={TodoId}",
                session.UserAccount?.Id,
                session.SessionId,
                id
            );
            return NotFound("ToDo項目の取得に失敗しました。");
        }
        
        _logger.LogInformation(
            "ToDo項目を取得しました。ユーザーID={UserId} セッションID={SessionId} ToDoID={TodoId} 内容={Content}",
            session.UserAccount?.Id,
            session.SessionId,
            id,
            todo.Content
        );
        
        return Json(new TodoItemResponse
        {
            Id = todo.Id,
            Todo = todo.Content
        });
    }

    // POST /todos
    [Route("/todos")]
    [HttpPost]
    public IActionResult AddTodo([FromBody] TodoAddRequest request)
    {
        // セッション情報を取得
        var session = GetSession();
        // ToDoを追加
        TodoService.Add(session.UserAccount, request.Todo);

        // 追加されたToDo項目を取得
        var todos = TodoService.GetAll(session.UserAccount);
        var addedTodo = todos.LastOrDefault();

        if (addedTodo is null)
        {
            _logger.LogWarning("ToDo項目の追加に失敗しました。ユーザーID={UserId} セッションID={SessionId} 内容={Content}",
                session.UserAccount?.Id,
                session.SessionId,
                request.Todo
            );
            return BadRequest("ToDo項目の追加に失敗しました。");
        }

        _logger.LogInformation(
            "ToDo項目を追加しました。ユーザーID={UserId} セッションID={SessionId} ToDoID={TodoId} 内容={Content}",
            session.UserAccount?.Id,
            session.SessionId,
            addedTodo.Id,
            request.Todo
        );

        // ToDo追加をSSE経由で通知する
        _notifier.Notify(session.UserAccount?.Id ?? "", new TodoChangeEvent
        {
            Source = GetSourceId(),
            Event = "add",
            TodoItem = new TodoItemResponse 
            { 
                Id = addedTodo.Id,
                Todo = addedTodo.Content 
            }
        });

        return CreatedAtAction(
            nameof(GetTodo),           // GetTodoアクションを参照
            new { id = addedTodo.Id }, // ルートパラメータ
            new TodoItemResponse       // レスポンスボディ
            {
                Id = addedTodo.Id,
                Todo = addedTodo.Content
            }
        );
    }

    // PUT /todos/{id}
    [Route("/todos/{id}")]
    [HttpPut]
    public IActionResult UpdateTodo(string id, [FromBody] TodoEditRequest request)
    {
        // セッション情報を取得
        var session = GetSession();

        // URLのIDとボディのIDの一致チェック（Go版と同様）
        if (id != request.Id)
        {
            _logger.LogWarning(
                "IDの不一致。ユーザーID={UserId} セッションID={SessionId} URLのID={UrlId} ボディのID={BodyId}",
                session.UserAccount?.Id,
                session.SessionId,
                id,
                request.Id
            );
            return BadRequest("IDが一致しません。");
        }

        // ToDoを更新
        var result = TodoService.Update(session.UserAccount, id, request.Todo);

        if (result is null)
        {
            _logger.LogWarning("ToDo項目の更新に失敗しました。ユーザーID={UserId} セッションID={SessionId} ToDoID={TodoId}",
                session.UserAccount?.Id,
                session.SessionId,
                id
            );
            return NotFound("ToDo項目が見つかりませんでした。");
        }

        _logger.LogInformation(
            "ToDo項目を更新しました。ユーザーID={UserId} セッションID={SessionId} ToDoID={TodoId} 内容={Content}",
            session.UserAccount?.Id,
            session.SessionId,
            id,
            request.Todo
        );

        // ToDo更新をSSE経由で通知する
        _notifier.Notify(session.UserAccount?.Id ?? "", new TodoChangeEvent
        {
            Source = GetSourceId(),
            Event = "update",
            TodoItem = new TodoItemResponse 
            { 
                Id = result.Id,
                Todo = result.Content 
            }
        });
        
        // 更新後のデータをJSON形式で返す
        return Json(new TodoItemResponse
        {
            Id = result.Id,
            Todo = result.Content
        });
    }

    // DELETE /todos/{id}
    [Route("/todos/{id}")]
    [HttpDelete]
    public IActionResult DeleteTodo(string id)
    {
        // セッション情報を取得
        var session = GetSession();

        // 削除前に存在確認
        var todo = TodoService.Get(session.UserAccount, id);
        if (todo is null)
        {
            _logger.LogWarning("ToDo項目が見つかりませんでした。ユーザーID={UserId} セッションID={SessionId} ToDoID={TodoId}",
                session.UserAccount?.Id,
                session.SessionId,
                id
            );
            return NoContent();
        }

        // ToDoを削除
        TodoService.Delete(session.UserAccount, id);

        _logger.LogInformation(
            "ToDo項目を削除しました。ユーザーID={UserId} セッションID={SessionId} ToDoID={TodoId}",
            session.UserAccount?.Id,
            session.SessionId,
            id
        );

        // ToDo削除をSSE経由で通知する
        _notifier.Notify(session.UserAccount?.Id ?? "", new TodoChangeEvent
        {
            Source = GetSourceId(),
            Event = "delete",
            TodoItem = new TodoItemResponse
            { 
                Id = todo.Id, 
                Todo = todo.Content 
            }
        });

        return NoContent();
    }

    // /logoutエンドポイント
    [Route("/logout")]
    [HttpPost]
    public IActionResult Logout()
    {
        var session = GetSession();
        
        _logger.LogInformation(
            "ユーザーがログアウトしました。ユーザーID={UserId} セッションID={SessionId}",
            session.UserAccount?.Id,
            session.SessionId
        );
        
        SessionService.Instance.RevokeSession(HttpContext, session.SessionId);
        SessionService.Instance.StartSession(HttpContext);
        return Redirect("/login");
    }
}