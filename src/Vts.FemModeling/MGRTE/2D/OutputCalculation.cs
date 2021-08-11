using System;
using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D
{
    public class OutputCalculation
    {
        public double Pi = GlobalConstants.Pi;

        public Measurement RteOutput(double[][][] flux, double[][][] q, AngularMesh amesh, SpatialMesh smesh, BoundaryCoupling b, int vacuum)
        
        {  
            int i, j, k;            
            int nxy;
            int[][] t;
            int nt = smesh.Nt, ns = amesh.Ns, np = smesh.Np;       
            double dtheta = 2 * Pi / ns;
            t = smesh.T;

            Measurement Det = new Measurement();
          

            nxy = (int)Math.Ceiling(Math.Sqrt(nt / 2.0)) + 1;            
            
            Det.fluence = new double[np];

            Det.radiance = new double[np][];
            for (i = 0; i < np; i++)
                Det.radiance[i] = new double[ns];

            Det.uxy = new double[nxy][];
            for (i = 0; i < nxy; i++)
                Det.uxy[i] = new double[nxy];

            Det.xloc = new double[nxy];
            Det.zloc = new double[nxy];
            Det.dx = new double[nxy];
            Det.dz = new double[nxy];
            Det.inten = new double[nxy*nxy];      

            
            // compute radiance at each node
            for (i = 0; i < nt; i++)
            {                
                for (j = 0; j < ns; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        Det.radiance[t[i][k]][j] = flux[j][i][k];
                    }
                }
            }

            // compute fluence at each node
            for (i = 0; i < np; i++)
            {
                Det.fluence[i] = 0;
                for (j = 0; j < ns; j++)
                {
                    Det.fluence[i] += Det.radiance[i][j];                    
                }
                Det.fluence[i] *= dtheta;
            }

            MathFunctions.SquareTriMeshToGrid(ref smesh, ref Det.xloc, ref Det.zloc, ref Det.uxy, Det.fluence, nxy);

            for (i = 0; i < nxy; i++)
                for (j = 0; j < nxy; j++)
            {
                Det.inten[i * nxy + j] = Det.uxy[i][j];
            }

            for (i = 0; i < nxy; i++)
            {
                Det.dx[i] = Det.xloc[1] - Det.xloc[0];
                Det.dz[i] = Det.zloc[1] - Det.zloc[0];
            }


            
            return Det;
        }
    }
}

