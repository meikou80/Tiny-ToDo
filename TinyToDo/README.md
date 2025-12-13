# Tiny ToDo - C#版

## このプロジェクトについて

このプロジェクトは、書籍「[改訂新版]プロになるためのWeb技術入門」（小森裕介 著、技術評論社）のTiny ToDoアプリケーションを、自習目的でC#（ASP.NET Core）で実装したものです。

### 原著・オリジナルリポジトリ

- **書籍**: [[改訂新版]プロになるためのWeb技術入門](https://direct.gihyo.jp/view/item/000000003591)（技術評論社）
- **オリジナルリポジトリ（Go言語版）**: [little-forest/webtech-fundamentals](https://github.com/little-forest/webtech-fundamentals/tree/v1-latest)

> ⚠️ **注意**: このリポジトリは個人の学習目的で作成されたものであり、原著者・出版社とは無関係です。

## ファイル構成

```
Tiny ToDo/
├── Program.cs                    # アプリケーションのエントリーポイント
├── TinyToDo.csproj               # プロジェクトファイル
│
├── Controllers/                  # コントローラー
│   ├── TodoController.cs         # ToDo CRUD操作
│   ├── LoginController.cs        # ログイン処理
│   ├── CreateAccountController.cs # アカウント作成処理
│   ├── NewAccountController.cs   # 新規アカウント画面
│   ├── SseController.cs          # Server-Sent Events（リアルタイム通知）
│   └── WebSocketController.cs    # WebSocket（リアルタイム通知）
│
├── Services/                     # ビジネスロジック
│   ├── SessionService.cs         # セッション管理
│   ├── SessionIdSigner.cs        # セッションID署名・検証
│   ├── TodoService.cs            # ToDo管理
│   ├── TodoIdGenerator.cs        # ToDo ID生成
│   ├── TodoChangeNotifier.cs     # ToDo変更通知（SSE/WebSocket共通）
│   └── UserAccountService.cs     # ユーザーアカウント管理
│
├── Models/                       # データモデル
│   ├── HttpSession.cs            # セッション情報
│   ├── UserAccount.cs            # ユーザーアカウント
│   ├── TodoModel.cs              # ToDoデータ
│   ├── TodoRequest.cs            # ToDoリクエスト
│   ├── TodoChangeEvent.cs        # 変更イベント
│   └── PageData.cs               # ページデータ
│
├── Middlewares/                  # ミドルウェア
│   ├── SessionMiddleware.cs      # セッション管理ミドルウェア
│   └── ErrorHandlingMiddleware.cs # エラーハンドリング
│
├── Filters/                      # フィルター
│   └── RequireAuthenticationAttribute.cs # 認証必須属性
│
├── Configuration/                # 設定
│   └── AppSettings.cs            # アプリケーション設定
│
├── Views/                        # Razorビュー
│   ├── Login/                    # ログイン画面
│   └── Todo/                     # ToDo画面
│
└── wwwroot/static/               # 静的ファイル
    ├── style.css                 # スタイルシート
    └── todo.js                   # クライアントサイドJavaScript
```

## 実行方法

### 前提条件

- .NET 8.0 SDK

### 起動手順

1. プロジェクトディレクトリに移動
2. 以下のコマンドを実行：

```bash
dotnet run
```

3. ブラウザで `http://localhost:8080` にアクセス

## 主な機能

- ユーザー認証（ログイン/ログアウト/アカウント作成）
- ToDo管理（追加/編集/削除）
- リアルタイム同期
  - **SSE（Server-Sent Events）**: `/observe`
  - **WebSocket**: `/ws/observe`

## Go版からの主な変更点

| Go版 | C#版 |
|------|------|
| `net/http` | ASP.NET Core |
| `html/template` | Razorビューエンジン |
| `{{range .}}` | `@foreach` |
| `static/` | `wwwroot/static/` |
| `gorilla/websocket` | `System.Net.WebSockets` |

## リアルタイム通知の切り替え

`wwwroot/static/todo.js` の `OBSERVE_TYPE` で通信方式を切り替えられます：

```javascript
// 通信方式の設定
const OBSERVE_TYPE = "websocket"; // "sse" または "websocket"
```

## セキュリティ機能

### セッションID署名検証

Go版の`session.go`に実装されている署名付きセッションID生成・検証機能を`SessionIdSigner`クラスとして実装しています。

**特徴：**
- MD5ハッシュによる署名
- Base64 URL-safe エンコーディング
- タイミング攻撃対策（`CryptographicOperations.FixedTimeEquals`）

## ライセンス

このプロジェクトは、オリジナルの[webtech-fundamentals](https://github.com/little-forest/webtech-fundamentals)と同様に**MITライセンス**の下で公開されています。

### オリジナルの著作権表示

```
MIT License

Copyright (c) 2024 KOMORI Yusuke

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```