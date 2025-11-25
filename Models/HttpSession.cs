using Microsoft.AspNetCore.Http;
namespace TinyToDo.Models;

// セッション情報を保持するクラス
public class HttpSession
{
    // セッションID
    public string SessionId { get; set; } = string.Empty;
    // セッションの有効期限
    public DateTime Expires { get; set; }
    // Post-Redirect-Getでの遷移先に表示するデータ
    public object? PageData { get; set; }
    // ユーザアカウント情報への参照
    public UserAccount? UserAccount { get; set; }

    // 新しいセッション情報を生成する
    public HttpSession(string sessionId, TimeSpan validityTime)
    {
        SessionId = sessionId;
        Expires = DateTime.UtcNow.Add(validityTime);
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
        var cookieOptions = new CookieOptions
        {
            Expires = new DateTimeOffset(Expires),
            HttpOnly = true,
            Path = "/",
        };
        context.Response.Cookies.Append("sessionId", SessionId, cookieOptions);
    }
}