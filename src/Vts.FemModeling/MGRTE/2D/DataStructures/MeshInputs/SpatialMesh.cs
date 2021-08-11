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
        public int Np; 
        
        /// <summary>
        /// number of boundary edges    
        /// </summary>
        public int Ne;                   
 
        /// <summary>
        /// number of triangles 
        /// </summary>
        public int Nt;

        /// <summary>
        /// tissue region Region[np]
        /// </summary>
        public int []Region;
        
        /// <summary>
        /// nodal coordinates: p[np][2] 
        /// </summary>
        public double[][] P;             

        /// <summary>
        /// nodes contained in one triangle: t[nt][3] 
        /// </summary>
        public int[][] T;                

        /// <summary>
        /// nodes contained in one boundary edge: e[ne][4] 
        /// </summary>
        public int[][] E;                

        /// <summary>
        /// sweep ordering: so[ns][nt] 
        /// </summary>
        public int[][] So;               

        /// <summary>
        /// center of triangles: c[nt][2] (see "initialization")
        /// </summary>
        public double[][] C;             

        /// <summary>
        /// center of edges: ec[ne][2] (see "initialization")
        /// </summary>
        public double[][] Ec;            

        /// <summary>
        /// area of triangles: a[nt] (see "initialization")
        /// </summary>
        public double[] A;    

        /// <summary>
        /// triangles adjacent to one node: p2[np][p2[np][0]+1] (see "initialization")
        /// </summary>
        public int[][] P2;               

        /// <summary>
        /// spatial position mapping between coarse and fine mesh: smap[nt_c][smap[nt_c][0]+1] (see "spatialmapping")
        /// </summary>
        public int[][] Smap;            

        /// <summary>
        /// spatial coarse-to-fine nodal-value mapping: cf[nt_c][3][smap[nt_c][0]][3] (see "spatialmapping2")
        /// </summary>
        public double[][][][] Cf;        
        
        /// <summary>
        /// spatial fine-to-coarse nodal-value mapping: fc[nt_c][3][smap[nt_c][0]][3] (see "spatialmapping2")
        /// </summary>
        public double[][][][] Fc;        

        /// <summary>
        /// boundary information: ori[ne] (see "boundary")
        /// </summary>
        public int[] Ori;                

        /// <summary>
        /// boundary information: e2[ne][2] (see "boundary")
        /// </summary>
        public int[][] E2;               

        /// <summary>
        /// boundary information: so2[nt][3] (see "boundary")
        /// </summary>
        public int[][] So2;              

        /// <summary>
        /// boundary information: n[ne][2] (see "boundary")
        /// </summary>
        public double[][] N;             

        /// <summary>
        /// edge upwind or outgoing flux: bd[ns][nt][3*3] (see "edgeterm")
        /// </summary>
        public int[][][] Bd;            

        /// <summary>
        /// edge upwind or outgoing flux: bd[ns][nt][3]   (see "edgeterm")    
        /// </summary>
        public double[][][] Bd2;
    }
}
