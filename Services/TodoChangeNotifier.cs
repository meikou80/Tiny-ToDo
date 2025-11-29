using System.Threading.Channels;
using TinyToDo.Models;

namespace TinyToDo.Services;

// ユーザーごとのToDo変更を通知するサービス（シングルトン）
public class TodoChangeNotifier
{
    // ユーザーIDごとにObserver（Channel）のリストを管理
    private readonly Dictionary<string, List<Channel<TodoChangeEvent>>> _observers = new();
    private readonly object _lock = new();

    // Observerを生成して登録
    public Channel<TodoChangeEvent> CreateObserver(string userId)
    {
        var channel = Channel.CreateUnbounded<TodoChangeEvent>();
        lock (_lock)
        {
            if (!_observers.ContainsKey(userId))
            {
                _observers[userId] = new List<Channel<TodoChangeEvent>>();
            }
            _observers[userId].Add(channel);
            return channel;
        }
    }

    // Observerを削除
    public void RemoveObserver(string userId, Channel<TodoChangeEvent> channel)
    {
        lock (_lock)
        {
            if (_observers.ContainsKey(userId))
            {
                _observers[userId].Remove(channel);
            }
        }
    }
    
    // 全Observerに通知
    public void Notify(string userId, TodoChangeEvent ev)
    {
        lock (_lock)
        {
            if (_observers.TryGetValue(userId, out var channels))
            {
                foreach (var channel in channels)
                {
                    channel.Writer.TryWrite(ev);
                }
            }
        }
    }
}