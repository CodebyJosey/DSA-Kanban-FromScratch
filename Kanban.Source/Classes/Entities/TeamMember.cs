namespace Kanban.Source.Classes.Entities;

/// <summary>
/// Represents a team member that tasks can be assigned to.
/// </summary>
public sealed class TeamMember
{
    /// <summary>
    /// Gets or sets the unique identifier of the team member.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the displayname of the team member.
    /// </summary>
    public required string Name { get; set; }
}