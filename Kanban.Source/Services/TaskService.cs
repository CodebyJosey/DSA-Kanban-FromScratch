

using Kanban.Source.Classes.Dtos;
using Kanban.Source.Classes.Entities;
using Kanban.Source.Collections.LinkedLists;
using Kanban.Source.Interfaces;

namespace Kanban.Source.Services;

/// <summary>
/// Task service (business logic).
/// </summary>
public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
    private readonly IClock _clock;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public TaskService(ITaskRepository repository, IClock clock)
    {
        _repo = repository;
        _clock = clock;
    }

    /// <inheritdoc/>
    public IMyCollection<TaskItem> GetAll()
    {
        return _repo.GetAll();
    }

    /// <inheritdoc/>
    public TaskItem Create(CreateTaskDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.Title);

        TaskItem? task = new TaskItem
        {
            Title = dto.Title.Trim(),
            Description = dto.Description,
            Priority = dto.Priority,
            Status = Enums.TaskStatus.ToDo,
            DependsOnTaskId = dto.DependsOnTaskId,
            IsLocked = false,
            CreatedAtUtc = _clock.UtcNow
        };

        if (dto.DependsOnTaskId.HasValue)
        {
            TaskItem? prerequisite = _repo.GetById(dto.DependsOnTaskId.Value);
            if (prerequisite == null)
            {
                throw new InvalidOperationException($"Dependency task with id {dto.DependsOnTaskId.Value} was not found.");
            }

            task.IsLocked = prerequisite.Status != Enums.TaskStatus.Done;
        }

        _repo.Add(task);
        _repo.SaveChanges();

        return task;
    }

    /// <inheritdoc/>
    public void Update(int id, UpdateTaskDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        TaskItem? task = _repo.GetById(id);
        if (task == null)
        {
            throw new InvalidOperationException($"Task with id {id} was not found.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            task.Title = dto.Title.Trim();
        }
        if (dto.Description != null)
        {
            task.Description = dto.Description;
        }
        if (dto.Priority.HasValue)
        {
            task.Priority = dto.Priority.Value;
        }

        _repo.Update(task);
        _repo.SaveChanges();
    }

    /// <inheritdoc/>
    public void Update(int id, UpdateTaskDto dto, int actingMemberId)
    {
        ArgumentNullException.ThrowIfNull(dto);

        TaskItem? task = _repo.GetById(id);
        if (task == null)
        {
            throw new InvalidOperationException($"Task with id {id} was not found.");
        }

        EnsureCanModify(task, actingMemberId);

        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            task.Title = dto.Title.Trim();
        }

        if (dto.Description != null)
        {
            task.Description = dto.Description;
        }

        if (dto.Priority.HasValue)
        {
            task.Priority = dto.Priority.Value;
        }

        _repo.Update(task);
        _repo.SaveChanges();
    }

    /// <inheritdoc/>
    public void MoveStatus(int id, Enums.TaskStatus status)
    {
        TaskItem? task = _repo.GetById(id);

        if (task == null)
        {
            throw new InvalidOperationException($"Task with id {id} was not found.");
        }

        if (task.Status == status) return;

        EnsureTaskCanMove(task, status);

        task.Status = status;

        if (status == Enums.TaskStatus.Done)
        {
            UnlockDependentTasks(task.Id);
        }

        _repo.Update(task);
        _repo.SaveChanges();
    }

    /// <inheritdoc/>
    public void MoveStatus(int id, Enums.TaskStatus status, int actingMemberId)
    {
        TaskItem? task = _repo.GetById(id);
        if (task == null)
        {
            throw new InvalidOperationException($"Task with id {id} was not found.");
        }

        EnsureCanModify(task, actingMemberId);

        if (task.Status == status) // we do not have to change anything because the status is the same
        {
            return;
        }

        EnsureTaskCanMove(task, status);

        task.Status = status;

        if (status == Enums.TaskStatus.Done)
        {
            UnlockDependentTasks(task.Id);
        }

        _repo.Update(task);
        _repo.SaveChanges();
    }

    /// <inheritdoc/>
    public bool Delete(int id)
    {
        bool deleted = _repo.DeleteById(id);

        if (deleted)
        {
            _repo.SaveChanges();
        }

        return deleted;
    }

    /// <inheritdoc/>
    public bool Delete(int id, int actingMemberId)
    {
        TaskItem? task = _repo.GetById(id);
        if (task == null)
        {
            return false;
        }

        EnsureCanModify(task, actingMemberId);

        bool deleted = _repo.DeleteById(id);
        if (deleted)
        {
            _repo.SaveChanges();
        }

        return deleted;
    }

    /// <inheritdoc/>
    public void Assign(AssignTaskDto dto)
    {
        TaskItem? task = _repo.GetById(dto.TaskId);
        if (task == null)
        {
            throw new InvalidOperationException($"Task with id {dto.TaskId} was not found.");
        }

        task.AssignedToMemberId = dto.MemberId;
        _repo.Update(task);
        _repo.SaveChanges();
    }

    /// <inheritdoc/>
    public void SetDependency(int taskId, int? dependsOnTaskId)
    {
        TaskItem? task = _repo.GetById(taskId);
        if (task == null)
        {
            throw new InvalidOperationException($"Task with id {taskId} was not found.");
        }

        ApplyDependency(task, dependsOnTaskId);
        _repo.Update(task);
        _repo.SaveChanges();
    }

    /// <inheritdoc/>
    public void SetDependency(int taskId, int? dependsOnTaskId, int actingMemberId)
    {
        TaskItem? task = _repo.GetById(taskId);
        if (task == null)
        {
            throw new InvalidOperationException($"Task with id {taskId} was not found.");
        }

        EnsureCanModify(task, actingMemberId);

        ApplyDependency(task, dependsOnTaskId);
        _repo.Update(task);
        _repo.SaveChanges();
    }

    private static void EnsureCanModify(TaskItem task, int actingMemberId)
    {
        if (task.AssignedToMemberId is null)
        {
            return;
        }
        
        if(task.AssignedToMemberId.Value != actingMemberId)
        {
            throw new InvalidOperationException(
                $"Task ${task.Id} is assigned to member #{task.AssignedToMemberId.Value}. Acting member #{actingMemberId} is not allowed."
            );
        }
    }

    private void ApplyDependency(TaskItem task, int? dependsOnTaskId)
    {
        if (!dependsOnTaskId.HasValue)
        {
            task.DependsOnTaskId = null;
            task.IsLocked = false;
            return;
        }

        if (dependsOnTaskId.Value == task.Id)
        {
            throw new InvalidOperationException("A task cannot depend on itself.");
        }

        TaskItem? prerequisite = _repo.GetById(dependsOnTaskId.Value);
        if (prerequisite == null)
        {
            throw new InvalidOperationException($"Dependency task with id {dependsOnTaskId.Value} was not found.");
        }

        if (WouldCreateDependencyCycle(task.Id, prerequisite.Id))
        {
            throw new InvalidOperationException("This dependency would create a cycle.");
        }

        bool shouldLock = prerequisite.Status != Enums.TaskStatus.Done;
        if (shouldLock && task.Status != Enums.TaskStatus.ToDo)
        {
            throw new InvalidOperationException(
                $"Task #{task.Id} cannot be locked while its status is {task.Status}. Move it to ToDo first.");
        }

        task.DependsOnTaskId = prerequisite.Id;
        task.IsLocked = shouldLock;
    }

    private bool WouldCreateDependencyCycle(int taskId, int newDependsOnTaskId)
    {
        TaskItem? current = _repo.GetById(newDependsOnTaskId);
        LinkedListCollection<int> visited = new LinkedListCollection<int>();

        while (current != null)
        {
            if (current.Id == taskId)
            {
                return true;
            }

            if (Contains(visited, current.Id))
            {
                return false;
            }

            visited.Add(current.Id);

            if (!current.DependsOnTaskId.HasValue)
            {
                return false;
            }

            current = _repo.GetById(current.DependsOnTaskId.Value);
        }

        return false;
    }

    private static bool Contains(IMyCollection<int> values, int item)
    {
        IMyIterator<int> iterator = values.GetIterator();
        while (iterator.HasNext())
        {
            if (iterator.Next() == item)
            {
                return true;
            }
        }

        return false;
    }

    private static void EnsureTaskCanMove(TaskItem task, Enums.TaskStatus newStatus)
    {
        if (newStatus == Enums.TaskStatus.ToDo)
        {
            return;
        }

        if (task.IsLocked)
        {
            string dependencyLabel = task.DependsOnTaskId.HasValue
                ? $"#{task.DependsOnTaskId.Value}"
                : "another task";

            throw new InvalidOperationException(
                $"Task #{task.Id} is locked. Complete dependency {dependencyLabel} first.");
        }
    }

    private void UnlockDependentTasks(int completedTaskId)
    {
        LinkedListCollection<int> queue = new LinkedListCollection<int>();
        queue.Add(completedTaskId);

        while (queue.Count > 0)
        {
            if (!queue.TryGetAt(0, out int nextCompletedTaskId))
            {
                break;
            }

            queue.RemoveAt(0);

            IMyIterator<TaskItem> iterator = _repo.GetAll().GetIterator();
            while (iterator.HasNext())
            {
                TaskItem candidate = iterator.Next();
                if (!candidate.DependsOnTaskId.HasValue || candidate.DependsOnTaskId.Value != nextCompletedTaskId)
                {
                    continue;
                }

                if (!candidate.IsLocked)
                {
                    continue;
                }

                candidate.IsLocked = false;
                _repo.Update(candidate);

                if (candidate.Status == Enums.TaskStatus.Done)
                {
                    queue.Add(candidate.Id);
                }
            }
        }
    }
}