

using Kanban.Source.Classes.Dtos;
using Kanban.Source.Classes.Entities;
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
            CreatedAtUtc = _clock.UtcNow
        };

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

        task.Status = status;

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

        task.Status = status;
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
}