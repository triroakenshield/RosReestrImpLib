using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
//
using RosReestrImp.Data;

namespace RRViewer1
{
    public class MyFieldPropertyDescriptor : PropertyDescriptor
    {

        private FieldValue owner;

        public MyFieldPropertyDescriptor(FieldValue nowner)
            : base(nowner.Rule.FName, null)
        {
            this.owner = nowner;
        }

        public override string DisplayName
        {
            get { return owner.Rule.FName; }
        }

        public override Type ComponentType
        {
            get { return typeof(MyRecordView); } //!!!
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); } //!!!
        }

        public override object GetValue(object component)
        {
            // !!!
            return owner.GetString();
            //return null;
        }

        public override void SetValue(object component, object value)
        {
            //
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
