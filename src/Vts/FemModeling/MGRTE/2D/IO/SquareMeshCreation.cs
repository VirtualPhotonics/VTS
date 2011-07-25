using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D.IO
{
    /// <summary>
    /// Generation of square mesh for 2D MG RTE Solver
    /// </summary>
    public class SquareMesh
    {
        /// <summary>
        /// Create a squarte mesh for given spatial mesh level
        /// </summary>
        /// <param name="slevel">number of spatial mesh levels</param>
        public static void CreateSquareMesh(ref SpatialMesh[] smesh, int slevel)
        {   
	        int i;
	        int np,nt,ne;
	        
	        //SQUARE MESH
	        np = 5;	
	        ne = 4;
            nt = 4;            

            double[][] pts = new double [np][];	
            for (i= 0; i <np; i++)
                pts[i] = new double [4];

            int[][] edge = new int [ne][];	
            for (i= 0; i <ne; i++)
                edge[i] = new int [2];

            int[][] tri = new int [nt][];	
            for (i= 0; i <nt; i++)
               tri[i] = new int [3];            
	
	        //create the basic square mesh
	        BasicSquareMesh(pts, edge, tri);

            AssignSpatialMesh(ref smesh, pts, edge, tri, np, ne, nt, 0);

            string str = "PET" + 0;
            str = str + ".txt";
            WritePTEData(str, pts, edge, tri, np, ne, nt);

	        if (slevel>0)
                CreateMultigrid(ref smesh, pts, edge, tri, np, ne, nt, slevel);	
	
        }

        /// <summary>
        /// Define Basic Square Mesh
        /// </summary>
        /// <param name="pts">points data</param>
        /// <param name="edge">edge data</param>
        /// <param name="tri">triangle data</param>
        public static void BasicSquareMesh(
            double[][] pts, 
            int[][] edge, 
            int[][] tri)
        {
            pts[0][0] = -1; pts[0][1] = 1;  pts[0][2] = 0; pts[0][3] = 1;
            pts[1][0] = 1;  pts[1][1] = 1;  pts[1][2] = 1; pts[1][3] = 1;
            pts[2][0] = 1;  pts[2][1] = -1; pts[2][2] = 2; pts[2][3] = 1;
            pts[3][0] = -1; pts[3][1] = -1; pts[3][2] = 3; pts[3][3] = 1;
            pts[4][0] = 0;  pts[4][1] = 0;  pts[4][2] = 4; pts[4][3] = 0;

            edge[0][0] = (int)pts[0][2]; edge[0][1] = (int)pts[1][2]; 
            edge[1][0] = (int)pts[1][2]; edge[1][1] = (int)pts[2][2]; 
            edge[2][0] = (int)pts[2][2]; edge[2][1] = (int)pts[3][2]; 
            edge[3][0] = (int)pts[3][2]; edge[3][1] = (int)pts[0][2];             

            tri[0][0] = (int)pts[0][2]; tri[0][1] = (int)pts[1][2]; tri[0][2] = (int)pts[4][2];
            tri[1][0] = (int)pts[1][2]; tri[1][1] = (int)pts[2][2]; tri[1][2] = (int)pts[4][2];
            tri[2][0] = (int)pts[2][2]; tri[2][1] = (int)pts[3][2]; tri[2][2] = (int)pts[4][2];
            tri[3][0] = (int)pts[3][2]; tri[3][1] = (int)pts[0][2]; tri[3][2] = (int)pts[4][2];
        }


        /// <summary>
        /// Create Multigrid based on spatial mesh level
        /// </summary>
        /// <param name="p">points data</param>
        /// <param name="t">edge data</param>
        /// <param name="e">edge data</param>
        /// <param name="np">number of points</param>
        /// <param name="nt">number of triangles</param>
        /// <param name="ne">number of edges</param>
        ///  <param name="nebound">number of boundary edges</param>
        /// <param name="slevel">spatial mesh levels</param>
        public static void CreateMultigrid(ref SpatialMesh[] smesh, double[][] p, int[][] e, int[][] t, int np, int ne, int nt, int slevel)
        {	        
            int i, j, nt2, ne2, np2, npt, trint;

	        //Input arrays
	        double[][] oldp = p;
	        int[][] oldt = t;
	        int[][] olde = e;

            //char[] str[80];
            //char[] level[4];	       

	        for (j = 1; j<slevel; j++)
	        {
                //npt gets the total number of point after division
                npt = 0;

                trint = 3 * nt;

                double[][] ptemp = new double[trint][];
                for (i = 0; i < trint; i++)
                    ptemp[i] = new double[6];

                FindPTemp(ptemp, oldp, oldt, nt);
                ReindexingPTemp(ptemp, ref npt, trint, np);

                np2 = npt;
                ne2 = 2 * ne;
                nt2 = 4 * nt;

                double[][] newp = new double [np2][];	
                for (i= 0; i <np2; i++)
                    newp[i] = new double [4];

                int[][] newe = new int [ne2][];	
                for (i= 0; i <ne2; i++)
                    newe[i] = new int [2];

                int[][] newt = new int [nt2][];	
                for (i= 0; i <nt2; i++)
                   newt[i] = new int [3];

                NewPET(newp, oldp, newe, newt, oldt, ptemp, np, nt, trint); 

		        //Update np, nt and ne
		        np = np2;
                ne = ne2;
		        nt = nt2;                

		        //Assign output arrays to input arrays
		        oldp = newp;                
                olde = newe;
                oldt = newt;

                string str = "PET" + j;
                str = str + ".txt";
                WritePTEData(str, newp, newe, newt, np, ne, nt);

                AssignSpatialMesh(ref smesh, oldp, olde, oldt, np, ne, nt, j);
	        }
        }

        public static void FindPTemp(double[][] ptemp, double[][] p, int[][] t, int nt)

        {
	        int i;
            int p0, p1, p2;

	        for(i=0; i<nt; i++)
	        {
		        //x cordinates
		        ptemp[3*i][0]   = 0.5 * (p[t[i][0]][0] +  p[t[i][1]][0]);
		        ptemp[3*i+1][0] = 0.5 * (p[t[i][0]][0] +  p[t[i][2]][0]);
		        ptemp[3*i+2][0] = 0.5 * (p[t[i][1]][0] +  p[t[i][2]][0]);

		        //y cordinates
		        ptemp[3*i][1]   = 0.5 * (p[t[i][0]][1] +  p[t[i][1]][1]);
		        ptemp[3*i+1][1] = 0.5 * (p[t[i][0]][1] +  p[t[i][2]][1]);
		        ptemp[3*i+2][1] = 0.5 * (p[t[i][1]][1] +  p[t[i][2]][1]);
				
		        //P index - initialize to negative values
		        ptemp[3*i][2]   = -1;
		        ptemp[3*i+1][2] = -1;
		        ptemp[3*i+2][2] = -1;

		        //find the edge vertex of a triangle
                p0 = (int)p[t[i][0]][3];
                p1 = (int)p[t[i][1]][3];
                p2 = (int)p[t[i][2]][3];

		        if(p0 + p1 == 2)
		        {
			        ptemp[3*i][3] = 1;
			        ptemp[3*i][4] = p[t[i][0]][2];
			        ptemp[3*i][5] = p[t[i][1]][2];                   
		        }
                else if (p0 + p2 == 2)
		        {
			        ptemp[3*i+1][3] = 1;
			        ptemp[3*i+1][4] = p[t[i][0]][2];
			        ptemp[3*i+1][5] = p[t[i][2]][2];
		        }
		        else if(p1 + p2 == 2)
		        {
			        ptemp[3*i+2][3] = 1;
			        ptemp[3*i+2][4] = p[t[i][1]][2];
			        ptemp[3*i+2][5] = p[t[i][2]][2];
		        }
	        }
	
        }

        public static void ReindexingPTemp(double[][] ptemp, ref int count, int trint, int np)
        {
            int i, j;
	
	        count = np;
	        for(i=0; i<trint-1; i++)
	        {	
		        for(j=i+1; j<trint; j++)
		        {			
			        if (ptemp[i][0] == ptemp[j][0])
				        if (ptemp[i][1] == ptemp[j][1])
				        {
					        ptemp[j][2] = count;
					        ptemp[i][2] = count;
					        count++;
				        }
		        }
	        }
	        for(i=0; i<trint; i++)
	        {
		        if (ptemp[i][2] < 0)
		        {
			        ptemp[i][2] = count;
			        count++;
		        }
	        }
        }

        /// <summary>
        /// Assign P E T
        /// </summary>
        /// <param name="newp"></param>
        /// <param name="newe"></param>
        /// <param name="newt"></param>
        /// <param name="oldt"></param>
        /// <param name="ptemp"></param>
        /// <param name="np"></param>
        /// <param name="nt"></param>
        /// <param name="trint"></param>
        public static void NewPET(double[][] newp, double[][] oldp, int[][] newe, int[][] newt, int[][] oldt,  double[][] ptemp, int np, int nt, int trint)

        {
	        int i, count;
	        
            //Assign P
	        for(i=0; i<np; i++)
	        {
		        count = (int)oldp[i][2];
		        newp[count][0]  = oldp[i][0];       
		        newp[count][1]  = oldp[i][1]; 
		        newp[count][2]  = oldp[i][2];
		        newp[count][3]  = oldp[i][3];		
	        }

	        for(i=0; i<trint; i++)
	        {
		        count = (int)ptemp[i][2];
		        newp[count][0]  = ptemp[i][0];       
		        newp[count][1]  = ptemp[i][1]; 
		        newp[count][2]  = ptemp[i][2];
		        newp[count][3]  = ptemp[i][3];	
	        }

            
            //Assign E
            count = 0;
            for (i = 0; i < trint; i++)
            {
                if (ptemp[i][3] == 1)
                {
                    newe[count][0] = (int)ptemp[i][4];
                    newe[count][1] = (int)ptemp[i][2];
                    count++;
                    newe[count][0] = (int)ptemp[i][2];
                    newe[count][1] = (int)ptemp[i][5];
                    count++;
                }
            }

            //Assign T
            for (i = 0; i < nt; i++)
            {
                newt[4 * i][0] = oldt[i][0];               newt[4 * i][1] = (int)ptemp[3 * i + 1][2];     newt[4 * i][2] = (int)ptemp[3 * i][2];
                newt[4 * i + 3][0] = oldt[i][1];           newt[4 * i + 3][1] = (int)ptemp[3 * i][2];     newt[4 * i + 3][2] = (int)ptemp[3 * i + 2][2];
                newt[4 * i + 2][0] = oldt[i][2];           newt[4 * i + 2][1] = (int)ptemp[3 * i + 2][2]; newt[4 * i + 2][2] = (int)ptemp[3 * i + 1][2];
                newt[4 * i + 1][0] = (int)ptemp[3 * i][2]; newt[4 * i + 1][1] = (int)ptemp[3 * i + 1][2]; newt[4 * i + 1][2] = (int)ptemp[3 * i + 2][2];
            }
        }

       

        public static void AssignSpatialMesh(ref SpatialMesh[] smesh, double[][] p, int[][] e, int[][] t, int np, int ne, int nt, int level)
        {
            int i, j, k;

            smesh[level].np = np;
            smesh[level].ne = ne;
            smesh[level].nt = nt;

            smesh[level].p = new double[np][];
            for (i = 0; i < np; i++)
            {
                smesh[level].p[i] = new double[2];
                for (j = 0; j < 2; j++)
                    smesh[level].p[i][j] = p[i][j];
            }

            smesh[level].t = new int[nt][];
            for (i = 0; i < nt; i++)
            {
                smesh[level].t[i] = new int[3];
                for (j = 0; j < 3; j++)
                    smesh[level].t[i][j] = t[i][j];
            }

            
            smesh[level].e = new int[ne][];
            for (i = 0; i < ne; i++)
            {               
                smesh[level].e[i] = new int[2];
                for (j = 0; j < 2; j++)
                    smesh[level].e[i][j] = e[i][j];
            }
            
        }
        
        /// <summary>
        /// Write output data (for debugging ONLY)
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="p"></param>
        /// <param name="t"></param>
        /// <param name="e"></param>
        /// <param name="np"></param>
        /// <param name="nt"></param>
        /// <param name="ne"></param>
        public static void WritePTEData(string filename, double[][] p, int[][] e, int[][] t, int np, int ne, int nt)
        {
            int i, j;
            StreamWriter writer;

            writer = new StreamWriter(filename);
            writer.Write("{0}\t", np);
            writer.Write("{0}\t", ne);
            writer.Write("{0}\t", nt);

            //write nodal points
            for (i = 0; i < np; i++)
                writer.Write("{0}\t{1}\t", p[i][0], p[i][1]);
            //Write Egde indices
            for (i = 0; i < ne; i++)
                writer.Write("{0}\t{1}\t", e[i][0], e[i][1]);
            //Write triangle indices
            for (i = 0; i < nt; i++)
                writer.Write("{0}\t{1}\t{2}\t", t[i][0], t[i][1], t[i][2]);

            writer.Close();
        }
    }
}
