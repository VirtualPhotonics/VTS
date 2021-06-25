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
            
            Det.Fluence = new double[np];

            Det.Radiance = new double[np][];
            for (i = 0; i < np; i++)
                Det.Radiance[i] = new double[ns];

            Det.uxy = new double[nxy][];
            for (i = 0; i < nxy; i++)
                Det.uxy[i] = new double[nxy];

            Det.zloc = new double[nxy];
            Det.zloc = new double[nxy];
            Det.Dx = new double[nxy];
            Det.Dz = new double[nxy];
            Det.Inten = new double[nxy*nxy];      

            
            // compute radiance at each node
            for (i = 0; i < nt; i++)
            {                
                for (j = 0; j < ns; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        Det.Radiance[t[i][k]][j] = flux[j][i][k];
                    }
                }
            }

            // compute fluence at each node
            for (i = 0; i < np; i++)
            {
                Det.Fluence[i] = 0;
                for (j = 0; j < ns; j++)
                {
                    Det.Fluence[i] += Det.Radiance[i][j];                    
                }
                Det.Fluence[i] *= dtheta;
            }

            MathFunctions.SquareTriMeshToGrid(ref smesh, ref Det.xloc, ref Det.zloc, ref Det.uxy, Det.Fluence, nxy);

            for (i = 0; i < nxy; i++)
                for (j = 0; j < nxy; j++)
            {
                Det.Inten[i * nxy + j] = Det.uxy[i][j];
            }

            for (i = 0; i < nxy; i++)
            {
                Det.Dx[i] = Det.xloc[1] - Det.xloc[0];
                Det.Dz[i] = Det.zloc[1] - Det.zloc[0];
            }


            
            return Det;
        }
    }
}

