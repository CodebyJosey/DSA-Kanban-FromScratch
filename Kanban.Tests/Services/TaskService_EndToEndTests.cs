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

        JsonTaskRepository? repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock? clock = new SystemClock();
        TaskService? service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        service.Create(new CreateTaskDto
        {
            Title = "From service",
            Description = "Created via TaskService",
            Priority = TaskPriority.High
        });

        JsonTaskRepository? newRepository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        TaskService? newService = new TaskService(newRepository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

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

        JsonTaskRepository? repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock? clock = new SystemClock();
        TaskService? service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        TaskItem? created = service.Create(new CreateTaskDto
        {
            Title = "A"
        });

        service.MoveStatus(created.Id, Source.Enums.TaskStatus.Done);

        JsonTaskRepository? newRepository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
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

        JsonTaskRepository? repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock? clock = new SystemClock();
        TaskService? service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

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

        JsonTaskRepository? newRepository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
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

        JsonTaskRepository? repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock? clock = new SystemClock();
        TaskService? service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        TaskItem? created = service.Create(new CreateTaskDto
        {
            Title = "Task to delete"
        });

        bool deleted = service.Delete(created.Id);

        Assert.True(deleted);

        JsonTaskRepository? newRepository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        Assert.Equal(0, newRepository.GetAll().Count);
        Assert.Null(newRepository.GetById(created.Id));
    }

    [Fact]
    public void CreateTask_WithDependency_StartsLockedWhenPrerequisiteNotDone()
    {
        string filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock clock = new SystemClock();
        TaskService service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        TaskItem prerequisite = service.Create(new CreateTaskDto
        {
            Title = "Task A"
        });

        TaskItem dependent = service.Create(new CreateTaskDto
        {
            Title = "Task B",
            DependsOnTaskId = prerequisite.Id
        });

        JsonTaskRepository reloadedRepository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        TaskItem? reloadedDependent = reloadedRepository.GetById(dependent.Id);

        Assert.NotNull(reloadedDependent);
        Assert.True(reloadedDependent!.IsLocked);
        Assert.Equal(prerequisite.Id, reloadedDependent.DependsOnTaskId);
    }

    [Fact]
    public void MoveStatus_WhenTaskIsLocked_Throws()
    {
        string filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock clock = new SystemClock();
        TaskService service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        TaskItem prerequisite = service.Create(new CreateTaskDto
        {
            Title = "Task A"
        });

        TaskItem dependent = service.Create(new CreateTaskDto
        {
            Title = "Task B",
            DependsOnTaskId = prerequisite.Id
        });

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
            () => service.MoveStatus(dependent.Id, Source.Enums.TaskStatus.InProgress));

        Assert.Contains("locked", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MoveStatus_WhenPrerequisiteCompletes_UnlocksDependent()
    {
        string filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock clock = new SystemClock();
        TaskService service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        TaskItem prerequisite = service.Create(new CreateTaskDto
        {
            Title = "Task A"
        });

        TaskItem dependent = service.Create(new CreateTaskDto
        {
            Title = "Task B",
            DependsOnTaskId = prerequisite.Id
        });

        service.MoveStatus(prerequisite.Id, Source.Enums.TaskStatus.Done);

        JsonTaskRepository reloadedRepository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        TaskItem? reloadedDependent = reloadedRepository.GetById(dependent.Id);

        Assert.NotNull(reloadedDependent);
        Assert.False(reloadedDependent!.IsLocked);
    }

    [Fact]
    public void SetDependency_AddsDependency_AndLocksTask()
    {
        string filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock clock = new SystemClock();
        TaskService service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        TaskItem prerequisite = service.Create(new CreateTaskDto { Title = "Task A" });
        TaskItem dependent = service.Create(new CreateTaskDto { Title = "Task B" });

        service.SetDependency(dependent.Id, prerequisite.Id);

        JsonTaskRepository reloadedRepository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        TaskItem? reloadedDependent = reloadedRepository.GetById(dependent.Id);

        Assert.NotNull(reloadedDependent);
        Assert.Equal(prerequisite.Id, reloadedDependent!.DependsOnTaskId);
        Assert.True(reloadedDependent.IsLocked);
    }

    [Fact]
    public void SetDependency_ClearDependency_UnlocksTask()
    {
        string filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock clock = new SystemClock();
        TaskService service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        TaskItem prerequisite = service.Create(new CreateTaskDto { Title = "Task A" });
        TaskItem dependent = service.Create(new CreateTaskDto
        {
            Title = "Task B",
            DependsOnTaskId = prerequisite.Id
        });

        service.SetDependency(dependent.Id, null);

        JsonTaskRepository reloadedRepository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        TaskItem? reloadedDependent = reloadedRepository.GetById(dependent.Id);

        Assert.NotNull(reloadedDependent);
        Assert.Null(reloadedDependent!.DependsOnTaskId);
        Assert.False(reloadedDependent.IsLocked);
    }

    [Fact]
    public void SetDependency_WhenCycleWouldBeCreated_Throws()
    {
        string filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();

        JsonTaskRepository repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));
        SystemClock clock = new SystemClock();
        TaskService service = new TaskService(repository, clock, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        TaskItem taskA = service.Create(new CreateTaskDto { Title = "Task A" });
        TaskItem taskB = service.Create(new CreateTaskDto { Title = "Task B", DependsOnTaskId = taskA.Id });

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
            () => service.SetDependency(taskA.Id, taskB.Id));

        Assert.Contains("cycle", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}