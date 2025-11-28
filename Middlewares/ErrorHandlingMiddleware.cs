namespace TinyToDo.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Internal Server Error");
            
            // レスポンスが既に開始されている場合は、ヘッダーを変更できない
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(
                    $"500 Internal Server Error\n\n{ex.Message}"
                );
            }
            else
            {
                // レスポンスが既に開始されている場合はログのみ出力
                _logger.LogWarning("Cannot write error response, response already started");
            }
        }
    }
}