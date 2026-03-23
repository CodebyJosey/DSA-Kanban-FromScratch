namespace Kanban.Source.Collections.HashMap;
/// <summary>
/// Forms keyvalue s
/// </summary>
public class KeyValue<TKey, TValue> : IComparable
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
    /// Constructor for the KeyValue
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public KeyValue(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
    /// <inheritdoc/>
    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;

        if (obj is not KeyValue<TKey, TValue> other)
            throw new ArgumentException("Object is not a compatible KeyValue");

        if (Value is IComparable<TValue> genericComparable)
            return genericComparable.CompareTo(other.Value);

        if (Value is IComparable comparableValue)
            return comparableValue.CompareTo(other.Value);

        throw new InvalidOperationException("TValue is not comparable");
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not KeyValuePair<TKey, TValue> other)
            return false;

        return Equals(Key, other.Key) && Equals(Value, other.Value);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Key, Value);
    }
}