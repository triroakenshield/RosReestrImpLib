using System;
using System.Collections.Generic;
using System.ComponentModel;

using RosReestrImp.Data;

namespace RRViewer1
{
    public class MyLayerDataView : IBindingList
    {

        private DataLayer data;
        private List<MyRecordView> rows;

        public event ListChangedEventHandler ListChanged;

        #region Constructors

        public MyLayerDataView(DataLayer l)
        {
            this.data = l;
            this.rows = new List<MyRecordView>();
            foreach (MyRecord r in l.Table)
            {
                this.rows.Add(new MyRecordView(r));
            }
        }

        #endregion // Constructors

        #region Properties

        public string[] ColumnNames => this.data.GetColumnNames();

        public int RowCount => this.data.Table.Count;

        public int ColumnsCount => this.data.Rule.FieldList.Count;

        #endregion

        public void Reset()
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public void AddIndex(PropertyDescriptor property)
        {

        }

        public bool AllowNew => false;

        public void ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
        {

        }

        public PropertyDescriptor SortProperty => null;

        public int Find(PropertyDescriptor property, object key)
        {
            return 0;
        }

        public bool SupportsSorting => false;

        public bool IsSorted => false;

        public bool AllowRemove => false;

        public bool SupportsSearching => false;

        public ListSortDirection SortDirection => new ListSortDirection();


        private void OnListChanged(ListChangedEventArgs e)
        {
            ListChanged?.Invoke(this, e);
        }

        public bool SupportsChangeNotification => true;

        public void RemoveSort()
        {

        }

        public object AddNew()
        {
            return null;
        }

        public bool AllowEdit => true;

        public void RemoveIndex(PropertyDescriptor property)
        {

        }

        public bool IsReadOnly => true;

        public object this[int index] { get => rows[index]; set => throw new NotSupportedException(); }

        public void RemoveAt(int index)
        {

        }

        public void Insert(int index, object value)
        {

        }

        public void Remove(object value)
        {

        }

        public bool Contains(object value)
        {
            return false;
        }

        public void Clear()
        {

        }

        public int IndexOf(object value)
        {
            return 0;
        }

        public int Add(object value)
        {
            return 0;
        }

        public bool IsFixedSize => true;

        public bool IsSynchronized => false;

        public int Count => rows.Count;

        public void CopyTo(Array array, int index)
        {

        }

        public object SyncRoot => null;

        public System.Collections.IEnumerator GetEnumerator()
        {
            return rows.GetEnumerator();
        }
    }
}