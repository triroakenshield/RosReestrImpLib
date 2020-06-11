using System;
using System.ComponentModel;
//
using RosReestrImp.Data;

namespace RRViewer1
{
    public class MyRecordView : ICustomTypeDescriptor
    {

        private MyRecord owner;

        public MyRecordView(MyRecord nowner)
        {
            this.owner = nowner;
        }

        public string Name => owner.Rule.LName;

        public object GetColumn(int index)
        {
            return index < this.owner.FileldList.Count ? this.owner.FileldList[index].ToString() : null;
        }

        #region ICustomTypeDescriptor Members
        
        public TypeConverter GetConverter()
        {
            return null;
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return EventDescriptorCollection.Empty;
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return EventDescriptorCollection.Empty;
        }

        public string GetComponentName()
        {
            return null;
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return owner;
        }

        public AttributeCollection GetAttributes()
        {
            return AttributeCollection.Empty;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            PropertyDescriptor[] prop;
            int cols = owner.FileldList.Count;
            prop = new PropertyDescriptor[cols];
            for (int i = 0; i < cols; i++)
                prop[i] = new MyFieldPropertyDescriptor(owner.FileldList[i]);

            return new PropertyDescriptorCollection(prop);
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return GetProperties(null);
        }

        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        public string GetClassName()
        {
            return this.GetType().Name;
        }

        #endregion
    }
}