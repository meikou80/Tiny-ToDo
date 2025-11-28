namespace TinyToDo.Configuration;

public class AppSettings
{
    // SESSION_SECRET環境変数を読み取る
    public static ulong GetSessionSecret()
    {
        var secretStr = Environment.GetEnvironmentVariable("SESSION_SECRET");
        if (!string.IsNullOrEmpty(secretStr) && 
            ulong.TryParse(secretStr, out var secret))
        {
            return secret;
        }
        // デフォルト値（ランダム生成）
        // Go版のrand.Uint64()と同等の正の値を生成
        return (ulong)Random.Shared.NextInt64(1, long.MaxValue);
    }

    // SECURE_COOKIE環境変数を読み取る
    public static bool GetSecureCookie()
    {
        return Environment.GetEnvironmentVariable("SECURE_COOKIE") == "yes";
    }

    // PORT環境変数を読み取る
    public static int GetPortNumber()
    {
        var portStr = Environment.GetEnvironmentVariable("PORT");
        if (!string.IsNullOrEmpty(portStr) && 
            int.TryParse(portStr, out var port))
        {
            return port;
        }
        return 8080; // デフォルト
    }
}