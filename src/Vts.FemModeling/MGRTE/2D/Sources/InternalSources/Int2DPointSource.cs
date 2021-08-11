using System;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// Internal 2D Point source
    /// </summary>
    public class Int2DPointSource : IIntSource
    {
        /// <summary>
        /// General constructor for 2D point source
        /// </summary>
        /// <param name="center"></param>
        /// <param name="thetaRange"></param>
        public Int2DPointSource(
            DoubleRange center,
            DoubleRange thetaRange)
        {
            Center = center;
            ThetaRange = thetaRange;
        }

        /// <summary>
        /// Default constructor for 2D point source
        /// </summary>
        public Int2DPointSource()
            : this(
             new DoubleRange(0, 0.5),
             new DoubleRange(0, 2 * Math.PI)) { }

        /// <summary>
        /// Center cooordinates (x,z) of the geometry 
        /// </summary>
        public DoubleRange Center { get; set; }
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
        /// <param name="rhs"></param>
        public void AssignMeshForIntSource(AngularMesh[] aMesh, int aMeshLevel, SpatialMesh[] sMesh, int sMeshLevel, int level, double[][][][] rhs)
        {
            double[] distance = new double[sMesh[sMeshLevel].Nt];
            int i,j,k;
            double x,x1,x2,x3,z,z1,z2,z3;
            int ns_start, ns_stop;
            
        
            double[] area = new double[3];
            double area_total;

            x = Center.Start;
            z = Center.Stop;

            ns_start = (int)(0.5*ThetaRange.Start * aMesh[aMeshLevel].Ns/ Math.PI);
            ns_stop = (int)(0.5 * ThetaRange.Stop * aMesh[aMeshLevel].Ns / Math.PI);

            for (i = 0; i < sMesh[sMeshLevel].Nt; i++)
            {
                x1 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].T[i][0]][0];
                x2 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].T[i][1]][0];
                x3 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].T[i][2]][0];
                z1 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].T[i][0]][1];
                z2 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].T[i][1]][1];
                z3 = sMesh[sMeshLevel].P[sMesh[sMeshLevel].T[i][2]][1];

                double detT = (z2 - z3) * (x1 - x3) + (x3 - x2) * (z1 - z3);

                double l1 = ((z2 - z3) * (x - x3) + (x3 - x2) * (z - z3)) / detT;
                double l2 = ((z3 - z1) * (x - x3) + (x1 - x3) * (z - z3)) / detT;
                double l3 = 1 - l1 - l2;


                if (l1 >= 0.0)
                {
                    if (l1 <= 1.0)
                    {
                        if (l2 >= 0.0)
                        {
                            if (l2 <= 1.0)
                            {
                                if (l3 >= 0.0)
                                {
                                    if (l3 <= 1.0)
                                    {
                                        area[0] = MathFunctions.Area(x, z, x2, z2, x3, z3);
                                        area[1] = MathFunctions.Area(x1, z1, x, z, x3, z3);
                                        area[2] = MathFunctions.Area(x1, z1, x2, z2, x, z);
                                        area_total = area[0] + area[1] + area[2];
                                        for (j = 0; j < 3; j++)
                                        {
                                            for (k = 0; k < aMesh[aMeshLevel].Ns; k++)
                                            {
                                                if ((k < ns_start) || (k > ns_stop))
                                                    rhs[level][aMesh[aMeshLevel].Ns-1-k][i][j] = 0;
                                                else
                                                    rhs[level][aMesh[aMeshLevel].Ns-1-k][i][j] = area[j] / area_total;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                
            }
        }

    }
}
