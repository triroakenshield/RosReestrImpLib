using System;
using System.ComponentModel;

namespace RRViewer1
{
    /// <summary></summary>
    public class MyRecordTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static readonly TypeDescriptionProvider DefaultTypeProvider = TypeDescriptor.GetProvider(typeof(MyRecordView));
        private static MyRecordView _instance;

        /// <summary></summary>
        public MyRecordTypeDescriptionProvider() : base(DefaultTypeProvider) { }

        /// <summary></summary>
        /// <param name="instance"></param>
        public static void SetProperties(MyRecordView instance)
        {
            _instance = instance;
        }

        /// <summary></summary>
        /// <param name="objectType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor defaultDescriptor = base.GetTypeDescriptor(objectType, instance);
            return _instance == null ? defaultDescriptor : _instance;
        }
    }
}