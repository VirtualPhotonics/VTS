using System;

namespace Vts
{
    /// <summary>
    /// This is an attribute that when applied to a property will create a dependency for
    /// when that property changes. The DependsOn attribute contains no logic, just a list
    /// of dependencies. It is used by the PropertyDependencyManager to determine if a
    /// property has changed due to a change in a dependent property.
    /// Usage:
    /// [DependsOn("Second", third)]
    /// public int First { get; set; }
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DependsOnAttribute : Attribute
    {
        /// <summary>
        /// Takes a string of properties that the property depends upon
        /// </summary>
        /// <param name="properties"></param>
        public DependsOnAttribute(params string[] properties)
        {
            Properties = properties;
        }

        /// <summary>
        /// The list of properties with dependencies
        /// </summary>
        public string[] Properties { get; private set; }
    }
}
