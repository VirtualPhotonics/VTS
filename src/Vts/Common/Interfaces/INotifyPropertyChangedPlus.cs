using System.ComponentModel;

namespace Vts
{
    /// <summary>
    /// An adaptation of the INotifyPropertyChanged interface to include
    /// OnPropertyChanged method.
    /// </summary>
    public interface INotifyPropertyChangedPlus : INotifyPropertyChanged
    {
        /// <summary>
        /// Method to raises the property changed event
        /// </summary>
        /// <param name="propertyName">The property that changed</param>
        void OnPropertyChanged(string propertyName);
    }

    //public class Order : INotifyPropertyChangedPlus
    //{
    //    public Order()
    //    {
    //        PropertyDependencyManager.Register(this);
    //    }

    //    public decimal ItemPrice
    //    {
    //        get { return itemPrice; }
    //        set
    //        {
    //            itemPrice = value;
    //            OnPropertyChanged("ItemPrice");
    //        }
    //    } private decimal itemPrice;

    //    public int Quantity
    //    {
    //        get { return quantity; }
    //        set
    //        {
    //            quantity = value;
    //            OnPropertyChanged("Quantity");
    //        }
    //    } private int quantity;

    //    [DependsOn("ItemPrice", "Quantity")]
    //    public decimal TotalPrice
    //    {
    //        get { return ItemPrice * Quantity; }
    //    }

    //    public void OnPropertyChanged(string propertyName)
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //}


    //public class SalesOrder : Order
    //{
    //    public decimal SalesCommission
    //    {
    //        get { return salesCommission; }
    //        set
    //        {
    //            salesCommission = value;
    //            OnPropertyChanged("SalesCommission");
    //        }
    //    } private decimal salesCommission;

    //    [DependsOn("TotalPrice", "SalesCommission")]
    //    public decimal TotalCommission
    //    {
    //        get { return TotalPrice * SalesCommission; }
    //    }
    //}

    ///// <summary>
    ///// The DependsOn attribute contains no logic at all, it is just an attribute with a string collection property.
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Property)]
    //public class DependsOnAttribute : Attribute
    //{
    //    public DependsOnAttribute(params string[] properties)
    //    {
    //        Properties = properties;
    //    }

    //    public string[] Properties { get; private set; }
    //}

    ///// <summary>
    ///// Now for the class with the magic, the PropertyDependencyManager. This class builds a dependency graph the "right" way. It knows for example that if Quantity is changed, TotalPrice has asked to be informed about this. The PropertyDependencyManager listens to the PropertyChanged event for any class that is registered with it and uses the dependency graph to propagate the events the right way.
    ///// </summary>
    //public class PropertyDependencyManager
    //{
    //    private static readonly List<PropertyDependencyManager> registeredInstances = new List<PropertyDependencyManager>();
    //    private readonly INotifyPropertyChangedPlus notifyTarget;
    //    private readonly Type targetType;
    //    private Dictionary<string, List<string>> dependencyGraph;

    //    private PropertyDependencyManager(INotifyPropertyChangedPlus target)
    //    {
    //        notifyTarget = target;
    //        targetType = target.GetType();
    //        notifyTarget.PropertyChanged += notifyTarget_PropertyChanged;
    //        CreateDependencyGraph();
    //    }

    //    public static void Register(INotifyPropertyChangedPlus target)
    //    {
    //        registeredInstances.Add(new PropertyDependencyManager(target));
    //    }

    //    private void CreateDependencyGraph()
    //    {
    //        dependencyGraph = new Dictionary<string, List<string>>();

    //        foreach (var property in targetType.GetProperties())
    //        {
    //            foreach (DependsOnAttribute attribute in property.GetCustomAttributes(typeof(DependsOnAttribute), true))
    //            {
    //                foreach (var propertyWithDependee in attribute.Properties)
    //                {
    //                    if (!dependencyGraph.ContainsKey(propertyWithDependee))
    //                    {
    //                        dependencyGraph.Add(propertyWithDependee, new List<string>());
    //                    }
    //                    dependencyGraph[propertyWithDependee].Add(property.Name);
    //                }
    //            }
    //        }
    //    }

    //    private void notifyTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        if (dependencyGraph.ContainsKey(e.PropertyName))
    //        {
    //            foreach (var dependeeProperty in dependencyGraph[e.PropertyName])
    //            {
    //                notifyTarget.OnPropertyChanged(dependeeProperty);
    //            }
    //        }
    //    }
    //}
}
