using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.Arrays;

/// <summary>
/// Singly linked list implementation of <see cref="IHashMap{TKey, TValue}"/>.
/// This class does not rely on System.Collections.Generic.
/// </summary>
/// <typeparam name="TKey">Element type.</typeparam>
/// <typeparam name="TValue">Element type.</typeparam>
public class HashmapCollection<TKey, TValue> : IHashMap<TKey, TValue>
{
    private (TKey, TValue)[] _items;
    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="HashmapCollection{TKey, TValue}"/> class.
    /// </summary>
    public HashmapCollection()
    {
        _items = new (TKey, TValue)[0];
        _count = 0;
        Dirty = false;
    }
    /// <inheritdoc/>
    public int Count => _count;
    /// <inheritdoc/>
    public bool Dirty {get; set;}

    /// <inheritdoc/>
    public IEnumerable<TKey> GetKeys() => throw new NotImplementedException();
    /// <inheritdoc/>
    public IEnumerable<TValue> GetValues() => throw new NotImplementedException();
    /// <inheritdoc/>
    public void Add(TKey key, TValue value) => throw new NotImplementedException();
    /// <inheritdoc/>
    public bool Remove(TKey key) => throw new NotImplementedException();
    /// <inheritdoc/>
    public IHashMap<TKey, TValue> Filter(Func<TValue, bool> predicate) => throw new NotImplementedException();
    /// <inheritdoc/>
    public IMyIterator<(TKey, TValue)> GetIterator() => throw new NotImplementedException();
}