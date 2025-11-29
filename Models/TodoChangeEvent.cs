namespace TinyToDo.Models;

// クライアントへSourceIdを通知するためのイベントデータ
public class SourceIdNotification
{
    public string Source { get; set; } = string.Empty;
}

// クライアントへToDoの変化を通知するためのイベントデータ
public class TodoChangeEvent
{
    public string Source { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public TodoModel TodoItem { get; set; } = new ();
}