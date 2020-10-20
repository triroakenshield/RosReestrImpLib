using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RRViewer1
{
    public class MyRecordTypeDescriptionProvider : TypeDescriptionProvider
    {
        private static TypeDescriptionProvider defaultTypeProvider = TypeDescriptor.GetProvider(typeof(MyRecordView));
        private static MyRecordView _instance = null;

        public MyRecordTypeDescriptionProvider() : base(defaultTypeProvider) { }

        public static void SetProperties(MyRecordView instance)
        {
            _instance = instance;
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            ICustomTypeDescriptor defaultDescriptor = base.GetTypeDescriptor(objectType, instance);
            return _instance == null ? defaultDescriptor : _instance;
        }
    }
}