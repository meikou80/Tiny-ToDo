using TinyToDo.Models;

namespace TinyToDo.Services;

public static class TodoService
{
    private static List<TodoItem> Todos { get; }
    private static int nextId = 1;

    static TodoService()
    {
        Todos = new List<TodoItem>();
    }

    public static List<TodoItem> GetAll() => Todos;

    public static TodoItem? Get(int id) => Todos.FirstOrDefault(t => t.Id == id);

    public static void Add(string content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            Todos.Add(new TodoItem 
            { 
                Id = nextId++, 
                Content = content, 
                IsCompleted = false 
            });
        }
    }

    public static void Delete(int id)
    {
        var todo = Get(id);
        if (todo is null)
            return;
        Todos.Remove(todo);
    }

    public static void Toggle(int id)
    {
        var todo = Get(id);
        if (todo is null)
            return;
        todo.IsCompleted = !todo.IsCompleted;
    }
}

