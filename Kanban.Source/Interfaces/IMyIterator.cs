namespace Kanban.Source.Interfaces;

/// <summary>
/// Custom iterator abstraction (since System.Collections.Generic iterators are not allowed).
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public interface IMyIterator<T>
{
    /// <summary>
    /// Returns <c>true</c> if there is another element.
    /// </summary>
    bool HasNext();

    /// <summary>
    /// Returns the next element.
    /// </summary>
    T Next();

    /// <summary>
    /// Resets the iterator to the beginning.
    /// </summary>
    void Reset();
}