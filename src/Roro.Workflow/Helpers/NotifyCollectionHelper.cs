using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class NotifyCollectionHelper<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        private readonly List<T> _items;

        private bool _changing;

        public event NotifyCollectionChangingEventHandler CollectionChanging;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private bool OnCollectionChanging(NotifyCollectionChangedAction action, T changingItem, int index)
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    try
                    {
                        if (this._changing)
                        {
                            throw new InvalidOperationException("The collection does not allow concurrent changes.");
                        }
                        var e = new NotifyCollectionChangingEventArgs(action, changingItem, index);
                        this._changing = true;
                        this.CollectionChanging?.Invoke(this, e);
                        this._changing = false;
                        return e.Cancel;
                    }
                    catch (Exception ex)
                    {
                        this._changing = false;
                        throw ex;
                    }

                default:
                    throw new NotSupportedException("The collection does not support '" + action + "' action.");
            }
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, T changedItem, int index)
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    var e = new NotifyCollectionChangedEventArgs(action, changedItem, index);
                    this.CollectionChanged?.Invoke(this, e);
                    break;

                default:
                    throw new NotSupportedException("The collection does not support '" + action + "' action.");
            }
        }

        public NotifyCollectionHelper() : this(null)
        {
            ;
        }

        public NotifyCollectionHelper(IEnumerable<T> items)
        {
            this._items = items == null ? new List<T>() : new List<T>(items);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._items.GetEnumerator();
        }

        public void Add(T item)
        {
            this.Insert(this._items.Count, item);
        }

        public void Insert(int index, T item)
        {
            var cancel = this.OnCollectionChanging(NotifyCollectionChangedAction.Add, item, index);
            if (cancel)
            {
                return;
            }
            this._items.Insert(index, item);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        public void Remove(T item)
        {
            this.RemoveAt(this.IndexOf(item));
        }

        public void RemoveAt(int index)
        {
            var item = this._items[index];
            var cancel = this.OnCollectionChanging(NotifyCollectionChangedAction.Remove, item, index);
            if (cancel)
            {
                return;
            }
            this._items.RemoveAt(index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        public void RemoveAll(Func<T, bool> predicate)
        {
            this._items.Where(predicate).ToList().ForEach(x => this.Remove(x));
        }

        public void Clear()
        {
            this.RemoveAll(x => true);
        }

        public int IndexOf(T item)
        {
            return this._items.IndexOf(item);
        }

        public bool Contains(T item)
        {
            return this._items.Contains(item);
        }

        public bool Contains(Func<T, bool> predicate)
        {
            return this._items.FirstOrDefault(predicate) is T;
        }

        public void ForEach(Action<T> action)
        {
            this._items.ForEach(action);
        }
    }

    public delegate void NotifyCollectionChangingEventHandler(object sender, NotifyCollectionChangingEventArgs e);

    public sealed class NotifyCollectionChangingEventArgs : NotifyCollectionChangedEventArgs
    {
        private bool _cancel;
        public bool Cancel
        {
            get => this._cancel;
            set => this._cancel = this._cancel || value;
        }

        public NotifyCollectionChangingEventArgs(NotifyCollectionChangedAction action, object changingItem, int index) : base(action, changingItem, index)
        {
            ;
        }
    }
}
