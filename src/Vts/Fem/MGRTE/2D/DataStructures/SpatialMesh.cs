namespace Vts.Fem.MGRTE._2D.DataStructures
{
    public struct SpatialMesh  // angular mesh
    {
        public int nt;                   // number of triangles (data from MATLAB)
        public int np;                   // number of nodes (data from MATLAB)
        public int ne;                   // number of edges (data from MATLAB)
        public int[][] so;               // sweep ordering: so[ns][nt] (data from MATLAB)
        public double[][] p;             // nodal coordinates: p[np][2] (data from MATLAB)
        public int[][] t;                // nodes contained in one triangle: t[nt][3] (data from MATLAB)
        public int[][] e;                // nodes contained in one boundary edge: e[ne][4] (data from MATLAB and see "boundary")
        public double[][] c;             // center of triangles: c[nt][2] (see "initialization")
        public double[][] ec;            // center of edges: ec[ne][2] (see "initialization")
        public double[] a;               // area of triangles: a[nt] (see "initialization")
        public int[][] p2;               // triangles adjacent to one node: p2[np][p2[np][0]+1] (see "initialization")
        public int[][] smap;             // spatial position mapping between coarse and fine mesh: smap[nt_c][smap[nt_c][0]+1] (see "spatialmapping")
        public double[][][][] cf;        // spatial coarse-to-fine nodal-value mapping: cf[nt_c][3][smap[nt_c][0]][3] (see "spatialmapping2")
        public double[][][][] fc;        // spatial fine-to-coarse nodal-value mapping: fc[nt_c][3][smap[nt_c][0]][3] (see "spatialmapping2")
        public int[] ori;                // boundary information: ori[ne] (see "boundary")
        public int[][] e2;               // boundary information: e2[ne][2] (see "boundary")
        public int[][] so2;              // boundary information: so2[nt][3] (see "boundary")
        public double[][] n;             // boundary information: n[ne][2] (see "boundary")
        public int[][][] bd;             // edge upwind or outgoing flux: bd[ns][nt][3*3] (see "edgeterm")
        public double[][][] bd2;         // edge upwind or outgoing flux: bd[ns][nt][3]   (see "edgeterm")    
    }
}
