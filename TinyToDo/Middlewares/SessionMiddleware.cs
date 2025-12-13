using TinyToDo.Services;
using TinyToDo.Models;

namespace TinyToDo.Middlewares;

public class SessionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SessionMiddleware> _logger;

    public SessionMiddleware(RequestDelegate next, ILogger<SessionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sessionService = SessionService.Instance;
        var session = sessionService.EnsureSession(context);
        
        // HttpContext.Itemsにセッションを格納
        context.Items["Session"] = session;
        
        // リクエストログ出力
        LogRequest(context, session);
        
        await _next(context);
    }

    private void LogRequest(HttpContext context, HttpSession session)
    {
        var userId = session.UserAccount?.Id ?? "";
        var requestUri = context.Request.Path + context.Request.QueryString;
        _logger.LogInformation(
            "{Method} {RequestUri} {SessionId} {UserId}",
            context.Request.Method,
            requestUri,
            session.SessionId,
            userId
        );
    }
}