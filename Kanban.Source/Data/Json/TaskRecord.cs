using Kanban.Source.Enums;

namespace Kanban.Source.Data.Json;

/// <summary>
/// Persistence model used for JSON storage.
/// </summary>
public sealed class TaskRecord
{
    /// <summary>
    /// The id of the task record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The title for the task record.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// The (optional) description for the task record.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The status of the task record.
    /// </summary>
    public Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// The priority for the task record.
    /// </summary>
    public TaskPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets the assigned member id.
    /// </summary>
    public int? AssignedToMemberId { get; set; }

    /// <summary>
    /// Optional prerequisite task id.
    /// </summary>
    public int? DependsOnTaskId { get; set; }

    /// <summary>
    /// Indicates whether the task is currently locked.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// The time (UTC) when the task record was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; }
}