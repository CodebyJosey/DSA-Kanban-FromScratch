using Kanban.Source.Enums;

namespace Kanban.Source.Classes.Dtos;

/// <summary>
/// Input model for creating a new task.
/// </summary>
public sealed class CreateTaskDto
{
    /// <summary>
    /// Title of the task.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Optional description of the task.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Priority of the task.
    /// </summary>
    public TaskPriority Priority { get; init; } = TaskPriority.Normal;

    /// <summary>
    /// Optional prerequisite task id that must be done before this task can be started.
    /// </summary>
    public int? DependsOnTaskId { get; init; }
}