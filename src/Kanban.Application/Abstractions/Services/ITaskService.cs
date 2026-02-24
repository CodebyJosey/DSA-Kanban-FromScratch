using Kanban.Application.Dtos;
using Kanban.Domain.Abstractions.Collections;
using Kanban.Domain.Entities;

namespace Kanban.Application.Abstractions.Services;

/// <summary>
/// Use-case boundary for task operations.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Gets all tasks.
    /// </summary>
    IMyCollection<TaskItem> GetAll();

    /// <summary>
    /// Creates a task.
    /// </summary>
    TaskItem Create(CreateTaskDto dto);

    /// <summary>
    /// Updates a task.
    /// </summary>
    void Update(int id, UpdateTaskDto dto);

    /// <summary>
    /// Moves a task to a new status.
    /// </summary>
    void MoveStatus(int id, Domain.Enums.TaskStatus status);

    /// <summary>
    /// Deletes a task.
    /// </summary>
    bool Delete(int id);
}