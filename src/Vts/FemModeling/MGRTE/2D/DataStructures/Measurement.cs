namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// Measurement structure
    /// </summary>
    public struct Measurement
    {
        private double[] _fluence;
        private double[][] _radiance;
        private double[] _inten;
        private double[] _dx;
        private double[] _dz;
        /// <summary>
        /// fluence at each node
        /// </summary>
        public double[] Fluence { get { return _fluence; } set { _fluence = value; } }  
        
        /// <summary>
        /// radiance at each node
        /// </summary>
        public double[][] Radiance { get { return _radiance; } set { _radiance = value; } }
        // the following 3 are not properties because they are passed by reference to
        // a method and properties cannot be passed by reference
        /// <summary>
        /// rectangular grid array
        /// </summary>
        public double[][] uxy;

        /// <summary>
        /// x coordinates
        /// </summary>
        public double[] xloc;

        /// <summary>
        /// z coordinates
        /// </summary>
        public double[] zloc;
        
        /// <summary>
        /// Intensity
        /// </summary>
        public double[] Inten { get { return _inten; } set { _inten = value; } }     
        
        /// <summary>
        /// dx spacing between each grid
        /// </summary>
        public double[] Dx { get { return _dx; } set { _dx = value; } }
        
        /// <summary>
        /// dz spacing between each grid
        /// </summary>
        public double[] Dz { get { return _dz; } set { _dz = value; } }        
    }
}

