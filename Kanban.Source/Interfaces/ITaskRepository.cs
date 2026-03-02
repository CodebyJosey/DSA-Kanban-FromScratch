using Kanban.Source.Classes.Entities;

namespace Kanban.Source.Interfaces;

/// <summary>
/// Task-specific repository abstraction (extension point for later).
/// </summary>
public interface ITaskRepository : IRepository<TaskItem>
{
    // Add task-specific queries if needed (e.g., GetByStatus, GetByAssignee)
}