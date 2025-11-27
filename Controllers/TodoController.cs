using Microsoft.AspNetCore.Mvc;
using TinyToDo.Services;
using TinyToDo.Filters;
using TinyToDo.Models;

namespace TinyToDo.Controllers;

[RequireAuthentication]
public class TodoController : Controller
{
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
    public IActionResult Add(string todo)
    {
        // セッション情報を取得
        var session = GetSession();
        // ToDoを追加
        TodoService.Add(session.UserAccount, todo);
        return RedirectToAction("Todo");
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
            // 更新失敗（ToDoが見つからない、またはゲストユーザー）
            return StatusCode(500, "ToDo項目の更新に失敗しました。");
        }

        Console.WriteLine($"Todo item updated. sessionId={session.SessionId} itemId={id} todo={todo}");
        return Ok();
    }

    // /logoutエンドポイント
    [Route("/logout")]
    [HttpPost]
    public IActionResult Logout()
    {
        var session = GetSession();
        SessionService.Instance.RevokeSession(HttpContext, session.SessionId);
        SessionService.Instance.StartSession(HttpContext);
        return Redirect("/login");
    }
}