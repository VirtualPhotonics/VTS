namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    /// <summary>
    /// Spatial mesh structure
    /// </summary>
    public struct SpatialMesh
    {
        private int _np;
        private int _ne;
        private int _nt;
        private int[] _region;
        private double[][] _p;
        private int[][] _t;
        private int[][] _e;
        private int[][] _so;
        private double[][] _c;
        private double[][] _ec;
        private double[] _a;
        private int[][] _p2;
        private int[][] _smap;
        private double[][][][] _cf;
        private double[][][][] _fc;
        private int[] _ori;
        private int[][] _e2;
        private int[][] _so2;
        private double[][] _n;
        private int[][][] _bd;
        private double[][][] _bd2;
        /// <summary>
        /// number of node points 
        /// </summary>
        public int Np { get { return _np; } set { _np = value; } } 
        
        /// <summary>
        /// number of boundary edges    
        /// </summary>
        public int Ne { get { return _ne; } set { _ne = value; } }

        /// <summary>
        /// number of triangles 
        /// </summary>
        public int Nt { get { return _nt; } set { _nt = value; } }

        /// <summary>
        /// tissue region Region[np]
        /// </summary>
        public int []Region { get { return _region; } set { _region = value; } }

        /// <summary>
        /// nodal coordinates: p[np][2] 
        /// </summary>
        public double[][] P { get { return _p; } set { _p = value; } }

        /// <summary>
        /// nodes contained in one triangle: t[nt][3] 
        /// </summary>
        public int[][] T { get { return _t; } set { _t = value; } }

        /// <summary>
        /// nodes contained in one boundary edge: e[ne][4] 
        /// </summary>
        public int[][] E { get { return _e; } set { _e = value; } }

        /// <summary>
        /// sweep ordering: so[ns][nt] 
        /// </summary>
        public int[][] So { get { return _so; } set { _so = value; } }

        /// <summary>
        /// center of triangles: c[nt][2] (see "initialization")
        /// </summary>
        public double[][] C { get { return _c; } set { _c = value; } }

        /// <summary>
        /// center of edges: ec[ne][2] (see "initialization")
        /// </summary>
        public double[][] Ec { get { return _ec; } set { _ec = value; } }

        /// <summary>
        /// area of triangles: a[nt] (see "initialization")
        /// </summary>
        public double[] A { get { return _a; } set { _a = value; } }

        /// <summary>
        /// triangles adjacent to one node: p2[np][p2[np][0]+1] (see "initialization")
        /// </summary>
        public int[][] P2 { get { return _p2; } set { _p2 = value; } }

        /// <summary>
        /// spatial position mapping between coarse and fine mesh: smap[nt_c][smap[nt_c][0]+1] (see "spatialmapping")
        /// </summary>
        public int[][] Smap { get { return _smap; } set { _smap = value; } }

        /// <summary>
        /// spatial coarse-to-fine nodal-value mapping: cf[nt_c][3][smap[nt_c][0]][3] (see "spatialmapping2")
        /// </summary>
        public double[][][][] Cf { get { return _cf; } set { _cf = value; } }

        /// <summary>
        /// spatial fine-to-coarse nodal-value mapping: fc[nt_c][3][smap[nt_c][0]][3] (see "spatialmapping2")
        /// </summary>
        public double[][][][] Fc { get { return _fc; } set { _fc = value; } }

        /// <summary>
        /// boundary information: ori[ne] (see "boundary")
        /// </summary>
        public int[] Ori { get { return _ori; } set { _ori = value; } }

        /// <summary>
        /// boundary information: e2[ne][2] (see "boundary")
        /// </summary>
        public int[][] E2 { get { return _e2; } set { _e2 = value; } }

        /// <summary>
        /// boundary information: so2[nt][3] (see "boundary")
        /// </summary>
        public int[][] So2 { get { return _so2; } set { _so2 = value; } }

        /// <summary>
        /// boundary information: n[ne][2] (see "boundary")
        /// </summary>
        public double[][] N { get { return _n; } set { _n = value; } }

        /// <summary>
        /// edge upwind or outgoing flux: bd[ns][nt][3*3] (see "edgeterm")
        /// </summary>
        public int[][][] Bd { get { return _bd; } set { _bd = value; } }

        /// <summary>
        /// edge upwind or outgoing flux: bd[ns][nt][3]   (see "edgeterm")    
        /// </summary>
        public double[][][] Bd2 { get { return _bd2; } set { _bd2 = value; } }
    }
}
