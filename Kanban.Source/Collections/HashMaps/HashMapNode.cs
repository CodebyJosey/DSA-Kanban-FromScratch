namespace Kanban.Source.Collections.HashMaps;

/// <summary>
/// Internal node used by <see cref="HashMapCollection{T}"/>.
/// </summary>
internal sealed class HashMapNode<T>
{
    public T Value { get; set; }
    public HashMapNode<T>? NextBucket { get; set; }
    public HashMapNode<T>? NextInserted { get; set; }
    public HashMapNode<T>? PreviousInserted { get; set; }

    public HashMapNode(T value)
    {
        Value = value;
    }
}