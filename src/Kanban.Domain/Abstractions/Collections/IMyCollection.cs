namespace Kanban.Domain.Abstractions.Collections;

/// <summary>
/// Represents a custom collection abstraction. Since built-in generic collections are 
/// not allowed, all storage/manipulations was implemented by our team.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public interface IMyCollection<T>
{
    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    void Add(T item);

    /// <summary>
    /// Removes the first occurence of an item from the collection.
    /// </summary>
    /// <returns><c>true</c> if an item was removed; otherwise <c>false</c>.</returns>
    bool Remove(T item);

    /// <summary>
    /// Returns the number of items in the collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Indicates the collection has been modified since last persistence / flush.
    /// </summary>
    bool Dirty { get; set; }

    /// <summary>
    /// Finds an item by using a custom comparer.
    /// </summary>
    T? FindBy<TKey>(TKey key, Func<T, TKey, bool> comparer);

    /// <summary>
    /// Creates a filtered collection using a predicate.
    /// </summary>
    IMyCollection<T> Filter(Func<T, bool> predicate);

    /// <summary>
    /// Sorts the collection using the given comparison.
    /// </summary>
    void Sort(Comparison<T> comparison);

    /// <summary>
    /// Reduces the collection into a single value.
    /// </summary>
    TResult Reduce<TResult>(TResult initial, Func<TResult, T, TResult> accumulator);

    /// <summary>
    /// Returns a custom iterator for this collection.
    /// </summary>
    IMyIterator<T> GetIterator();
}