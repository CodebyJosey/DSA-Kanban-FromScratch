using Kanban.Source.Collections.LinkedLists;
using Kanban.Source.Interfaces;

namespace Kanban.Tests.Collections.Contracts;

/// <summary>
/// 
/// </summary>
public sealed class LinkedListCollectionContractTests : MyCollectionContractTests
{
    /// <inheritdoc/>
    protected override IMyCollection<int> CreateIntCollection()
    {
        return new LinkedListCollection<int>();
    }

    /// <inheritdoc/>
    protected override IMyCollection<string> CreateStringCollection()
    {
        return new LinkedListCollection<string>();
    }
}