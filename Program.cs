using TinyToDo.Configuration;
using TinyToDo.Middlewares;
using TinyToDo.Services;

var builder = WebApplication.CreateBuilder(args);

// サービスの追加
builder.Services.AddControllersWithViews();

// ロギング設定（コンソール出力）
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

// SSE通信に伴うDI登録
builder.Services.AddSingleton<TodoChangeNotifier>();

var app = builder.Build();

// ===== ミドルウェアの設定（順序が重要） =====
// 1. エラーハンドリング（最初に配置）
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. 静的ファイルの提供
app.UseStaticFiles();

// 3. WebSocketのサポートを有効化
// ※ HTTPリクエストをWebSocketにアップグレードするために必要
app.UseWebSockets();

// 4. セッション管理（認証前に実行）
app.UseMiddleware<SessionMiddleware>();

// 5. ルーティング
app.UseRouting();

// ===== エンドポイントのマッピング =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todo}/{action=Index}/{id?}");

// favicon.icoは404を返す
app.MapGet("/favicon.ico", () => Results.NotFound());

// ===== ポート設定とサーバー起動 =====
var port = AppSettings.GetPortNumber();
Console.WriteLine($"listening port : {port}");

app.Run($"http://localhost:{port}");