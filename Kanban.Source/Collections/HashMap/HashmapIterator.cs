using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.HashMap;

/// <summary>
/// Iterator implementation for hashmap-backed collections.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>

public class HashmapIterator<T> : IMyIterator<T>
{
    /// <inheritdoc/>
    public bool HasNext() => throw new NotImplementedException();

    /// <inheritdoc/>
    public T Next() => throw new NotImplementedException();

    /// <inheritdoc/>
    public void Reset() => throw new NotImplementedException();
}
