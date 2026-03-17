namespace Kanban.Source.Collections.BinarySearchTrees;

internal sealed class BinarySearchTreeNode<T>
{
    public BinarySearchTreeNode(T value)
    {
        Value = value;
    }

    public T Value { get; set; }

    public BinarySearchTreeNode<T>? Left { get; set; }
    public BinarySearchTreeNode<T>? Right { get; set; }
    public BinarySearchTreeNode<T>? Parent { get; set; }

    // For iterator / insertion order contract
    public BinarySearchTreeNode<T>? PreviousInserted { get; set; }
    public BinarySearchTreeNode<T>? NextInserted { get; set; }
}