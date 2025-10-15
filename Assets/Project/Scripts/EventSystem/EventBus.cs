using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> _eventTable = new Dictionary<Type, Delegate>();

    public static void Subscribe<T>(Action<T> listener)
    {
        var type = typeof(T);

        if (_eventTable.TryGetValue(type, out var existingDelegate))
        {
            _eventTable[type] = Delegate.Combine(existingDelegate, listener);
        }
        else
        {
            _eventTable[type] = listener;
        }
    }

    public static void Unsubscribe<T>(Action<T> listener)
    {
        var type = typeof(T);

        if (_eventTable.TryGetValue(type, out var existingDelegate))
        {
            var newDelegate = Delegate.Remove(existingDelegate, listener);

            if (newDelegate == null)
            {
                _eventTable.Remove(type);
            }
            else
            {
                _eventTable[type] = newDelegate;
            }
        }
    }

    public static void Publish<T>(T publishedEvent)
    {
        var type = typeof(T);

        if (_eventTable.TryGetValue(type, out var existingDelegate))
        {
            var callback = existingDelegate as Action<T>;
            callback?.Invoke(publishedEvent);
        }
    }

    public static void ClearAll()
    {
        _eventTable.Clear();
    }
}