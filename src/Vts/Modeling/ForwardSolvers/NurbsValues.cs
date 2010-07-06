using System.Runtime.Serialization;
using Vts.IO;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// Specifies the physical dimension of the NURBS characteristic values.
    /// Space refers to both rho or fx.
    /// </summary>
    public enum NurbsValuesDimensions
    {
        time,
        space,
    }
    /// <summary>
    /// Class where the reference values read from binary files in resources are stored.
    /// </summary>
    [DataContract]
    public class NurbsValues
    {
        [DataMember]
        public NurbsValuesDimensions ValuesDimensions{ get; set;}
        [DataMember]
        public double[] KnotVector { get; set; }
        [DataMember]
        public double[] ControlPoints { get; set; }//used only in tests
        [DataMember]
        public double MaxValue { get; set; }
        [DataMember]
        public int Degree { get; set; }

        #region constructor

        /// <summary>
        /// Constructor to instantiate a class to write XML file.
        /// </summary>
        /// <param name="valuesDimension">physical dimension, space or time</param>
        /// <param name="knots">knots vector</param>
        /// <param name="cpoints">control points</param>
        /// <param name="max">max value along the physical dimension</param>
        /// <param name="degree">degree</param>
        public NurbsValues
            (NurbsValuesDimensions valuesDimension, double[] knots, double max, int degree)
        {
            ValuesDimensions = valuesDimension;
            KnotVector = knots;
            MaxValue = max;
            Degree = degree;
        }

        /// <summary>
        /// Constructor used for Unit testing of NurbsGenerator class.
        /// </summary>
        /// <param name="max">max value mapped to the interval 0-1</param>
        public NurbsValues(double max)
        {
            MaxValue = max;
        }

        /// <summary>
        /// Constructor used for Unit testing of NurbsGenerator class
        /// </summary>
        /// <param name="degree">degree of the NURBS curve</param>
        public NurbsValues(int degree)
        {
            Degree = degree;
        }

        /// <summary>
        /// Constructor used for Unit testing of NurbsGenerator class
        /// </summary>
        /// <param name="knots">knots vector</param>
        /// <param name="degree">degree of the NURBS curve</param>
        /// <param name="max">max value mapped to the interval 0-1</param>
        /// <param name="controlPoints">control points</param>
        public NurbsValues(double[] knots, int degree, double max, double[] controlPoints)
        {
            KnotVector = knots;
            Degree = degree;
            MaxValue = max;
            ControlPoints = controlPoints;
        }

        #endregion constructor
    }
}
