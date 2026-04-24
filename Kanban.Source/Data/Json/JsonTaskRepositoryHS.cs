using System.Text.Json;
using Kanban.Source.Classes.Entities;
using Kanban.Source.Collections.HashMap;
using Kanban.Source.Collections.Arrays;
using Kanban.Source.Interfaces;

namespace Kanban.Source.Data.Json;

/// <summary>
/// JSON-based repository for <see cref="TaskItem"/>.
/// Uses custom collections (no List&lt;T&gt;/Dictionary&lt;T&gt;/LINQ).
/// </summary>
public sealed class JsonTaskRepositoryHS : ITaskRepository
{
    private readonly string _filePath;
    private readonly IMyCollection<KeyValue<int, TaskItem>> _tasks;

    /// <summary>
    /// Creates a new instance with a collection factory.
    /// </summary>
    /// <param name="filePath">Path to the JSON file.</param>
    /// <param name="collectionFactory">Factory used to create the internal collection.</param>
    public JsonTaskRepositoryHS(string filePath, IMyCollectionFactory collectionFactory)
    {
        ArgumentNullException.ThrowIfNull(collectionFactory);

        _filePath = filePath;
        _tasks = collectionFactory.CreateHashMap<int, TaskItem>();
        LoadFromDisk();
    }

    /// <inheritdoc/>
    public IMyCollection<TaskItem> GetAll()
    {
        var arrayAsReturn = new ArrayCollection<TaskItem>();
        IMyIterator<KeyValue<int, TaskItem>> iterator = _tasks.GetIterator();

        while (iterator.HasNext())
            arrayAsReturn.Add(iterator.Next().Value);

        return arrayAsReturn;
    }

    /// <inheritdoc/>
    public TaskItem? GetById(int id)
    {
        IMyIterator<KeyValue<int, TaskItem>> iterator = _tasks.GetIterator();

        while (iterator.HasNext())
        {
            TaskItem? task = iterator.Next().Value;
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
        KeyValue<int, TaskItem> newItem = new KeyValue<int, TaskItem>(entity.Id, entity);
        _tasks.Add(newItem);
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

        var item = new  KeyValue<int, TaskItem>(task.Id, task);
        bool removed = _tasks.Remove(item);

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

            var taskItem = new TaskItem
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
            };
            _tasks.Add(new KeyValue<int, TaskItem>(r.Id, taskItem));
        }

        RecalculateLocks();

        _tasks.Dirty = false;
    }

    private int GetNextId()
    {
        int maxId = 0;

        IMyIterator<KeyValue<int, TaskItem>> iterator = _tasks.GetIterator();

        while (iterator.HasNext())
        {
            TaskItem? task = iterator.Next().Value;
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

        IMyIterator<KeyValue<int, TaskItem>>? iterator = _tasks.GetIterator();
        int index = 0;

        while (iterator.HasNext())
        {
            TaskItem? task = iterator.Next().Value;

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
        IMyIterator<KeyValue<int, TaskItem>> iterator = _tasks.GetIterator();

        while (iterator.HasNext())
        {
            TaskItem task = iterator.Next().Value;

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