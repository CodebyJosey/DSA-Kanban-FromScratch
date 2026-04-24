namespace Kanban.Source.Collections.HashMap;
/// <summary>
/// Forms keyvalue s
/// </summary>
public class KeyValue<TKey, TValue>
{
    /// <summary>
    /// The hashmap key
    /// </summary>
    public TKey Key { get; set; }
    /// <summary>
    /// The hashmap value
    /// </summary>
    public TValue Value { get; set; }

    /// <summary>
    /// Is deleted field
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Constructor for the KeyValue
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public KeyValue(TKey key, TValue value)
    {
        Key = key;
        Value = value;
        IsDeleted = false;
    }
}