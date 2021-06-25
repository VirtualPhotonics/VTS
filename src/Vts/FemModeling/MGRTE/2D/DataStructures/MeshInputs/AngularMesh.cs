namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// Angular mesh structure
    /// </summary>
    public struct AngularMesh
    {
        private int _ns;
        private double[][] _ang;
        private double[][][] _w;
        /// <summary>
        /// number of angular nodes
        /// </summary>
        public int Ns { get { return _ns; } set { _ns = value; } }         
        /// <summary>
        /// angular coordinates: a[ns][3] with a[i][3]:=(cos(theta(i)), sin(theta(i)), theta(i))
        /// </summary>
        public double[][] Ang { get { return _ang; } set { _ang = value; } }    
        /// <summary>
        /// angular weights: w[ns][ns] 
        /// </summary>
        public double[][][] W { get { return _w; } set { _w = value; } }   
    }
}
