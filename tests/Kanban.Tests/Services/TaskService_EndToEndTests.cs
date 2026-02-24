using Kanban.Application.Dtos;
using Kanban.Application.Services;
using Kanban.Domain.Entities;
using Kanban.Domain.Enums;
using Kanban.Infrastructure.Persistence.Json;
using Kanban.Infrastructure.Services;
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
    /// 
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

        service.MoveStatus(created.Id, Domain.Enums.TaskStatus.Done);

        JsonTaskRepository? newRepository = new JsonTaskRepository(filePath);
        TaskItem? reloaded = newRepository.GetById(created.Id);

        Assert.NotNull(reloaded);
        Assert.Equal(Domain.Enums.TaskStatus.Done, reloaded!.Status);
    }
}