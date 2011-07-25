using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public struct Measurement
    {
        public int B;              // B=1 for boundary measurement; B=0 for internal measurement.
        public int A;              // A=1 for angular-averaged measurement; A=0 for angular-resolved measurement.
        public int n;              // number of measurements
        public double[][] coord;     // coord[n][N]:     N=1 (x) for angular-averaged measurement; N=2 (x,s) for angular-resolved measurement.
        //            Note: when B=1, coord[n][0]="0" or "1", indicating the boundary index.
        public double[] output;     // output[n]:       the output
        public double[][] flux;     // flux[ns][nt]:    the averaged flux at each interval for each direction
        public double[] density;    // density[nt]:     the averaged density at each interval
        public double[] fluence;  //fluence at each node
        public double[][] radiance;  //fluence at each node
        public double[][] uxy;   //rect grid
    }
}

