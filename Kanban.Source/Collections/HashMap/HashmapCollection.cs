using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.HashMap;

/// <summary>
/// Hashmap implementation of <see cref="IMyCollection{T}"/>.
/// This class does not rely on System.Collections.Generic.
/// </summary>
/// <typeparam name="TKey">Element type.</typeparam>
/// <typeparam name="TValue">Element type.</typeparam>
public class HashmapCollection<TKey, TValue> : IMyCollection<KeyValue<TKey, TValue>>
{
    private KeyValue<TKey, TValue>[] _items = new KeyValue<TKey, TValue>[1];
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
    public void Add( KeyValue<TKey, TValue> item)
    {
        KeyValue<TKey, TValue>[] newItems = new  KeyValue<TKey, TValue>[_count];
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
    public bool Remove(KeyValue<TKey, TValue> item)
    {
        for(int i = 0; i < _count; i++)
        {
            if(_items[i].Equals(item))
            {
                Shift(i, false);
                _items[^1] = default!;
                _count--;
                return true;
            }
        }
        return false;
    }
    /// <inheritdoc/>
    public  KeyValue<TKey, TValue> FindBy<Key>(Key key, Func< KeyValue<TKey, TValue>, Key, bool> comparer)
    {
        for(int i = 0; i < _count; i++)
        {
            if(comparer(_items[i], key)) return _items[i];
        }
        return default!;
    }
    /// <inheritdoc/>
    public IMyCollection< KeyValue<TKey, TValue>> Filter(Func< KeyValue<TKey, TValue>, bool> predicate)
    {
        IMyCollection< KeyValue<TKey, TValue>> result = new HashmapCollection<TKey, TValue>();
        for(int i = 0; i < _count; i++)
        {
            if(predicate(_items[i])) result.Add(_items[i]);
        }
        return result;
    }
    /// <inheritdoc/>
    public void Sort(Comparison< KeyValue<TKey, TValue>> comparison) => throw new NotImplementedException();
    /// <inheritdoc/>
    public TResult Reduce<TResult>(TResult initial, Func<TResult,  KeyValue<TKey, TValue>, TResult> accumulator)
    {
        for(int i = 0; i < _count; i++)
        {
            accumulator(initial, _items[i]);
        }
        return initial;
    }
    /// <inheritdoc/>
    public IMyIterator<KeyValue<TKey, TValue>> GetIterator() =>
        new HashmapIterator<KeyValue<TKey, TValue>>(_items, _count);

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
            _items[i] = default!;
        }
    }
}