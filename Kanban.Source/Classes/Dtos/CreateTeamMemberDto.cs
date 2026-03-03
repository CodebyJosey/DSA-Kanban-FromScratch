namespace Kanban.Source.Classes.Dtos;

/// <summary>
/// DTO used to create a new team member.
/// </summary>
public sealed class CreateTeamMemberDto
{
    /// <summary>
    /// Gets or sets the name of the member.
    /// </summary>
    public required string Name { get; set; }
}