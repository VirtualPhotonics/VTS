using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vts.Modeling.ForwardSolvers.MGRTE._2D.DataStructures;

namespace Vts.Modeling.ForwardSolvers.MGRTE._2D.IO
{
    //Purpose: in this function, we load the following four ".txt" files:
    //             1) "amesh.txt": "ns", "a" and "w" for angular mesh
    //             2) "smesh.txt": spatial mesh
    //             3) "ua.txt": absorption coefficient
    //             4) "us.txt": scattering coefficient
    public class ReadDataFiles
    {
        public static void ReadAmesh(
            int alevel, 
            int slevel,
            ref AngularMesh[] amesh)
          
        {
            int i, j, k, count;
            double tempd;
       
            string angularMeshFile = "amesh.txt";

            // 1.1. load amesh.txt
            if (File.Exists(angularMeshFile))
            {
                using (TextReader reader = File.OpenText(angularMeshFile))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    count = 0;
                    for (i = 0; i <= alevel; i++)
                    {
                        tempd = double.Parse(bits[count]);
                        amesh[i].ns = (int)tempd;
                        count++;
                        amesh[i].a = new double[amesh[i].ns][];
                        for (j = 0; j < amesh[i].ns; j++)
                        {
                            amesh[i].a[j] = new double[3];
                            for (k = 0; k < 3; k++)
                            {
                                amesh[i].a[j][k] = double.Parse(bits[count]);
                                count++;
                            }
                        }

                        amesh[i].w = new double[amesh[i].ns, amesh[i].ns];
                        for (j = 0; j < amesh[i].ns; j++)
                        {
                            for (k = 0; k < amesh[i].ns; k++)
                            {
                                amesh[i].w[j, k] = double.Parse(bits[count]);
                                count++;
                            }
                        }
                    }
                    reader.Close();
                }
            }
            else
            {
                Console.WriteLine(angularMeshFile + " does not exist!");
            }
        }
        

        public static void ReadSmesh(
            int alevel,
            int slevel,
            AngularMesh[] amesh,
            ref SpatialMesh[] smesh)
        {
            int i, j, k, count;
            double tempd;           

            string spatialMeshFile = "smesh.txt";

            // 1.2. load smesh.txt
            //      Notice the index difference in c programming: array indexes from 0 instead of 1,
            //      we subtract "1" from every integer-valued index here as for "so", "t" and "e" as follow.

            if (File.Exists(spatialMeshFile))
            {
                using (TextReader reader = File.OpenText(spatialMeshFile))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    count = 0;

                    for (i = 0; i <= slevel; i++)
                    {
                        tempd = double.Parse(bits[count]); smesh[i].nt = (int)tempd; count++;
                        tempd = double.Parse(bits[count]); smesh[i].np = (int)tempd; count++;
                        tempd = double.Parse(bits[count]); smesh[i].ne = (int)tempd; count++;

                        smesh[i].so = new int[amesh[alevel].ns][];
                        for (j = 0; j < amesh[alevel].ns; j++)
                        {
                            smesh[i].so[j] = new int[smesh[i].nt];
                            for (k = 0; k < smesh[i].nt; k++)
                            {
                                tempd = double.Parse(bits[count]);
                                smesh[i].so[j][k] = (int)tempd - 1;
                                count++;
                            }
                        }

                        smesh[i].p = new double[smesh[i].np][];
                        for (j = 0; j < smesh[i].np; j++)
                        {
                            smesh[i].p[j] = new double[2];
                            for (k = 0; k < 2; k++)
                            {
                                tempd = double.Parse(bits[count]);
                                smesh[i].p[j][k] = tempd;
                                count++;
                            }
                        }

                        smesh[i].t = new int[smesh[i].nt][];
                        for (j = 0; j < smesh[i].nt; j++)
                        {
                            smesh[i].t[j] = new int[3];
                            for (k = 0; k < 3; k++)
                            {
                                tempd = double.Parse(bits[count]);
                                smesh[i].t[j][k] = (int)tempd - 1;
                                count++;
                            }
                        }

                        smesh[i].e = new int[smesh[i].ne][];
                        for (j = 0; j < smesh[i].ne; j++)
                        {
                            smesh[i].e[j] = new int[4];
                            smesh[i].e[j][0] = -1;
                            smesh[i].e[j][3] = -1;
                            for (k = 1; k < 3; k++)
                            {
                                tempd = double.Parse(bits[count]);
                                smesh[i].e[j][k] = (int)tempd - 1;
                                count++;
                            }
                        }
                    }
                    reader.Close();
                }
            }
            else
            {
                Console.WriteLine(spatialMeshFile + " does not exist!");
            }
        }


        public static void ReadMua(
            int slevel,
            SpatialMesh[] smesh,
            ref double[][][] mua)
        {
            int j, k, count;
            string absorptionFile = "mua.txt";

            //Collect mua data
            mua[slevel] = new double[smesh[slevel].nt][];

            if (File.Exists(absorptionFile))
            {
                using (TextReader reader = File.OpenText(absorptionFile))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    count = 0;

                    for (j = 0; j < smesh[slevel].nt; j++)
                    {
                        mua[slevel][j] = new double[3];
                        for (k = 0; k < 3; k++)
                        {
                            mua[slevel][j][k] = double.Parse(bits[count]);
                            count++;
                        }
                    }
                    reader.Close();
                }
            }
            else
            {
                Console.WriteLine(absorptionFile + " does not exist!");
            }
        }


        public static void ReadMus(            
            int slevel, 
            SpatialMesh[] smesh,
            ref double[][][] mus)
        {
            int j, k, count;                     
            string scatteringFile = "mus.txt";

            //Collect mus data
            mus[slevel] = new double[smesh[slevel].nt][];

            if (File.Exists(scatteringFile))
            {
                using (TextReader reader = File.OpenText(scatteringFile))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    count = 0;

                    for (j = 0; j < smesh[slevel].nt; j++)
                    {
                        mus[slevel][j] = new double[3];
                        for (k = 0; k < 3; k++)
                        {
                            mus[slevel][j][k] = double.Parse(bits[count]);
                            count++;
                        }
                    }
                    reader.Close();
                }
            }
            else
            {
                Console.WriteLine(scatteringFile + " does not exist!");
            }

        }
    }
}
