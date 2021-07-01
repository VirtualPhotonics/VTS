namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// Measurement structure
    /// </summary>
    public struct Measurement
    {
        /// <summary>
        /// fluence at each node
        /// </summary>
        public double[] Fluence { get; set; }  
        
        /// <summary>
        /// radiance at each node
        /// </summary>
        public double[][] Radiance { get; set; }
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
        public double[] Inten { get; set; }     
        
        /// <summary>
        /// dx spacing between each grid
        /// </summary>
        public double[] Dx { get; set; }
        
        /// <summary>
        /// dz spacing between each grid
        /// </summary>
        public double[] Dz { get; set; }        
    }
}

