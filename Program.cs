var builder = WebApplication.CreateBuilder(args);

// サービスの追加
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 静的ファイルの提供を有効化
app.UseStaticFiles();

// ルーティングの設定
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ポート8080でリッスン
app.Run("http://localhost:8080");

