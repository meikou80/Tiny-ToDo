using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace TinyToDo.Services;

public static class SessionService
{
    private const string CookieSessionId = "sessionId";

    // セッションが開始されていることを保証する。
    // セッションが存在しなければ、新しく発行する。
    public static string EnsureSession(HttpContext context)
    {
         // CookieからセッションIDを取得
        if (context.Request.Cookies.TryGetValue(CookieSessionId, out var sessionId))
        {
            // CookieにセッションIDが入っている場合は、それを返す
            return sessionId;
        }

        // CookieにセッションIDが入っていない場合は、新しく発行して返す
        sessionId = StartSession(context);
        return sessionId;
    }

    // セッションを開始する。
    private static string StartSession(HttpContext context)
    {
        var sessionId = MakeSessionId();

        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddSeconds(600),
            HttpOnly = true,
            Path = "/",
        };

        context.Response.Cookies.Append(CookieSessionId, sessionId, cookieOptions);
        return sessionId;
    }

    // セッションIDを生成する。
    private static string MakeSessionId()
    {
        var randomBytes = new byte[16];
        RandomNumberGenerator.Fill(randomBytes);
        return Convert.ToBase64String(randomBytes)
        .Replace("+", "-")
        .Replace("/", "_")
        .Replace("=", "");
    }
}