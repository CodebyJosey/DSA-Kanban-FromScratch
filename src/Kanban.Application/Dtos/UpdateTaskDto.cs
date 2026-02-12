using Kanban.Domain.Enums;

namespace Kanban.Application.Dtos;

/// <summary>
/// Input model for updating an existing task.
/// </summary>
public sealed class UpdateTaskDto
{
    /// <summary>
    /// Optional new title. If null, the title will not be changed.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Optional new description. If null, the description will not be changed.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Optional new priority. If null, the priority will not be changed.
    /// </summary>
    public TaskPriority? Priority { get; init; }
}