using Kanban.Source.Collections;
using Kanban.Source.Enums;
using Kanban.Source.Interfaces;

namespace Kanban.Tests.Collections.Contracts;

/// <summary>
/// Generic behavior tests for all collection implementations through <see cref="IMyCollection{T}"/>.
/// </summary>
public sealed class MyCollectionFactoryGenericTests
{
    [Theory]
    [InlineData(CollectionImplementation.Array)]
    [InlineData(CollectionImplementation.LinkedList)]
    [InlineData(CollectionImplementation.BinarySearchTree)]
    [InlineData(CollectionImplementation.HashMap)]
    public void AddFindAndReduce_WorkForAllImplementations(CollectionImplementation implementation)
    {
        IMyCollectionFactory factory = new MyCollectionFactory(implementation);
        IMyCollection<int> collection = factory.Create<int>();

        collection.Add(5);
        collection.Add(7);
        collection.Add(11);

        int? found = collection.FindBy(7, static (item, key) => item == key);
        int sum = collection.Reduce(0, static (acc, x) => acc + x);

        Assert.Equal(3, collection.Count);
        Assert.Equal(7, found);
        Assert.Equal(23, sum);
    }

    [Theory]
    [InlineData(CollectionImplementation.Array)]
    [InlineData(CollectionImplementation.LinkedList)]
    [InlineData(CollectionImplementation.BinarySearchTree)]
    [InlineData(CollectionImplementation.HashMap)]
    public void SortAndFilter_WorkForAllImplementations(CollectionImplementation implementation)
    {
        IMyCollectionFactory factory = new MyCollectionFactory(implementation);
        IMyCollection<int> collection = factory.Create<int>();

        collection.Add(4);
        collection.Add(1);
        collection.Add(3);
        collection.Add(2);

        collection.Sort(static (a, b) => a.CompareTo(b));
        IMyCollection<int> filtered = collection.Filter(static x => x % 2 == 0);

        IMyIterator<int> sortedIterator = collection.GetIterator();
        Assert.Equal(1, sortedIterator.Next());
        Assert.Equal(2, sortedIterator.Next());
        Assert.Equal(3, sortedIterator.Next());
        Assert.Equal(4, sortedIterator.Next());
        Assert.False(sortedIterator.HasNext());

        IMyIterator<int> filteredIterator = filtered.GetIterator();
        Assert.Equal(2, filteredIterator.Next());
        Assert.Equal(4, filteredIterator.Next());
        Assert.False(filteredIterator.HasNext());
    }
}