using Microsoft.AspNetCore.Mvc;
using TinyToDo.Services;
using TinyToDo.Filters;
using TinyToDo.Models;

namespace TinyToDo.Controllers;

[RequireAuthentication]
public class TodoController : Controller
{
    private readonly ILogger<TodoController> _logger;

    // コンストラクタでILoggerを注入
    public TodoController(ILogger<TodoController> logger)
    {
        _logger = logger;
    }

    // セッション情報をHttpContext.Itemsから取得するヘルパーメソッド
    private HttpSession GetSession()
    {
        return (HttpSession)HttpContext.Items["Session"]!;
    }

    // /todoエンドポイント
    [Route("/todo")]
    [HttpGet]
    public IActionResult Todo()
    {
        // セッション情報を取得
        var session = GetSession();
        
        // ユーザー情報をViewBagに設定
        ViewBag.UserId = session.UserAccount?.Id ?? "Guest";
        ViewBag.Expires = session.UserAccount?.ExpiresText() ?? "";
        
        // ユーザーアカウントに紐付くToDoリストを取得
        var todos = TodoService.GetAll(session.UserAccount);
        
        // ビューを返す
        return View(todos);
    }

    // /addエンドポイント
    [Route("/add")]
    [HttpPost]
    public IActionResult Add([FromBody] TodoAddRequest request)
    {
        // セッション情報を取得
        var session = GetSession();
        // ToDoを追加
        TodoService.Add(session.UserAccount, request.Todo);

        // 追加されたToDo項目を取得
        var todos = TodoService.GetAll(session.UserAccount);
        var addedTodo = todos.LastOrDefault();
        
        // ログ出力
        _logger.LogInformation(
            "ToDo項目を追加しました。ユーザーID={UserId} セッションID={SessionId} 内容={Content}",
            session.UserAccount?.Id,
            session.SessionId,
            request.Todo
        );
        
        return Json(new TodoItemResponse
        {
            Id = addedTodo?.Id ?? string.Empty,
            Todo = addedTodo?.Content ?? string.Empty
        });
    }

    // /editエンドポイント
    [Route("/edit")]
    [HttpPost]
    public IActionResult Edit(string id, string todo)
    {
        // セッション情報を取得
        var session = GetSession();
        // ToDoを更新
        var result = TodoService.Update(session.UserAccount, id, todo);
        if (result is null)
        {
            // 更新失敗（ToDoが見つからない）
            _logger.LogWarning(
                "ToDo項目の更新に失敗しました。ユーザーID={UserId} セッションID={SessionId} ToDoID={TodoId}",
                session.UserAccount?.Id,
                session.SessionId,
                id
            );
            return NotFound("ToDo項目が見つかりませんでした。");
        }

        // 成功ログ
        _logger.LogInformation(
            "ToDo項目を更新しました。ユーザーID={UserId} セッションID={SessionId} ToDoID={TodoId} 内容={Content}",
            session.UserAccount?.Id,
            session.SessionId,
            id,
            todo
        );
        
        return Ok();
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