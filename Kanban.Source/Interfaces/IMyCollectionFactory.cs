namespace Kanban.Source.Interfaces;
using Kanban.Source.Collections.HashMap;

/// <summary>
/// Creates <see cref="IMyCollection{T}"/> instances for a selected implementation.
/// </summary>
public interface IMyCollectionFactory
{
    /// <summary>
    /// Creates a new collection for type <typeparamref name="T"/>.
    /// </summary>
    IMyCollection<T> Create<T>();

    /// <summary>
    /// returns hashmap collection
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <returns></returns>
    public IMyCollection<KeyValue<K, V>> CreateHashMap<K, V>();
}