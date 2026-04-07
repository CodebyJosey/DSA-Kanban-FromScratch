using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.HashMaps;

/// <summary>
/// Hash-map-backed implementation of <see cref="IMyCollection{T}"/>.
/// Iteration order is insertion order.
/// </summary>
public sealed class HashMapCollection<T> : IMyCollection<T>
{
    private const int DefaultCapacity = 16;
    private const double MaxLoadFactor = 0.75;

    private HashMapNode<T>?[] _buckets;
    private HashMapNode<T>? _firstInserted;
    private HashMapNode<T>? _lastInserted;
    private int _count;

    /// <summary>
    /// Creates a new hash map collection.
    /// </summary>
    public HashMapCollection(int initialCapacity = DefaultCapacity)
    {
        if (initialCapacity < 1)
        {
            initialCapacity = 1;
        }

        _buckets = new HashMapNode<T>?[initialCapacity];
        _count = 0;
        Dirty = false;
    }

    /// <inheritdoc/>
    public int Count => _count;

    /// <inheritdoc/>
    public bool Dirty { get; set; }

    /// <inheritdoc/>
    public void Add(T item)
    {
        EnsureCapacity(_count + 1);

        HashMapNode<T> node = new HashMapNode<T>(item);
        int bucketIndex = ComputeBucketIndex(item, _buckets.Length);

        node.NextBucket = _buckets[bucketIndex];
        _buckets[bucketIndex] = node;

        AppendInsertionNode(node);

        _count++;
        Dirty = true;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        HashMapNode<T>? target = FindFirstInsertionNode(item);
        if (target == null)
        {
            return false;
        }

        RemoveFromBucketChain(target);
        RemoveFromInsertionChain(target);

        _count--;
        Dirty = true;
        return true;
    }

    /// <inheritdoc/>
    public T? FindBy<TKey>(TKey key, Func<T, TKey, bool> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        HashMapNode<T>? current = _firstInserted;
        while (current != null)
        {
            if (comparer(current.Value, key))
            {
                return current.Value;
            }

            current = current.NextInserted;
        }

        return default;
    }

    /// <inheritdoc/>
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        HashMapCollection<T> result = new HashMapCollection<T>();
        HashMapNode<T>? current = _firstInserted;

        while (current != null)
        {
            if (predicate(current.Value))
            {
                result.Add(current.Value);
            }

            current = current.NextInserted;
        }

        result.Dirty = false;
        return result;
    }

    /// <inheritdoc/>
    public void Sort(Comparison<T> comparison)
    {
        ArgumentNullException.ThrowIfNull(comparison);

        if (_count <= 1)
        {
            Dirty = true;
            return;
        }

        T[] values = new T[_count];
        CopyInsertionOrderToArray(values);
        BubbleSort(values, comparison);

        ClearInternal();

        for (int i = 0; i < values.Length; i++)
        {
            Add(values[i]);
        }

        Dirty = true;
    }

    /// <inheritdoc/>
    public TResult Reduce<TResult>(TResult initial, Func<TResult, T, TResult> accumulator)
    {
        ArgumentNullException.ThrowIfNull(accumulator);

        TResult result = initial;
        HashMapNode<T>? current = _firstInserted;

        while (current != null)
        {
            result = accumulator(result, current.Value);
            current = current.NextInserted;
        }

        return result;
    }

    /// <inheritdoc/>
    public IMyIterator<T> GetIterator()
    {
        return new HashMapIterator<T>(_firstInserted);
    }

    private void EnsureCapacity(int requiredCount)
    {
        int currentCapacity = _buckets.Length;
        int threshold = (int)(currentCapacity * MaxLoadFactor);

        if (requiredCount <= threshold)
        {
            return;
        }

        int newCapacity = currentCapacity * 2;
        if (newCapacity < requiredCount)
        {
            newCapacity = requiredCount;
        }

        Rehash(newCapacity);
    }

    private void Rehash(int newCapacity)
    {
        HashMapNode<T>?[] newBuckets = new HashMapNode<T>?[newCapacity];
        HashMapNode<T>? current = _firstInserted;

        while (current != null)
        {
            int newIndex = ComputeBucketIndex(current.Value, newCapacity);
            current.NextBucket = newBuckets[newIndex];
            newBuckets[newIndex] = current;
            current = current.NextInserted;
        }

        _buckets = newBuckets;
    }

    private void AppendInsertionNode(HashMapNode<T> node)
    {
        if (_firstInserted == null)
        {
            _firstInserted = node;
            _lastInserted = node;
            return;
        }

        node.PreviousInserted = _lastInserted;
        _lastInserted!.NextInserted = node;
        _lastInserted = node;
    }

    private HashMapNode<T>? FindFirstInsertionNode(T item)
    {
        HashMapNode<T>? current = _firstInserted;
        while (current != null)
        {
            if (object.Equals(current.Value, item))
            {
                return current;
            }

            current = current.NextInserted;
        }

        return null;
    }

    private void RemoveFromBucketChain(HashMapNode<T> target)
    {
        int bucketIndex = ComputeBucketIndex(target.Value, _buckets.Length);
        HashMapNode<T>? current = _buckets[bucketIndex];
        HashMapNode<T>? previous = null;

        while (current != null)
        {
            if (ReferenceEquals(current, target))
            {
                if (previous == null)
                {
                    _buckets[bucketIndex] = current.NextBucket;
                }
                else
                {
                    previous.NextBucket = current.NextBucket;
                }

                current.NextBucket = null;
                return;
            }

            previous = current;
            current = current.NextBucket;
        }
    }

    private void RemoveFromInsertionChain(HashMapNode<T> target)
    {
        HashMapNode<T>? previous = target.PreviousInserted;
        HashMapNode<T>? next = target.NextInserted;

        if (previous == null)
        {
            _firstInserted = next;
        }
        else
        {
            previous.NextInserted = next;
        }

        if (next == null)
        {
            _lastInserted = previous;
        }
        else
        {
            next.PreviousInserted = previous;
        }

        target.NextInserted = null;
        target.PreviousInserted = null;
    }

    private static int ComputeBucketIndex(T value, int bucketCount)
    {
        if (value == null)
        {
            return 0;
        }

        int hash = value.GetHashCode() & 0x7fffffff;
        return hash % bucketCount;
    }

    private void CopyInsertionOrderToArray(T[] destination)
    {
        int index = 0;
        HashMapNode<T>? current = _firstInserted;

        while (current != null)
        {
            destination[index++] = current.Value;
            current = current.NextInserted;
        }
    }

    private static void BubbleSort(T[] items, Comparison<T> comparison)
    {
        for (int i = 0; i < items.Length; i++)
        {
            for (int j = 0; j < items.Length - 1 - i; j++)
            {
                if (comparison(items[j], items[j + 1]) > 0)
                {
                    T temp = items[j];
                    items[j] = items[j + 1];
                    items[j + 1] = temp;
                }
            }
        }
    }

    private void ClearInternal()
    {
        _buckets = new HashMapNode<T>?[DefaultCapacity];
        _firstInserted = null;
        _lastInserted = null;
        _count = 0;
    }
}