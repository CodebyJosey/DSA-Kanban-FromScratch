using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.HashMaps;

/// <summary>
/// Iterates hash map items in insertion order.
/// </summary>
public sealed class HashMapIterator<T> : IMyIterator<T>
{
    private readonly HashMapNode<T>? _head;
    private HashMapNode<T>? _current;

    /// <summary>
    /// Creates a new iterator.
    /// </summary>
    internal HashMapIterator(HashMapNode<T>? head)
    {
        _head = head;
        _current = head;
    }

    /// <inheritdoc/>
    public bool HasNext()
    {
        return _current != null;
    }

    /// <inheritdoc/>
    public T Next()
    {
        if (_current == null)
        {
            throw new InvalidOperationException("No more items in iterator.");
        }

        T value = _current.Value;
        _current = _current.NextInserted;
        return value;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        _current = _head;
    }
}