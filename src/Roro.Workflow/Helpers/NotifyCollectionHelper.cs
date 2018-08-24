using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Roro.Workflow
{
    public sealed class NotifyCollectionHelper<T> : IList, IList<T>, INotifyCollectionChanged
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

        #region Implements IList<T>

        public int Count => this._items.Count;

        bool ICollection<T>.IsReadOnly => ((IList<T>)this._items).IsReadOnly;

        public T this[int index]
        {
            get => this._items [index];
            set => throw new NotSupportedException(); // this._items [index] = value;
        }

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
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


        public bool Remove(T item) => this.RemoveAtWithResult(this.IndexOf(item));

        public void RemoveAt(int index) => this.RemoveAtWithResult(index);

        private bool RemoveAtWithResult(int index)
        {
            var item = this._items[index];
            var cancel = this.OnCollectionChanging(NotifyCollectionChangedAction.Remove, item, index);
            if (cancel)
            {
                return false;
            }
            this._items.RemoveAt(index);
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
            return true;
        }

        public void Clear() => this._items.ToList().ForEach(x => this.Remove(x));

        public bool Contains(T item) => this._items.Contains(item);

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => this._items.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => this._items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._items.GetEnumerator();

        #endregion

        #region Implements IList

        bool IList.IsFixedSize => ((IList)this._items).IsFixedSize;

        bool IList.IsReadOnly => ((IList)this._items).IsReadOnly;

        int ICollection.Count => ((IList)this._items).Count;

        bool ICollection.IsSynchronized => ((IList)this._items).IsSynchronized;

        object ICollection.SyncRoot => ((IList)this._items).SyncRoot;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        int IList.Add(object value)
        {
            this.Add((T)value);
            return this.Count - 1;
        }

        void IList.Clear() => this.Clear();

        bool IList.Contains(object value) => this.Contains((T)value);

        int IList.IndexOf(object value) => this.IndexOf((T)value);

        void IList.Insert(int index, object value) => this.Insert(index, (T)value);

        void IList.Remove(object value) => this.Remove((T)value);

        void IList.RemoveAt(int index) => this.RemoveAt(index);

        void ICollection.CopyTo(Array array, int index) => (this._items as IList).CopyTo(array, index);

        #endregion

        public NotifyCollectionHelper() : this(null)
        {
            ;
        }

        public NotifyCollectionHelper(IEnumerable<T> items)
        {
            this._items = items == null ? new List<T>() : new List<T>(items);
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
