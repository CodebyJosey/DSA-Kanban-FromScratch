using System.Text.Json;
using Kanban.Source.Classes.Entities;
using Kanban.Source.Collections;
using Kanban.Source.Interfaces;

namespace Kanban.Source.Data.Json;

/// <summary>
/// JSON-based repository for <see cref="TaskItem"/>.
/// Uses custom collections (no List&lt;T&gt;/Dictionary&lt;T&gt;/LINQ).
/// </summary>
public sealed class JsonTaskRepository : ITaskRepository
{
    private readonly string _filePath;
    private readonly IMyCollection<TaskItem> _tasks;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="filePath">Path to the JSON file.</param>
    public JsonTaskRepository(string filePath)
        : this(filePath, new MyCollectionFactory(Enums.CollectionImplementation.Array))
    {
    }

    /// <summary>
    /// Creates a new instance with a collection factory.
    /// </summary>
    /// <param name="filePath">Path to the JSON file.</param>
    /// <param name="collectionFactory">Factory used to create the internal collection.</param>
    public JsonTaskRepository(string filePath, IMyCollectionFactory collectionFactory)
    {
        ArgumentNullException.ThrowIfNull(collectionFactory);

        _filePath = filePath;
        _tasks = collectionFactory.Create<TaskItem>();
        LoadFromDisk();
    }

    /// <inheritdoc/>
    public IMyCollection<TaskItem> GetAll()
    {
        return _tasks;
    }

    /// <inheritdoc/>
    public TaskItem? GetById(int id)
    {
        IMyIterator<TaskItem> iterator = _tasks.GetIterator();

        while (iterator.HasNext())
        {
            TaskItem? task = iterator.Next();
            if (task.Id == id)
            {
                return task;
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public void Add(TaskItem entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.Id = GetNextId();

        _tasks.Add(entity);
        _tasks.Dirty = true;
    }

    /// <inheritdoc/>
    public void Update(TaskItem entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _tasks.Dirty = true;
    }

    /// <inheritdoc/>
    public bool DeleteById(int id)
    {
        TaskItem? task = GetById(id);

        if (task == null)
        {
            return false;
        }

        bool removed = _tasks.Remove(task);

        if (removed)
        {
            _tasks.Dirty = true;
        }

        return removed;
    }

    /// <inheritdoc/>
    public void SaveChanges()
    {
        if (!_tasks.Dirty) return;

        string? directoryPath = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        TaskRecord[]? records = ToRecordArray();

        string? json = JsonSerializer.Serialize(records, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
        _tasks.Dirty = false;
    }

    private void LoadFromDisk()
    {
        if (!File.Exists(_filePath))
        {
            return;
        }

        string? json = File.ReadAllText(_filePath);
        TaskRecord[]? records = JsonSerializer.Deserialize<TaskRecord[]>(json) ?? Array.Empty<TaskRecord>();

        for (int i = 0; i < records.Length; i++)
        {
            TaskRecord? r = records[i];

            _tasks.Add(new TaskItem
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status,
                Priority = r.Priority,
                CreatedAtUtc = r.CreatedAtUtc,
                AssignedToMemberId = r.AssignedToMemberId,
                DependsOnTaskId = r.DependsOnTaskId,
                IsLocked = r.IsLocked
            });
        }

        RecalculateLocks();

        _tasks.Dirty = false;
    }

    private int GetNextId()
    {
        int maxId = 0;

        IMyIterator<TaskItem> iterator = _tasks.GetIterator();

        while (iterator.HasNext())
        {
            TaskItem? task = iterator.Next();
            if (task.Id > maxId)
            {
                maxId = task.Id;
            }
        }

        return maxId + 1;
    }

    private TaskRecord[] ToRecordArray()
    {
        TaskRecord[]? array = new TaskRecord[_tasks.Count];

        IMyIterator<TaskItem>? iterator = _tasks.GetIterator();
        int index = 0;

        while (iterator.HasNext())
        {
            TaskItem? task = iterator.Next();

            array[index] = new TaskRecord
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                CreatedAtUtc = task.CreatedAtUtc,
                AssignedToMemberId = task.AssignedToMemberId,
                DependsOnTaskId = task.DependsOnTaskId,
                IsLocked = task.IsLocked
            };

            index++;
        }

        return array;
    }

    private void RecalculateLocks()
    {
        IMyIterator<TaskItem> iterator = _tasks.GetIterator();

        while (iterator.HasNext())
        {
            TaskItem task = iterator.Next();

            if (!task.DependsOnTaskId.HasValue)
            {
                task.IsLocked = false;
                continue;
            }

            TaskItem? prerequisite = GetById(task.DependsOnTaskId.Value);
            task.IsLocked = prerequisite is null || prerequisite.Status != Enums.TaskStatus.Done;
        }
    }
}