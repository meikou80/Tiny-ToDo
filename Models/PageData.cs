namespace TinyToDo.Models;

// ログインページの表示データ
public class LoginPageData
{
    public string UserId { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

// アカウント作成ページの表示データ
public class CreateAccountPageData
{
    public string ErrorMessage { get; set; } = string.Empty;
}

// 新規アカウント情報表示ページのデータ
public class NewAccountPageData
{
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Expires { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

