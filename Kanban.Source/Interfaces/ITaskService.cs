using Kanban.Source.Classes.Dtos;
using Kanban.Source.Classes.Entities;

namespace Kanban.Source.Interfaces;

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
    void MoveStatus(int id, Enums.TaskStatus status);

    /// <summary>
    /// Deletes a task.
    /// </summary>
    bool Delete(int id);

    /// <summary>
    /// Updates a task if the acting member has rights.
    /// </summary>
    void Update(int id, UpdateTaskDto dto, int actingMemberId);

    /// <summary>
    /// Moves a task status if the acting member has rights.
    /// </summary>
    void MoveStatus(int id, Enums.TaskStatus status, int actingMemberId);

    /// <summary>Deletes a task if the acting member has rights.</summary>
    bool Delete(int id, int actingMemberId);

    /// <summary>
    /// Assigns or unassigns a task to a team member.
    /// </summary>
    void Assign(AssignTaskDto dto);
}