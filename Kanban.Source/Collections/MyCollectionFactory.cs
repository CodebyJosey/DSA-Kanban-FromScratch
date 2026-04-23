using Kanban.Source.Classes.Entities;
using Kanban.Source.Collections.Arrays;
using Kanban.Source.Collections.BinarySearchTrees;
using Kanban.Source.Collections.HashMap;
using Kanban.Source.Collections.LinkedLists;
using Kanban.Source.Enums;
using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections;

/// <summary>
/// Creates collection instances based on a configured implementation.
/// </summary>
public sealed class MyCollectionFactory : IMyCollectionFactory
{
    private readonly CollectionImplementation _implementation;

    /// <summary>
    /// Creates a new factory instance.
    /// </summary>
    public MyCollectionFactory(CollectionImplementation implementation)
    {
        _implementation = implementation;
    }

    /// <inheritdoc/>
    public IMyCollection<T>  Create<T>()
    {
        return _implementation switch
        {
            CollectionImplementation.Array => new ArrayCollection<T>(),
            CollectionImplementation.LinkedList => new LinkedListCollection<T>(),
            CollectionImplementation.BinarySearchTree => new BinarySearchTreeCollection<T>(BuildComparison<T>()),
            _ => new ArrayCollection<T>()
        };
    }

    /// <summary>
    /// returns hashmap collection
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <returns></returns>
    public IMyCollection<KeyValue<K, V>> CreateHashMap<K, V>() =>
        new HashmapCollection<K, V>();

    private static Comparison<T> BuildComparison<T>()
    {
        if (typeof(T) == typeof(TaskItem))
        {
            return (left, right) => CompareTaskItems(left, right);
        }

        if (typeof(T) == typeof(TeamMember))
        {
            return (left, right) => CompareTeamMembers(left, right);
        }

        return static (left, right) => CompareSimpleValues(left, right);
    }

    private static int CompareTaskItems<T>(T left, T right)
    {
        TaskItem? l = left as TaskItem;
        TaskItem? r = right as TaskItem;
        if (l is null && r is null) return 0;
        if (l is null) return -1;
        if (r is null) return 1;

        if (l.Id < r.Id) return -1;
        if (l.Id > r.Id) return 1;

        return string.CompareOrdinal(l.Title, r.Title);
    }

    private static int CompareTeamMembers<T>(T left, T right)
    {
        TeamMember? l = left as TeamMember;
        TeamMember? r = right as TeamMember;
        if (l is null && r is null) return 0;
        if (l is null) return -1;
        if (r is null) return 1;

        if (l.Id < r.Id) return -1;
        if (l.Id > r.Id) return 1;

        return string.CompareOrdinal(l.Name, r.Name);
    }

    private static int CompareSimpleValues<T>(T left, T right)
    {
        if (left is null && right is null) return 0;
        if (left is null) return -1;
        if (right is null) return 1;

        object l = left;
        object r = right;

        if (l is int li && r is int ri)
        {
            if (li < ri) return -1;
            if (li > ri) return 1;
            return 0;
        }

        if (l is string ls && r is string rs)
        {
            return string.CompareOrdinal(ls, rs);
        }

        if (l is DateTimeOffset ldt && r is DateTimeOffset rdt)
        {
            if (ldt < rdt) return -1;
            if (ldt > rdt) return 1;
            return 0;
        }

        throw new InvalidOperationException($"No comparison configured for type '{typeof(T).Name}'.");
    }
}