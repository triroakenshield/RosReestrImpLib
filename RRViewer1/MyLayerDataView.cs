using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using RosReestrImp.Data;

namespace RRViewer1
{
    /// <summary>Биндинг для слоя</summary>
    public class MyLayerDataView : IBindingList
    {
        //private readonly DataLayer _data;
        private readonly List<MyRecordView> _rows;

        /// <inheritdoc />
        public event ListChangedEventHandler ListChanged;
        
        /// <summary>Конструктор</summary>
        /// <param name="l">Слой</param>
        public MyLayerDataView(DataLayer l)
        {
            this._rows = l.Table.Select(r=>new MyRecordView(r)).ToList();
        }
        
        /// <inheritdoc />
        public void AddIndex(PropertyDescriptor property)
        {

        }

        /// <inheritdoc />
        public bool AllowNew => false;

        /// <inheritdoc />
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {

        }

        /// <inheritdoc />
        public PropertyDescriptor SortProperty => null;

        /// <inheritdoc />
        public int Find(PropertyDescriptor property, object key)
        {
            return 0;
        }

        /// <inheritdoc />
        public bool SupportsSorting => true;

        /// <inheritdoc />
        public bool IsSorted => false;

        /// <inheritdoc />
        public bool AllowRemove => false;

        /// <inheritdoc />
        public bool SupportsSearching => false;

        /// <inheritdoc />
        public ListSortDirection SortDirection => new ListSortDirection();
        
        /// <inheritdoc />
        public bool SupportsChangeNotification => true;

        /// <inheritdoc />
        public void RemoveSort()
        {

        }

        /// <inheritdoc />
        public object AddNew()
        {
            return null;
        }

        /// <inheritdoc />
        public bool AllowEdit => true;

        /// <inheritdoc />
        public void RemoveIndex(PropertyDescriptor property)
        {

        }

        /// <inheritdoc />
        public bool IsReadOnly => true;

        /// <inheritdoc />
        public object this[int index] { get => _rows[index]; set => throw new NotSupportedException(); }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {

        }

        /// <inheritdoc />
        public void Insert(int index, object value)
        {

        }

        /// <inheritdoc />
        public void Remove(object value)
        {

        }

        /// <inheritdoc />
        public bool Contains(object value)
        {
            return false;
        }

        /// <inheritdoc />
        public void Clear()
        {

        }

        /// <inheritdoc />
        public int IndexOf(object value)
        {
            return 0;
        }

        /// <inheritdoc />
        public int Add(object value)
        {
            return 0;
        }

        /// <inheritdoc />
        public bool IsFixedSize => true;

        /// <inheritdoc />
        public bool IsSynchronized => false;

        /// <inheritdoc />
        public int Count => _rows.Count;

        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
        }

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public System.Collections.IEnumerator GetEnumerator()
        {
            return _rows.GetEnumerator();
        }
    }
}