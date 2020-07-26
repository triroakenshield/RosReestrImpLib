using System;
using System.ComponentModel;
//
using RosReestrImp.Data;

namespace RRViewer1
{
    /// <summary>Обёртка для значения</summary>
    public class MyFieldPropertyDescriptor : PropertyDescriptor
    {
        private readonly FieldValue _owner;

        /// <summary>Конструктор</summary>
        /// <param name="nOwner">значение</param>
        public MyFieldPropertyDescriptor(FieldValue nOwner) : base(nOwner.Rule.FName, null)
        {
            this._owner = nOwner;
        }

        /// <inheritdoc />
        public override string DisplayName => _owner.Rule.FName;

        /// <inheritdoc />
        public override Type ComponentType => typeof(MyRecordView); //!!!

        /// <inheritdoc />
        public override bool IsReadOnly => false;

        /// <inheritdoc />
        public override Type PropertyType => typeof(string); //!!!

        /// <inheritdoc />
        public override object GetValue(object component)
        {
            return _owner.GetString();
        }

        /// <inheritdoc />
        public override void SetValue(object component, object value)
        {
        }

        /// <inheritdoc />
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <inheritdoc />
        public override void ResetValue(object component)
        {
        }

        /// <inheritdoc />
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}