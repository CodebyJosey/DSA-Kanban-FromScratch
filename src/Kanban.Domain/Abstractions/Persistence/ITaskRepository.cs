using Kanban.Domain.Entities;

namespace Kanban.Domain.Abstractions.Persistence;

/// <summary>
/// Task-specific repository abstraction (extension point for later).
/// </summary>
public interface ITaskRepository : IRepository<TaskItem>
{
    // Add task-specific queries if needed (e.g., GetByStatus, GetByAssignee)
}