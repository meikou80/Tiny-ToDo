using Microsoft.AspNetCore.Mvc;
using TinyToDo.Services;

namespace TinyToDo.Controllers;

public class TodoController : Controller
{
    // /todoエンドポイント
    [Route("todo")]
    [HttpGet]
    public IActionResult Todo()
    {
        // セッションIDを取得
        var sessionId = SessionService.EnsureSession(HttpContext);
        // セッションに紐付くToDoリストを取得
        var todos = TodoService.GetAll(sessionId);
        // ビューを返す
        return View(todos);
    }

    // /addエンドポイント
    [Route("add")]
    [HttpPost]
    public IActionResult Add(string todo)
    {
        // セッションIDを取得
        var sessionId = SessionService.EnsureSession(HttpContext);
        // ToDoを追加
        TodoService.Add(sessionId, todo);
        return RedirectToAction("Todo");
    }
}