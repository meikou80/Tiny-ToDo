using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TinyToDo.Services;
using TinyToDo.Filters;
using TinyToDo.Models;

namespace TinyToDo.Controllers;

[RequireAuthentication]
public class WebSocketController : Controller
{
    private readonly ILogger<WebSocketController> _logger;
    private readonly TodoChangeNotifier _notifier;

    public WebSocketController(ILogger<WebSocketController> logger, TodoChangeNotifier notifier)
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

    [Route("/ws/observe")]
    [HttpGet]
    public async Task Observe()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }

        using var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await ObserveTodo(websocket);
    }

    private async Task ObserveTodo(WebSocket websocket)
    {
        var session = GetSession();
        var userId = session.UserAccount?.Id ?? "";
        var sourceId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        _logger.LogInformation(
            "WebSocket接続開始。ユーザーID={UserId} セッションID={SessionId} SourceId={SourceId}",
            userId,
            session.SessionId,
            sourceId
        );

        // initialイベント送信
        await SendWebSocketEvent(websocket, "initial", new SourceIdNotification { Source = sourceId });

        // Observer登録
        var channel = _notifier.CreateObserver(userId);

        try
        {
            // イベント待ちループ
            await foreach (var ev in channel.Reader.ReadAllAsync())
            {
                await SendWebSocketEvent(websocket, ev.Event, ev);
            }
        }
        finally
        {
            // 接続終了時にObserverを削除
            _notifier.RemoveObserver(userId, channel);
            _logger.LogInformation(
                "WebSocket接続終了。ユーザーID={UserId} セッションID={SessionId} SourceId={SourceId}",
                userId,
                session.SessionId,
                sourceId
            );
        }
    }
    
    // WebSocketイベントを送信するヘルパーメソッド
    private async Task SendWebSocketEvent(WebSocket websocket, string eventName, object data)
    {
        var message = new { @event = eventName, data = data };
        var json = JsonSerializer.Serialize(message, _jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);

        await websocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            endOfMessage: true,
            CancellationToken.None
        );
    }
}