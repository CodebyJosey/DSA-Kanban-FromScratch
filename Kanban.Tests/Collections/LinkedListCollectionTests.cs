using Kanban.Source.Collections.LinkedLists;

namespace Kanban.Tests.Collections;

/// <summary>
/// Unit tests for <see cref="LinkedListCollection{T}"/> specific behaviors.
/// </summary>
public sealed class LinkedListCollectionTests
{
    /// <summary>
    /// Verifies default state after construction.
    /// </summary>
    [Fact]
    public void Constructor_StartsWithCountZeroAndDirtyFalse()
    {
        var list = new LinkedListCollection<int>();

        Assert.Equal(0, list.Count);
        Assert.False(list.Dirty);
    }

    /// <summary>
    /// Verifies RemoveAt removes the target entry and updates count/order.
    /// </summary>
    [Fact]
    public void RemoveAt_RemovesExpectedItemAndUpdatesCount()
    {
        var list = new LinkedListCollection<int>();
        list.Add(10);
        list.Add(20);
        list.Add(30);

        list.RemoveAt(1);

        Assert.Equal(2, list.Count);
        Assert.False(list.TryGetAt(2, out _));
        Assert.True(list.TryGetAt(0, out int first));
        Assert.True(list.TryGetAt(1, out int second));
        Assert.Equal(10, first);
        Assert.Equal(30, second);
    }

    /// <summary>
    /// Verifies TryGetAt returns false for invalid indices.
    /// </summary>
    [Fact]
    public void TryGetAt_NegativeOrOutOfRange_ReturnsFalse()
    {
        var list = new LinkedListCollection<int>();
        list.Add(5);

        Assert.False(list.TryGetAt(-1, out _));
        Assert.False(list.TryGetAt(1, out _));
    }

    /// <summary>
    /// Verifies Filter returns a non-dirty result collection.
    /// </summary>
    [Fact]
    public void Filter_ReturnsCleanCollection()
    {
        var list = new LinkedListCollection<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        var filtered = list.Filter(x => x > 1);

        Assert.Equal(2, filtered.Count);
        Assert.False(filtered.Dirty);
    }

    /// <summary>
    /// Verifies Sort marks the collection as dirty.
    /// </summary>
    [Fact]
    public void Sort_SetsDirtyToTrue()
    {
        var list = new LinkedListCollection<int>();
        list.Add(2);
        list.Add(1);

        list.Dirty = false;
        list.Sort((a, b) => a.CompareTo(b));

        Assert.True(list.Dirty);
    }
}
