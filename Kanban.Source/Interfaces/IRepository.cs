namespace Kanban.Source.Interfaces;

/// <summary>
/// Generic repository abstraction using custom collections.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IRepository<T>
{
    /// <summary>
    /// Gets all the entities.
    /// </summary>
    IMyCollection<T> GetAll();

    /// <summary>
    /// Gets an entity by integer id.
    /// </summary>
    T? GetById(int id);

    /// <summary>
    /// Adds an entity.
    /// </summary>
    void Add(T entity);

    /// <summary>
    /// Updates an entity.
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Deletes an entity by id.
    /// </summary>
    bool DeleteById(int id);

    /// <summary>
    /// Saves changes (persistence boundary).
    /// </summary>
    void SaveChanges();
}