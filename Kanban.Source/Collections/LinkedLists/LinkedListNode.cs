namespace Kanban.Source.Collections.LinkedLists;

internal sealed class LinkedListNode<T>
{
    public LinkedListNode(
        T value,
        LinkedListNode<T>? previous = null,
        LinkedListNode<T>? next = null)
    {
        Value = value;
        Previous = previous;
        Next = next;
    }

    public T Value { get; set; }

    public LinkedListNode<T>? Previous { get; set; }

    public LinkedListNode<T>? Next { get; set; }
}
