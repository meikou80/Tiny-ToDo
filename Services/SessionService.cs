using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using TinyToDo.Models;

namespace TinyToDo.Services;

// セッションを管理するクラス
public  class SessionService
{
    private const string CookieSessionId = "sessionId";
    private const ulong SessionIdSecret = 123456789;

    // セッションIDをキーとしてセッション情報を保持するディクショナリ
    private readonly Dictionary<string, HttpSession> _sessions = new();
    private readonly SessionIdSigner _sessionIdSigner;

    // シングルトンインスタンス
    private static SessionService? _instance;
    private static readonly object _lock = new object();

    // プライベートコンストラクタ
    private SessionService() 
    {
        _sessionIdSigner = new SessionIdSigner(SessionIdSecret);
    }

    // シングルトンインスタンスを取得
    public static SessionService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SessionService();
                    }
                }
            }
            return _instance;
        }
    }

    // セッションを開始してCookieにセッションIDを書き込む
    public HttpSession StartSession(HttpContext context)
    {
        // 新しいセッションIDを生成する
        var sessionId = MakeSessionId();

        // セッション情報を生成する
        Console.WriteLine($"start session : {sessionId}");
        var session = new HttpSession(sessionId, TimeSpan.FromMinutes(10));
        _sessions[sessionId] = session;
        session.SetCookie(context);

        return session;
    }

    // セッションIDを生成する。
    private string MakeSessionId()
    {
        return _sessionIdSigner.GenerateSignedSessionId();
    }

    // セッションを削除する
    public void RevokeSession(HttpContext? context, string sessionId)
    {
        // セッション情報を削除
        _sessions.Remove(sessionId);
        Console.WriteLine($"session revoked : {sessionId}");

        if (context is null)
        {
            return;
        }

        var cookieOptions = new CookieOptions
        {
            MaxAge = TimeSpan.FromSeconds(-1),
            Expires = DateTimeOffset.FromUnixTimeSeconds(1)
        };
        context.Response.Cookies.Append(CookieSessionId, "", cookieOptions);
    }

    // セッションが存在するかチェックする
    // セッションが存在しなければ、ログイン画面へリダイレクトさせる
    public HttpSession? CheckSession(HttpContext context, out bool shouldRedirect)
    {
        shouldRedirect = false;

        // CookieのセッションIDに紐付くセッション情報を取得する
        var session = GetValidSession(context);
        if (session != null)
        {
            return session;
        }

        // セッションが有効期限切れまたは不正な場合、セッションを作り直す
        Console.WriteLine("session check failed");
        session = StartSession(context);

        // Refererヘッダの有無で他の画面からの遷移かどうかを判定
        // アプリケーショントップのURLに直接アクセスした際は、セッションが存在しないのが
        // 正常であるため、エラーを表示しないための措置
        if (!string.IsNullOrEmpty(context.Request.Headers.Referer))
        {
            session.PageData = new { ErrorMessage = "セッションが不正です。" };
        }

        shouldRedirect = true;
        return session;
    }

    // Cookieから有効なセッションを取得する
    // CookieにセッションIDがなければnullを返す
    // セッションIDが不正な場合や、セッションの有効期限が切れている場合もnullを返す
    public HttpSession? GetValidSession(HttpContext context)
    {
        // CookieからセッションIDを取得
        if (!context.Request.Cookies.TryGetValue(CookieSessionId, out var sessionId))
        {
            // CookieにセッションIDが存在しない場合
            return null;
        }

        // 署名を検証
        if (!_sessionIdSigner.VerifySessionId(sessionId))
        {
            Console.WriteLine($"Invalid session ID signature: {sessionId}");
            return null;
        }

        // セッションを取得して返す
        return GetSession(sessionId);
    }

    // セッションIDに紐付くセッション情報を返す
    private HttpSession? GetSession(string sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            // セッションの有効期限をチェックする
            if (DateTime.UtcNow > session.Expires)
            {
                // 有効期限が切れていたらセッション情報を削除してnullを返す
                _sessions.Remove(sessionId);
                return null;
            }
            return session;
        }

        return null;
    }

    // セッションが開始されていることを保証する。
    // セッションが存在しなければ、新しく発行する。
    public HttpSession EnsureSession(HttpContext context)
    {
         // CookieからセッションIDを取得
        var session = GetValidSession(context);
        if (session != null)
        {
            return session;
        }

        // セッションが存在しないか不正な場合は新しく開始する
        Console.WriteLine("session check failed");
        session = StartSession(context);
        return session;
    }
}