using System;

namespace TinyToDo.Models;

// ユーザアカウント情報を保持するクラス
public class UserAccount
{
    public const int AccountLimitInMinute = 60;
    public const int PasswordLength = 10;
    public const string PasswordChars = "23456789abcdefghijkmnpqrstuvwxyz";

    // ユーザID
    public string Id { get; set; } = string.Empty;
    // ハッシュ化されたパスワード
    public string HashedPassword { get; set; } = string.Empty;
    // アカウントの有効期限
    public DateTime Expires { get; set; }
    // ToDoリスト
    public List<string> ToDoList { get; set; } = new();

    // ユーザアカウント情報を生成する
    public UserAccount(string userId, string plainPassword, DateTime expires)
    {
        Id = userId;
        HashedPassword = HashPassword(plainPassword);
        Expires = expires;
        ToDoList = new List<string>();
    }

    // bcryptアルゴリズムでパスワードをハッシュ化する
    private static string HashPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }

    // パスワードを検証する
    public bool VerifyPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, HashedPassword);
    }

    // 有効期限をテキスト形式で返す（JST）
    public string ExpiresText()
    {
        // UTC時刻をJST（UTC+9時間）に変換
        return Expires.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss");
    }
}