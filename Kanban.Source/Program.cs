using Kanban.Source.Collections;
using Kanban.Source.Data.Json;
using Kanban.Source.Enums;
using Kanban.Source.Interfaces;
using Kanban.Source.Services;
using Kanban.Source.UI.Menus;
using Spectre.Console;

namespace Kanban.Source;

/// <summary>
/// Application entry point.
/// </summary>
public static class Program
{
    /// <summary>Main method.</summary>
    public static void Main()
    {
        CollectionImplementation implementation = AskCollectionImplementation();
        IMyCollectionFactory collectionFactory = new MyCollectionFactory(implementation);

        ITaskRepository repo = new JsonTaskRepository("Kanban.Source/Data/tasks.json", collectionFactory);
        IClock clock = new SystemClock();
        ITaskService service = new TaskService(repo, clock, collectionFactory);
        ITeamMemberRepository memberRepo = new JsonTeamMemberRepository("Kanban.Source/Data/members.json", collectionFactory);
        TeamMemberService members = new TeamMemberService(memberRepo);

        MainMenu? menu = new MainMenu(service, members);
        menu.Run();
    }

    private static CollectionImplementation AskCollectionImplementation()
    {
        string selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select [green]collection implementation[/] for app runtime:")
                .AddChoices(new[]
                {
                    "Array",
                    "LinkedList",
                    "BinarySearchTree",
                    "HashMap"
                }));

        return selected switch
        {
            "LinkedList" => CollectionImplementation.LinkedList,
            "BinarySearchTree" => CollectionImplementation.BinarySearchTree,
            "HashMap" => CollectionImplementation.HashMap,
            _ => CollectionImplementation.Array
        };
    }
}