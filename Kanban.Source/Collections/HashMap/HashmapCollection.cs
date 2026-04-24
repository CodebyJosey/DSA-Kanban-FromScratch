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
    private KeyValue<TKey, TValue>[] _items;
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="HashmapCollection{TKey, TValue}"/> class.
    /// </summary>
    public HashmapCollection(int size=20)
    {
        _items = new KeyValue<TKey, TValue>[size];
        _count = 0;
        Dirty = false;
    }
    /// <inheritdoc/>
    public int Count => _count;
    /// <inheritdoc/>
    public bool Dirty {get; set;}

    /// <inheritdoc/>
    public void Add(KeyValue<TKey, TValue> item)
    {
        if(Count >= _items.Length) rehash(_items.Length + 10);

        int hash = item.Key!.GetHashCode();
        int index = Math.Abs(hash % _items.Length);

        while(_items[index] is not null && !_items[index].IsDeleted)
        {   
            if(_items[index].Key!.Equals(item.Key)) return;
            index = (index + 1) % _items.Length;
        }
        
        _items[index] = item;
        _count++;
        Dirty = true;
    }
    /// <inheritdoc/>
    public bool Remove(KeyValue<TKey, TValue> item)
    {
        int hash = item.Key!.GetHashCode();
        int index = Math.Abs(hash % _items.Length);

        while(_items[index] is not null)
        {
            if(!_items[index].IsDeleted)
                if(_items[index].Key!.Equals(item.Key))
                {
                    _items[index].IsDeleted = true;
                    _count--;
                    Dirty = true;
                    return true;
                }

            index = (index + 1) % _items.Length;
        }
        return false;
    }

    /// <summary>
    /// get item by key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public KeyValue<TKey, TValue>? Get(int key)
    {
        int hash = key.GetHashCode();
        int index = Math.Abs(hash % _items.Length);

        while(_items[index] is not null)
        {
            if(_items[index].IsDeleted)
                if(_items[index].Key!.Equals(key))
                    return _items[index];
        }
        return null;
    }
    /// <inheritdoc/>
    public KeyValue<TKey, TValue>? FindBy<Key>(Key key, Func<KeyValue<TKey, TValue>, Key, bool> comparer)
    {
        for(int i = 0; i < _items.Length; i++)
        {
            if(_items[i] is null || _items[i].IsDeleted) continue;
            if(comparer(_items[i], key)) return _items[i];
        }
        return default;
    }
    /// <inheritdoc/>
    public IMyCollection< KeyValue<TKey, TValue>> Filter(Func< KeyValue<TKey, TValue>, bool> predicate)
    {
        IMyCollection< KeyValue<TKey, TValue>> result = new HashmapCollection<TKey, TValue>(_items.Length);
        for(int i = 0; i < _items.Length; i++)
        {
            if(_items[i] is null || _items[i].IsDeleted) continue;
            if(predicate(_items[i])) result.Add(_items[i]);
        }
        return result;
    }
    /// <inheritdoc/>
    public void Sort(Comparison< KeyValue<TKey, TValue>> comparison) => throw new NotImplementedException();
    /// <inheritdoc/>
    public TResult Reduce<TResult>(TResult initial, Func<TResult,  KeyValue<TKey, TValue>, TResult> accumulator)
    {
        for(int i = 0; i < _items.Length; i++)
        {
            if(_items[i] is null || _items[i].IsDeleted) continue;
            initial = accumulator(initial, _items[i]);
        }
        
        return initial;
    }
    /// <inheritdoc/>
    public IMyIterator<KeyValue<TKey, TValue>> GetIterator() =>
        new HashmapIterator<KeyValue<TKey, TValue>>(_items, _count);

    private void rehash(int newSize)
    {
        KeyValue<TKey, TValue>[] tempItems = _items;
        _items = new KeyValue<TKey, TValue>[newSize];

        for(int i = 0; i < tempItems.Length; i++)
            if(tempItems[i] is not null && !tempItems[i].IsDeleted)
                Add(tempItems[i]);
    }
}