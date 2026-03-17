using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.BinarySearchTrees;

internal sealed class BinarySearchTreeIterator<T> : IMyIterator<T>
{
    private readonly BinarySearchTreeNode<T>? _head;
    private BinarySearchTreeNode<T>? _current;
    private bool _started;

    public BinarySearchTreeIterator(BinarySearchTreeNode<T>? head)
    {
        _head = head;
        _current = null;
        _started = false;
    }

    public bool HasNext()
    {
        if (!_started)
        {
            return _head is not null;
        }

        return _current?.NextInserted is not null;
    }

    public T Next()
    {
        if (!_started)
        {
            _current = _head;
            _started = true;
        }
        else
        {
            _current = _current?.NextInserted;
        }

        if (_current is null)
        {
            throw new InvalidOperationException("No more elements.");
        }

        return _current.Value;
    }

    public void Reset()
    {
        _current = null;
        _started = false;
    }
}