namespace Kanban.Domain.Enums;

/// <summary>
/// Task status / Kanban column.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// The task is still in the to-do list.
    /// </summary>
    ToDo,

    /// <summary>
    /// The task is in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// The task is done.
    /// </summary>
    Done
}