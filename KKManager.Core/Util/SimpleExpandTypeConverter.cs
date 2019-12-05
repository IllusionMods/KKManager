using System;
using System.ComponentModel;

namespace KKManager.Util
{
    /// <summary>
    /// Make the class expandable in property grids
    /// </summary>
    /// <typeparam name="T">Type of the class</typeparam>
    /// <inheritdoc />
    public sealed class SimpleExpandTypeConverter<T> : TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(typeof(T), attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
