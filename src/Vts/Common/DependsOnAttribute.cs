using System;

namespace Vts
{
    /// <summary>
    /// The DependsOn attribute contains no logic at all, it is just an attribute with a string collection property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DependsOnAttribute : Attribute
    {
        public DependsOnAttribute(params string[] properties)
        {
            Properties = properties;
        }

        public string[] Properties { get; private set; }
    }
}
