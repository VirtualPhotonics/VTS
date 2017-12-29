using System;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.LookupTablePhaseFunctionData
{
    public class PolarAndAzimuthalLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        /// <summary>
        /// Polar and azimuthal lookup table CKH: show lutAngles be double[,]?
        /// </summary>
        /// <param name="lutAngles"></param>
        /// <param name="lutPdf"></param>
        /// <param name="lutCdf"></param>
        public PolarAndAzimuthalLookupTablePhaseFunctionData(double[] lutAngles, double[] lutPdf, double[] lutCdf)
        {
            LookupTablePhaseFunctionDataType = "PolarAndAzimuthal";
            LutAngles = lutAngles;
            LutPdf = lutPdf;
            LutCdf = lutCdf;
        }

        /// <summary>
        /// default constructor for serialization purposes
        /// </summary>
        public PolarAndAzimuthalLookupTablePhaseFunctionData() : this(
            new double[]{0, Math.PI/6, Math.PI/3, Math.PI/2, 2*Math.PI/3, Math.PI*5/6, Math.PI},
            new double[] {0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5},
            new double[] {0, 0.5*(1 - Math.Sqrt(3)/2), 0.25, 0.5, 0.75, 0.5*(1 + Math.Sqrt(3)/2), 1}
            )
        {}

        /// <summary>
        /// Type of data
        /// </summary>
        public string LookupTablePhaseFunctionDataType { get; set; }
        /// <summary>
        /// theta, phi angles
        /// </summary>
        public double[] LutAngles { get; set; }
        /// <summary>
        /// lookup pdf given theta and phi
        /// </summary>
        public double[] LutPdf { get; set; }
        /// <summary>
        /// lookup cdf given theta and phi
        /// </summary>
        public double[] LutCdf { get; set; }
    }
}