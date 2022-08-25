using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vts
{
    /// <summary>
    /// Helper methods for enums
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// Method to get values from generic type
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <returns>An array of generic type</returns>
        public static T[] GetValues<T>()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<T> values = new List<T>();

            var fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(enumType);
                values.Add((T)value);
            }

            return values.ToArray();
        }
        /// <summary>
        /// Method to get names from generic type
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <returns>An array of name strings</returns>
        public static string[] GetNames<T>()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            return (from field in enumType.GetFields()
                         where field.IsLiteral
                         select field.Name).ToArray();
        }
        /// <summary>
        /// Method to get values of enum
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <returns>An array of object</returns>
        public static object[] GetValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<object> values = new List<object>();

            var fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(enumType);
                values.Add(value);
            }

            return values.ToArray();
        }
    }
}
