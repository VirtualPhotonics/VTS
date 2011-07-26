using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D.IO
{
    class Source
    {

        public const int NO_SOURCE = 0;                            // no light source
        public const int POINT_SOURCE = 1;                         // case 1
        public const int POINT_SOURCE_ISO = 2;                     // case 2
        public const int NODAL_VALUED_SOURCE_ISO = 3;              // case 3

        public void Inputsource(int alevel, AngularMesh[] amesh, int slevel, SpatialMesh[] smesh, int level, double[][][][] RHS, double[][][][] q)


        // Purpose: this function is to load the light sources and assembly internal sources into "RHS" and boundary source into "q".
        //
        //          In this function, we load the following two ".txt" files:
        //              1.1. "source.txt":
        //                      Case 1: "SOURCE_TYPE", "n", "a", "s_x" and "s_int"
        //                      Case 2: "SOURCE_TYPE", "n", "s_x" and "s_int"
        //                      Case 3: "SOURCE_TYPE", "n" and "i"
        //              1.2. "bsource.txt":
        //                      Case 1: "BSOURCE_TYPE", "a2" and "i2"
        //                      Case 2: "BSOURCE_TYPE", "i2"
        //              Here,
        //                      s_x[n]: x coordinate of the source
        //                      s_int[n]: source intensity
        //
        //          and then
        //              2.1. assembly internal source into "RHS"
        //              2.2. assembly boundary source into "q"
        {
            string internalSourceFile = "source.txt";
            string boundarySourceFile = "bsource.txt";
            
            double[][] s_xy;
            double[] s_int;
            double[] s_int2 = new double[3];
            int nt, ne, ns, i, j, k, tri, edge, count;
            double source_corr, temp, x, y, x1, x2, x3, y1, y2, y3;
            double[] distance;
            double[] area = new double[3];

            LightSource Source = new LightSource();
            MultiGridCycle Mgrid = new MultiGridCycle();

            nt = smesh[slevel].nt;
            ne = smesh[slevel].ne;
            ns = amesh[alevel].ns;

           
                   
             //Todo: Hard coded source file for July 26meeting. Correct it
                   
                    Source.S_TYPE = 2;                

                    if (Source.S_TYPE != NO_SOURCE)
                    {
                        Source.n = 1;
                        Source.i = new double[Source.n, 3];
                        Source.a = new int[Source.n];
                        Source.t = new int[Source.n];

                        if (Source.S_TYPE == NODAL_VALUED_SOURCE_ISO)
                        {
                            for (i = 0; i < Source.n; i++)
                            {
                                Source.i[i, 0] = 0;
                                Source.i[i, 1] = 0;
                                Source.i[i, 2] = 1;
                            }
                                
                        }
                        else
                        {
                            if (Source.S_TYPE == POINT_SOURCE)
                            {
                                for (i = 0; i < Source.n; i++)                                                                  
                                    Source.a[i] = 0;   
                            }

                            s_xy = new double[Source.n][];
                            for (i = 0; i < Source.n; i++)
                            {
                                s_xy[i] = new double[2];
                                s_xy[i][0] = 0;
                                s_xy[i][1] = 0;
                            }

                            s_int = new double[Source.n];
                            for (i = 0; i < Source.n; i++)
                            {
                                s_int[i] = 1;                                
                            }

                            //computation of "t" and "i"
                            distance = new double[nt];

                            for (i = 0; i < Source.n; i++)
                            {
                                for (j = 0; j < nt; j++)
                                {
                                    distance[j] = 0;
                                    temp = 0;
                                    for (k = 0; k < 2; k++)
                                    {
                                        temp = s_xy[i][k] - smesh[slevel].c[j][k];
                                        distance[j] += temp * temp;
                                    }
                                }
                                x = s_xy[i][0]; y = s_xy[i][1];
                                for (j = 0; j < nt; j++)
                                {
                                    tri = MathFunctions.FindMin(nt, distance);
                                    distance[tri] = 1e10;
                                    x1 = smesh[slevel].p[smesh[slevel].t[tri][0]][0];
                                    x2 = smesh[slevel].p[smesh[slevel].t[tri][1]][0];
                                    x3 = smesh[slevel].p[smesh[slevel].t[tri][2]][0];
                                    y1 = smesh[slevel].p[smesh[slevel].t[tri][0]][1];
                                    y2 = smesh[slevel].p[smesh[slevel].t[tri][1]][1];
                                    y3 = smesh[slevel].p[smesh[slevel].t[tri][2]][1];
                                    area[0] = MathFunctions.Area(x, y, x2, y2, x3, y3);
                                    area[1] = MathFunctions.Area(x1, y1, x, y, x3, y3);
                                    area[2] = MathFunctions.Area(x1, y1, x2, y2, x, y);
                                    temp = area[0] + area[1] + area[2];
                                    if (Math.Abs(temp - smesh[slevel].a[tri]) / smesh[slevel].a[tri] < 1e-2)
                                    {
                                        Source.t[i] = tri;
                                        for (k = 0; k < 3; k++)
                                        { 
                                            Source.i[i, k] = area[k] / temp * s_int[i]; 
                                        }
                                        goto stop;
                                    }
                                }
                            stop: ;
                            }
                        }

                        // 2.1. assembly of internal Source into "RHS"
                        if (Source.S_TYPE == NODAL_VALUED_SOURCE_ISO)
                        // Case 3: direct value assignment
                        {
                            for (j = 0; j < nt; j++)
                            {
                                for (k = 0; k < 3; k++)
                                {
                                    for (i = 0; i < ns; i++)
                                    { 
                                        RHS[level][i][j][k] = Source.i[j, k]; 
                                    }
                                }
                            }
                        }
                        else
                        {   // Case 1 and 2: computation of effective nodal values for point (delta) sources
                            //               due to DG integral formulation for spatial variables as in "relaxation"
                            //         Note: since the nodal-valued sources are assumed in DG formulation, this computation is not needed in Case 3.
                            if (Source.S_TYPE == POINT_SOURCE_ISO)
                            {
                                for (i = 0; i < Source.n; i++)
                                {
                                    tri = Source.t[i];
                                    source_corr = smesh[slevel].a[tri] / 12;
                                    temp = 0;
                                    for (k = 0; k < 3; k++)
                                    {
                                        s_int2[k] = Source.i[i, k] / source_corr;
                                        temp += s_int2[k];
                                    }
                                    temp = temp / 4;
                                    for (k = 0; k < 3; k++)
                                    { 
                                        s_int2[k] = s_int2[k] - temp; 
                                    }

                                    for (j = 0; j < ns; j++)// isotropic assignment in Case 2
                                    {
                                        for (k = 0; k < 3; k++)
                                        {
                                            RHS[level][j][tri][k] = s_int2[k];
                                        }
                                    }
                                }

                            }
                            else
                            {
                                nt = smesh[slevel].nt; ns = amesh[alevel].ns;
                                for (i = 0; i < Source.n; i++)
                                {
                                    tri = Source.t[i];
                                    source_corr = smesh[slevel].a[tri] / 12;
                                    temp = 0;
                                    for (k = 0; k < 3; k++)
                                    {
                                        s_int2[k] = Source.i[i, k] / source_corr;
                                        temp += s_int2[k];
                                    }
                                    temp = temp / 4;
                                    for (k = 0; k < 3; k++)
                                    { 
                                        s_int2[k] = s_int2[k] - temp; 
                                    }

                                    for (k = 0; k < 3; k++)// directional assignment in Case 1
                                    { 
                                        RHS[level][Source.a[i]][tri][k] = s_int2[k]; 
                                    }
                                }
                            }
                        }
                    }
                    
                   

            // 1.2. load "bsource.txt"
            if (File.Exists(boundarySourceFile))
            {
                using (TextReader reader = File.OpenText(boundarySourceFile))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    count = 0;

                    temp = double.Parse(bits[count]);
                    Source.BS_TYPE = (int)temp;
                    count++;

                    temp = double.Parse(bits[count]);
                    Source.n2 = (int)temp;
                    count++;

                    // computation of "e" and "i2"

                    Source.i2 = new double[Source.n2, 2];
                    Source.e = new int[Source.n2];
                    Source.a2 = new int[Source.n2];

                    Source.i = new double[Source.n2, 2];

                    if (Source.BS_TYPE == NODAL_VALUED_SOURCE_ISO)
                    {

                        for (i = 0; i < Source.n2; i++)
                        {
                            for (j = 0; j < 2; j++)
                            {
                                Source.i2[i, j] = double.Parse(bits[count]);
                                count++;
                            }
                        }
                        reader.Close();
                    }
                    else
                    {

                        if (Source.BS_TYPE == POINT_SOURCE)
                        {
                            for (i = 0; i < Source.n2; i++)
                            {
                                temp = double.Parse(bits[count]);
                                Source.a2[i] = (int)temp - 1;
                                count++;
                            }
                        }


                        s_xy = new double[Source.n2][];
                        for (i = 0; i < Source.n2; i++)
                        {
                            s_xy[i] = new double[2];
                            for (j = 0; j < 2; j++)
                            {
                                s_xy[i][j] = double.Parse(bits[count]);
                                count++;
                            }
                        }
                        s_int = new double[Source.n2];
                        for (i = 0; i < Source.n2; i++)
                        {
                            s_int[i] = double.Parse(bits[count]);
                            count++;
                        }
                        reader.Close();

                        distance = new double[ne];
                        for (i = 0; i < Source.n2; i++)
                        {
                            for (j = 0; j < ne; j++)
                            {
                                distance[j] = 0;
                                temp = 0;
                                for (k = 0; k < 2; k++)
                                {
                                    temp = s_xy[i][k] - smesh[slevel].ec[j][k];
                                    distance[j] += temp * temp;
                                }
                            }
                            x = s_xy[i][0]; y = s_xy[i][1];
                            edge = MathFunctions.FindMin(ne, distance);
                            x1 = smesh[slevel].p[smesh[slevel].e[edge][1]][0]; y1 = smesh[slevel].p[smesh[slevel].e[edge][1]][1];
                            x2 = smesh[slevel].p[smesh[slevel].e[edge][2]][0]; y2 = smesh[slevel].p[smesh[slevel].e[edge][2]][1];
                            area[0] = MathFunctions.Length(x, y, x2, y2);
                            area[1] = MathFunctions.Length(x1, y1, x, y);
                            temp = area[0] + area[1];
                            Source.e[i] = edge;
                            for (k = 0; k < 2; k++)
                            { 
                                Source.i2[i, k] = area[k] / temp * s_int[i]; 
                            }
                        }
                    }

                    // 2.2. Assembly of boundary Source into "q"
                    // Note: we need to compute for effective nodal values in the presence of delta boundary sources.
                    if (Source.BS_TYPE != NO_SOURCE)
                    {
                        if (Source.BS_TYPE == NODAL_VALUED_SOURCE_ISO)
                        {
                            for (j = 0; j < Source.n2; j++)
                            {
                                for (k = 0; k < 2; k++)
                                {
                                    for (i = 0; i < ns; i++)
                                    { 
                                        q[level][i][j][k] = Source.i2[j, k]; 
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Source.BS_TYPE == POINT_SOURCE_ISO)
                            {
                                for (i = 0; i < Source.n2; i++)
                                {
                                    edge = Source.e[i];
                                    x1 = smesh[slevel].p[smesh[slevel].e[edge][1]][0]; y1 = smesh[slevel].p[smesh[slevel].e[edge][1]][1];
                                    x2 = smesh[slevel].p[smesh[slevel].e[edge][2]][0]; y2 = smesh[slevel].p[smesh[slevel].e[edge][2]][1];
                                    source_corr = MathFunctions.Length(x1, y1, x2, y2) / 6;
                                    temp = 0;
                                    for (k = 0; k < 2; k++)
                                    {
                                        s_int2[k] = Source.i2[i, k] / source_corr;
                                        temp += s_int2[k];
                                    }
                                    temp = temp / 3;
                                    for (k = 0; k < 2; k++)
                                    { 
                                        s_int2[k] = s_int2[k] - temp; 
                                    }

                                    for (j = 0; j < ns; j++)// isotropic assignment in Case 2
                                    {
                                        for (k = 0; k < 2; k++)
                                        { 
                                            q[level][j][edge][k] = s_int2[k]; 
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (i = 0; i < Source.n2; i++)
                                {
                                    edge = Source.e[i];
                                    x1 = smesh[slevel].p[smesh[slevel].e[edge][1]][0]; y1 = smesh[slevel].p[smesh[slevel].e[edge][1]][1];
                                    x2 = smesh[slevel].p[smesh[slevel].e[edge][2]][0]; y2 = smesh[slevel].p[smesh[slevel].e[edge][2]][1];
                                    source_corr = MathFunctions.Length(x1, y1, x2, y2) / 6;
                                    temp = 0;
                                    for (k = 0; k < 2; k++)
                                    {
                                        s_int2[k] = Source.i2[i, k] / source_corr;
                                        temp += s_int2[k];
                                    }
                                    temp = temp / 3;
                                    for (k = 0; k < 2; k++)
                                    { 
                                        s_int2[k] = s_int2[k] - temp; 
                                    }

                                    for (k = 0; k < 2; k++)
                                    { 
                                        q[level][Source.a2[i]][edge][k] = s_int2[k]; 
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine(boundarySourceFile + " does not exist!"); 
            }
        }


    }    
}
