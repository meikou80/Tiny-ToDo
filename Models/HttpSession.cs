using Microsoft.AspNetCore.Http;
using TinyToDo.Services;
using TinyToDo.Configuration;

namespace TinyToDo.Models;

// セッション情報を保持するクラス
public class HttpSession
{
    // セッションの有効期限（分）
    public const int SessionLimitInMinute = 60;

    // セッションID
    public string SessionId { get; set; } = string.Empty;
    // セッションの有効期限
    public DateTime Expires { get; set; }
    // Post-Redirect-Getでの遷移先に表示するデータ
    public object? PageData { get; set; }
    // ユーザアカウント情報への参照
    public UserAccount? UserAccount { get; set; }

    // 新しいセッション情報を生成する
    public HttpSession(string sessionId)
    {
        SessionId = sessionId;
        Expires = DateTime.UtcNow.AddMinutes(SessionLimitInMinute);
        PageData = null;
        UserAccount = null;
    }

    // ページデータを削除する
    public void ClearPageData()
    {
        PageData = null;
    }

    // セッションIDをCookieへ書き込む
    public void SetCookie(HttpContext context)
    {
        var sameSiteMode = AppSettings.GetSameSiteMode();
        
        // SameSite=None の場合は Secure 必須
        var secure = sameSiteMode == SameSiteMode.None
            || SessionService.Instance.IsSecureCookie;
        
        var cookieOptions = new CookieOptions
        {
            Expires = new DateTimeOffset(Expires),
            HttpOnly = true,
            Path = "/",
            Secure = secure,
            SameSite = sameSiteMode
        };
        
        context.Response.Cookies.Append("sessionId", SessionId, cookieOptions);
    }
}