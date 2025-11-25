using System.Text.RegularExpressions;
using TinyToDo.Models;

namespace TinyToDo.Services;

// ユーザアカウントを管理するクラス
public class UserAccountService
{
    // ユーザIDの形式を検証する正規表現
    private static readonly Regex RegexAccountId = new Regex(@"^[A-Za-z0-9_.+@-]{1,32}$");
    // ユーザIDをキーとしてアカウント情報を保持するディクショナリ
    private readonly Dictionary<string, UserAccount> _userAccounts = new();
    // シングルトンインスタンス
    private static UserAccountService? _instance;
    private static readonly object _lock = new object();
    // プライベートコンストラクタ
    private UserAccountService() { }
    // シングルトンインスタンスを取得
    public static UserAccountService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserAccountService();
                    }
                }
            }
            return _instance;
        }
    }

    // ユーザIDの形式を検証する
    public bool ValidateUserId(string userId)
    {
        return RegexAccountId.IsMatch(userId);
    }

    // 新しいユーザアカウントを作成する
    public UserAccount NewUserAccount(string userId, string password)
    {
        // ユーザIDの形式検証
        if (!ValidateUserId(userId))
        {
            throw new ArgumentException("ユーザIDの形式が不正です");
        }
        // ユーザIDの重複チェック
        if (GetUserAccount(userId) != null)
        {
            throw new InvalidOperationException("このユーザーIDは既に使用されています");
        }
        // アカウントの有効期限を設定
        var expires = DateTime.UtcNow.AddMinutes(UserAccount.AccountLimitInMinute);

        // ユーザアカウント情報を生成
        var account = new UserAccount(userId, password, expires);
        _userAccounts[userId] = account;

        Console.WriteLine($"user account created : {account.Id}");
        return account;
    }

    // ユーザアカウントを取得する
    public UserAccount? GetUserAccount(string userId)
    {
        if (_userAccounts.TryGetValue(userId, out var account))
        {
            return account;
        }
        return null;
    }

    // ユーザアカウントを認証する
    public UserAccount Authenticate(string userId, string password)
    {
        // <1> アカウントの存在チェック
        var account = GetUserAccount(userId);
        if (account == null)
        {
            throw new UnauthorizedAccessException("ログインに失敗しました");
        }

        // <2> パスワードのチェック
        if (!account.VerifyPassword(password))
        {
            throw new UnauthorizedAccessException("ログインに失敗しました");
        }

        return account;
    }

    // ランダムなパスワードを生成する
    public static string MakePassword()
    {
        var random = new Random();
        var password = new char[UserAccount.PasswordLength];

        for (int i = 0; i < UserAccount.PasswordLength; i++)
        {
            password[i] = UserAccount.PasswordChars[random.Next(UserAccount.PasswordChars.Length)];
        }

        return new string(password);
    }

    // 全てのアカウント情報を取得する（デバッグ用）
    public List<UserAccount> GetAllAccounts()
    {
        return _userAccounts.Values.ToList();
    }

    // アカウントを削除する（デバッグ用）
    public void DeleteAccount(string userId)
    {
        _userAccounts.Remove(userId);
        Console.WriteLine($"user account deleted : {userId}");
    }
}