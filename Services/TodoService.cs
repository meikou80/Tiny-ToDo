using TinyToDo.Models;

namespace TinyToDo.Services;

public static class TodoService
{
    // ユーザーアカウントに紐付く全ToDoを取得する。
    public static List<TodoModel> GetAll(UserAccount? account)
    {
        if (account is null)
        {
            return new List<TodoModel>();
        }
        return account.ToDoList;
    }

    // 指定されたIDのToDoを取得する。
    public static TodoModel? Get(UserAccount? account, string id)
    {
        if (account is null)
        {
            return null;
        }
        return account.ToDoList.FirstOrDefault(t => t.Id == id);
    }

    // ユーザーアカウント上のToDoリストにToDoを追加する。
    public static void Add(UserAccount? account, string content)
    {
        if (account is null)
        {
            return;
        }

        content = content?.Trim() ?? "";
        if (!string.IsNullOrEmpty(content))
        {
            var id = TodoIdGenerator.GenerateId(content);

            account.ToDoList.Add(new TodoModel
            {
                Id = id,
                Content = content,
                IsCompleted = false
            });
        }
    }

    // 指定されたIDのToDoを削除する。
    public static void Delete(UserAccount? account, string id)
    {
        if (account is null)
        {
            return;
        }
        var todo = Get(account, id);
        if (todo is null)
        {
            return;
        }
        account.ToDoList.Remove(todo);
    }

    // 指定されたIDのToDoの完了状態を切り替える。
    public static void Toggle(UserAccount? account, string id)
    {
        if (account is null)
        {
            return;
        }
        var todo = Get(account, id);
        if (todo is null)
            return;
        todo.IsCompleted = !todo.IsCompleted;
    }

    // 指定されたIDのToDoの内容を更新する。
    public static TodoModel? Update(UserAccount? account, string id, string newContent)
    {
        if (account is null)
        {
            return null;
        }

        var todo = Get(account, id);
        if (todo is null)
        {
            return null;
        }

        newContent = newContent?.Trim() ?? "";
        if (!string.IsNullOrEmpty(newContent))
        {
            todo.Content = newContent;
        }

        return todo;
    }
}