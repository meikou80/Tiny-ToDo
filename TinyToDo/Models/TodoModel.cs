namespace TinyToDo.Models;

public class TodoModel
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}