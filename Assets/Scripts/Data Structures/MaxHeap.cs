using System;
using System.Collections.Generic;

public class MaxHeap<T> where T : IComparable<T>
{
	private List<T> _heap;

	public int Count => _heap.Count;
	public bool IsEmpty => _heap.Count == 0;

	public MaxHeap()
	{
		_heap = new List<T>();
	}

	public MaxHeap(IEnumerable<T> collection)
	{
		_heap = new List<T>(collection);
		for (int i = Parent(_heap.Count - 1); i >= 0; i--)
		{
			HeapifyDown(i);
		}
	}

	public void Insert(T value)
	{
		_heap.Add(value);
		HeapifyUp(_heap.Count - 1);
	}

	public T ExtractMax()
	{
		if (IsEmpty)
			throw new InvalidOperationException("Heap is empty");

		T max = _heap[0];
		_heap[0] = _heap[_heap.Count - 1];
		_heap.RemoveAt(_heap.Count - 1);
		HeapifyDown(0);

		return max;
	}

	public T Peek()
	{
		if (IsEmpty)
			throw new InvalidOperationException("Heap is empty");
		return _heap[0];
	}

	private void HeapifyUp(int index)
	{
		while (index > 0)
		{
			int parentIndex = Parent(index);
			if (_heap[index].CompareTo(_heap[parentIndex]) <= 0)
				break;

			Swap(index, parentIndex);
			index = parentIndex;
		}
	}

	private void HeapifyDown(int index)
	{
		while (true)
		{
			int left = LeftChild(index);
			int right = RightChild(index);
			int largest = index;

			if (left < _heap.Count && _heap[left].CompareTo(_heap[largest]) > 0)
				largest = left;
			if (right < _heap.Count && _heap[right].CompareTo(_heap[largest]) > 0)
				largest = right;

			if (largest == index)
				break;

			Swap(index, largest);
			index = largest;
		}
	}

	private void Swap(int i, int j)
	{
		T temp = _heap[i];
		_heap[i] = _heap[j];
		_heap[j] = temp;
	}

	private int Parent(int index) => (index - 1) / 2;
	private int LeftChild(int index) => 2 * index + 1;
	private int RightChild(int index) => 2 * index + 2;
}
