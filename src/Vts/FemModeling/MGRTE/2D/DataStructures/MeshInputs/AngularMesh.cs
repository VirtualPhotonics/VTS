﻿namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// Angular mesh structure
    /// </summary>
    public struct AngularMesh
    {
        /// <summary>
        /// number of angular nodes
        /// </summary>
        public int Ns { get; set; }         
        /// <summary>
        /// angular coordinates: a[ns][3] with a[i][3]:=(cos(theta(i)), sin(theta(i)), theta(i))
        /// </summary>
        public double[][] Ang { get; set; }    
        /// <summary>
        /// angular weights: w[ns][ns] 
        /// </summary>
        public double[][][] W { get; set; }   
    }
}
