using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KKManager.Util
{
    public class ListTypeConverter : CollectionConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));

            if (value is ICollection arr)
                return $"{arr.Count} item{(arr.Count == 1 ? "" : "s")}";

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            IList list = value as IList;
            if (list == null)
            {
                if (value is ICollection c)
                    list = c.Cast<object>().ToList().AsReadOnly();
                else
                    return base.GetProperties(context, value, attributes);
            }

            var items = new PropertyDescriptorCollection(null);
            for (int i = 0; i < list.Count; i++)
            {
                //object item = list[i];
                items.Add(new ExpandableCollectionPropertyDescriptor(list, i));
            }
            return items;
        }

        public class ExpandableCollectionPropertyDescriptor : PropertyDescriptor
        {
            private readonly IList _collection;
            private readonly int _index;

            public ExpandableCollectionPropertyDescriptor(IList coll, int idx)
                : base(GetDisplayName(coll, idx), null)
            {
                _collection = coll;
                _index = idx;
            }

            private static string GetDisplayName(IList list, int index)
            {
                var digitCount = Math.Ceiling(Math.Log10(list.Count));
                return "[" + index.ToString("D" + digitCount) + "]  " + CSharpName(list[index].GetType());
            }

            private static string CSharpName(Type type)
            {
                var sb = new StringBuilder();
                var name = type.Name;
                if (!type.IsGenericType)
                    return name;
                sb.Append(name.Substring(0, name.IndexOf('`')));
                sb.Append("<");
                sb.Append(string.Join(", ", type.GetGenericArguments()
                    .Select(CSharpName)));
                sb.Append(">");
                return sb.ToString();
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override Type ComponentType => _collection.GetType();

            public override object GetValue(object component)
            {
                return _collection[_index];
            }

            public override bool IsReadOnly => _collection.IsReadOnly;

            public override string Name => _index.ToString(CultureInfo.InvariantCulture);

            public override Type PropertyType => _collection[_index].GetType();

            public override void ResetValue(object component)
            {
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }

            public override void SetValue(object component, object value)
            {
                _collection[_index] = value;
            }
        }
    }
}
