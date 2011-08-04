using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vts.FemModeling.MGRTE._2D.DataStructures;


namespace Vts.FemModeling.MGRTE._2D
{
    class ReadFiles
    {
//        private static TextReader GetTextReader()
//        {
////#if SILVERLIGHT
////            File.OpenText(OpenText(angularMeshFile)
////#else
////            File.OpenText(angularMeshFile)
////#endif
//        }

        public static void ReadAmesh(ref AngularMesh[] amesh, int aMeshLevel)
        {
            string angularMeshFile = "amesh.txt";


            //Purpose: in this function, to load "amesh.txt": "ns", "a" and "w" for angular mesh

            int i, j, k;
            double tempd;
            int count;

            // 1.1. load amesh.txt
            if (File.Exists(angularMeshFile))
            {
                using (TextReader reader = File.OpenText(angularMeshFile))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    count = 0;
                    for (i = 0; i <= aMeshLevel; i++)
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

                        amesh[i].w = new double[amesh[i].ns][];
                        for (j = 0; j < amesh[i].ns; j++)
                        {
                            amesh[i].w[j] = new double[amesh[i].ns];
                            for (k = 0; k < amesh[i].ns; k++)
                            {
                                amesh[i].w[j][k] = double.Parse(bits[count]);
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




        public static void ReadSmesh(ref SpatialMesh[] smesh, int sMeshLevel, int aMeshLevel, AngularMesh[] amesh)
        {
            string spatialMeshFile = "smesh.txt";


            //Purpose: in this function, to load "amesh.txt": "ns", "a" and "w" for angular mesh

            int i, j, k;
            double tempd;
            int count;

            // 1.1. load amesh.txt
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

                    for (i = 0; i <= sMeshLevel; i++)
                    {
                        tempd = double.Parse(bits[count]); smesh[i].nt = (int)tempd; count++;
                        tempd = double.Parse(bits[count]); smesh[i].np = (int)tempd; count++;
                        tempd = double.Parse(bits[count]); smesh[i].ne = (int)tempd; count++;

                        smesh[i].so = new int[amesh[aMeshLevel].ns][];
                        for (j = 0; j < amesh[aMeshLevel].ns; j++)
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


        public static void ReadMua(ref double[][][] ua, int sMeshLevel, int nt)
        {
            string absorptionFile = "mua.txt";

            int j, k, count;            
        
            //Collect ua data
            ua[sMeshLevel] = new double[nt][];
            if (File.Exists(absorptionFile))
            {
                using (TextReader reader = File.OpenText(absorptionFile))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    count = 0;

                    for (j = 0; j < nt; j++)
                    {
                        ua[sMeshLevel][j] = new double[3];
                        for (k = 0; k < 3; k++)
                        {
                            ua[sMeshLevel][j][k] = double.Parse(bits[count]);
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


        public static void ReadMus(ref double[][][] us, int sMeshLevel, int nt)
        {
            string absorptionFile = "mus.txt";
            
            int j, k, count;

            //Collect us data
            us[sMeshLevel] = new double[nt][];
            if (File.Exists(absorptionFile))
            {
                using (TextReader reader = File.OpenText(absorptionFile))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    count = 0;

                    for (j = 0; j < nt; j++)
                    {
                        us[sMeshLevel][j] = new double[3];
                        for (k = 0; k < 3; k++)
                        {
                            us[sMeshLevel][j][k] = double.Parse(bits[count]);
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
    }
}
