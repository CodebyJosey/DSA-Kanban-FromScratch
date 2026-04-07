using Kanban.Source.Data.Json;
using Kanban.Tests.Helpers;

namespace Kanban.Tests.Persistence;

/// <summary>
/// Tests for loading JSON into the repository.
/// </summary>
public sealed class JsonTaskRepository_LoadTests
{
    /// <summary>
    /// Checks if the given seed from JSON (premade) gives the 2 actual made tasks.
    /// </summary>
    [Fact]
    public void Constructor_LoadsTasksFromSeedJson()
    {
        string? filePath = TestFileHelper.CreateTempTasksJsonFromSeed();
        JsonTaskRepository? repository = new JsonTaskRepository(filePath, new Kanban.Source.Collections.MyCollectionFactory(Kanban.Source.Enums.CollectionImplementation.Array));

        Assert.Equal(2, repository.GetAll().Count);
        Assert.NotNull(repository.GetById(1));
        Assert.NotNull(repository.GetById(2));
    }
}