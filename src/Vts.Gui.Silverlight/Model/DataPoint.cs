using System;
using System.Linq;
using System.Numerics;

namespace Vts.Gui.Silverlight.Model
{
    public class Test
    {
        private IDataPoint[][] _myDataPoints;

        public Test()
        {
            _myDataPoints = new IDataPoint[2][];
        }

        void TestMethod()
        {
            var dp = new ComplexDataPoint(0, 0);
            var complex_number = dp.Y;

            _myDataPoints[0] = Enumerable.Range(0, 10).Select(i => new ComplexDataPoint(0, 0)).ToArray();
            _myDataPoints[1] = Enumerable.Range(0, 10).Select(i => new DoubleDataPoint(0, 0)).ToArray();
        }
    }

    public interface IDataPoint
    {
    }

    public class ComplexDataPoint : IDataPoint
    {
        private double _x;
        private Complex _y;

        public ComplexDataPoint(double x, Complex y)
        {
            _x = x;
            _y = y;
        }

        //
        // Summary:
        //     Gets or sets the X-coordinate value
        //
        // Returns:
        //     The X-coordinate value of this ComplexDataPoint
        //     structure. The default value is 0.
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        //
        // Summary:
        //     Gets or sets the Y-coordinate value 
        //
        // Returns:
        //     The SY-coordinate value of this ComplexDataPoint
        //     structure. The default value is 0.
        public Complex Y
        {
            get { return _y; }
            set { _y = value; }
        }

        // Summary:
        //     Compares two ComplexDataPoint structures for inequality
        //
        // Parameters:
        //   point1:
        //     The first point to compare.
        //
        //   point2:
        //     The second point to compare.
        //
        // Returns:
        //     true if point1 and point2 have different ComplexDataPoint.X or ComplexDataPoint.Y
        //     values; false if point1 and point2 have the same ComplexDataPoint.X and
        //     ComplexDataPoint.Y values.
        public static bool operator !=(ComplexDataPoint point1, ComplexDataPoint point2)
        {
            return point1.X != point2.X || point1.Y != point2.Y;
        }

        //
        // Summary:
        //     Compares two ComplexDataPoint structures for equality.
        //
        // Parameters:
        //   point1:
        //     The first ComplexDataPoint structure to compare.
        //
        //   point2:
        //     The second ComplexDataPoint structure to compare.
        //
        // Returns:
        //     true if both the ComplexDataPoint.X and ComplexDataPoint.Y values
        //     of point1 and point2 are equal; otherwise, false.
        public static bool operator ==(ComplexDataPoint point1, ComplexDataPoint point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        // Summary:
        //     Determines whether the specified object is a ComplexDataPoint and whether
        //     it contains the same values as this ComplexDataPoint.
        //
        // Parameters:
        //   o:
        //     The object to compare.
        //
        // Returns:
        //     true if obj is a ComplexDataPoint and contains the same ComplexDataPoint.X
        //     and ComplexDataPoint.Y values as this ComplexDataPoint; otherwise,
        //     false.
        public override bool Equals(object o)
        {
            return this.Equals((ComplexDataPoint)o);
        }

        //
        // Summary:
        //     Compares two ComplexDataPoint structures for equality.
        //
        // Parameters:
        //   value:
        //     The point to compare to this instance.
        //
        // Returns:
        //     true if both ComplexDataPoint structures contain the same ComplexDataPoint.X
        //     and ComplexDataPoint.Y values; otherwise, false.
        public bool Equals(ComplexDataPoint value)
        {
            return X == value.X && Y == value.Y;
        }
        //
        // Summary:
        //     Returns the hash code for this ComplexDataPoint.
        //
        // Returns:
        //     The hash code for this ComplexDataPoint structure.
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Creates a System.String representation of this ComplexDataPoint.
        //
        // Returns:
        //     A System.String containing the ComplexDataPoint.X and ComplexDataPoint.Y
        //     values of this ComplexDataPoint structure.
        public override string ToString()
        {
            return X + ", " + Y;
        }
    }

    public class DoubleDataPoint : IDataPoint
    {
        private double _x;
        private double _y;

        //
        // Summary:
        //     Initializes a DoubleDataPoint structure that contains the specified
        //     values.
        //
        // Parameters:
        //   x:
        //     The x-coordinate value of the DoubleDataPoint structure.
        //
        //   y:
        //     The y-coordinate value of the DoubleDataPoint structure.
        public DoubleDataPoint(double x, double y)
        {
            _x = x;
            _y = y;
        }

        //
        // Summary:
        //     Gets or sets the X-coordinate value
        //
        // Returns:
        //     The X-coordinate value of this
        //     structure. The default value is 0.
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        //
        // Summary:
        //     Gets or sets the Y-coordinate value 
        //
        // Returns:
        //     The SY-coordinate value of this
        //     structure. The default value is 0.
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        // Summary:
        //     Compares two DoubleDataPoint structures for inequality
        //
        // Parameters:
        //   point1:
        //     The first point to compare.
        //
        //   point2:
        //     The second point to compare.
        //
        // Returns:
        //     true if point1 and point2 have different DoubleDataPoint.X or DoubleDataPoint.Y
        //     values; false if point1 and point2 have the same DoubleDataPoint.X and
        //     DoubleDataPoint.Y values.
        public static bool operator !=(DoubleDataPoint point1, DoubleDataPoint point2)
        {
            return point1.X != point2.X || point1.Y != point2.Y;
        }

        //
        // Summary:
        //     Compares two DoubleDataPoint structures for equality.
        //
        // Parameters:
        //   point1:
        //     The first System.Windows.Point structure to compare.
        //
        //   point2:
        //     The second System.Windows.Point structure to compare.
        //
        // Returns:
        //     true if both the DoubleDataPoint.X and DoubleDataPoint.Y values
        //     of point1 and point2 are equal; otherwise, false.
        public static bool operator ==(DoubleDataPoint point1, DoubleDataPoint point2)
        {
            return point1.X == point2.X && point1.Y == point2.Y;
        }

        // Summary:
        //     Determines whether the specified object is a DoubleDataPoint and whether
        //     it contains the same values as this DoubleDataPoint.
        //
        // Parameters:
        //   o:
        //     The object to compare.
        //
        // Returns:
        //     true if obj is a DoubleDataPoint and contains the same DoubleDataPoint.X
        //     and DoubleDataPoint.Y values as this DoubleDataPoint; otherwise,
        //     false.
        public override bool Equals(object o)
        {
            return this.Equals((DoubleDataPoint)o);
        }

        //
        // Summary:
        //     Compares two DoubleDataPoint structures for equality.
        //
        // Parameters:
        //   value:
        //     The point to compare to this instance.
        //
        // Returns:
        //     true if both DoubleDataPoint structures contain the same DoubleDataPoint.X
        //     and DoubleDataPoint.Y values; otherwise, false.
        public bool Equals(DoubleDataPoint value)
        {
            return X == value.X && Y == value.Y;
        }
        //
        // Summary:
        //     Returns the hash code for this DoubleDataPoint.
        //
        // Returns:
        //     The hash code for this DoubleDataPoint structure.
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        //
        // Summary:
        //     Creates a System.String representation of this DoubleDataPoint.
        //
        // Returns:
        //     A System.String containing the DoubleDataPoint and DoubleDataPoint.Y
        //     values of this DoubleDataPoint structure.
        public override string ToString()
        {
            return X + ", " + Y;
        }
    }
}
