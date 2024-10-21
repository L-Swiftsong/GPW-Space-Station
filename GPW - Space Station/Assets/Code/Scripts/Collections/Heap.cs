using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    private T[] _items;
    private int _currentItemCount;

    public Heap(int maxHeapSize)
    {
        _items = new T[maxHeapSize];
    }


    public void Add(T item)
    {
        item.HeapIndex = _currentItemCount;
        _items[_currentItemCount] = item;
        SortUp(item);
        _currentItemCount++;
    }
    public T RemoveFirst()
    {
        T firstItem = _items[0];

        // Decrement the current item count.
        _currentItemCount--;

        // Move the last item to the top of the heap.
        _items[0] = _items[_currentItemCount];
        _items[0].HeapIndex = 0;

        // Sort the heap.
        SortDown(_items[0]);

        // Return the original first item.
        return firstItem;
    }
    public void UpdateItem(T item)
    {
        // Sort the heap.
        SortUp(item);
        SortDown(item);
    }
    public void Clear() => _currentItemCount = 0;
    public bool Contains(T item)
    {
        if (item.HeapIndex < _currentItemCount)
        {
            // The heap may contain the item to check. Compare them using Equals().
            return Equals(_items[item.HeapIndex], item);
        }
        else
        {
            // The item cannot be contained within the heap (Since it was last cleared).
            return false;
        }
    }
    public int Count => _currentItemCount;


    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while(true)
        {
            T parentItem = _items[parentIndex];

            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }
    private void SortDown(T item)
    {
        while(true)
        {
            int leftChildIndex = (item.HeapIndex * 2) + 1;
            int rightChildIndex = (item.HeapIndex * 2) + 2;
            int swapIndex = 0;

            if (leftChildIndex < _currentItemCount)
            {
                // The index has at least 1 child.
                swapIndex = leftChildIndex;

                if (rightChildIndex < _currentItemCount)
                {
                    // The index has two children.
                    // The swap index should be the child with the highest priority.
                    swapIndex = _items[leftChildIndex].CompareTo(_items[rightChildIndex]) < 0 ? rightChildIndex : leftChildIndex;
                }


                if (item.CompareTo(_items[swapIndex]) < 0)
                {
                    // The child has a higher priority than the parent.
                    Swap(item, _items[swapIndex]);
                }
                else
                {
                    // The parent has a higher priority than its children. It is in the correct spot.
                    return;
                }
            }
            else
            {
                // The index has no children. It is in the correct spot.
                return;
            }
        }
    }
    private void Swap(T itemA, T itemB)
    {
        // Swap array positions.
        _items[itemA.HeapIndex] = itemB;
        _items[itemB.HeapIndex] = itemA;

        // Swap item HeapIndexes.
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}
