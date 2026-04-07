using Kanban.Source.Collections.HashMaps;
using Kanban.Source.Interfaces;

namespace Kanban.Tests.Collections.Contracts;

/// <summary>
/// Contract tests for <see cref="HashMapCollection{T}"/>.
/// </summary>
public sealed class HashMapCollectionContractTests : MyCollectionContractTests
{
    /// <inheritdoc/>
    protected override IMyCollection<int> CreateIntCollection()
    {
        return new HashMapCollection<int>();
    }

    /// <inheritdoc/>
    protected override IMyCollection<string> CreateStringCollection()
    {
        return new HashMapCollection<string>();
    }
}