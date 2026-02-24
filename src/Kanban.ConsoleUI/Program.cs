using Kanban.Application.Abstractions.Services;
using Kanban.Application.Services;
using Kanban.Domain.Abstractions.Persistence;
using Kanban.Infrastructure.Persistence.Json;
using Kanban.ConsoleUI.Menus;
using Kanban.Domain.Abstractions.Services;
using Kanban.Infrastructure.Services;

namespace Kanban.ConsoleUI;

/// <summary>
/// Application entry point.
/// </summary>
public static class Program
{
    /// <summary>Main method.</summary>
    public static void Main()
    {
        // Let op: JsonTaskRepository moet jij/je team (straks) implementeren.
        ITaskRepository repo = new JsonTaskRepository("src/Kanban.Infrastructure/Data/tasks.json");
        IClock clock = new SystemClock();
        ITaskService service = new TaskService(repo, clock);

        MainMenu? menu = new MainMenu(service);
        menu.Run();
    }
}