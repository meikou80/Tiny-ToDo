using Microsoft.AspNetCore.Mvc;
using TinyToDo.Models;
using TinyToDo.Services;

namespace TinyToDo.Controllers;

public class CreateAccountController : Controller
{
    private readonly SessionService _sessionService;
    private readonly UserAccountService _userAccountService;

    // コンストラクタ - サービスのインスタンスを取得
    public CreateAccountController()
    {
        _sessionService = SessionService.Instance;
        _userAccountService = UserAccountService.Instance;
    }

    // アカウント作成画面を表示する
    [Route("/create-user-account")]
    [HttpGet]
    public IActionResult Index()
    {
        var session = _sessionService.EnsureSession(HttpContext);
        return ShowCreateAccount(session);
    }

    // アカウント作成処理を行う
    [Route("/create-user-account")]
    [HttpPost]
    public IActionResult CreateAccount(string userId)
    {
        // セッションをチェックする
        var session = _sessionService.CheckSession(HttpContext, out bool shouldRedirect);
        if (shouldRedirect || session == null)
        {
            return RedirectToAction("Index", "Login");
        }

        // ランダムなパスワードを生成
        var password = UserAccountService.MakePassword();
        Console.WriteLine($"create user attempt: userId={userId}");

        try
        {
            // アカウント作成
            var account = _userAccountService.NewUserAccount(userId, password);
            // ユーザアカウント作成成功
            Console.WriteLine($"create user success : userId={userId}");
            // リダイレクト先画面で表示するためにユーザ情報をセッションへ格納
            session.PageData = new NewAccountPageData
            {
                UserId = account.Id,
                Password = password,
                Expires = account.ExpiresText(),
            };

            // アカウント情報表示画面へリダイレクト
            return RedirectToAction("Index", "NewAccount");
        }
        catch (InvalidOperationException ex)
        {
            // ユーザIDが既に存在する場合
            Console.WriteLine($"create user failed : userId={userId} cause={ex.Message}");
            session.PageData = new CreateAccountPageData
            {
                ErrorMessage = "すでに使われているユーザIDです。他のIDを試してください。",
            };

            // アカウント作成画面へリダイレクト（Post-Redirect-Get パターン）
            return RedirectToAction("Index");
        }
        catch (ArgumentException ex)
        {
            // ユーザIDの形式が不正な場合
            Console.WriteLine($"create user failed : userId={userId} cause={ex.Message}");
            session.PageData = new CreateAccountPageData
            {
                ErrorMessage = "ユーザIDの形式が違います。",
            };

            // アカウント作成画面へリダイレクト（Post-Redirect-Get パターン）
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // その他のエラー
            Console.WriteLine($"create user failed : userId={userId} cause={ex.Message}");
            session.PageData = new CreateAccountPageData
            {
                ErrorMessage = ex.Message,
            };

            // アカウント作成画面へリダイレクト（Post-Redirect-Get パターン）
            return RedirectToAction("Index");
        }
    }

    // アカウント作成画面を表示する（プライベートメソッド）
    private IActionResult ShowCreateAccount(HttpSession session)
    {
        // セッションからページデータを取得
        CreateAccountPageData pageData;
        if (session.PageData is CreateAccountPageData data)
        {
            pageData = data;
        }
        else
        {
            pageData = new CreateAccountPageData();
        }

        ViewBag.ErrorMessage = pageData.ErrorMessage;
        session.ClearPageData();
        return View("CreateAccount");
    }
}