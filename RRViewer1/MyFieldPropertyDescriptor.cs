using System;
using System.ComponentModel;
//
using RosReestrImp.Data;

namespace RRViewer1
{
    public class MyFieldPropertyDescriptor : PropertyDescriptor
    {

        private FieldValue owner;

        public MyFieldPropertyDescriptor(FieldValue nowner) : base(nowner.Rule.FName, null)
        {
            this.owner = nowner;
        }

        public override string DisplayName => owner.Rule.FName;

        public override Type ComponentType => typeof(MyRecordView); //!!!

        public override bool IsReadOnly => false;

        public override Type PropertyType => typeof(string); //!!!

        public override object GetValue(object component)
        {
            return owner.GetString();
        }

        public override void SetValue(object component, object value)
        {
        }
        
        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {

        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}