namespace TinyToDo.Models;

// /add エンドポイント用リクエストモデル
public class TodoAddRequest
{
    public string Todo { get; set; } = string.Empty;
}

// /edit エンドポイント用リクエストモデル
public class TodoEditRequest
{
    public string Id { get; set; } = string.Empty;
    public string Todo { get; set; } = string.Empty;
}

// レスポンス用モデル
public class TodoItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string Todo { get; set; } = string.Empty;
}