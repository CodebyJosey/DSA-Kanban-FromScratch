using Kanban.Source.Classes.Dtos;
using Kanban.Source.Classes.Entities;
using Kanban.Source.Enums;
using Kanban.Source.Interfaces;
using Kanban.Source.Services;
using Kanban.Source.UI.Views;
using Spectre.Console;

namespace Kanban.Source.UI.Menus;

/// <summary>
/// Main menu for interacting with tasks using Spectre.Console.
/// </summary>
public sealed class MainMenu
{
    private readonly ITaskService _tasks;
    private readonly TeamMemberService _members;

    private int _activeMemberId;

    /// <summary>Creates a new instance.</summary>
    public MainMenu(ITaskService tasks, TeamMemberService members)
    {
        _tasks = tasks;
        _members = members;

        // 0 means "Guest" (no member). Unassigned tasks can still be modified.
        _activeMemberId = 0;
    }

    /// <summary>Runs the main menu loop.</summary>
    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]Kanban[/]").RuleStyle("grey").LeftJustified());
            AnsiConsole.MarkupLine($"[grey]Active member:[/] [yellow]{Markup.Escape(GetActiveMemberLabel())}[/]\n");

            // Use an array (no List<T>)
            string action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose an [green]action[/]:")
                    .AddChoices(new[]
                    {
                        "Board view",
                        "List tasks",
                        "Add task",
                        "Update task",
                        "Move task status",
                        "Delete task",
                        "Assign task",
                        "Add team member",
                        "Switch active member",
                        "Exit"
                    }));

            switch (action)
            {
                case "Board view":
                    ShowBoard();
                    break;

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

                case "Assign task":
                    AssignTask();
                    break;

                case "Add team member":
                    AddTeamMember();
                    break;

                case "Switch active member":
                    SwitchActiveMember();
                    break;

                case "Exit":
                    return;
            }
        }
    }

    /// <summary>
    /// Shows a Kanban board view grouped by status.
    /// </summary>
    private void ShowBoard()
    {
        // Board view handles its own clear + pause
        KanbanBoardView.Show(_tasks.GetAll());
    }

    private void ShowTasks()
    {
        IMyCollection<TaskItem> tasks = _tasks.GetAll();

        Table table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[deepskyblue1]Tasks[/]");

        table.AddColumn("Id");
        table.AddColumn("Title");
        table.AddColumn("Status");
        table.AddColumn("Priority");
        table.AddColumn("Assigned");

        IMyIterator<TaskItem> iterator = tasks.GetIterator();
        while (iterator.HasNext())
        {
            TaskItem t = iterator.Next();
            table.AddRow(
                t.Id.ToString(),
                Markup.Escape(t.Title),
                t.Status.ToString(),
                t.Priority.ToString(),
                FormatAssigned(t)
            );
        }

        AnsiConsole.Write(table);
    }

    private void AddTask()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Add Task[/]");

        string title = AnsiConsole.Ask<string>("Title:");
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
            newDescription = null;

        bool changePriority = AnsiConsole.Confirm("Change priority?", defaultValue: false);
        TaskPriority? newPriority = null;

        if (changePriority)
        {
            newPriority = AnsiConsole.Prompt(
                new SelectionPrompt<TaskPriority>()
                    .Title("New priority:")
                    .AddChoices(new[] { TaskPriority.Low, TaskPriority.Normal, TaskPriority.High }));
        }

        try
        {
            _tasks.Update(id, new UpdateTaskDto
            {
                Title = newTitle,
                Description = newDescription,
                Priority = newPriority
            }, _activeMemberId);

            AnsiConsole.MarkupLine("[green]Task updated.[/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
        }

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

        try
        {
            _tasks.MoveStatus(id, status, _activeMemberId);
            AnsiConsole.MarkupLine("[green]Status updated.[/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
        }

        Pause();
    }

    private void DeleteTask()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[red]Delete Task[/]");

        int id = AnsiConsole.Ask<int>("Task Id:");
        bool confirm = AnsiConsole.Confirm($"Are you sure you want to delete task [yellow]{id}[/]?", defaultValue: false);

        if (!confirm)
            return;

        try
        {
            bool deleted = _tasks.Delete(id, _activeMemberId);
            AnsiConsole.MarkupLine(deleted ? "[green]Task deleted.[/]" : "[yellow]Task not found.[/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
        }

        Pause();
    }

    /// <summary>
    /// Assigns or unassigns a task to a team member.
    /// </summary>
    private void AssignTask()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[purple]Assign Task[/]");

        int taskId = AnsiConsole.Ask<int>("Task Id:");

        IMyCollection<TeamMember> members = _members.GetAll();
        if (members.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No team members exist yet. Create one first.[/]");
            Pause();
            return;
        }

        // Build member choices as a string array
        string[] choices = new string[members.Count + 1];
        choices[0] = "(Unassign)";

        int idx = 1;
        IMyIterator<TeamMember> it = members.GetIterator();
        while (it.HasNext())
        {
            TeamMember m = it.Next();
            choices[idx++] = $"#{m.Id} - {m.Name}";
        }

        string selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Assign to:")
                .AddChoices(choices));

        try
        {
            if (selected == "(Unassign)")
            {
                _tasks.Assign(new AssignTaskDto
                {
                    TaskId = taskId,
                    MemberId = null
                });

                AnsiConsole.MarkupLine($"[green]Task[/] [yellow]#{taskId}[/] [green]is now unassigned.[/]");
                Pause();
                return;
            }

            int memberId = ParseIdFromChoice(selected);
            _tasks.Assign(new AssignTaskDto
            {
                TaskId = taskId,
                MemberId = memberId
            });
            AnsiConsole.MarkupLine($"[green]Task[/] [yellow]#{taskId}[/] [green]assigned to member[/] [yellow]#{memberId}[/].");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
        }

        Pause();
    }

    /// <summary>
    /// Adds a new team member.
    /// </summary>
    private void AddTeamMember()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Add Team Member[/]");

        string name = AnsiConsole.Ask<string>("Name:").Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            AnsiConsole.MarkupLine("[yellow]Name cannot be empty.[/]");
            Pause();
            return;
        }

        TeamMember created = _members.Create(new CreateTeamMemberDto { Name = name });

        AnsiConsole.MarkupLine($"[green]Created member[/] [yellow]#{created.Id}[/] [bold]{Markup.Escape(created.Name)}[/].");
        Pause();
    }

    /// <summary>
    /// Switches the active member used for enforcing modification rights.
    /// </summary>
    private void SwitchActiveMember()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[blue]Switch Active Member[/]");

        IMyCollection<TeamMember> all = _members.GetAll();

        if (all.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No team members exist yet. Create one first.[/]");
            Pause();
            return;
        }

        string[] choices = new string[all.Count + 1];
        choices[0] = "Guest (no member)";

        int idx = 1;
        IMyIterator<TeamMember> it = all.GetIterator();
        while (it.HasNext())
        {
            TeamMember m = it.Next();
            choices[idx++] = $"#{m.Id} - {m.Name}";
        }

        string selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select active member:")
                .AddChoices(choices));

        if (selected == "Guest (no member)")
        {
            _activeMemberId = 0;
            AnsiConsole.MarkupLine("[green]Active member set to Guest.[/]");
            Pause();
            return;
        }

        try
        {
            _activeMemberId = ParseIdFromChoice(selected);
            AnsiConsole.MarkupLine($"[green]Active member set to[/] [yellow]#{_activeMemberId}[/].");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
        }

        Pause();
    }

    /// <summary>
    /// Formats assignment info for the tasks table.
    /// </summary>
    /// <param name="task">The task to format.</param>
    /// <returns>A markup-safe string representing assignment.</returns>
    private string FormatAssigned(TaskItem task)
    {
        if (task.AssignedToMemberId is null)
            return "[grey](unassigned)[/]";

        TeamMember? m = _members.FindById(task.AssignedToMemberId.Value);
        if (m is null)
            return $"[yellow]#{task.AssignedToMemberId.Value}[/]";

        return $"[yellow]#{m.Id}[/] {Markup.Escape(m.Name)}";
    }

    /// <summary>
    /// Returns a readable label for the currently active member.
    /// </summary>
    /// <returns>A readable label.</returns>
    private string GetActiveMemberLabel()
    {
        if (_activeMemberId == 0)
            return "Guest";

        TeamMember? m = _members.FindById(_activeMemberId);
        return m is null ? $"#{_activeMemberId}" : $"#{m.Id} - {m.Name}";
    }

    /// <summary>
    /// Parses an integer id from a selection choice in the format "#&lt;id&gt; - &lt;name&gt;".
    /// </summary>
    /// <param name="choice">The choice string.</param>
    /// <returns>The parsed id.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the id cannot be parsed.</exception>
    private static int ParseIdFromChoice(string choice)
    {
        // Expected: "#12 - Josey"
        int dash = choice.IndexOf(" - ", StringComparison.Ordinal);
        string idPart = dash > 0 ? choice.Substring(1, dash - 1) : choice.Substring(1);

        if (!int.TryParse(idPart, out int id))
            throw new InvalidOperationException("Could not parse the selected id.");

        return id;
    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}