using System;
using System.ComponentModel;
using System.Linq;
//
using RosReestrImp.Data;

namespace RRViewer1
{
    /// <summary>Обёртка записи для биндинга</summary>
    public class MyRecordView : ICustomTypeDescriptor
    {

        private readonly MyRecord _owner;

        /// <summary>Конструктор</summary>
        /// <param name="nOwner">запись</param>
        public MyRecordView(MyRecord nOwner)
        {
            this._owner = nOwner;
        }
        
        #region ICustomTypeDescriptor Members

        /// <inheritdoc />
        public TypeConverter GetConverter()
        {
            return null;
        }

        /// <inheritdoc />
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        /// <inheritdoc />
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        /// <inheritdoc />
        public string GetComponentName()
        {
            return null;
        }

        /// <inheritdoc />
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _owner;
        }

        /// <inheritdoc />
        public AttributeCollection GetAttributes()
        {
            return AttributeCollection.Empty;
        }

        /// <inheritdoc />
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(_owner.FieldList
                .Select(f => new MyFieldPropertyDescriptor(f)).ToArray<PropertyDescriptor>());
        }

        /// <inheritdoc />
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return GetProperties(null);
        }

        /// <inheritdoc />
        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        /// <inheritdoc />
        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        /// <inheritdoc />
        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        /// <inheritdoc />
        public string GetClassName()
        {
            return this.GetType().Name;
        }

        #endregion
    }
}