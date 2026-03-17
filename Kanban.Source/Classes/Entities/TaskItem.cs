namespace Kanban.Source.Classes.Entities;

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
    /// Gets or sets the team member id this task is assigned to.
    /// If <c>null</c>, the task is unassigned.
    /// </summary>
    public int? AssignedToMemberId { get; set; }

    /// <summary>
    /// Optional prerequisite task id that must be completed first.
    /// If <c>null</c>, this task has no dependency.
    /// </summary>
    public int? DependsOnTaskId { get; set; }

    /// <summary>
    /// Indicates whether this task is currently locked by an unfinished dependency.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Creation time (UTC).
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; }
}