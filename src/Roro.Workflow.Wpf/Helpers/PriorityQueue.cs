using System;
using System.Collections.Generic;
using System.Linq;

namespace Roro.Workflow.Wpf
{
    public sealed class PriorityQueue<T>
    {
        private readonly SortedList<double, Queue<T>> _items;

        public int Count => this._items.SelectMany(x => x.Value).Count();

        public PriorityQueue()
        {
            this._items = new SortedList<double, Queue<T>>();
        }

        public void Enqueue(T item, double priority)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (this._items.TryGetValue(priority, out Queue<T> existingQueue))
            {
                existingQueue.Enqueue(item);
            }
            else
            {
                var newQueue = new Queue<T>();
                this._items.Add(priority, newQueue);
                newQueue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            var first = this._items.First();
            var queue = first.Value;
            var item = queue.Dequeue();
            if (queue.Count == 0)
            {
                this._items.Remove(first.Key);
            }
            return item;
        }

        public void Clear()
        {
            this._items.Clear();
        }
    }
}