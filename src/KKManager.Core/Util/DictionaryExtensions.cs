// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace KKManager.Util
{
    // Feel free to delete this file, this was an attempt to add all the messy class-dictionary wrapping stuff here and just inherit into IllusionObject.
    public static class DictionaryExtensions
    {
        public static T Get<T>(this IDictionary<string, object> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out object value))
            {
                if (value is T result)
                {
                    return result;
                }
                else
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }

            return default;
        }

        public static void Set<T>(this IDictionary<string, object> dictionary, string key, T value)
        {
            dictionary[key] = value;
        }

        public static bool Is<T>(this object obj)
        {
            return obj is T;
        }

        public static bool Has(this object obj, string propertyName)
        {
            if (obj == null)
            {
                return false;
            }

            return obj.GetType().GetProperty(propertyName) != null;
        }

        public static bool HasMethod(this object obj, string methodName)
        {
            if (obj == null)
            {
                return false;
            }

            return obj.GetType().GetMethod(methodName) != null;
        }

        public static T Invoke<T>(this object obj, string methodName, params object[] args)
        {
            return (T)obj.GetType().GetMethod(methodName).Invoke(obj, args);
        }

        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName).SetValue(obj, value);
        }

        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            return (T)obj.GetType().GetProperty(propertyName).GetValue(obj);
        }

        public static dynamic GetDynamicObject(this object obj)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                expando.Add(propertyInfo.Name, propertyInfo.GetValue(obj, null));
            }
            return expando as ExpandoObject;
        }

        public static void SetDynamicObject(this object obj, IDictionary<string, object> dictionary)
        {
            foreach (KeyValuePair<string, object> kvp in dictionary)
            {
                obj.GetType().GetProperty(kvp.Key)?.SetValue(obj, kvp.Value);
            }
        }

        public static bool Equals<T>(this IDictionary<string, object> dictionary, string key, T value)
        {
            return dictionary.TryGetValue(key, out object val) && val is T t && EqualityComparer<T>.Default.Equals(t, value);
        }
    }


}
