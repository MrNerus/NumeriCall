using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumeriCall.MathOps;

public class Stack<T>(int maxCapacity = 0)
{
    private List<T> _items = [];
    private readonly int _maxCapacity = maxCapacity;
    public int Count {get; set;} = 0;

 
    public int Push(T item)
    {
        if (_maxCapacity > 0 && Count >= _maxCapacity) throw new StackException("Stack is full", StackExceptionMode.STACK_IS_FULL);

        _items.Add(item);
        Count += 1;
        return Count;
    }

    public T Peek() 
    {
        if (Count == 0) throw new StackException("Stack is empty", StackExceptionMode.STACK_IS_EMPTY);
        
        return _items[Count - 1];

    }
    public T Pop()
    {
        if (Count == 0) throw new StackException("Stack is empty", StackExceptionMode.STACK_IS_EMPTY);

        T popped = _items[Count - 1];
        _items.RemoveAt(Count - 1);
        Count -= 1;
        return popped;

    }

    public bool IsEmpty() 
    {
        return Count == 0;
    }

    public bool IsFull() 
    {
        return Count == _maxCapacity;
    }

    public void Clear() 
    {
        _items.Clear();
        Count = 0;
    }

    public void Reverse() 
    {
        if (IsEmpty()) return;
        _items.Reverse();
    }
    
    public List<T> ToList() 
    {
        return JsonSerializer.Deserialize<List<T>>(JsonSerializer.Serialize(_items)) ?? throw new StackException("Unable to clone the stack.", StackExceptionMode.STACK_CLONING_ERROR);;
    }
    

    public string ToString(bool isReversed = false)
    {
        if (Count == 0) throw new StackException("Stack is empty", StackExceptionMode.STACK_IS_EMPTY);

        StringBuilder stringBuilder = new();
        
        if (!isReversed) 
        {
            for (int i = Count - 1; i >= 0; i--) stringBuilder.Append(_items[i]);
        } 
        else 
        {
            for (int i = 0; i < Count; i++) stringBuilder.Append(_items[i]);
        }

        return stringBuilder.ToString();
    }

}

public class StackException(string message, StackExceptionMode mode) : Exception(message)
{
    public StackExceptionMode Mode { get; set; } = mode;
}

public enum StackExceptionMode {
    DEFAULT,
    STACK_IS_EMPTY,
    STACK_IS_FULL,
    STACK_CLONING_ERROR,
}