using System;
using System.Runtime.Serialization;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.LookupTablePhaseFunctionData
{
    public class PolarLookupTablePhaseFunctionData : ILookupTablePhaseFunctionData
    {
        public PolarLookupTablePhaseFunctionData(double [] lutAngles, double[] lutPdf)
        {
            LookupTablePhaseFunctionDataType = "Polar";
            LutAngles = lutAngles;
            LutPdf = lutPdf;
            //LutCdf = lutCdf;
            CreateCdfFromPdf();
        }

        /// <summary>
        /// default constructor for serialization purposes
        /// </summary>
        public PolarLookupTablePhaseFunctionData() : this(
            new double[] {0, Math.PI/6, Math.PI/3, Math.PI/2, 2*Math.PI/3, Math.PI*5/6, Math.PI},
            new double[] {0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5}
           // new double[] {0, 0.5*(1 - Math.Sqrt(3)/2), 0.25, 0.5, 0.75, 0.5*(1 + Math.Sqrt(3)/2), 1}
            )
        {}

        /// <summary>
        /// Type of data
        /// </summary>
        [DataMember]
        public string LookupTablePhaseFunctionDataType { get; set; }
        /// <summary>
        /// LutAngles are the theta angles
        /// </summary>
        [DataMember]
        public double[] LutAngles { get; set; }
        /// <summary>
        /// LutPdf is the scattering angles probability distribution function (PDF) for each theta angle
        /// </summary>
        [DataMember]
        public double[] LutPdf { get; set; }
        /// <summary>
        /// lutCdf is the cumulative distribution function (CDF) associated with the PDF
        /// </summary>
        [IgnoreDataMember]
        public double[] LutCdf { get; set; }

        private void CreateCdfFromPdf()
        {
            var numAngles = LutAngles.Length;
            // determine norm of pdf in case not already normalized 
            double norm = 0.0;
            for (int i = 1; i < numAngles; i++)
            {
                // trapezoid rule to integrate
                norm += (LutPdf[i] * Math.Sin(LutAngles[i]) + LutPdf[i-1] * Math.Sin(LutAngles[i-1])) / 2 * (LutAngles[i] - LutAngles[i-1]);
            }
            // determine cdf from pdf: cdf(x) = int^x pdf(theta) sin(theta) dtheta
            LutCdf = new double[LutPdf.Length];
            LutCdf[0] = 0.0;
            double sum = 0.0;
            for (int i = 1; i < numAngles; i++)
            {
                // trapezoid rule to integrate
                sum += (LutPdf[i] * Math.Sin(LutAngles[i]) + LutPdf[i-1]*Math.Sin(LutAngles[i-1])) / ( 2 * norm ) * (LutAngles[i] - LutAngles[i - 1]);
                LutCdf[i] = sum;
            }
        }
    }
}