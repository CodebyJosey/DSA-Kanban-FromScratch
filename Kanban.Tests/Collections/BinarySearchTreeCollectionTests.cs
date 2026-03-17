using Kanban.Source.Collections.BinarySearchTrees;
using Kanban.Source.Interfaces;

namespace Kanban.Tests.Collections;

public sealed class BinarySearchTreeCollectionTests
{
    [Fact]
    public void Add_Duplicates_AreAllowed()
    {
        BinarySearchTreeCollection<int> collection = new();

        collection.Add(10);
        collection.Add(10);
        collection.Add(10);

        Assert.Equal(3, collection.Count);

        IMyIterator<int> iterator = collection.GetIterator();
        Assert.True(iterator.HasNext());
        Assert.Equal(10, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(10, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(10, iterator.Next());
        Assert.False(iterator.HasNext());
    }

    [Fact]
    public void Remove_RemovesFirstMatchingItemInInsertionOrder()
    {
        BinarySearchTreeCollection<string> collection = new();

        collection.Add("b");
        collection.Add("a");
        collection.Add("b");
        collection.Add("c");

        bool removed = collection.Remove("b");

        Assert.True(removed);
        Assert.Equal(3, collection.Count);

        IMyIterator<string> iterator = collection.GetIterator();
        Assert.Equal("a", iterator.Next());
        Assert.Equal("b", iterator.Next());
        Assert.Equal("c", iterator.Next());
        Assert.False(iterator.HasNext());
    }

    [Fact]
    public void Sort_ChangesIteratorOrderToSortedOrder()
    {
        BinarySearchTreeCollection<int> collection = new();

        collection.Add(30);
        collection.Add(10);
        collection.Add(20);

        collection.Sort((left, right) =>
        {
            if (left < right) return -1;
            if (left > right) return 1;
            return 0;
        });

        IMyIterator<int> iterator = collection.GetIterator();

        Assert.True(iterator.HasNext());
        Assert.Equal(10, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(20, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(30, iterator.Next());
        Assert.False(iterator.HasNext());
    }

    [Fact]
    public void Filter_ReturnsOnlyMatchingItems_AndIsNotDirty()
    {
        BinarySearchTreeCollection<int> collection = new();

        collection.Add(1);
        collection.Add(2);
        collection.Add(3);
        collection.Add(4);

        IMyCollection<int> filtered = collection.Filter(value => value % 2 == 0);

        Assert.Equal(2, filtered.Count);
        Assert.False(filtered.Dirty);

        IMyIterator<int> iterator = filtered.GetIterator();
        Assert.Equal(2, iterator.Next());
        Assert.Equal(4, iterator.Next());
        Assert.False(iterator.HasNext());
    }

    [Fact]
    public void FindBy_FindsExpectedItem()
    {
        BinarySearchTreeCollection<string> collection = new();

        collection.Add("todo");
        collection.Add("doing");
        collection.Add("done");

        string? result = collection.FindBy("doing", (item, key) => item == key);

        Assert.NotNull(result);
        Assert.Equal("doing", result);
    }

    [Fact]
    public void Reduce_SumsValues()
    {
        BinarySearchTreeCollection<int> collection = new();

        collection.Add(5);
        collection.Add(10);
        collection.Add(20);

        int sum = collection.Reduce(0, (acc, value) => acc + value);

        Assert.Equal(35, sum);
    }

    [Fact]
    public void Remove_RootWithTwoChildren_Works()
    {
        BinarySearchTreeCollection<int> collection = new();

        collection.Add(20);
        collection.Add(10);
        collection.Add(30);
        collection.Add(25);
        collection.Add(40);

        bool removed = collection.Remove(20);

        Assert.True(removed);
        Assert.Equal(4, collection.Count);

        // contract behavior check: insertion order of remaining items
        IMyIterator<int> iterator = collection.GetIterator();
        Assert.Equal(10, iterator.Next());
        Assert.Equal(30, iterator.Next());
        Assert.Equal(25, iterator.Next());
        Assert.Equal(40, iterator.Next());
        Assert.False(iterator.HasNext());
    }
}