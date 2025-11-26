# Tiny ToDo - C#版

このプロジェクトは、元のGo言語版をASP.NET Coreで書き換えたバージョンです。

## ファイル構成

- `Program.cs` - アプリケーションのエントリーポイント（main.goに相当）
- `Controllers/` - コントローラー
- `Services/` - ビジネスロジック
  - `SessionService.cs` - セッション管理
  - `SessionIdSigner.cs` - セッションID署名・検証（session.goのmakeSessionId/verifySessionIdに相当）
  - `TodoService.cs` - ToDo管理
  - `UserAccountService.cs` - ユーザーアカウント管理
- `Models/` - データモデル
- `Views/` - Razorビュー
- `wwwroot/static/` - 静的ファイル（CSS, JavaScript）
- `TinyToDo.csproj` - プロジェクトファイル

## 実行方法

1. .NET 8.0 SDKがインストールされていることを確認してください
2. プロジェクトディレクトリで以下のコマンドを実行：

```bash
dotnet run
```

3. ブラウザで `http://localhost:8080/todo` にアクセス

## 主な変更点

- Go言語のnet/httpパッケージ → ASP.NET Core
- html/templateパッケージ → Razorビューエンジン
- テンプレート構文: `{{range .}}` → `@foreach`
- 静的ファイル: `static/` → `wwwroot/static/`
- ポート8080で同じように動作します

## セキュリティ機能

### セッションID署名検証

Go版の`session.go`に実装されている署名付きセッションID生成・検証機能を`SessionIdSigner`クラスとして実装しています。

**特徴：**
- MD5ハッシュによる署名
- Base64 URL-safe エンコーディング
- タイミング攻撃対策（`CryptographicOperations.FixedTimeEquals`）

**動作確認方法：**

C# Interactiveやlinqpadで以下のコードを実行：

```csharp
using TinyToDo.Services;

var signer = new SessionIdSigner(123456789);

// 1. 生成と検証
var sessionId = signer.GenerateSignedSessionId();
Console.WriteLine($"Generated: {sessionId}");
Console.WriteLine($"Valid: {signer.VerifySessionId(sessionId)}"); // → True

// 2. 改ざん検出
var tampered = sessionId.Substring(0, sessionId.Length - 5) + "XXXXX";
Console.WriteLine($"Tampered Valid: {signer.VerifySessionId(tampered)}"); // → False

// 3. 不正な形式
Console.WriteLine($"Invalid Valid: {signer.VerifySessionId("invalid")}"); // → False
```