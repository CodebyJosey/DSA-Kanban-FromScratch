using Kanban.Domain.Abstractions.Collections;
using Kanban.Infrastructure.Collections.Arrays;

namespace Kanban.Tests.Collections;

/// <summary>
/// Unit tests for <see cref="ArrayCollection{T}"/> and its iterator.
/// </summary>
public sealed class ArrayCollectionTests
{
    /// <summary>
    /// Ensures Add() increases Count.
    /// </summary>
    [Fact]
    public void Add_IncreaseCount()
    {
        ArrayCollection<int> array = new ArrayCollection<int>();

        array.Add(1);
        array.Add(2);

        Assert.Equal(2, array.Count);
    }

    /// <summary>
    /// Ensures collection grows even though the initial capacity was lower, so it won't crash the application. 
    /// </summary>
    [Fact]
    public void Add_UnderInitialCapacity_IncreasesCount()
    {
        ArrayCollection<int> array = new ArrayCollection<int>(initialCapacity: 2);

        // Add more than the capacity to force a resize
        array.Add(1);
        array.Add(2);
        array.Add(3);
        array.Add(4);
        array.Add(5);

        Assert.Equal(5, array.Count);
    }

    /// <summary>
    /// Ensures Remove returns false if item does not exist.
    /// </summary>
    [Fact]
    public void Remove_NonExisting_ReturnsFalse()
    {
        ArrayCollection<int> array = new ArrayCollection<int>();
        array.Add(1);
        array.Add(2);

        bool removed = array.Remove(3);

        Assert.False(removed);
        Assert.Equal(2, array.Count);
    }

    /// <summary>
    /// Ensures Remove returns false if item does not exist.
    /// </summary>
    [Fact]
    public void Remove_ExistingItem_DecreasesCount()
    {
        ArrayCollection<int> array = new ArrayCollection<int>();

        array.Add(1);
        array.Add(2);
        array.Add(3);

        bool removed = array.Remove(2);

        Assert.True(removed);
        Assert.Equal(2, array.Count);

        // Here we make sure that 2 is really out of our array
        int found = array.FindBy(2, static (x, key) => x == key);
        Assert.Equal(default(int), found); // default(int) is 0, but FindBy returns int?; default == null
    }

    /// <summary>
    /// Ensures FindBy returns the correct item when present.
    /// </summary>
    [Fact]
    public void FindBy_WhenItemExists_ReturnsItem()
    {
        ArrayCollection<string> array = new ArrayCollection<string>();

        array.Add("a");
        array.Add("b");
        array.Add("c");

        string? found = array.FindBy("b", (item, key) => item == key);
        Assert.Equal("b", found);
    }

    /// <summary>
    /// Ensures FindBy returns null/default when missing.
    /// </summary>
    [Fact]
    public void FindBy_WhenItemMissing_ReturnsNull()
    {
        ArrayCollection<string> array = new ArrayCollection<string>();

        array.Add("a");
        array.Add("b");
        array.Add("c");

        string? found = array.FindBy("zzz", (item, key) => item == key);

        Assert.Null(found);
    }

    /// <summary>
    /// Ensures iterator returns items in insertion order.
    /// </summary>
    [Fact]
    public void Iterator_IteratesInInsertionOrder()
    {
        ArrayCollection<int> array = new ArrayCollection<int>();

        array.Add(1);
        array.Add(2);
        array.Add(3);

        IMyIterator<int> iterator = array.GetIterator();

        Assert.True(iterator.HasNext());
        Assert.Equal(1, iterator.Next());

        Assert.True(iterator.HasNext());
        Assert.Equal(2, iterator.Next());

        Assert.True(iterator.HasNext());
        Assert.Equal(3, iterator.Next());

        Assert.False(iterator.HasNext());
    }

    /// <summary>
    /// Ensures iterator Reset starts iteration from the beginning.
    /// </summary>
    [Fact]
    public void Iterator_Reset_AllowsReIteration()
    {
        ArrayCollection<int> array = new ArrayCollection<int>();

        array.Add(1);
        array.Add(2);

        IMyIterator<int> iterator = array.GetIterator();

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

    /// <summary>
    /// Ensures Filter returns a new collection with only matching items in it.
    /// </summary>
    [Fact]
    public void Filter_ReturnsOnlyMatchingItems()
    {
        ArrayCollection<int> array = new ArrayCollection<int>();

        array.Add(1);
        array.Add(2);
        array.Add(3);
        array.Add(4);

        IMyCollection<int>? filtered = array.Filter(x => x % 2 == 0);

        Assert.Equal(2, filtered.Count);

        IMyIterator<int> iterator = filtered.GetIterator();

        Assert.True(iterator.HasNext());
        Assert.Equal(2, iterator.Next());

        Assert.True(iterator.HasNext());
        Assert.Equal(4, iterator.Next());

        Assert.False(iterator.HasNext());
    }

    /// <summary>
    /// Ensures Reduce aggregates items correctly.
    /// </summary>
    [Fact]
    public void Reduce_SumsValuesCorrectly()
    {
        ArrayCollection<int> array = new ArrayCollection<int>();

        array.Add(1);
        array.Add(2);
        array.Add(3);

        int sum = array.Reduce(0, (acc, x) => acc + x);

        Assert.Equal(6, sum); // 1 + 2 + 3 = 6
    }

    /// <summary>
    /// Ensures Sort orders items ascending using provided comparison.
    /// </summary>
    [Fact]
    public void Sort_OrdersItemsAscending()
    {
        ArrayCollection<int> array = new ArrayCollection<int>();

        array.Add(5);
        array.Add(1);
        array.Add(3);
        array.Add(2);
        array.Add(4);
        array.Add(6);

        array.Sort((a, b) => a.CompareTo(b));

        IMyIterator<int> iterator = array.GetIterator();

        Assert.Equal(1, iterator.Next());
        Assert.Equal(2, iterator.Next());
        Assert.Equal(3, iterator.Next());
        Assert.Equal(4, iterator.Next());
        Assert.Equal(5, iterator.Next());
        Assert.Equal(6, iterator.Next());
        Assert.False(iterator.HasNext());
    }

    /// <summary>
    /// Ensures Remove can remove first and last elements correctly.
    /// </summary>
    [Fact]
    public void Remove_FirstAndLast_Works()
    {
        ArrayCollection<int> array = new ArrayCollection<int>();

        array.Add(1);
        array.Add(2);
        array.Add(3);

        Assert.True(array.Remove(1));
        Assert.Equal(2, array.Count);

        Assert.True(array.Remove(3));
        Assert.Equal(1, array.Count);

        int remaining = array.FindBy(2, (x, key) => x == key);
        Assert.Equal(2, remaining);
    }
}
