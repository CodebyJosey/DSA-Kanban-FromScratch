using System.Text.Json;
using Kanban.Source.Classes.Entities;
using Kanban.Source.Collections;
using Kanban.Source.Interfaces;

namespace Kanban.Source.Data.Json;

/// <summary>
/// JSON repository for team members.
/// </summary>
public sealed class JsonTeamMemberRepository : ITeamMemberRepository
{
    private readonly string _filePath;
    private readonly IMyCollectionFactory _collectionFactory;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public JsonTeamMemberRepository(string filePath)
        : this(filePath, new MyCollectionFactory(Enums.CollectionImplementation.Array))
    {
    }

    /// <summary>
    /// Creates a new instance with a collection factory.
    /// </summary>
    public JsonTeamMemberRepository(string filePath, IMyCollectionFactory collectionFactory)
    {
        _filePath = filePath;
        ArgumentNullException.ThrowIfNull(collectionFactory);
        _collectionFactory = collectionFactory;
    }

    /// <inheritdoc />
    public IMyCollection<TeamMember> LoadAll()
    {
        // Use your own collection implementation.
        IMyCollection<TeamMember> members = _collectionFactory.Create<TeamMember>();

        if (!File.Exists(_filePath))
        {
            return members;
        }

        string json = File.ReadAllText(_filePath);
        TeamMemberRecord[]? records = JsonSerializer.Deserialize<TeamMemberRecord[]>(json);

        if (records is null)
        {
            return members;
        }

        for (int i = 0; i < records.Length; i++)
        {
            TeamMemberRecord r = records[i];
            if (r.Name is null)
            {
                continue;
            }

            members.Add(new TeamMember
            {
                Id = r.Id,
                Name = r.Name
            });
        }

        members.Dirty = false;
        return members;
    }

    /// <inheritdoc />
    public void SaveAll(IMyCollection<TeamMember> members)
    {
        string? directoryPath = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Serialize via array (no List<T>)
        TeamMemberRecord[] arr = new TeamMemberRecord[members.Count];

        int index = 0;
        IMyIterator<TeamMember> it = members.GetIterator();
        while (it.HasNext())
        {
            TeamMember m = it.Next();
            arr[index++] = new TeamMemberRecord { Id = m.Id, Name = m.Name };
        }

        string json = JsonSerializer.Serialize(arr, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);

        members.Dirty = false;
    }
}