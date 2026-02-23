using Kanban.Domain.Abstractions.Collections;

namespace Kanban.Infrastructure.Collections.Arrays;

/// <summary>
/// Dynamic array-backed implementation of <see cref="IMyCollection{T}"/>.
/// This class does not rely on System.Collections.Generic.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class ArrayCollection<T> : IMyCollection<T>
{
    private T?[] _items;
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayCollection{T}"/> class.
    /// </summary>
    /// <param name="initialCapacity">Initial storage capacity (minimum 1).</param>
    public ArrayCollection(int initialCapacity = 8)
    {
        if (initialCapacity < 1)
        {
            initialCapacity = 1;
        }

        _items = new T?[initialCapacity];
        _count = 0;
        Dirty = false;
    }

    /// <inheritdoc/>
    public int Count => _count;

    /// <inheritdoc/>
    public bool Dirty { get; set; }

    /// <inheritdoc/>
    public void Add(T item)
    {
        EnsureCapacity(_count + 1);
        _items[_count] = item;
        _count++;
        Dirty = true;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index < 0) // was not found -> IndexOf returns -1 when not found
        {
            return false;
        }

        RemoveAt(index);
        Dirty = true;
        return true;
    }

    /// <inheritdoc/>
    public T? FindBy<TKey>(TKey key, Func<T, TKey, bool> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        for (int i = 0; i < _count; i++)
        {
            T? current = _items[i];
            if (current is null)
            {
                continue;
            }

            if (comparer(current, key))
            {
                return current;
            }
        }

        return default;
    }


    /// <inheritdoc/>
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        // Start small, will autogrow
        ArrayCollection<T>? result = new ArrayCollection<T>(initialCapacity: 4);
        for (int i = 0; i < _count; i++)
        {
            T? current = _items[i];
            if (current is null) continue;

            if (predicate(current))
            {
                result.Add(current);
            }
        }

        // Filter is a new collection, so it can't be considered as "dirty" by default.
        result.Dirty = false;
        return result;
    }

    /// <inheritdoc/>
    public void Sort(Comparison<T> comparison)
    {
        ArgumentNullException.ThrowIfNull(comparison);

        // Simple bubble sort
        for (int i = 0; i < _count; i++)
        {
            for (int j = 0; j < _count - 1 - i; j++)
            {
                T? a = _items[j];
                T? b = _items[j + 1];

                // No nulls
                if (a is null || b is null)
                {
                    continue;
                }

                if (comparison(a, b) > 0)
                {
                    (_items[j], _items[j + 1]) = (_items[j + 1], _items[j]);
                }
            }
        }

        Dirty = true;
    }

    /// <inheritdoc/>
    public TResult Reduce<TResult>(TResult initial, Func<TResult, T, TResult> accumulator)
    {
        ArgumentNullException.ThrowIfNull(accumulator);

        TResult? result = initial;

        for (int i = 0; i < _count; i++)
        {
            T? current = _items[i];
            if (current is null) continue;

            result = accumulator(result, current);
        }

        return result;
    }

    /// <inheritdoc/>
    public IMyIterator<T> GetIterator()
    {
        return new ArrayIterator<T>(_items, _count);
    }

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">Index of the element to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        for (int i = index; i < _count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        _items[_count - 1] = default;
        _count--;
        Dirty = true;
    }

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">Zero-based index.</param>
    /// <param name="value">The found value when successful.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    public bool TryGetAt(int index, out T? value)
    {
        if (index < 0 || index >= _count)
        {
            value = default;
            return false;
        }

        value = _items[index];
        return true;
    }

    private void EnsureCapacity(int required)
    {
        if (_items.Length >= required) return;

        int newCapacity = _items.Length * 2;
        if (newCapacity < required)
        {
            newCapacity = required;
        }

        T?[] newArray = new T?[newCapacity];
        for (int i = 0; i < _count; i++)
        {
            newArray[i] = _items[i];
        }

        _items = newArray;
    }

    private int IndexOf(T item)
    {
        for (int i = 0; i < _count; i++)
        {
            T? current = _items[i];
            if (current is null) continue;
            if (Equals(current, item)) return i;
        }

        return -1;
    }
}