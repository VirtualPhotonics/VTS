using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Vts
{
    /// <summary>
    /// Class to manage dependencies of INotifyPropertyChanged objects,
    /// as suggested by Tomas Elison here:
    /// http://neilmosafi.blogspot.com/2008/07/is-inotifypropertychanged-anti-pattern.html
    /// This class builds a dependency graph the "right" way. It knows for example
    /// that if Quantity is changed, TotalPrice has asked to be informed about this.
    /// The PropertyDependencyManager listens to the PropertyChanged event for any
    /// class that is registered with it and uses the dependency graph to propagate
    /// the events the right way.
    /// </summary>
    public class PropertyDependencyManager
    {
        private static readonly List<PropertyDependencyManager> registeredInstances = new List<PropertyDependencyManager>();
        private readonly INotifyPropertyChangedPlus notifyTarget;
        private readonly Type targetType;
        private Dictionary<string, List<string>> dependencyGraph;

        private PropertyDependencyManager(INotifyPropertyChangedPlus target)
        {
            notifyTarget = target;
            targetType = target.GetType();
            notifyTarget.PropertyChanged += notifyTarget_PropertyChanged;
            CreateDependencyGraph();
        }

        /// <summary>
        /// Registers the class to notify changes to properties and their dependencies
        /// </summary>
        /// <param name="target">The target class that implements INotifyPropertyChangedPlus</param>
        public static void Register(INotifyPropertyChangedPlus target)
        {
            registeredInstances.Add(new PropertyDependencyManager(target));
        }

        private void CreateDependencyGraph()
        {
            dependencyGraph = new Dictionary<string, List<string>>();

            foreach (var property in targetType.GetProperties())
            {
                foreach (DependsOnAttribute attribute in property.GetCustomAttributes(typeof(DependsOnAttribute), true))
                {
                    foreach (var propertyWithDependee in attribute.Properties)
                    {
                        if (!dependencyGraph.ContainsKey(propertyWithDependee))
                        {
                            dependencyGraph.Add(propertyWithDependee, new List<string>());
                        }
                        dependencyGraph[propertyWithDependee].Add(property.Name);
                    }
                }
            }
        }

        private void notifyTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (dependencyGraph.ContainsKey(e.PropertyName))
            {
                foreach (var dependeeProperty in dependencyGraph[e.PropertyName])
                {
                    notifyTarget.OnPropertyChanged(dependeeProperty);
                }
            }
        }
    }
}
