using System;
using System.Linq;
using Vts.Common.Math;
using Vts.Extensions;

namespace Vts.SiteVisit.Model
{
    public class MapData : BindableObject
    {
        /// <summary>
        /// Model object to represent 2D data for plotting
        /// </summary>
        /// <param name="rawData">1D row-major array of 2D data</param>
        /// <param name="xValues">The 1D horizontal axis independent values</param>
        /// <param name="yValues">The 1D vertical axis independent values</param>
        /// <param name="dxValues">Values representing the area or length of each horizontal independent value, for calculation of expectation</param>
        /// <param name="dyValues">Values representing the area or length of each vertical independent value, for calculation of expectation</param>
        /// <remarks>Example of dx, dy values for curvilinear coordinates: dx=(2*Pi*rho*drho), dy=dz</remarks>
        public MapData(double[] rawData, double[] xValues, double[] yValues, double[] dxValues, double[] dyValues)
        {
            this.XValues = xValues;
            this.YValues = yValues;
            this.DxValues = dxValues;
            this.DyValues = dyValues;
            this.RawData = rawData;
            this.Min = rawData.Min();
            this.Max = rawData.Max();
        }

        public int Width { get { return XValues.Length; } }
        public int Height { get { return YValues.Length; } }
        public double[] RawData { get; private set; }
        public double[] XValues { get; private set; }
        public double[] YValues { get; private set; }
        public double[] DxValues { get; private set; }
        public double[] DyValues { get; private set; }
        public double Min { get; private set; }
        public double Max { get; private set; }
        public double YExpectationValue { get { return Statistics.MeanSamplingDepth(RawData, XValues, YValues, DxValues, DyValues); } }

        public static MapData Create(double[,] rawData, double[] x, double[] y, double[] dx, double[] dy)
        {
            if (rawData.GetLength(0) != x.Length || rawData.GetLength(1) != y.Length)
                throw new ArgumentException("Array lengths do not match.");

            return new MapData(rawData.ToEnumerable<double>().ToArray(), x, y, dx, dy);
        }

        public static MapData Create(double[] rawData, double[] x, double[] y, double[] dx, double[] dy)
        {
            if (rawData.Length != x.Length * y.Length)
                throw new ArgumentException("Array lengths do not match.");

            return new MapData(rawData.ToArray(), x, y, dx, dy);
        }
    }
}
