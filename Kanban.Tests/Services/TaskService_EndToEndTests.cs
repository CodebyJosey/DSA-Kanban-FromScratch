using Kanban.Source.Classes.Dtos;
using Kanban.Source.Classes.Entities;
using Kanban.Source.Data.Json;
using Kanban.Source.Enums;
using Kanban.Source.Services;
using Kanban.Tests.Helpers;

namespace Kanban.Tests.Services;

/// <summary>
/// End-to-end tests for TaskService using the real JSON repository.
/// </summary>
public sealed class TaskService_EndToEndTests
{
    /// <summary>
    /// Creates a task and checks if it actually creates a task in the JSON file.
    /// </summary>
    [Fact]
    public void CreateTask_PersistsAndReloads()
    {
        string? filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository? repository = new JsonTaskRepository(filePath);
        SystemClock? clock = new SystemClock();
        TaskService? service = new TaskService(repository, clock);

        service.Create(new CreateTaskDto
        {
            Title = "From service",
            Description = "Created via TaskService",
            Priority = TaskPriority.High
        });

        JsonTaskRepository? newRepository = new JsonTaskRepository(filePath);
        TaskService? newService = new TaskService(newRepository, clock);

        Assert.Equal(1, newService.GetAll().Count);
        Assert.Equal("From service", newRepository.GetById(1)!.Title);
        Assert.Equal("Created via TaskService", newRepository.GetById(1)!.Description);
        Assert.Equal(TaskPriority.High, newRepository.GetById(1)!.Priority);
    }

    /// <summary>
    /// Moves the status of an task and checks if the change persists after reload.
    /// </summary>
    [Fact]
    public void MoveStatus_PersistsChange()
    {
        string? filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository? repository = new JsonTaskRepository(filePath);
        SystemClock? clock = new SystemClock();
        TaskService? service = new TaskService(repository, clock);

        TaskItem? created = service.Create(new CreateTaskDto
        {
            Title = "A"
        });

        service.MoveStatus(created.Id, Source.Enums.TaskStatus.Done);

        JsonTaskRepository? newRepository = new JsonTaskRepository(filePath);
        TaskItem? reloaded = newRepository.GetById(created.Id);

        Assert.NotNull(reloaded);
        Assert.Equal(Source.Enums.TaskStatus.Done, reloaded!.Status);
    }

    /// <summary>
    /// Updates an existing task and checks if the change perists after reload.
    /// </summary>
    [Fact]
    public void Update_PersistsChange()
    {
        string? filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository? repository = new JsonTaskRepository(filePath);
        SystemClock? clock = new SystemClock();
        TaskService? service = new TaskService(repository, clock);

        TaskItem? created = service.Create(new CreateTaskDto
        {
            Title = "Old title",
            Description = "Old description",
            Priority = TaskPriority.Normal
        });

        service.Update(created.Id, new UpdateTaskDto
        {
            Title = "New title",
            Description = "New description",
            Priority = TaskPriority.High
        });

        JsonTaskRepository? newRepository = new JsonTaskRepository(filePath);
        TaskItem? reloaded = newRepository.GetById(created.Id);

        Assert.NotNull(reloaded);
        Assert.Equal("New title", reloaded.Title);
        Assert.Equal("New description", reloaded.Description);
        Assert.Equal(TaskPriority.High, reloaded.Priority);
    }

    /// <summary>
    /// Deletes an existing task and checks if it is gone after reload.
    /// </summary>
    [Fact]
    public void Delete_RemovesAndPersists()
    {
        string? filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository? repository = new JsonTaskRepository(filePath);
        SystemClock? clock = new SystemClock();
        TaskService? service = new TaskService(repository, clock);

        TaskItem? created = service.Create(new CreateTaskDto
        {
            Title = "Task to delete"
        });

        bool deleted = service.Delete(created.Id);

        Assert.True(deleted);

        JsonTaskRepository? newRepository = new JsonTaskRepository(filePath);

        Assert.Equal(0, newRepository.GetAll().Count);
        Assert.Null(newRepository.GetById(created.Id));
    }
}