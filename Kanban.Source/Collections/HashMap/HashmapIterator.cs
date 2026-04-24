using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.HashMap;

/// <summary>
/// Iterator implementation for hashmap-backed collections.
/// </summary>

public class HashmapIterator<TKey, TValue> : IMyIterator<KeyValue<TKey, TValue>>
{
    private readonly KeyValue<TKey, TValue>?[] _items;
    private readonly int _count;
    private int _index;
    private int _passed;

    /// <summary>
    /// constructor for the hashmap iterator
    /// </summary>
    /// <param name="items"></param>
    /// <param name="count"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HashmapIterator(KeyValue<TKey, TValue>?[] items, int count)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _count = count;
        _index = 0;
        _passed = 0;
    }

    /// <inheritdoc/>
    public bool HasNext() {
        return _passed < _count;
    }

    /// <inheritdoc/>
    public KeyValue<TKey, TValue> Next()
    {
        if (!HasNext()) 
            throw new InvalidOperationException("No more elements.");
        

        while(_index < _items.Length)
        {
            KeyValue<TKey, TValue>? item = _items[_index++];
            if (item is not null && !item.IsDeleted) 
            {
                _passed++;
                return item;
            }
        }

        throw new InvalidOperationException("No more elements.");
    }

    /// <inheritdoc/>
    public void Reset()
    {
        _index = 0;
        _passed = 0;
    }
}
