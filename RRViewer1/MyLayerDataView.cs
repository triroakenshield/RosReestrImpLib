using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.ComponentModel;
using RosReestrImp.Data;

namespace RRViewer1
{
    public class MyLayerDataView : IBindingList
    {

        private Layer data;
        private List<MyRecordView> rows;
        //
        //private string[] columnNames;

        public event System.ComponentModel.ListChangedEventHandler ListChanged;

        #region Constructors

        public MyLayerDataView(Layer l)
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

        public string[] ColumnNames
        {
            get
            {
                return this.data.GetColumnNames(); 
            }
        }

        public int RowCount
        {
            get { return this.data.Table.Count; }
        }

        public int ColumnsCount
        {
            get { return this.data.Rule.FieldList.Count; }
        }

        #endregion

        public void Reset()
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        public void AddIndex(PropertyDescriptor property)
        {

        }

        public bool AllowNew
        {
            get { return false; }
        }

        public void ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
        {

        }

        public PropertyDescriptor SortProperty
        {
            get { return null; }
        }

        public int Find(PropertyDescriptor property, object key)
        {
            return 0;
        }

        public bool SupportsSorting
        {
            get { return false; }
        }

        public bool IsSorted
        {
            get { return false; }
        }

        public bool AllowRemove
        {
            get { return false; }
        }

        public bool SupportsSearching
        {
            get { return false; }
        }

        public ListSortDirection SortDirection
        {
            get { return new System.ComponentModel.ListSortDirection(); }
        }


        private void OnListChanged(ListChangedEventArgs e)
        {
            if (ListChanged != null)
            {
                ListChanged(this, e);
            }
        }

        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        public void RemoveSort()
        {

        }

        public object AddNew()
        {
            return null;
        }

        public bool AllowEdit
        {
            get { return true; }
        }

        public void RemoveIndex(PropertyDescriptor property)
        {

        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public object this[int index]
        {
            get { return rows[index]; }
            set { throw new NotSupportedException(); }
        }

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

        public bool IsFixedSize
        {
            get { return true; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public int Count
        {
            get { return rows.Count; }
        }

        public void CopyTo(Array array, int index)
        {

        }

        public object SyncRoot
        {
            get { return null; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return rows.GetEnumerator();
        }

    }
}
