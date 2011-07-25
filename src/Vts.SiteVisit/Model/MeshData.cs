using System;
using System.Linq;
using Vts.Common.Math;
using Vts.Extensions;

namespace Vts.SiteVisit.Model
{
    public class MapData : BindableObject
    {
        public MapData(double[] rawData, double[] xValues, double[] yValues)
        {
            this.XValues = xValues;
            this.YValues = yValues;
            this.RawData = rawData;
            this.Min = rawData.Min();
            this.Max = rawData.Max();
        }

        public int Width { get { return XValues.Length; } }
        public int Height { get { return YValues.Length; } }
        public double[] RawData { get; private set; }
        public double[] XValues { get; private set; }
        public double[] YValues { get; private set; }
        public double Min { get; private set; }
        public double Max { get; private set; }
        public double YExpectationValue { get { return Statistics.MeanSamplingDepth(RawData, XValues, YValues); } }

        public static MapData Create(double[,] rawData, double[] x, double[] y)
        {
            if (rawData.GetLength(0) != x.Length || rawData.GetLength(1) != y.Length)
                throw new ArgumentException("Array lengths do not match.");

            return new MapData(rawData.ToEnumerable<double>().ToArray(), x, y);
        }

        public static MapData Create(double[] rawData, double[] x, double[] y)
        {
            if (rawData.Length != x.Length * y.Length)
                throw new ArgumentException("Array lengths do not match.");

            return new MapData(rawData.ToArray(), x, y);
        }
    }
}
