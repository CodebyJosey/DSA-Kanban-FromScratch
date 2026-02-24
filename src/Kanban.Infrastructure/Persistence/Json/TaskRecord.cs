using Kanban.Domain.Enums;

namespace Kanban.Infrastructure.Persistence.Json;

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
    public Domain.Enums.TaskStatus Status { get; set; }

    /// <summary>
    /// The priority for the task record.
    /// </summary>
    public TaskPriority Priority { get; set; }

    /// <summary>
    /// The time (UTC) when the task record was created.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; set; }
}