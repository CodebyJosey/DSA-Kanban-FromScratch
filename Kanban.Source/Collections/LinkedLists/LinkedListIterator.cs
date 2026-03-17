using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.LinkedLists;

internal sealed class LinkedListIterator<T> : IMyIterator<T>
{
    private readonly LinkedListNode<T>? _headNode;
    private LinkedListNode<T>? _currentNode;
    private bool _started;

    internal LinkedListIterator(LinkedListNode<T>? head)
    {
        _headNode = head;
        _currentNode = null;
        _started = false;
    }

    public bool HasNext()
    {
        if (!_started)
        {
            return _headNode is not null;
        }

        return _currentNode?.Next is not null;
    }

    public T Next()
    {
        if (!_started)
        {
            _currentNode = _headNode;
            _started = true;
        }
        else
        {
            _currentNode = _currentNode?.Next;
        }

        if (_currentNode is null)
        {
            throw new InvalidOperationException("No more elements.");
        }

        return _currentNode.Value;
    }

    public void Reset()
    {
        _currentNode = null;
        _started = false;
    }
}