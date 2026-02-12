namespace Kanban.Domain.Entities;

/// <summary>
/// Represents a task in the Kanban system.
/// </summary>
public sealed class TaskItem
{
    /// <summary>
    /// Task id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Task title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Optional description for the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Task status (column).
    /// </summary>
    public Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// Task priority.
    /// </summary>
    public Enums.TaskPriority Priority { get; set; }

    /// <summary>
    /// Creation time (UTC).
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; }
}