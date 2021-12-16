using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// External point source
    /// </summary>
    public class ExtPointSource : IExtSource
    {
        /// <summary>
        /// General constructor for external point source
        /// </summary>
        /// <param name="launchPoint"></param>
        /// <param name="thetaRange"></param>
        public ExtPointSource(
            DoubleRange launchPoint,
            DoubleRange thetaRange)
        {
            LaunchPoint = launchPoint;
            ThetaRange = thetaRange;
        }

        /// <summary>
        /// Default constructor for external point source
        /// </summary>
        public ExtPointSource()
            : this(
             new DoubleRange(0, 0),
             new DoubleRange(Math.PI, 2.0 * Math.PI)) { }

        /// <summary>
        /// Launching coordinates (x,z) of the boundary point source
        /// </summary>
        public DoubleRange LaunchPoint { get; set; }

        /// <summary>
        /// Theta angle range
        /// </summary>
        public DoubleRange ThetaRange { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aMesh"></param>
        /// <param name="aMeshLevel"></param>
        /// <param name="sMesh"></param>
        /// <param name="sMeshLevel"></param>
        /// <param name="level"></param>
        /// <param name="q"></param>
        public void AssignMeshForExtSource(AngularMesh[] aMesh, int aMeshLevel, SpatialMesh[] sMesh, int sMeshLevel, int level, double[][][][] q)
        {
            double[] distance = new double[sMesh[sMeshLevel].Nt];
            int i, k;
            double x,x1,x2,z,z1,z2;
            int ns_start, ns_stop;
            
            x = LaunchPoint.Start;
            z = LaunchPoint.Stop;

            ns_start = (int)(0.5 * ThetaRange.Start * aMesh[aMeshLevel].Ns/ Math.PI);
            ns_stop = (int)(0.5 * ThetaRange.Stop * aMesh[aMeshLevel].Ns / Math.PI);

            for (i = 0; i < sMesh[sMeshLevel].Ne; i++)
            {
                x1 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].E[i][1]][0];
                x2 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].E[i][2]][0];
                z1 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].E[i][1]][1];
                z2 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].E[i][2]][1];

                if ((z1 == z) && (z2 == z))
                {
                    if ((x1 - x >= 0) && (x - x2 >= 0))
                    {
                        for (k = 0; k < aMesh[aMeshLevel].Ns; k++)
                        {
                            if ((k < ns_start) || (k > ns_stop))
                                q[level][aMesh[aMeshLevel].Ns-1-k][i][0] = 0;
                            else
                            {
                                q[level][aMesh[aMeshLevel].Ns - 1 - k][i][0] = 0.5*Math.Abs((x2 - x)/(x1 - x2));
                                q[level][aMesh[aMeshLevel].Ns - 1 - k][i][1] = 0.5*Math.Abs((x1 - x)/(x1 - x2));

                            }
                        }
                    }
                    if ((x2 - x >= 0) && (x - x1 >= 0))
                    {
                        for (k = 0; k < aMesh[aMeshLevel].Ns; k++)
                        {
                            if ((k < ns_start) || (k > ns_stop))
                                q[level][aMesh[aMeshLevel].Ns - 1 - k][i][0] = 0;
                            else
                            {
                                q[level][aMesh[aMeshLevel].Ns - 1 - k][i][0] = 0.5 * Math.Abs((x2 - x) / (x1 - x2));
                                q[level][aMesh[aMeshLevel].Ns - 1 - k][i][1] = 0.5 * Math.Abs((x1 - x) / (x1 - x2));

                            }
                        }
                    }
                }
            }
        }
    }

}
