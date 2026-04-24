using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.HashMap;

/// <summary>
/// Iterator implementation for hashmap-backed collections.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>

public class HashmapIterator<T> : IMyIterator<T>
{
    private readonly T?[] _items;
    private readonly int _count;
    private int _index;

    /// <summary>
    /// constructor for the hashmap iterator
    /// </summary>
    /// <param name="items"></param>
    /// <param name="count"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HashmapIterator(T?[] items, int count)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _count = count;
        _index = 0;
    }

    /// <inheritdoc/>
    public bool HasNext()
    {
        while (_index < _count && _items[_index] is null)
        {
            _index++;
        }

        return _index < _count;
    }

    /// <inheritdoc/>
    public T Next()
    {
        if (!HasNext())
        {
            throw new InvalidOperationException("No more elements.");
        }

        return _items[_index++]!;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        _index = 0;
    }
}
