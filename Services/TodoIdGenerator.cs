using System.Security.Cryptography;
using System.Text;

namespace TinyToDo.Services;

// ToDo項目の一意なIDを生成するクラス
public class TodoIdGenerator
{
    // 現在時刻とToDo内容からMD5ハッシュベースのIDを生成する。
    public static string GenerateId(string todoContent)
    {
        // 現在のUnixナノ秒タイムスタンプを取得
        long unixNano = DateTime.UtcNow.Ticks * 100;
        // タイムスタンプを文字列化してバイト配列に変換
        byte[] timeBytes = Encoding.UTF8.GetBytes(unixNano.ToString());
        // ToDo内容をバイト配列に変換
        byte[] todoBytes = Encoding.UTF8.GetBytes(todoContent);
        // MD5ハッシュを計算
        using var md5 = MD5.Create();
        // タイムスタンプとToDo内容を結合してハッシュ化
        md5.TransformBlock(timeBytes, 0, timeBytes.Length, null, 0);
        md5.TransformFinalBlock(todoBytes, 0, todoBytes.Length);

        byte[] hash = md5.Hash!;

        // ハッシュ値を16進数文字列に変換
        return Convert.ToHexString(hash).ToLower();
    }
}