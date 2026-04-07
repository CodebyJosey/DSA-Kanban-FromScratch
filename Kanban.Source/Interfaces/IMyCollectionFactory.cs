namespace Kanban.Source.Interfaces;

/// <summary>
/// Creates <see cref="IMyCollection{T}"/> instances for a selected implementation.
/// </summary>
public interface IMyCollectionFactory
{
    /// <summary>
    /// Creates a new collection for type <typeparamref name="T"/>.
    /// </summary>
    IMyCollection<T> Create<T>();
}