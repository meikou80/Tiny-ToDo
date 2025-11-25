# Tiny ToDo - C#版

このプロジェクトは、元のGo言語版をASP.NET Coreで書き換えたバージョンです。

## ファイル構成

- `Program.cs` - アプリケーションのエントリーポイント（main.goに相当）
- `Controllers/HomeController.cs` - ToDoリストのコントローラー
- `Views/Home/Todo.cshtml` - ToDoリストのビュー（templates/todo.htmlに相当）
- `wwwroot/static/todo.css` - CSSファイル（static/todo.cssに相当）
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