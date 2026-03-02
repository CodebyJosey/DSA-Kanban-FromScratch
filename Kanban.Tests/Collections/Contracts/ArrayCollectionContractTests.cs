using Kanban.Source.Collections.Arrays;
using Kanban.Source.Interfaces;

namespace Kanban.Tests.Collections.Contracts;

/// <summary>
/// 
/// </summary>
public sealed class ArrayCollectionContractTests : MyCollectionContractTests
{
    /// <inheritdoc/>
    protected override IMyCollection<int> CreateIntCollection()
    {
        return new ArrayCollection<int>();
    }

    /// <inheritdoc/>
    protected override IMyCollection<string> CreateStringCollection()
    {
        return new ArrayCollection<string>();
    }
}