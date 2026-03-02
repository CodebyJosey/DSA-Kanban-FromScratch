

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
    public bool Delete(int id)
    {
        bool deleted = _repo.DeleteById(id);

        if (deleted)
        {
            _repo.SaveChanges();
        }

        return deleted;
    }
}