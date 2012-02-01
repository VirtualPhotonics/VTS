namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// Angular mesh structure
    /// </summary>
    public struct AngularMesh
    {
        /// <summary>
        /// number of angular nodes
        /// </summary>
        public int ns;         
        /// <summary>
        /// angular coordinates: a[ns][3] with a[i][3]:=(cos(theta(i)), sin(theta(i)), theta(i))
        /// </summary>
        public double[][] a;    
        /// <summary>
        /// angular weights: w[ns][ns] 
        /// </summary>
        public double[][] w;   
    }
}
