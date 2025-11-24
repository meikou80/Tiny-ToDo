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
        var todos = TodoService.GetAll();
        return View(todos);
    }

    // /addエンドポイント
    [Route("add")]
    [HttpPost]
    public IActionResult Add(string todo)
    {
        TodoService.Add(todo);
        return RedirectToAction("Todo");
    }
}