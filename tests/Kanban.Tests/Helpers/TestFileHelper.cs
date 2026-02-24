namespace Kanban.Tests.Helpers;

/// <summary>
/// Helper methods for working with test files.
/// </summary>
public static class TestFileHelper
{
    /// <summary>
    /// Creates a unique temporary directory.
    /// </summary>
    public static string CreateUniqueTempDirectory()
    {
        string? dir = Path.Combine(Path.GetTempPath(), "KanbanTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    /// <summary>
    /// Gives the absolute path to the tasks seed file (JSON) inside the repo.
    /// </summary>
    public static string SeedJsonPath()
    {
        string? repoRoot = FindRepoRoot(AppContext.BaseDirectory);
        return Path.Combine(repoRoot, "tests", "Kanban.Tests", "TestData", "tasks.seed.json");
    }

    /// <summary>
    /// Creates a temporary tasks.json file by copying the seed json.
    /// </summary>
    public static string CreateTempTasksJsonFromSeed()
    {
        string? tempDir = CreateUniqueTempDirectory();
        string? dest = Path.Combine(tempDir, "tasks.json");

        string? seed = SeedJsonPath();

        // Helpful error message if path is still wrong
        if (!File.Exists(seed))
        {
            throw new FileNotFoundException("Seed json not found.", seed);
        }

        File.Copy(seed, dest, overwrite: true);

        return dest;
    }

    /// <summary>
    /// Creates an empty temporary tasks.json path (file may not exist yet).
    /// </summary>
    public static string CreateEmptyTempTaskJsonPath()
    {
        string? tempDir = CreateUniqueTempDirectory();
        return Path.Combine(tempDir, "tasks.json");
    }

    private static string FindRepoRoot(string startDirectory)
    {
        DirectoryInfo? current = new DirectoryInfo(startDirectory);

        while (current != null)
        {
            // Repo root: contains *.sln
            FileInfo[] slnFiles = current.GetFiles("*.sln");
            if (slnFiles.Length > 0)
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repo root (no .sln file found).");
    }
}