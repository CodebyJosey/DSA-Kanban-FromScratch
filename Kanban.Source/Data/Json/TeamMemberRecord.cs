namespace Kanban.Source.Data.Json;

/// <summary>
/// Serializable record for persisting team members.
/// </summary>
public sealed class TeamMemberRecord
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string? Name { get; set; }
}