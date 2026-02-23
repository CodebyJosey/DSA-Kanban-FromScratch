using Kanban.Domain.Abstractions.Collections;

namespace Kanban.Infrastructure.Collections.Arrays;

/// <summary>
/// Iterator implementation for array-backed collections.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class ArrayIterator<T> : IMyIterator<T>
{
    private readonly T?[] _items;
    private readonly int _count;
    private int _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayIterator{T}"/> class.
    /// </summary>
    /// <param name="items">Backing array.</param>
    /// <param name="count">Number of valid elements in the array.</param>
    public ArrayIterator(T?[] items, int count)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _count = count;
        _index = 0;
    }

    /// <inheritdoc />
    public bool HasNext()
    {
        while (_index < _count && _items[_index] is null)
        {
            _index++;
        }

        return _index < _count;
    }

    /// <inheritdoc />
    public T Next()
    {
        if (!HasNext())
        {
            throw new InvalidOperationException("No more elements.");
        }

        return _items[_index++]!;
    }

    /// <inheritdoc />
    public void Reset()
    {
        _index = 0;
    }
}