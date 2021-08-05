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
        public double[] fluence;  
        
        /// <summary>
        /// radiance at each node
        /// </summary>
        public double[][] radiance; 
        
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
        public double[] inten;     
        
        /// <summary>
        /// dx spacing between each grid
        /// </summary>
        public double[] dx;
        
        /// <summary>
        /// dz spacing between each grid
        /// </summary>
        public double[] dz;        
    }
}

