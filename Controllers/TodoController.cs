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
        
        // セッションに紐付くToDoリストを取得
        var todos = TodoService.GetAll(session.SessionId);
        
        // ビューを返す
        return View(todos);
    }

    // /addエンドポイント
    [Route("/add")]
    [HttpPost]
    public IActionResult Add(string todo)
    {
        // セッションIDを取得
        var session = GetSession();
        // ToDoを追加
        TodoService.Add(session.SessionId, todo);
        return RedirectToAction("Todo");
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