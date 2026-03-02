using Kanban.Source.Classes.Entities;
using Kanban.Source.Data.Json;
using Kanban.Tests.Helpers;

namespace Kanban.Tests.Persistence;

/// <summary>
/// Tests for saving and reloading tasks.
/// </summary>
public sealed class JsonTaskRepository_SaveTests
{
    /// <summary>
    /// Tests if new made tasks are actually saved in a .json file.
    /// </summary>
    [Fact]
    public void SaveChanges_WritesFile_AndReloadPreservesTask()
    {
        string? filePath = TestFileHelper.CreateEmptyTempTaskJsonPath();
        JsonTaskRepository? repository = new JsonTaskRepository(filePath);

        repository.Add(new TaskItem
        {
            Title = "Persisted",
            Description = "Should reload",
            Status = Source.Enums.TaskStatus.ToDo,
            Priority = Source.Enums.TaskPriority.Normal,
            CreatedAtUtc = DateTimeOffset.UtcNow
        });

        repository.SaveChanges();

        Assert.True(File.Exists(filePath));

        JsonTaskRepository newRepository = new JsonTaskRepository(filePath);

        Assert.Equal(1, newRepository.GetAll().Count);
        Assert.NotNull(newRepository.GetById(1));
        Assert.Equal("Persisted", newRepository.GetById(1)!.Title);
    }
}