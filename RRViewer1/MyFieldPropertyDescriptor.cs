using System;
using System.ComponentModel;
//
using RosReestrImp.Rule;
using RosReestrImp.Data;

namespace RRViewer1
{
    /// <summary>Обёртка для значения</summary>
    public class MyFieldPropertyDescriptor : PropertyDescriptor
    {
        private readonly FieldRule _rule;
        private readonly FieldValue _owner;

        /// <summary></summary>
        /// <param name="nOwnerRule"></param>
        public MyFieldPropertyDescriptor(FieldRule nOwnerRule) : base(nOwnerRule.FName, null)
        {
            this._rule = nOwnerRule;
        }

        /// <summary>Конструктор</summary>
        /// <param name="nOwner">значение</param>
        public MyFieldPropertyDescriptor(FieldValue nOwner) : base(nOwner.Rule.FName, null)
        {
            this._owner = nOwner;
            this._rule = _owner.Rule;
        }

        /// <inheritdoc />
        public override string DisplayName => this._rule.FName;

        /// <inheritdoc />
        public override Type ComponentType => typeof(MyRecordView); //!!!

        /// <inheritdoc />
        public override bool IsReadOnly => false;

        /// <inheritdoc />
        public override Type PropertyType => typeof(string); //!!!

        /// <inheritdoc />
        public override object GetValue(object component)
        {
            if (component is MyRecordView rec) return rec.GetValue(_rule);
            return _owner?.GetString() ?? "";
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