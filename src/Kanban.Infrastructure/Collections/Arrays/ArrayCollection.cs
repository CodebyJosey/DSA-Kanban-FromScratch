using Kanban.Domain.Abstractions.Collections;

namespace Kanban.Infrastructure.Collections.Arrays;

/// <summary>
/// Array-backed collection implementation (to be implemented in sprint 1).
/// </summary>
public sealed class ArrayCollection<T> : IMyCollection<T>
{
    /// <inheritdoc/>
    public int Count => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool Dirty { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <inheritdoc/>
    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public T? FindBy<TKey>(TKey key, Func<T, TKey, bool> comparer)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Sort(Comparison<T> comparison)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public TResult Reduce<TResult>(TResult initial, Func<TResult, T, TResult> accumulator)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IMyIterator<T> GetIterator()
    {
        throw new NotImplementedException();
    }
}