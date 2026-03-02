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
    public void Add(T item) => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool Remove(T item) => throw new NotImplementedException();

    /// <inheritdoc/>
    public T? FindBy<TKey>(TKey key, Func<T, TKey, bool> comparer) => throw new NotImplementedException();


    /// <inheritdoc/>
    public IMyCollection<T> Filter(Func<T, bool> predicate) => throw new NotImplementedException();

    /// <inheritdoc/>
    public void Sort(Comparison<T> comparison) => throw new NotImplementedException();

    /// <inheritdoc/>
    public TResult Reduce<TResult>(TResult initial, Func<TResult, T, TResult> accumulator) => throw new NotImplementedException();

    /// <inheritdoc/>
    public IMyIterator<T> GetIterator() => throw new NotImplementedException();

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">Index of the element to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    public void RemoveAt(int index) => throw new NotImplementedException();

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">Zero-based index.</param>
    /// <param name="value">The found value when successful.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when index is out of range.</exception>
    public bool TryGetAt(int index, out T? value) => throw new NotImplementedException();

    private void EnsureCapacity(int required) => throw new NotImplementedException();

    private int IndexOf(T item) => throw new NotImplementedException();
}