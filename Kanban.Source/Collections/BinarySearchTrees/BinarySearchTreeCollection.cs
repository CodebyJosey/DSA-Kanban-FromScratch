using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Kanban.Source.Interfaces;

namespace Kanban.Source.Collections.BinarySearchTrees;

/// <summary>
/// Binary-search-tree-backed implementation of IMyCollection.
/// This implementation does not use System.Collections.Generic.
/// 
/// Important:
/// - Tree storage is used internally.
/// - Iteration follows insertion order to satisfy the IMyCollection contract tests.
/// - Remove removes the first matching item in insertion order.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public sealed class BinarySearchTreeCollection<T> : IMyCollection<T>
{
    private BinarySearchTreeNode<T>? _root;
    private BinarySearchTreeNode<T>? _firstInserted;
    private BinarySearchTreeNode<T>? _lastInserted;
    private int _count;
    private Comparison<T> _treeComparison;

    /// <summary>
    /// Creates a new BST collection using the default comparison strategy.
    /// The default strategy supports common comparable runtime types like:
    /// string, int, long, double, decimal, float, DateTime, Guid.
    /// For custom models, prefer the constructor overload that accepts Comparison&lt;T&gt;.
    /// </summary>
    public BinarySearchTreeCollection()
        : this(DefaultCompare)
    {
    }

    /// <summary>
    /// Creates a new BST collection using the provided tree comparison.
    /// </summary>
    public BinarySearchTreeCollection(Comparison<T> comparison)
    {
        ArgumentNullException.ThrowIfNull(comparison);

        _treeComparison = comparison;
        _root = null;
        _firstInserted = null;
        _lastInserted = null;
        _count = 0;
        Dirty = false;
    }

    /// <summary>
    /// Returns the number of items in the collection.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Indicates whether the collection was modified.
    /// </summary>
    public bool Dirty { get; set; }

    /// <summary>
    /// Adds an item to the collection.
    /// Tree insertion uses the internal tree comparison.
    /// Iterator order remains insertion order.
    /// </summary>
    public void Add(T item)
    {
        BinarySearchTreeNode<T> newNode = new BinarySearchTreeNode<T>(item);

        InsertIntoTree(newNode);
        AppendToInsertionChain(newNode);

        _count++;
        Dirty = true;
    }

    /// <summary>
    /// Removes the first matching item in insertion order.
    /// Returns true if an item was removed.
    /// </summary>
    public bool Remove(T item)
    {
        BinarySearchTreeNode<T>? nodeToRemove = FindFirstNodeInInsertionOrder(item);
        if (nodeToRemove is null)
        {
            return false;
        }

        RemoveFromInsertionChain(nodeToRemove);
        RemoveNodeFromTree(nodeToRemove);

        _count--;
        Dirty = true;
        return true;
    }

    /// <summary>
    /// Finds an item using the provided comparer.
    /// Because the contract accepts an arbitrary comparer delegate,
    /// this method traverses the collection in insertion order.
    /// </summary>
    public T? FindBy<TKey>(TKey key, Func<T, TKey, bool> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        BinarySearchTreeNode<T>? current = _firstInserted;
        while (current is not null)
        {
            if (comparer(current.Value, key))
            {
                return current.Value;
            }

            current = current.NextInserted;
        }

        return default;
    }

    /// <summary>
    /// Creates a filtered collection of the same type.
    /// The filtered collection is returned with Dirty = false.
    /// </summary>
    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        BinarySearchTreeCollection<T> result = new BinarySearchTreeCollection<T>(_treeComparison);

        BinarySearchTreeNode<T>? current = _firstInserted;
        while (current is not null)
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

    /// <summary>
    /// Sorts the collection using the supplied comparison.
    /// After sorting, iteration order becomes the sorted order.
    /// The internal tree is rebuilt using that same comparison.
    /// </summary>
    public void Sort(Comparison<T> comparison)
    {
        ArgumentNullException.ThrowIfNull(comparison);

        if (_count <= 1)
        {
            Dirty = true;
            _treeComparison = comparison;
            return;
        }

        T[] values = new T[_count];
        CopyInsertionOrderToArray(values);

        BubbleSort(values, comparison);

        ClearInternal();

        _treeComparison = comparison;

        for (int i = 0; i < values.Length; i++)
        {
            Add(values[i]);
        }

        Dirty = true;
    }

    /// <summary>
    /// Reduces the collection to a single value.
    /// Traversal is in insertion order.
    /// </summary>
    public TResult Reduce<TResult>(TResult initial, Func<TResult, T, TResult> accumulator)
    {
        ArgumentNullException.ThrowIfNull(accumulator);

        TResult result = initial;
        BinarySearchTreeNode<T>? current = _firstInserted;

        while (current is not null)
        {
            result = accumulator(result, current.Value);
            current = current.NextInserted;
        }

        return result;
    }

    /// <summary>
    /// Returns an iterator that traverses the collection in insertion order.
    /// </summary>
    public IMyIterator<T> GetIterator()
    {
        return new BinarySearchTreeIterator<T>(_firstInserted);
    }

    private void InsertIntoTree(BinarySearchTreeNode<T> newNode)
    {
        if (_root is null)
        {
            _root = newNode;
            return;
        }

        BinarySearchTreeNode<T>? current = _root;
        BinarySearchTreeNode<T>? parent = null;

        while (current is not null)
        {
            parent = current;

            int compareResult = _treeComparison(newNode.Value, current.Value);

            if (compareResult < 0)
            {
                current = current.Left;
            }
            else
            {
                current = current.Right;
            }
        }

        newNode.Parent = parent;

        if (parent is null)
        {
            _root = newNode;
            return;
        }

        if (_treeComparison(newNode.Value, parent.Value) < 0)
        {
            parent.Left = newNode;
        }
        else
        {
            parent.Right = newNode;
        }
    }

    private void AppendToInsertionChain(BinarySearchTreeNode<T> newNode)
    {
        if (_firstInserted is null)
        {
            _firstInserted = newNode;
            _lastInserted = newNode;
            return;
        }

        newNode.PreviousInserted = _lastInserted;
        _lastInserted!.NextInserted = newNode;
        _lastInserted = newNode;
    }

    private BinarySearchTreeNode<T>? FindFirstNodeInInsertionOrder(T item)
    {
        BinarySearchTreeNode<T>? current = _firstInserted;

        while (current is not null)
        {
            if (ValuesEqual(current.Value, item))
            {
                return current;
            }

            current = current.NextInserted;
        }

        return null;
    }

    private void RemoveFromInsertionChain(BinarySearchTreeNode<T> node)
    {
        if (node.PreviousInserted is not null)
        {
            node.PreviousInserted.NextInserted = node.NextInserted;
        }
        else
        {
            _firstInserted = node.NextInserted;
        }

        if (node.NextInserted is not null)
        {
            node.NextInserted.PreviousInserted = node.PreviousInserted;
        }
        else
        {
            _lastInserted = node.PreviousInserted;
        }

        node.PreviousInserted = null;
        node.NextInserted = null;
    }

    private void RemoveNodeFromTree(BinarySearchTreeNode<T> node)
    {
        if (node.Left is null)
        {
            Transplant(node, node.Right);
            return;
        }
        if (node.Right is null)
        {
            Transplant(node, node.Left);
            return;
        }

        BinarySearchTreeNode<T> successor = GetMinimum(node.Right);

        if (!ReferenceEquals(successor.Parent, node))
        {
            Transplant(successor, successor.Right);
            successor.Right = node.Right;

            if (successor.Right is not null)
            {
                successor.Right.Parent = successor;
            }
        }

        Transplant(node, successor);
        successor.Left = node.Left;

        if (successor.Left is not null)
        {
            successor.Left.Parent = successor;
        }
    }

    private void Transplant(BinarySearchTreeNode<T> oldNode, BinarySearchTreeNode<T>? replacementNode)
    {
        if (oldNode.Parent is null)
        {
            _root = replacementNode;
        }
        else if (ReferenceEquals(oldNode, oldNode.Parent.Left))
        {
            oldNode.Parent.Left = replacementNode;
        }
        else
        {
            oldNode.Parent.Right = replacementNode;
        }

        if (replacementNode is not null)
        {
            replacementNode.Parent = oldNode.Parent;
        }
    }

    private static BinarySearchTreeNode<T> GetMinimum(BinarySearchTreeNode<T> node)
    {
        BinarySearchTreeNode<T> current = node;

        while (current.Left is not null)
        {
            current = current.Left;
        }

        return current;
    }

    private void CopyInsertionOrderToArray(T[] destination)
    {
        int index = 0;
        BinarySearchTreeNode<T>? current = _firstInserted;

        while (current is not null)
        {
            destination[index] = current.Value;
            index++;
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
        _root = null;
        _firstInserted = null;
        _lastInserted = null;
        _count = 0;
    }

    private static bool ValuesEqual(T left, T right)
    {
        return object.Equals(left, right);
    }

    private static int DefaultCompare(T left, T right)
    {
        if (left is null && right is null)
        {
            return 0;
        }

        if (left is null)
        {
            return -1;
        }

        if (right is null)
        {
            return 1;
        }

        object leftObject = left;
        object rightObject = right;

        if (leftObject is string leftString && rightObject is string rightString)
        {
            return string.CompareOrdinal(leftString, rightString);
        }

        if (leftObject is int leftInt && rightObject is int rightInt)
        {
            if (leftInt < rightInt) return -1;
            if (leftInt > rightInt) return 1;
            return 0;
        }

        if (leftObject is long leftLong && rightObject is long rightLong)
        {
            if (leftLong < rightLong) return -1;
            if (leftLong > rightLong) return 1;
            return 0;
        }

        if (leftObject is double leftDouble && rightObject is double rightDouble)
        {
            if (leftDouble < rightDouble) return -1;
            if (leftDouble > rightDouble) return 1;
            return 0;
        }

        if (leftObject is float leftFloat && rightObject is float rightFloat)
        {
            if (leftFloat < rightFloat) return -1;
            if (leftFloat > rightFloat) return 1;
            return 0;
        }

        if (leftObject is decimal leftDecimal && rightObject is decimal rightDecimal)
        {
            if (leftDecimal < rightDecimal) return -1;
            if (leftDecimal > rightDecimal) return 1;
            return 0;
        }

        if (leftObject is DateTime leftDateTime && rightObject is DateTime rightDateTime)
        {
            if (leftDateTime < rightDateTime) return -1;
            if (leftDateTime > rightDateTime) return 1;
            return 0;
        }

        if (leftObject is Guid leftGuid && rightObject is Guid rightGuid)
        {
            byte[] leftBytes = leftGuid.ToByteArray();
            byte[] rightBytes = rightGuid.ToByteArray();

            for (int i = 0; i < leftBytes.Length; i++)
            {
                if (leftBytes[i] < rightBytes[i]) return -1;
                if (leftBytes[i] > rightBytes[i]) return 1;
            }

            return 0;
        }

        throw new InvalidOperationException(
            $"No default comparison exists for type '{typeof(T).FullName}'. " +
            $"Use the BinarySearchTreeCollection<T>(Comparison<T> comparison) constructor.");
    }
}