using System;
using System.ComponentModel;
using System.Linq;
//
using RosReestrImp.Rule;
using RosReestrImp.Data;

namespace RRViewer1
{
    /// <summary>Обёртка записи для биндинга</summary>
    [TypeDescriptionProvider(typeof(MyRecordTypeDescriptionProvider))]
    public class MyRecordView : ICustomTypeDescriptor
    {
        private readonly LayerRule _rule;
        private readonly MyRecord _owner;

        /// <summary>Конструктор</summary>
        /// <param name="nOwner">запись</param>
        public MyRecordView(MyRecord nOwner)
        {
            _owner = nOwner;
            _rule = _owner.Rule;
        }

        /// <summary></summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        public string GetValue(FieldRule rule)
        {
            return _owner.FieldList.FirstOrDefault(f=>f.Rule.FName == rule.FName)?.GetString();
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
            return _rule.LName;
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
            if (_owner != null)
            {
                return new PropertyDescriptorCollection(_owner.FieldList.Select(f => new MyFieldPropertyDescriptor(f)).ToArray<PropertyDescriptor>());
            }
            else return new PropertyDescriptorCollection(_rule.FieldList.Select(f => new MyFieldPropertyDescriptor(f)).ToArray<PropertyDescriptor>());
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
            return GetType().Name;
        }

        #endregion
    }
}