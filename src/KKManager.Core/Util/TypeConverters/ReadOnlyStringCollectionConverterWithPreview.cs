using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace KKManager.Data
{
    public class ReadOnlyStringCollectionConverterWithPreview : CollectionConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentNullException(nameof(destinationType));

            var arr = value as IReadOnlyCollection<string>;
            var arr2 = value as ICollection<string>;
            if (arr != null || arr2 != null)
            {
                var count = arr?.Count ?? arr2.Count;
                var result = $"{count} item{(count == 1 ? "" : "s")}";
                if (count > 0)
                {
                    result += $" - {string.Join(", ", ((IEnumerable<string>)arr ?? arr2).Take(3))}";
                    if (count > 3)
                        result += "...";
                }

                return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptor[] properties = null;

            var arr = value as IReadOnlyCollection<string>;
            var arr2 = value as ICollection<string>;
            if (arr != null || arr2 != null)
            {
                var count = arr?.Count ?? arr2.Count;

                properties = new PropertyDescriptor[count];
                for (int index = 0; index < count; ++index)
                    properties[index] = new StringCollectionPropertyDescriptor(value.GetType(), index);
            }
            return new PropertyDescriptorCollection(properties);
        }

        /// <summary>Gets a value indicating whether this object supports properties.</summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <returns>
        /// <see langword="true" /> because <see cref="M:System.ComponentModel.ArrayConverter.GetProperties(System.ComponentModel.ITypeDescriptorContext,System.Object,System.Attribute[])" /> should be called to find the properties of this object. This method never returns <see langword="false" />.</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

        private class StringCollectionPropertyDescriptor : TypeConverter.SimplePropertyDescriptor
        {
            private int index;

            public StringCollectionPropertyDescriptor(Type arrayType, int index)
                : base(arrayType, "[" + index.ToString() + "]", typeof(string), null)
            {
                this.index = index;
            }

            public override object GetValue(object instance)
            {
                var arr = instance as IReadOnlyCollection<string>;
                var arr2 = instance as ICollection<string>;
                if (arr != null || arr2 != null)
                {
                    var count = arr?.Count ?? arr2.Count;

                    if (count > this.index)
                        return arr.Skip(index).First();
                }
                return null;
            }

            public override void SetValue(object instance, object value)
            {
            }
        }
    }
}
