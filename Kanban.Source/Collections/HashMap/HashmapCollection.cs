using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.Arrays;

/// <summary>
/// Singly linked list implementation of <see cref="IMyCollection{T}"/>.
/// This class does not rely on System.Collections.Generic.
/// </summary>
/// <typeparam name="TKey">Element type.</typeparam>
/// <typeparam name="TValue">Element type.</typeparam>
public class HashmapCollection<TKey, TValue> : IMyCollection<(TKey, TValue)>
{
    private (TKey, TValue)[] _items = new (TKey, TValue)[0];
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="HashmapCollection{TKey, TValue}"/> class.
    /// </summary>
    public HashmapCollection()
    {
        _count = 0;
        Dirty = false;
    }
    /// <inheritdoc/>
    public int Count => _count;
    /// <inheritdoc/>
    public bool Dirty {get; set;}

    /// <inheritdoc/>
    public void Add((TKey, TValue) item)
    {
        (TKey, TValue)[] newItems = new (TKey, TValue)[_count];
        if(_count == _items.Length)
        {
            for(int i = 0; i < _items.Length; i++)
            {
                newItems[i] = _items[i];
            }
            newItems[^1] = item;
        }
        _items = newItems;
        _count++;
    }
    /// <inheritdoc/>
    public bool Remove((TKey, TValue) item)
    {
        for(int i = 0; i < _count; i++)
        {
            if(_items[i].Item1!.Equals(item.Item2))
            {
                Shift(i, false);
                _items[^1] = default;
                _count--;
                return true;
            }
        }
        return false;
    }
    /// <inheritdoc/>
    public (TKey, TValue) FindBy<Key>(Key key, Func<(TKey, TValue), Key, bool> comparer)
    {
        for(int i = 0; i < _count; i++)
        {
            if(comparer(_items[i], key)) return _items[i];
        }
        return default;
    }
    /// <inheritdoc/>
    public IMyCollection<(TKey, TValue)> Filter(Func<(TKey, TValue), bool> predicate) => throw new NotImplementedException();
    /// <inheritdoc/>
    public void Sort(Comparison<(TKey, TValue)> comparison) => throw new NotImplementedException();
    /// <inheritdoc/>
    public TResult Reduce<TResult>(TResult initial, Func<TResult, (TKey, TValue), TResult> accumulator) => throw new NotImplementedException();
    /// <inheritdoc/>
    public IMyIterator<(TKey, TValue)> GetIterator() => throw new NotImplementedException();

    private void Shift(int i, bool right = true)
    {
        if(!right)
        {
            for(int j = i; j < _items.Length-1; ++j)
            {
                _items[j] = _items[j+1];
            }
        }
        else
        {
            for(int j = _items.Length-1; j >= i; --j)
            {
                _items[j] = _items[j-1];
            }
            _items[i] = default;
        }
    }
}