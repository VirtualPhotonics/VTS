namespace Vts.Modeling.ForwardSolvers.MGRTE._2D.DataStructures
{
    public struct AngularMesh  // angular mesh  
    {
        public int ns;           // number of angular nodes (data from MATLAB)
        public double[][] a;    // angular coordinates: a[ns][3] with a[i][3]:=(cos(theta(i)), sin(theta(i)), theta(i)). (data from MATLAB)
        public double[,] w;    // angular weights: w[ns][ns] (data from MATLAB)
    }
}
