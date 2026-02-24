using System.Xml;
using Kanban.Application.Abstractions.Services;
using Kanban.Application.Dtos;
using Kanban.Domain.Abstractions.Collections;
using Kanban.Domain.Abstractions.Persistence;
using Kanban.Domain.Abstractions.Services;
using Kanban.Domain.Entities;

namespace Kanban.Application.Services;

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
            Status = Domain.Enums.TaskStatus.ToDo,
            CreatedAtUtc = _clock.UtcNow
        };

        _repo.Add(task);
        _repo.SaveChanges();

        return task;
    }

    /// <inheritdoc/>
    public void Update(int id, UpdateTaskDto dto)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void MoveStatus(int id, Domain.Enums.TaskStatus status)
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
        throw new NotImplementedException();
    }
}