using Kanban.Source.Classes.Dtos;
using Kanban.Source.Classes.Entities;
using Kanban.Source.Interfaces;

namespace Kanban.Source.Services;

/// <summary>
/// Business logic for managing team members.
/// </summary>
public sealed class TeamMemberService
{
    private readonly ITeamMemberRepository _repo;
    private readonly IMyCollection<TeamMember> _members;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TeamMemberService(ITeamMemberRepository repo)
    {
        _repo = repo;
        _members = _repo.LoadAll();
    }

    /// <summary>
    /// Gets all team members.
    /// </summary>
    public IMyCollection<TeamMember> GetAll()
    {
        return _members;
    }

    /// <summary>
    /// Creates a new team member and persists the change.
    /// </summary>
    public TeamMember Create(CreateTeamMemberDto dto)
    {
        int newId = NextId();

        TeamMember member = new TeamMember
        {
            Id = newId,
            Name = dto.Name
        };

        _members.Add(member);
        _members.Dirty = true;
        _repo.SaveAll(_members);

        return member;
    }

    /// <summary>
    /// Finds a member by id.
    /// </summary>
    public TeamMember? FindById(int id)
    {
        return _members.FindBy(id, static (m, key) => m.Id == key);
    }

    private int NextId()
    {
        int max = 0;
        IMyIterator<TeamMember> it = _members.GetIterator();
        while (it.HasNext())
        {
            TeamMember m = it.Next();
            if (m.Id > max)
            {
                max = m.Id;
            }
        }

        return max + 1;
    }
}