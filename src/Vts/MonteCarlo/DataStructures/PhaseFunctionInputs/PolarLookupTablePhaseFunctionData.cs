using System;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.LookupTablePhaseFunctionData
{
    public class PolarLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        public PolarLookupTablePhaseFunctionData(double [] lutAngles, double[] lutPdf, double[] lutCdf)
        {
            LookupTablePhaseFunctionDataType = "Polar";
            LutAngles = lutAngles;
            LutPdf = lutPdf;
            LutCdf = lutCdf;
            //CreateCdfFromPdf();
        }

        /// <summary>
        /// default constructor for serialization purposes
        /// </summary>
        public PolarLookupTablePhaseFunctionData() : this(
            new double[] {0, Math.PI/6, Math.PI/3, Math.PI/2, 2*Math.PI/3, Math.PI*5/6, Math.PI},
            new double[] {0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5},
            new double[] {0, 0.5*(1 - Math.Sqrt(3)/2), 0.25, 0.5, 0.75, 0.5*(1 + Math.Sqrt(3)/2), 1}
            )
        {}

        /// <summary>
        /// Type of data
        /// </summary>
        public string LookupTablePhaseFunctionDataType { get; set; }
        /// <summary>
        /// LutAngles are the theta angles
        /// </summary>
        public double[] LutAngles { get; set; }
        /// <summary>
        /// LutPdf is the scattering angles probability distribution function (PDF) for each theta angle
        /// </summary>
        public double[] LutPdf { get; set; }
        /// <summary>
        /// lutCdf is the cumulative distribution function (CDF) associated with the PDF
        /// </summary>
        public double[] LutCdf { get; set; }

        private void CreateCdfFromPdf()
        {
            var numAngles = LutAngles.Length;
            // determine norm of pdf in case not already normalized
            double norm = 0.0;
            for (int i = 1; i < numAngles; i++)
            {
                norm += LutPdf[i] * Math.Sin(LutAngles[i]) * (LutAngles[i] - LutAngles[i-1]);
            }
            // determine cdf from pdf
            double sum = 0.0;
            for (int i = 1; i < numAngles; i++)
            {
                sum += (LutPdf[i] / norm) * Math.Sin(LutAngles[i]) * (LutAngles[i] - LutAngles[i - 1]);
                LutCdf[i] = sum;
            }
        }
    }
}