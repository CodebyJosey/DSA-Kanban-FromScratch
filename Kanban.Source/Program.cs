using Kanban.Source.Data.Json;
using Kanban.Source.Interfaces;
using Kanban.Source.Services;
using Kanban.Source.UI.Menus;

namespace Kanban.Source;

/// <summary>
/// Application entry point.
/// </summary>
public static class Program
{
    /// <summary>Main method.</summary>
    public static void Main()
    {
        // Let op: JsonTaskRepository moet jij/je team (straks) implementeren.
        ITaskRepository repo = new JsonTaskRepository("Kanban.Source/Data/tasks.json");
        IClock clock = new SystemClock();
        ITaskService service = new TaskService(repo, clock);
        ITeamMemberRepository memberRepo = new JsonTeamMemberRepository("Kanban.Source/Data/members.json");
        TeamMemberService members = new TeamMemberService(memberRepo);

        MainMenu? menu = new MainMenu(service, members);
        menu.Run();
    }
}