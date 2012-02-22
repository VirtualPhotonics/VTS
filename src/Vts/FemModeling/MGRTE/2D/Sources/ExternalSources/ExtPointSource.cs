using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class ExtPointSource : IExtSource
    {
        public ExtPointSource(
            DoubleRange launchPoint,
            DoubleRange thetaRange)
        {
            LaunchPoint = launchPoint;
            ThetaRange = thetaRange;
        }

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


        public void AssignMeshForExtSource(AngularMesh[] amesh, int aMeshLevel, SpatialMesh[] smesh, int sMeshLevel, int level, double[][][][] q)
        {
            double[] distance = new double[smesh[sMeshLevel].nt];
            int i,j,k;
            double x,x1,x2,z,z1,z2;
            int ns_start, ns_stop;
            
            x = LaunchPoint.Start;
            z = LaunchPoint.Stop;

            ns_start = (int)(0.5 * ThetaRange.Start * amesh[aMeshLevel].ns/ Math.PI);
            ns_stop = (int)(0.5 * ThetaRange.Stop * amesh[aMeshLevel].ns / Math.PI);

            for (i = 0; i < smesh[sMeshLevel].ne; i++)
            {
                x1 = smesh[sMeshLevel].p[smesh[sMeshLevel].e[i][1]][0];
                x2 = smesh[sMeshLevel].p[smesh[sMeshLevel].e[i][2]][0];
                z1 = smesh[sMeshLevel].p[smesh[sMeshLevel].e[i][1]][1];
                z2 = smesh[sMeshLevel].p[smesh[sMeshLevel].e[i][2]][1];

                if ((z1 == z) && (z2 == z))
                {
                    if ((x1 - x >= 0) && (x - x2 >= 0))
                    {
                        for (k = 0; k < amesh[aMeshLevel].ns; k++)
                        {
                            if ((k < ns_start) || (k > ns_stop))
                                q[level][amesh[aMeshLevel].ns-1-k][i][0] = 0;
                            else
                            {
                                q[level][amesh[aMeshLevel].ns - 1 - k][i][0] = 0.5*Math.Abs((x2 - x)/(x1 - x2));
                                q[level][amesh[aMeshLevel].ns - 1 - k][i][1] = 0.5*Math.Abs((x1 - x)/(x1 - x2));

                            }
                        }
                    }
                    if ((x2 - x >= 0) && (x - x1 >= 0))
                    {
                        for (k = 0; k < amesh[aMeshLevel].ns; k++)
                        {
                            if ((k < ns_start) || (k > ns_stop))
                                q[level][amesh[aMeshLevel].ns - 1 - k][i][0] = 0;
                            else
                            {
                                q[level][amesh[aMeshLevel].ns - 1 - k][i][0] = 0.5 * Math.Abs((x2 - x) / (x1 - x2));
                                q[level][amesh[aMeshLevel].ns - 1 - k][i][1] = 0.5 * Math.Abs((x1 - x) / (x1 - x2));

                            }
                        }
                    }
                }
            }
        }
    }

}
