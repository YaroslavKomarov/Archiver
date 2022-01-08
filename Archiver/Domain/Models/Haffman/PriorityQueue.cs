using System;
using System.Collections.Generic;

namespace Archiver.Domain.Models.Haffman
{
    public class PriorityQueue<T>
    {
        public int Size { get; private set; }
        SortedDictionary<int, Queue<T>> storage;
        public PriorityQueue()
        {
            storage = new SortedDictionary<int, Queue<T>>();
            Size = 0;
        }

        public void Enqueue(int priority, T item)
        {
            if (!storage.ContainsKey(priority))
                storage.Add(priority, new Queue<T>());
            storage[priority].Enqueue(item);
            Size++;
        }

        public T Dequeue()
        {
            if (Size == 0) 
                throw new Exception("Queue is empty");
            Size--;
            foreach (var q in storage.Values)
                if (q.Count > 0)
                    return q.Dequeue();
            throw new Exception("Queue error");
        }
    }
}
