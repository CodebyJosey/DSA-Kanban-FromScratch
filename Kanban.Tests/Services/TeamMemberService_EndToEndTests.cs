using Kanban.Source.Classes.Dtos;
using Kanban.Source.Classes.Entities;
using Kanban.Source.Data.Json;
using Kanban.Source.Services;
using Kanban.Tests.Helpers;

namespace Kanban.Tests.Services;

/// <summary>
/// End-to-end tests for TeamMemberService using the real JSON repository.
/// </summary>
public sealed class TeamMemberService_EndToEndTests
{
    /// <summary>
    /// Checks if creating a member persists and reloads correctly.
    /// </summary>
    [Fact]
    public void CreateMember_PersistsAndReloads()
    {
        string filePath = TestFileHelper.CreateEmptyTempMemberJsonPath();

        JsonTeamMemberRepository repository = new JsonTeamMemberRepository(filePath);
        TeamMemberService service = new TeamMemberService(repository);

        TeamMember? created = service.Create(new CreateTeamMemberDto
        {
            Name = "Tester"
        });

        JsonTeamMemberRepository reloadedRepository = new JsonTeamMemberRepository(filePath);
        TeamMemberService reloadedService = new TeamMemberService(reloadedRepository);

        Assert.Equal(1, reloadedService.GetAll().Count);

        TeamMember? reloaded = reloadedService.FindById(created.Id);
        Assert.NotNull(reloaded);
        Assert.Equal("Tester", reloaded!.Name);
    }

    /// <summary>
    /// Creating multiple members should increment ids correctly.
    /// </summary>
    [Fact]
    public void CreateMultipleMembers_IncrementsIds()
    {
        string filePath = TestFileHelper.CreateEmptyTempMemberJsonPath();

        JsonTeamMemberRepository repository = new JsonTeamMemberRepository(filePath);
        TeamMemberService service = new TeamMemberService(repository);

        TeamMember? member1 = service.Create(new CreateTeamMemberDto { Name = "A" });
        TeamMember? member2 = service.Create(new CreateTeamMemberDto { Name = "B" });
        TeamMember? member3 = service.Create(new CreateTeamMemberDto { Name = "C" });

        Assert.Equal(1, member1.Id);
        Assert.Equal(2, member2.Id);
        Assert.Equal(3, member3.Id);
    }

    /// <summary>
    /// FindById should return null for unknown ids.
    /// </summary>
    [Fact]
    public void FindById_UnknownId_ReturnsNull()
    {
        string filePath = TestFileHelper.CreateEmptyTempMemberJsonPath();

        JsonTeamMemberRepository repository = new JsonTeamMemberRepository(filePath);
        TeamMemberService service = new TeamMemberService(repository);

        TeamMember? found = service.FindById(999);

        Assert.Null(found);
    }

    /// <summary>
    /// GetAll should reflect the correct number of members after reload.
    /// </summary>
    [Fact]
    public void GetAll_AfterReload_ReturnsCorrectCount()
    {
        string filePath = TestFileHelper.CreateEmptyTempMemberJsonPath();

        JsonTeamMemberRepository repository = new JsonTeamMemberRepository(filePath);
        TeamMemberService service = new TeamMemberService(repository);

        service.Create(new CreateTeamMemberDto { Name = "User1" });
        service.Create(new CreateTeamMemberDto { Name = "User2" });

        JsonTeamMemberRepository reloadedRepository = new JsonTeamMemberRepository(filePath);
        TeamMemberService reloadedService = new TeamMemberService(reloadedRepository);

        Assert.Equal(2, reloadedService.GetAll().Count);
    }
}