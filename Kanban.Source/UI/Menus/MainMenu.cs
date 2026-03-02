using Kanban.Source.Classes.Dtos;
using Kanban.Source.Classes.Entities;
using Kanban.Source.Enums;
using Kanban.Source.Interfaces;
using Spectre.Console;

namespace Kanban.Source.UI.Menus;

/// <summary>
/// Main menu for interacting with tasks using Spectre.Console.
/// </summary>
public sealed class MainMenu
{
    private readonly ITaskService _tasks;

    /// <summary>Creates a new instance.</summary>
    public MainMenu(ITaskService tasks)
    {
        _tasks = tasks;
    }

    /// <summary>Runs the main menu loop.</summary>
    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Kanban[/]").RuleStyle("grey").LeftJustified());

            // Gebruik een array i.p.v. List
            string? action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose an [green]action[/]:")
                    .AddChoices(new[]
                    {
                        "List tasks",
                        "Add task",
                        "Update task",
                        "Move task status",
                        "Delete task",
                        "Exit"
                    }));

            switch (action)
            {
                case "List tasks":
                    ShowTasks();
                    Pause();
                    break;

                case "Add task":
                    AddTask();
                    break;

                case "Update task":
                    UpdateTask();
                    break;

                case "Move task status":
                    MoveTaskStatus();
                    break;

                case "Delete task":
                    DeleteTask();
                    break;

                case "Exit":
                    return;
            }
        }
    }

    private void ShowTasks()
    {
        IMyCollection<TaskItem>? tasks = _tasks.GetAll();

        Table? table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[deepskyblue1]Tasks[/]");

        table.AddColumn("Id");
        table.AddColumn("Title");
        table.AddColumn("Status");
        table.AddColumn("Priority");

        IMyIterator<TaskItem>? iterator = tasks.GetIterator();
        while (iterator.HasNext())
        {
            TaskItem? t = iterator.Next();
            table.AddRow(
                t.Id.ToString(),
                t.Title,
                t.Status.ToString(),
                t.Priority.ToString()
            );
        }

        AnsiConsole.Write(table);
    }

    private void AddTask()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Add Task[/]");

        string? title = AnsiConsole.Ask<string>("Title:");
        string? description = AnsiConsole.Ask<string?>("Description (optional):", defaultValue: null);

        TaskPriority priority = AnsiConsole.Prompt(
            new SelectionPrompt<TaskPriority>()
                .Title("Priority:")
                .AddChoices(new[] { TaskPriority.Low, TaskPriority.Normal, TaskPriority.High }));

        _tasks.Create(new CreateTaskDto
        {
            Title = title,
            Description = description,
            Priority = priority
        });

        AnsiConsole.MarkupLine("[green]Task created.[/]");
        Pause();
    }

    private void UpdateTask()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[yellow]Update Task[/]");

        int id = AnsiConsole.Ask<int>("Task Id:");

        string? newTitle = AnsiConsole.Ask<string?>("New title (leave empty to skip):", defaultValue: null);
        if (string.IsNullOrWhiteSpace(newTitle))
            newTitle = null;

        string? newDescription = AnsiConsole.Ask<string?>("New description (leave empty to skip):", defaultValue: null);
        if (string.IsNullOrWhiteSpace(newDescription))
        {
            newDescription = null;
        }

        bool changePriority = AnsiConsole.Confirm("Change priority?", defaultValue: false);
        TaskPriority? newPriority = null;

        if (changePriority)
        {
            newPriority = AnsiConsole.Prompt(
                new SelectionPrompt<TaskPriority>()
                    .Title("New priority:")
                    .AddChoices(new[] { TaskPriority.Low, TaskPriority.Normal, TaskPriority.High }));
        }

        _tasks.Update(id, new UpdateTaskDto
        {
            Title = newTitle,
            Description = newDescription,
            Priority = newPriority
        });

        AnsiConsole.MarkupLine("[green]Task updated.[/]");
        Pause();
    }

    private void MoveTaskStatus()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[blue]Move Task Status[/]");

        int id = AnsiConsole.Ask<int>("Task Id:");

        Enums.TaskStatus status = AnsiConsole.Prompt(
            new SelectionPrompt<Enums.TaskStatus>()
                .Title("New status:")
                .AddChoices(new[] { Enums.TaskStatus.ToDo, Enums.TaskStatus.InProgress, Enums.TaskStatus.Done }));

        _tasks.MoveStatus(id, status);

        AnsiConsole.MarkupLine("[green]Status updated.[/]");
        Pause();
    }

    private void DeleteTask()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[red]Delete Task[/]");

        int id = AnsiConsole.Ask<int>("Task Id:");
        bool confirm = AnsiConsole.Confirm($"Are you sure you want to delete task [yellow]{id}[/]?", defaultValue: false);

        if (!confirm) return;

        bool deleted = _tasks.Delete(id);

        AnsiConsole.MarkupLine(deleted ? "[green]Task deleted.[/]" : "[yellow]Task not found.[/]");
        Pause();
    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}