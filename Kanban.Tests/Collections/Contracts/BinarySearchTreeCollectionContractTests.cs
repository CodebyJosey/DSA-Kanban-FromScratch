using Kanban.Source.Collections.BinarySearchTrees;
using Kanban.Source.Collections.LinkedLists;
using Kanban.Source.Interfaces;

namespace Kanban.Tests.Collections.Contracts;

/// <summary>
/// Contract tests for <see cref="LinkedListCollection{T}"/>.
/// </summary>
public sealed class BinarySearchTreeCollectionContractTests : MyCollectionContractTests
{
    /// <inheritdoc/>
    protected override IMyCollection<int> CreateIntCollection()
    {
        return new BinarySearchTreeCollection<int>();
    }

    /// <inheritdoc/>
    protected override IMyCollection<string> CreateStringCollection()
    {
        return new BinarySearchTreeCollection<string>();
    }
}
