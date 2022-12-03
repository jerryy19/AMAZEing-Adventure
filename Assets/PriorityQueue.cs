using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS PQ IS A MIN HEAP
// IN A MIN HEAP, LOWER PRIORITY IS BETTER

public class PriorityQueue<T> {
    
    // Entry<T, int> is a tuple of some data and priority
    List<Entry<T, int>> pq = new List<Entry<T, int>>();
    public int Count = 0;

    public void push(T data, int priority) {
        pq.Add(new Entry<T, int>(data, priority));
        Count++;
        bubbleUp(Count - 1);
    }


    public Entry<T, int> pop() {
        if (Count == 0) {
            return null;
        }

        swap(0, Count - 1);
        Entry<T, int> toReturn = pq[Count - 1];
        pq.RemoveAt(Count - 1);
        Count--;

        if (Count != 0) {
            bubbleDown(0);
        }

        return toReturn;
    }

    public Entry<T, int> peek() {
        return pq[0];
    }

    public bool contains(T item) {
        return indexOf(item) != -1;
    }

    public int indexOf(T item) {
        for (int i = 0; i < Count; i++) {
            if (pq[i].key.Equals(item)) {
                return i;
            }
        }
        return -1;
    }

    public void remove(T item) {
        int index = indexOf(item);
        if (index != -1) {
            swap(index, Count - 1);
            pq.RemoveAt(Count - 1);
            Count--;
            
            if (Count != 0) {
                bubbleDown(index);
            }
        }
    }

    void bubbleUp(int index) {
        if (index > 0) {
            int parentIndex = (index - 1) / 2;
            if (pq[index].value < pq[parentIndex].value) {
                swap(index, parentIndex);
                bubbleUp(parentIndex);
            }
        }
    }

    void bubbleDown(int index) {
        int leftIndex = 2 * index + 1;
        int rightIndex = 2 * index + 2;
        int toSwap = index;

        if (leftIndex < Count && pq[leftIndex].value < pq[index].value) {
            toSwap = leftIndex;
        }

        if (rightIndex < Count && pq[rightIndex].value < pq[toSwap].value) {
            toSwap = rightIndex;
        }

        if (toSwap != index) {
            swap(index, toSwap);
            bubbleDown(toSwap);
        }

    }

    void swap(int index1, int index2) {
        Entry<T, int> e1 = pq[index1];
        Entry<T, int> e2 = pq[index2];
        pq[index1] = e2;
        pq[index2] = e1;
    }


}

public class Entry<K, V> {
    public K key;
    public V value;

    public Entry(K key, V value) {
        this.key = key;
        this.value = value;
    }
}
