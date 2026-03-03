namespace Kanban.Source.Classes.Dtos;

/// <summary>
/// DTO used to assign a task to a team member.
/// </summary>
public sealed class AssignTaskDto
{
    /// <summary>
    /// Gets or sets the task id to assign.
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// Gets or sets the member id to assign the task to.
    /// Use <c>null</c> to unassign.
    /// </summary>
    public int? MemberId { get; set; }
}