using Kanban.Source.Collections.LinkedLists;
using Kanban.Source.Interfaces;

namespace Kanban.Tests.Collections;

/// <summary>
/// Unit tests for linked-list iterator behavior through <see cref="LinkedListCollection{T}"/>.
/// </summary>
public sealed class LinkedListIteratorTests
{
    /// <summary>
    /// Ensures HasNext is false and Next throws for an empty collection.
    /// </summary>
    [Fact]
    public void EmptyCollection_HasNoNext_AndNextThrows()
    {
        LinkedListCollection<int> list = new LinkedListCollection<int>();
        IMyIterator<int> iterator = list.GetIterator();

        Assert.False(iterator.HasNext());
        Assert.Throws<InvalidOperationException>(() => iterator.Next());
    }

    /// <summary>
    /// Ensures Next throws after iterator reaches the end.
    /// </summary>
    [Fact]
    public void Iterator_AfterEnd_NextThrows()
    {
        LinkedListCollection<int> list = new LinkedListCollection<int>();
        list.Add(10);
        list.Add(20);

        IMyIterator<int> iterator = list.GetIterator();

        Assert.True(iterator.HasNext());
        Assert.Equal(10, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(20, iterator.Next());
        Assert.False(iterator.HasNext());

        Assert.Throws<InvalidOperationException>(() => iterator.Next());
    }

    /// <summary>
    /// Ensures Reset restarts iteration from the beginning.
    /// </summary>
    [Fact]
    public void Iterator_Reset_AllowsReIterationFromStart()
    {
        LinkedListCollection<int> list = new LinkedListCollection<int>();
        list.Add(1);
        list.Add(2);

        IMyIterator<int> iterator = list.GetIterator();

        Assert.Equal(1, iterator.Next());
        Assert.Equal(2, iterator.Next());
        Assert.False(iterator.HasNext());

        iterator.Reset();

        Assert.True(iterator.HasNext());
        Assert.Equal(1, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(2, iterator.Next());
        Assert.False(iterator.HasNext());
    }
}
