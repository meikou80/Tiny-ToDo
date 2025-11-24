using TinyToDo.Models;

namespace TinyToDo.Services;

public static class TodoService
{
    // セッションIDをキーとしてToDoリストを保持するマップ
    private static Dictionary<string, List<TodoModel>> TodoLists { get; } = new();
    private static Dictionary<string, int> NextIds { get; } = new();

    // セッションIDに紐付くToDoリストを取得する。
    private static List<TodoModel> GetTodoList(string sessionId)
    {
        if (!TodoLists.TryGetValue(sessionId, out var todos))
        {
            todos = new List<TodoModel>();
            TodoLists[sessionId] = todos;
            NextIds[sessionId] = 1;
        }
        return todos;
    }

    // セッションに紐付く全ToDoを取得する。
    public static List<TodoModel> GetAll(string sessionId)
    {
        return GetTodoList(sessionId);
    }

    // 指定されたIDのToDoを取得する。
    public static TodoModel? Get(string sessionId, int id)
    {
        var todos = GetTodoList(sessionId);
        return todos.FirstOrDefault(t => t.Id == id);
    }

    // セッション上のToDoリストにToDoを追加する。
    public static void Add(string sessionId, string content)
    {
        content = content?.Trim() ?? "";
        if (!string.IsNullOrEmpty(content))
        {
            var todos = GetTodoList(sessionId);
            if (!NextIds.ContainsKey(sessionId))
            {
                NextIds[sessionId] = 1;
            }

            todos.Add(new TodoModel 
            { 
                Id = NextIds[sessionId]++, 
                Content = content, 
                IsCompleted = false 
            });
        }
    }

    // 指定されたIDのToDoを削除する。
    public static void Delete(string sessionId, int id)
    {
        var todo = Get(sessionId, id);
        if (todo is null)
            return;
        var todos = GetTodoList(sessionId);
        todos.Remove(todo);
    }

    // 指定されたIDのToDoの完了状態を切り替える。
    public static void Toggle(string sessionId, int id)
    {
        var todo = Get(sessionId, id);
        if (todo is null)
            return;
        todo.IsCompleted = !todo.IsCompleted;
    }
}