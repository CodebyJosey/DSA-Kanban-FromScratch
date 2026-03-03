namespace Kanban.Source.Interfaces;
/// <summary>
/// Represents a generic hash map.
/// </summary>
/// <typeparam name="TValue">The type of the values.</typeparam>
/// <typeparam name="TKey">The type of the keys.</typeparam>
public interface IHashMap<TKey, TValue>
{
    /// <summary>
    /// The amount of items in the hashmap
    /// </summary>
    int Count {get; }

    /// <summary>
    /// Presents if the hashmap has been modified since instatiation
    /// </summary>
    bool Dirty {get; set;}

    /// <summary>
    /// Returns all keys in the hashmap
    /// </summary>
    /// <returns>array of type K</returns>
    IEnumerable<TKey> GetKeys();

    /// <summary>
    /// Returns all values in the hashmap
    /// </summary>
    /// <returns>array of type T</returns>
    IEnumerable<TValue> GetValues();

    /// <summary>
    /// Adds item to the hashmap
    /// </summary>
    void Add(TKey key, TValue value);

    /// <summary>
    /// Removes item with the given key
    /// </summary>
    /// <param name="key"></param>
    /// <returns>true if succesfull false when not</returns>
    bool Remove(TKey key);

    /// <summary>
    /// Filters hashmap values based on function
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHashMap<TKey, TValue> Filter(Func<TValue, bool> predicate);

    /// <summary>
    /// Returns a custom iterator for this hashmap
    /// </summary>
    /// <returns></returns>
    IMyIterator<KeyValuePair<TKey, TValue>> GetIterator();
}
