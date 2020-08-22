using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KKManager.Util
{
    public class DictionaryTypeConverter<T, T2> : CollectionConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var d = value as Dictionary<T, T2>;
            if (d == null) return base.GetProperties(context, value, attributes);

            var propertyDescriptorCollection = new PropertyDescriptorCollection(null);
            foreach (var pluginData in d)
                propertyDescriptorCollection.Add(new KeyValuePairDescriptor(pluginData));
            return propertyDescriptorCollection;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public class KeyValuePairDescriptor : PropertyDescriptor
        {
            private readonly KeyValuePair<T, T2> _item;

            public KeyValuePairDescriptor(KeyValuePair<T, T2> item) : base(item.Key.ToString(), null)
            {
                _item = item;
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                return _item.Value;
            }

            public override void ResetValue(object component)
            {
                throw new NotImplementedException();
            }

            public override void SetValue(object component, object value)
            {
                throw new NotImplementedException();
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }

            public override Type ComponentType { get; } = typeof(KeyValuePair<T, T2>);
            public override bool IsReadOnly => true;
            public override Type PropertyType { get; } = typeof(T2);
        }
    }
}