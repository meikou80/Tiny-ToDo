using Microsoft.AspNetCore.Mvc;
using TinyToDo.Models;
using TinyToDo.Services;

namespace TinyToDo.Controllers;

public class LoginController : Controller
{
    private readonly SessionService _sessionService;
    private readonly UserAccountService _userAccountService;

    // コンストラクタ - サービスのインスタンスを取得
    public LoginController()
    {
        _sessionService = SessionService.Instance;
        _userAccountService = UserAccountService.Instance;
    }

    // ログイン画面を表示する
    [Route("/login")]
    [HttpGet]
    public IActionResult Index()
    {
        var session = _sessionService.EnsureSession(HttpContext);
        return ShowLogin(session);
    }

    // ログイン処理を行う
    [Route("/login")]
    [HttpPost]
    public IActionResult Login(string userId, string password)
    {
        var session = _sessionService.EnsureSession(HttpContext);
        Console.WriteLine($"login attempt: {userId}");

        try
        {
            var account = _userAccountService.Authenticate(userId, password);

            // <4> ログイン成功 - セッションにアカウント情報を保存
            session.UserAccount = account;
            Console.WriteLine($"login success : {account.Id}");

            // <5> ToDoページへリダイレクト
            return RedirectToAction("Index", "Todo");
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"login failed : {userId}");
            session.PageData = new LoginPageData
            {
                ErrorMessage = "ユーザIDまたはパスワードが違います",
            };

            return RedirectToAction("Index");
        }
    }

    // ログイン画面を表示する
    private IActionResult ShowLogin(HttpSession session)
    {
        LoginPageData pageData;
        if (session.PageData is LoginPageData data)
        {
            pageData = data;
        }
        else
        {
            pageData = new LoginPageData();
        }
        
        ViewBag.ErrorMessage = pageData.ErrorMessage;
        session.ClearPageData();
        return View("Login");
    }
}