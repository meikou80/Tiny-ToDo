using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TinyToDo.Services;
using TinyToDo.Filters;
using TinyToDo.Models;

namespace TinyToDo.Controllers;

[RequireAuthentication]
public class SseController : Controller
{
    private readonly ILogger<SseController> _logger;
    private readonly TodoChangeNotifier _notifier;

    // コンストラクタでILoggerとTodoChangeNotifierを注入
    public SseController(ILogger<SseController> logger, TodoChangeNotifier notifier)
    {
        _logger = logger;
        _notifier = notifier;
    }

    // セッション情報をHttpContext.Itemsから取得するヘルパーメソッド
    private HttpSession GetSession()
    {
        return (HttpSession)HttpContext.Items["Session"]!;
    }

    // camelCase用のJSONオプション
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // SSEイベントを送信するヘルパーメソッド
    private async Task SendSseEvent(string eventName, object data)
    {
        var id = $"ttd-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var json = JsonSerializer.Serialize(data, _jsonOptions);

        await Response.WriteAsync($"id: {id}\n");
        await Response.WriteAsync($"event: {eventName}\n");
        await Response.WriteAsync($"data: {json}\n\n");
        await Response.Body.FlushAsync();
    }

    // GET /observe - SSEエンドポイント
    [Route("/observe")]
    [HttpGet]
    public async Task Observe(CancellationToken cancellationToken)
    {
        // SSE用のヘッダを設定
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        // レスポンスのバッファリングを無効にして、即座にクライアントに送信されるようにする
        await Response.Body.FlushAsync();

        var session = GetSession();
        var userId = session.UserAccount?.Id ?? "";
        var sourceId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        _logger.LogInformation(
            "SSE接続開始。ユーザーID={UserId} セッションID={SessionId} SourceId={SourceId}",
            userId,
            session.SessionId,
            sourceId
        );

        // initialイベントを送信し、sourceIdを通知する
        await SendSseEvent("initial", new { source = sourceId });

        // Observerを登録
        var channel = _notifier.CreateObserver(userId);

        try
        {
            // イベント待ちループ
            await foreach (var ev in channel.Reader.ReadAllAsync(cancellationToken))
            {
                _logger.LogInformation(
                    "ToDo変更を受信。ユーザーID={UserId} SourceId={SourceId} Event={Event}",
                    userId,
                    sourceId,
                    ev.Event
                );
                await SendSseEvent(ev.Event, ev);
            }
        }
        finally
        {
            // 接続終了時にObserverを削除
            _notifier.RemoveObserver(userId, channel);
            _logger.LogInformation(
                "SSE接続終了。ユーザーID={UserId} セッションID={SessionId} SourceId={SourceId}",
                userId,
                session.SessionId,
                sourceId
            );
        }
    }
}

