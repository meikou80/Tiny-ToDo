using System.Security.Cryptography;
using System.Text;

namespace TinyToDo.Services;

// セッションIDの署名付き生成と検証を行うクラス
public class SessionIdSigner
{
    // セッションID生成に使用するランダムバイトの長さ
    private const int RandomBytesLength = 16;
    // MD5ハッシュの長さ
    private const int Md5HashLength = 16;
    // セッションID全体の長さ（ランダム部分 + ハッシュ部分）
    private const int TotalSessionIdLength = RandomBytesLength + Md5HashLength; // 32
    // Base64パディング計算用の基数
    private const int Base64PaddingUnit = 4;
    
    private readonly byte[] _secretKey;

    public SessionIdSigner(ulong secret)
    {
        // シークレットキーをバイト配列に変換
        _secretKey = BitConverter.GetBytes(secret);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(_secretKey);
        }
    }

    // 署名付きセッションIDを生成する
    // フォーマット: Base64URL(randomBytes[16] + MD5(secret + randomBytes))
    public string GenerateSignedSessionId()
    {
         // ランダムデータを生成
         var randomBytes = new byte[RandomBytesLength];
         RandomNumberGenerator.Fill(randomBytes);

         // シークレットキー + ランダムバイトからMD5ハッシュを計算
         var hashInput = new byte[_secretKey.Length + randomBytes.Length];
         Buffer.BlockCopy(_secretKey, 0, hashInput, 0, _secretKey.Length);
         Buffer.BlockCopy(randomBytes, 0, hashInput, _secretKey.Length, randomBytes.Length);

         byte[] hashBytes;
         using (var md5 = MD5.Create())
         {
            hashBytes = md5.ComputeHash(hashInput);
         }

         // ランダムバイト + ハッシュを結合
         var sessionIdBytes = new byte[TotalSessionIdLength];
         Buffer.BlockCopy(randomBytes, 0, sessionIdBytes, 0, RandomBytesLength);
         Buffer.BlockCopy(hashBytes, 0, sessionIdBytes, RandomBytesLength, Md5HashLength);

         // Base64エンコード（URL-safe、パディングなし）
         return Convert.ToBase64String(sessionIdBytes)
         .Replace("+", "-")
         .Replace("/", "_")
         .Replace("=", "");
    }

    // セッションIDの署名を検証する
    public bool VerifySessionId(string sessionId)
    {
        try
        {
            // Base64デコード（URL-safe形式から標準形式へ変換）
            var base64 = sessionId
                .Replace("-", "+")
                .Replace("_", "/");
            
            // パディングを追加
            var padding = (Base64PaddingUnit - base64.Length % Base64PaddingUnit) % Base64PaddingUnit;
            base64 = base64.PadRight(base64.Length + padding, '=');

            var decodedBytes = Convert.FromBase64String(base64);

            // セッションIDの長さを検証
            if (decodedBytes.Length != TotalSessionIdLength)
            {
                return false;
            }

            // ランダム部分とMAC部分を分離
            var randomBytes = new byte[RandomBytesLength];
            var receivedMac = new byte[Md5HashLength];
            Buffer.BlockCopy(decodedBytes, 0, randomBytes, 0, RandomBytesLength);
            Buffer.BlockCopy(decodedBytes, RandomBytesLength, receivedMac, 0, Md5HashLength);

            // 期待されるハッシュを計算
            var hashInput = new byte[_secretKey.Length + randomBytes.Length];
            Buffer.BlockCopy(_secretKey, 0, hashInput, 0, _secretKey.Length);
            Buffer.BlockCopy(randomBytes, 0, hashInput, _secretKey.Length, randomBytes.Length);

            byte[] expectedMac;
            using (var md5 = MD5.Create())
            {
                expectedMac = md5.ComputeHash(hashInput);
            }

            // MACを比較（タイミング攻撃対策）
            return CryptographicOperations.FixedTimeEquals(receivedMac, expectedMac);
        }
        catch
        {
            return false;
        }
    }
}