

using Kanban.Source.Interfaces;

namespace Kanban.Tests.Collections.Contracts;

/// <summary>
/// Contract tests for any <see cref="IMyCollection{T}"/> implementation.
/// Every custom collection implementation must pass these tests.
/// </summary>
public abstract class MyCollectionContractTests
{
    /// <summary>
    /// Factory method implemented by derived test classes (Array, LinkedList, etc.).
    /// </summary>
    protected abstract IMyCollection<int> CreateIntCollection();

    /// <summary>
    /// Factory method implemented by derived test classes (Array, LinkedList, etc.).
    /// </summary>
    protected abstract IMyCollection<string> CreateStringCollection();

    /// <summary>
    /// Checks if the Add method increases the count of the collection.
    /// </summary>
    [Fact]
    public void Add_IncreasesCount()
    {
        IMyCollection<int>? collection = CreateIntCollection();

        collection.Add(10);
        collection.Add(20);

        Assert.Equal(2, collection.Count);
    }

    /// <summary>
    /// Checks if the Remove method returns true and decreases the count of the collection.
    /// </summary>
    [Fact]
    public void Remove_ExistingItem_ReturnsTrueAndDecreasesCount()
    {
        IMyCollection<int>? collection = CreateIntCollection();

        collection.Add(10);
        collection.Add(20);
        collection.Add(30);

        bool removed = collection.Remove(20);

        Assert.True(removed);
        Assert.Equal(2, collection.Count);
    }

    /// <summary>
    /// Checks if the Remove method returns false and doesn't change count when the entry wasn't found.
    /// </summary>
    [Fact]
    public void Remove_NonExistingItem_ReturnsFalseAndDoesNotChangeCount()
    {
        IMyCollection<int>? collection = CreateIntCollection();

        collection.Add(10);
        collection.Add(20);
        collection.Add(30);

        bool removed = collection.Remove(40);

        Assert.False(removed);
        Assert.Equal(3, collection.Count);
    }

    /// <summary>
    /// Checks if the FindBy method returns an item when it exists.
    /// </summary>
    [Fact]
    public void FindBy_WhenItemExists_ReturnsItem()
    {
        IMyCollection<string>? collection = CreateStringCollection();

        collection.Add("a");
        collection.Add("b");
        collection.Add("c");

        string? found = collection.FindBy("b", (item, key) => item == key);

        Assert.NotNull(found);
        Assert.Equal("b", found);
    }

    /// <summary>
    /// Checks if the FindBy method returns null when the item doesn't exist.
    /// </summary>
    [Fact]
    public void FindBy_WhenItemMissing_ReturnsNull()
    {
        IMyCollection<string>? collection = CreateStringCollection();

        collection.Add("a");
        collection.Add("b");
        collection.Add("c");

        string? found = collection.FindBy("zzz", (item, key) => item == key);

        Assert.Null(found);
    }

    /// <summary>
    /// Checks if the iterator iterates through all items in the same order as insertions we're made.
    /// </summary>
    [Fact]
    public void Iterator_IteratesAllItemsInInsertionOrder()
    {
        IMyCollection<int>? collection = CreateIntCollection();

        collection.Add(10);
        collection.Add(20);
        collection.Add(30);

        IMyIterator<int>? iterator = collection.GetIterator();

        Assert.True(iterator.HasNext());
        Assert.Equal(10, iterator.Next());

        Assert.True(iterator.HasNext());
        Assert.Equal(20, iterator.Next());

        Assert.True(iterator.HasNext());
        Assert.Equal(30, iterator.Next());

        Assert.False(iterator.HasNext());
    }

    /// <summary>
    /// Checks if the Filter method only returns matching items.
    /// </summary>
    [Fact]
    public void Filter_ReturnsOnlyMatchingItems()
    {
        IMyCollection<int>? collection = CreateIntCollection();

        collection.Add(1);
        collection.Add(2);
        collection.Add(3);
        collection.Add(4);
        collection.Add(5);

        IMyCollection<int>? filtered = collection.Filter(myInt => myInt % 2 == 0);

        Assert.Equal(2, filtered.Count);

        IMyIterator<int>? iterator = filtered.GetIterator();
        Assert.True(iterator.HasNext());
        Assert.Equal(2, iterator.Next());

        Assert.True(iterator.HasNext());
        Assert.Equal(4, iterator.Next());

        Assert.False(iterator.HasNext());
    }

    /// <summary>
    /// Checks if the Reduce method aggregates correctly.
    /// </summary>
    [Fact]
    public void Reduce_AggregatesCorrectly()
    {
        IMyCollection<int>? collection = CreateIntCollection();

        collection.Add(1);
        collection.Add(2);
        collection.Add(3);

        int sum = collection.Reduce(0, (acc, x) => acc + x);

        Assert.Equal(6, sum);
    }

    /// <summary>
    /// Checks if the Sort method sorts in ascending order.
    /// </summary>
    [Fact]
    public void Sort_OrdersAscending()
    {
        IMyCollection<int>? collection = CreateIntCollection();

        collection.Add(3);
        collection.Add(1);
        collection.Add(2);

        collection.Sort((a, b) => a.CompareTo(b));

        IMyIterator<int>? iterator = collection.GetIterator();

        Assert.Equal(1, iterator.Next());
        Assert.Equal(2, iterator.Next());
        Assert.Equal(3, iterator.Next());
        Assert.False(iterator.HasNext());
    }
}