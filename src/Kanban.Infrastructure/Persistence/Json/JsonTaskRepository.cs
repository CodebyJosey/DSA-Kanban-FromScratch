using Kanban.Domain.Abstractions.Collections;
using Kanban.Domain.Abstractions.Persistence;
using Kanban.Domain.Entities;

namespace Kanban.Infrastructure.Persistence.Json;

/// <summary>
/// JSON persistence implementation (to be implemented after ArrayCollection works).
/// </summary>
public sealed class JsonTaskRepository : ITaskRepository
{
    /// <inheritdoc/>
    public IMyCollection<TaskItem> GetAll()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public TaskItem? GetById(int id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Add(TaskItem entity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Update(TaskItem entity)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool DeleteById(int id)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SaveChanges()
    {
        throw new NotImplementedException();
    }
}