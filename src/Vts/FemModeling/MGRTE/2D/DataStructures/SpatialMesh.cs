namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// Spatial mesh structure
    /// </summary>
    public struct SpatialMesh
    {   
        /// <summary>
        /// number of node points 
        /// </summary>
        public int np; 
        
        /// <summary>
        /// number of boundary edges    
        /// </summary>
        public int ne;                   
 
        /// <summary>
        /// number of triangles 
        /// </summary>
        public int nt;                  

        /// <summary>
        /// nodal coordinates: p[np][2] 
        /// </summary>
        public double[][] p;             

        /// <summary>
        /// nodes contained in one triangle: t[nt][3] 
        /// </summary>
        public int[][] t;                

        /// <summary>
        /// nodes contained in one boundary edge: e[ne][4] 
        /// </summary>
        public int[][] e;                

        /// <summary>
        /// sweep ordering: so[ns][nt] 
        /// </summary>
        public int[][] so;               

        /// <summary>
        /// center of triangles: c[nt][2] (see "initialization")
        /// </summary>
        public double[][] c;             

        /// <summary>
        /// center of edges: ec[ne][2] (see "initialization")
        /// </summary>
        public double[][] ec;            

        /// <summary>
        /// area of triangles: a[nt] (see "initialization")
        /// </summary>
        public double[] a;    

        /// <summary>
        /// triangles adjacent to one node: p2[np][p2[np][0]+1] (see "initialization")
        /// </summary>
        public int[][] p2;               

        /// <summary>
        /// spatial position mapping between coarse and fine mesh: smap[nt_c][smap[nt_c][0]+1] (see "spatialmapping")
        /// </summary>
        public int[][] smap;            

        /// <summary>
        /// spatial coarse-to-fine nodal-value mapping: cf[nt_c][3][smap[nt_c][0]][3] (see "spatialmapping2")
        /// </summary>
        public double[][][][] cf;        
        
        /// <summary>
        /// spatial fine-to-coarse nodal-value mapping: fc[nt_c][3][smap[nt_c][0]][3] (see "spatialmapping2")
        /// </summary>
        public double[][][][] fc;        

        /// <summary>
        /// boundary information: ori[ne] (see "boundary")
        /// </summary>
        public int[] ori;                

        /// <summary>
        /// boundary information: e2[ne][2] (see "boundary")
        /// </summary>
        public int[][] e2;               

        /// <summary>
        /// boundary information: so2[nt][3] (see "boundary")
        /// </summary>
        public int[][] so2;              

        /// <summary>
        /// boundary information: n[ne][2] (see "boundary")
        /// </summary>
        public double[][] n;             

        /// <summary>
        /// edge upwind or outgoing flux: bd[ns][nt][3*3] (see "edgeterm")
        /// </summary>
        public int[][][] bd;            

        /// <summary>
        /// edge upwind or outgoing flux: bd[ns][nt][3]   (see "edgeterm")    
        /// </summary>
        public double[][][] bd2;         
    }
}
