using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.LinkedLists;

/// <summary>
/// Singly linked list implementation of <see cref="IMyCollection{T}"/>.
/// This class does not rely on System.Collections.Generic.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>

public sealed class LinkedListCollection<T> : IMyCollection<T>
{
    private LinkedListNode<T>? _head;
    private LinkedListNode<T>? _tail;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedListCollection{T}"/> class.
    /// </summary>
    /// <param name="head">Initial head node.</param>
    /// <param name="tail">Initial tail node.</param>
    internal LinkedListCollection(LinkedListNode<T>? head = null, LinkedListNode<T>? tail = null)
    {
        _head = head;
        _tail = tail;
    }

    /// <inheritdoc/>
    public int Count => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool Dirty
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        var newNode = new LinkedListNode<T>(item);
        if (_head == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            _tail!.Next = newNode;
            _tail = newNode;
        }
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (_head == null) return false;

        if (EqualityComparer<T>.Default.Equals(_head.Value, item))
        {
            _head = _head.Next;
            if (_head == null) _tail = null; // List is now empty
            return true;
        }

        var current = _head;
        while (current != null && current.Next != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Next.Value, item))
            {
                current.Next = current.Next.Next;
                if (current.Next == null) _tail = current; // Update tail if necessary
                return true;
            }
            current = current.Next;
        }

        return false;
    }

    /// <inheritdoc/>
    public T? FindBy<TKey>(TKey key, Func<T, TKey, bool> comparer)
    {
        var current = _head;
        while (current != null)
        {
            if (comparer(current.Value, key))
            {
                return current.Value;
            }
            current = current.Next;
        }
        return default;
    }


    /// <inheritdoc/>
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        if (predicate == null) return null!;
        var filteredHead = default(LinkedListNode<T>);
        var filteredTail = default(LinkedListNode<T>);
        var current = _head;
        while (current != null)
        {
            if (predicate(current.Value))
            {
                var newNode = new LinkedListNode<T>(current.Value);
                if (filteredHead == null)
                {
                    filteredHead = newNode;
                    filteredTail = newNode;
                }
                else
                {
                    filteredTail!.Next = newNode;
                    filteredTail = newNode;
                }
            }
            current = current.Next;
        }
        return new LinkedListCollection<T>(filteredHead, filteredTail);
    }

    /// <inheritdoc/>
    public void Sort(Comparison<T> comparison)
    {
        if (comparison == null) return;
        
        // bubble sort implementation for simplicity; not efficient for large lists

        bool swapped;
        do
        {
            swapped = false;
            var current = _head;
            while (current != null && current.Next != null)
            {
                if (comparison(current.Value, current.Next.Value) > 0)
                {
                    // Swap values
                    var temp = current.Value;
                    current.Value = current.Next.Value;
                    current.Next.Value = temp;
                    swapped = true;
                }
                current = current.Next;
            }
        } while (swapped);
    }

    /// <inheritdoc/>
    public TResult Reduce<TResult>(TResult initial, Func<TResult, T, TResult> accumulator)
    {
        if (accumulator == null) return initial;
        var result = initial;
        var current = _head;
        while (current != null)
        {
            result = accumulator(result, current.Value);
            current = current.Next;
        }
        return result;
    }

    /// <inheritdoc/>
    public IMyIterator<T> GetIterator()
    {
        return new LinkedListIterator<T>(_head);
    }

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">Index of the element to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    public void RemoveAt(int index)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");

        if (_head == null) throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        if (index == 0)
        {
            _head = _head.Next;
            if (_head == null) _tail = null; // List is now empty
            return;
        }

        var current = _head;
        for (int i = 0; i < index - 1; i++)
        {
            if (current.Next == null) throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            current = current.Next;
        }

        if (current.Next == null) throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        current.Next = current.Next.Next;
        if (current.Next == null) _tail = current; // Update tail if necessary
    }

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">Zero-based index.</param>
    /// <param name="value">The found value when successful.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    public bool TryGetAt(int index, out T? value)
    {
        value = default;
        if (index < 0) return false;

        var current = _head;
        for (int i = 0; i < index; i++)
        {
            if (current == null) return false;
            current = current.Next;
        }

        if (current == null) return false;

        value = current.Value;
        return true;
    }

    private void EnsureCapacity(int required)
    {
        // No capacity management needed for linked list
    }

    private int IndexOf(T item)
    {
        var current = _head;
        int index = 0;
        while (current != null)
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, item))
            {
                return index;
            }
            current = current.Next;
            index++;
        }
        return -1; // Not found
    }
}