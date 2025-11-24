using TinyToDo.Models;

namespace TinyToDo.Services;

public static class TodoService
{
    private static List<TodoModel> Todos { get; }
    private static int nextId = 1;

    static TodoService()
    {
        Todos = new List<TodoModel>();
    }

    public static List<TodoModel> GetAll() => Todos;

    public static TodoModel? Get(int id) => Todos.FirstOrDefault(t => t.Id == id);

    public static void Add(string content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            Todos.Add(new TodoModel 
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