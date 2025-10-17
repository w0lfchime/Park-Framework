using System;
using System.Collections.Generic;

public class SetStateRequestHeap
{
	private List<(SetStateRequest request, int lifeTime)> heap = new();

	public int Count => heap.Count;

	public void Enqueue(SetStateRequest request, int lifeTime)
	{
		heap.Add((request, lifeTime));
		HeapifyUp(heap.Count - 1);
	}

	public SetStateRequest Dequeue()
	{
		if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");

		SetStateRequest top = heap[0].request;
		heap[0] = heap[^1];
		heap.RemoveAt(heap.Count - 1);
		HeapifyDown(0);
		return top;
	}

	public SetStateRequest Peek() //Change to GetHighestPriority
	{
		if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");
		return heap[0].request;
	}

	public void FixedFrameUpdate()
	{
		for (int i = heap.Count - 1; i >= 0; i--)
		{
			heap[i] = (heap[i].request, heap[i].lifeTime - 1); // Directly modify lifeTime

			if (heap[i].lifeTime <= 0)
			{
				heap.RemoveAt(i); // Remove expired entry
			}
		}
			
		Rebuild();
	}
	public void ClearClearOnSetState()
	{
		for (int i = heap.Count - 1; i >= 0; i--)
		{
			if (heap[i].request.ClearOnSetState == true)
			{
				heap.RemoveAt(i); // Remove expired entry
			}
		}

		Rebuild();
	}
	




	private void HeapifyUp(int index)
	{
		while (index > 0)
		{
			int parent = (index - 1) / 2;
			if (heap[index].request.PushForce > heap[parent].request.PushForce)
			{
				(heap[index], heap[parent]) = (heap[parent], heap[index]);
				index = parent;
			}
			else break;
		}
	}

	private void HeapifyDown(int index)
	{
		while (index < heap.Count)
		{
			int left = 2 * index + 1, right = 2 * index + 2, largest = index;

			if (left < heap.Count && heap[left].request.PushForce > heap[largest].request.PushForce)
				largest = left;

			if (right < heap.Count && heap[right].request.PushForce > heap[largest].request.PushForce)
				largest = right;

			if (largest == index) break;

			(heap[index], heap[largest]) = (heap[largest], heap[index]);
			index = largest;
		}
	}
	private void Rebuild()
	{
		// Rebuild the heap after removals
		for (int i = heap.Count / 2 - 1; i >= 0; i--)
		{
			HeapifyDown(i);
		}
	}

	public void Clear() => heap.Clear();


}


