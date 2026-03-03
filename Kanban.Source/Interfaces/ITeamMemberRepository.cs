using Kanban.Source.Classes.Entities;

namespace Kanban.Source.Interfaces;

/// <summary>
/// Defines persistence operations for team members.
/// </summary>
public interface ITeamMemberRepository
{
    /// <summary>
    /// Loads all the team members.
    /// </summary>
    IMyCollection<TeamMember> LoadAll();

    /// <summary>
    /// Saves all the team members.
    /// </summary>
    void SaveAll(IMyCollection<TeamMember> members);
}