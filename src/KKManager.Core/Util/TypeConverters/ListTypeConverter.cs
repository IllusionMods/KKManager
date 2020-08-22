using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace KKManager.Util
{
    public class ListTypeConverter : CollectionConverter
    {
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
                object item = list[i];
                items.Add(new ExpandableCollectionPropertyDescriptor(list, i));
            }
            return items;
        }

        public class ExpandableCollectionPropertyDescriptor : PropertyDescriptor
        {
            private IList collection;
            private readonly int _index;

            public ExpandableCollectionPropertyDescriptor(IList coll, int idx)
                : base(GetDisplayName(coll, idx), null)
            {
                collection = coll;
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

            public override Type ComponentType
            {
                get { return this.collection.GetType(); }
            }

            public override object GetValue(object component)
            {
                return collection[_index];
            }

            public override bool IsReadOnly
            {
                get { return collection.IsReadOnly; }
            }

            public override string Name
            {
                get { return _index.ToString(CultureInfo.InvariantCulture); }
            }

            public override Type PropertyType
            {
                get { return collection[_index].GetType(); }
            }

            public override void ResetValue(object component)
            {
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }

            public override void SetValue(object component, object value)
            {
                collection[_index] = value;
            }
        }
    }
}
