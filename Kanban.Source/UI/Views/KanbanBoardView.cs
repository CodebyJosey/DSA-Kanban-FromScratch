using Kanban.Source.Classes.Entities;
using Kanban.Source.Enums;
using Kanban.Source.Interfaces;
using Spectre.Console;

namespace Kanban.Source.UI.Views;

/// <summary>
/// Represents a console-based Kanban board view using Spectre.Console.
/// </summary>
/// <remarks>
/// This view is fully generic with respect to the underlying collection type.
/// It only depends on <see cref="IMyCollection{T}"/> and therefore works with
/// any custom collection implementation (e.g., ArrayCollection, LinkedListCollection).
/// </remarks>
public static class KanbanBoardView
{
    /// <summary>
    /// Renders the Kanban board grouped by <see cref="TaskStatus"/>.
    /// </summary>
    /// <param name="allTasks">
    /// The complete collection of tasks to display.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="allTasks"/> is <c>null</c>.
    /// </exception>
    public static void Show(IMyCollection<TaskItem> allTasks)
    {
        if (allTasks is null)
            throw new ArgumentNullException(nameof(allTasks));

        // Use custom Filter method — remains collection-agnostic
        IMyCollection<TaskItem> todo =
            allTasks.Filter(t => t.Status == Enums.TaskStatus.ToDo);

        IMyCollection<TaskItem> inProgress =
            allTasks.Filter(t => t.Status == Enums.TaskStatus.InProgress);

        IMyCollection<TaskItem> done =
            allTasks.Filter(t => t.Status == Enums.TaskStatus.Done);

        Render(allTasks, todo, inProgress, done);
    }

    /// <summary>
    /// Renders the three Kanban columns.
    /// </summary>
    /// <param name="allTasks">All tasks used for dependency flow rendering.</param>
    /// <param name="todo">Tasks in ToDo status.</param>
    /// <param name="inProgress">Tasks in InProgress status.</param>
    /// <param name="done">Tasks in Done status.</param>
    private static void Render(
        IMyCollection<TaskItem> allTasks,
        IMyCollection<TaskItem> todo,
        IMyCollection<TaskItem> inProgress,
        IMyCollection<TaskItem> done)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[bold deepskyblue1]Kanban Board[/]").RuleStyle("grey").LeftJustified());
        AnsiConsole.WriteLine($"[grey]Total:[/] [white]{allTasks.Count}[/]  [grey]ToDo:[/] [yellow]{todo.Count}[/]  [grey]InProgress:[/] [deepskyblue1]{inProgress.Count}[/]  [grey]Done:[/] [green]{done.Count}[/]");
        AnsiConsole.WriteLine();

        Table table = new Table()
            .Expand()
            .Border(TableBorder.Rounded);

        table.AddColumn(new TableColumn("[bold yellow]ToDo[/]").Centered());
        table.AddColumn(new TableColumn("[bold deepskyblue1]In Progress[/]").Centered());
        table.AddColumn(new TableColumn("[bold green]Done[/]").Centered());

        table.AddRow(
            new Panel(new Markup(RenderColumn(todo, allTasks))).Header("[yellow]ToDo[/]").BorderColor(Color.Yellow),
            new Panel(new Markup(RenderColumn(inProgress, allTasks))).Header("[deepskyblue1]In Progress[/]").BorderColor(Color.DeepSkyBlue1),
            new Panel(new Markup(RenderColumn(done, allTasks))).Header("[green]Done[/]").BorderColor(Color.Green)
        );

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Legend:[/] [red][[LOCKED]][/], [green][[READY]][/], [orange3]Flow:[/] #task <- #dependency");
        AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Converts a task collection into formatted markup text
    /// for rendering inside a board column.
    /// </summary>
    /// <param name="tasks">The tasks to render.</param>
    /// <param name="allTasks">The full task set used to draw dependency flow chains.</param>
    /// <returns>
    /// A formatted markup string suitable for Spectre.Console.
    /// </returns>
    private static string RenderColumn(IMyCollection<TaskItem> tasks, IMyCollection<TaskItem> allTasks)
    {
        if (tasks.Count == 0)
            return "[grey](empty)[/]";

        // Use Reduce to build output without generic collections
        string result = tasks.Reduce(
            initial: string.Empty,
            accumulator: (acc, task) =>
            {
                string prioColor = GetPriorityColor(task.Priority);

                string line =
                    $"[bold]#{task.Id}[/] {GetLockBadge(task)} {GetPriorityBadge(task.Priority)} " +
                    $"[white]{Markup.Escape(task.Title)}[/]";

                if (task.DependsOnTaskId.HasValue)
                {
                    line += $"\n[grey]Flow:[/] {BuildDependencyFlow(task, allTasks)}";
                }

                if (string.IsNullOrEmpty(acc))
                    return line;

                return acc + "\n\n" + line;
            });

        return result;
    }

    /// <summary>
    /// Determines the Spectre.Console color associated with a priority level.
    /// </summary>
    /// <param name="priority">The task priority.</param>
    /// <returns>A markup color string.</returns>
    private static string GetPriorityColor(TaskPriority priority)
    {
        return priority switch
        {
            TaskPriority.Low => "grey",
            TaskPriority.Normal => "orange3",
            TaskPriority.High => "red",
            _ => "white"
        };
    }

    private static string GetLockBadge(TaskItem task)
    {
        return task.IsLocked ? "[red][[LOCKED]][/]" : "[green][[READY]][/]";
    }

    private static string GetPriorityBadge(TaskPriority priority)
    {
        string color = GetPriorityColor(priority);
        return $"[{color}][[{Markup.Escape(priority.ToString().ToUpperInvariant())}]][/]";
    }

    private static string BuildDependencyFlow(TaskItem task, IMyCollection<TaskItem> allTasks)
    {
        string flow = $"[white]#{task.Id}[/]";
        int? current = task.DependsOnTaskId;
        int hops = 0;

        while (current.HasValue && hops < 12)
        {
            TaskItem? dependency = FindTaskById(allTasks, current.Value);
            if (dependency == null)
            {
                flow += $" [grey]<-[/] [red]#{current.Value}?[/]";
                return flow;
            }

            flow += $" [grey]<-[/] [yellow]#{dependency.Id}[/]";
            current = dependency.DependsOnTaskId;
            hops++;
        }

        if (hops >= 12 && current.HasValue)
        {
            flow += " [grey]<- ...[/]";
        }

        return flow;
    }

    private static TaskItem? FindTaskById(IMyCollection<TaskItem> allTasks, int id)
    {
        IMyIterator<TaskItem> iterator = allTasks.GetIterator();
        while (iterator.HasNext())
        {
            TaskItem task = iterator.Next();
            if (task.Id == id)
            {
                return task;
            }
        }

        return null;
    }
}