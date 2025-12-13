using Microsoft.AspNetCore.Mvc;
using TinyToDo.Models;
using TinyToDo.Services;

namespace TinyToDo.Controllers;

public class NewAccountController : Controller
{
    private readonly SessionService _sessionService;

    // コンストラクタ - サービスのインスタンスを取得
    public NewAccountController()
    {
        _sessionService = SessionService.Instance;
    }

    // 生成したアカウント情報の表示画面
    [Route("/new-user-account")]
    [HttpGet]
    public IActionResult Index()
    {
        // セッションをチェックする
        var session = _sessionService.CheckSession(HttpContext, out bool shouldRedirect);
        if (shouldRedirect || session == null)
        {
            return RedirectToAction("Index", "Login");
        }

        // セッションからページデータを取得
        if (session.PageData is NewAccountPageData pageData)
        {
            // アカウント情報をViewBagに設定
            ViewBag.UserId = pageData.UserId;
            ViewBag.Password = pageData.Password;
            ViewBag.Expires = pageData.Expires;

            // ページデータをクリア
            session.ClearPageData();
            return View("NewAccount");
        }
        else
        {
            return RedirectToAction("Index", "Login");
        }
    }
}