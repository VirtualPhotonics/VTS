using System;

namespace Vts.Extensions
{
    /// <summary>
    /// extension methods for type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks to see if a type implements a specified interface
        /// </summary>
        /// <typeparam name="T">The interface type</typeparam>
        /// <param name="type">The type to verify</param>
        /// <param name="interface">The interface implementation</param>
        /// <returns>True or false</returns>
        public static bool Implements<T>(this Type type, T @interface) where T : class
        {
            if (typeof(T).IsInterface && @interface != null)
            {
                return typeof(T).IsAssignableFrom(type);
            }
            throw new ArgumentException("Only interfaces can be 'implemented'.");
        }
    }
}
