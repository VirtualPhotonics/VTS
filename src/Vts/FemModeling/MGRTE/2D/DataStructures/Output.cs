using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class Output
    {
        public double Pi = GlobalConstants.Pi;

        

        public void RteOutput(double[][][] flux, double[][][] q, AngularMesh amesh, SpatialMesh smesh, BoundaryCoupling b, int vacuum)

        // Purpose: this function is to write measurements to three ".txt" files given the input file "det.txt".
        //          Input:
        //              "det.txt": "B", "A", "n", and "coord".
        //          Output:
        //              "flux.txt": "flux"
        //              "density.txt": "density"
        //              "output.txt": "output"
        {
            Measurement Det = new Measurement();
                      

            double temp, temp2;
            int i, j, k, m, ang, tri = -1, edge, count;
            int nt = smesh.nt, ne = smesh.ne, ns = amesh.ns, np = smesh.np;
            int nxy;
            int[][] t;
            int[][] e;
            int[][] e2;
            int[] w_a = new int[2];
            int[][][] so;
            int[][][] ro;
            double[][] p;
            double[][] n;
            double[] a;
            double[][] theta;
            double[][][] so2;
            double[][][] ro2;
            double tempd;
            double[] distance;
            double[] w_s = new double[3];
            double[] w_a2 = new double[2];
            double[] area = new double[3];
            double[] l = new double[2];
            double x, y, x1, x2, x3, y1, y2, y3, sn;
            double dtheta = 2 * Pi / ns;

            StreamWriter writer;            

            e = smesh.e; e2 = smesh.e2; t = smesh.t; p = smesh.p; n = smesh.n;
            a = smesh.a; theta = amesh.a;
            so = b.so; so2 = b.so2; ro = b.ro; ro2 = b.ro2;

            nxy = (int)Math.Ceiling(Math.Sqrt(nt / 2.0)) + 1;

            Det.density = new double[nt];
            Det.flux = new double[ns][];
            Det.fluence = new double[np];

            Det.radiance = new double[np][];
            for (i = 0; i < np; i++)
                Det.radiance[i] = new double[ns];

            Det.uxy = new double[nxy][];
            for (i = 0; i < nxy; i++)
                Det.uxy[i] = new double[nxy];


            for (i = 0; i < ns; i++)
            { Det.flux[i] = new double[nt]; }

            //load "Det.txt"

            if (File.Exists("det.txt"))
            {
                using (TextReader reader = File.OpenText("det.txt"))
                {
                    string text = reader.ReadToEnd();
                    string[] bits = text.Split('\t');
                    count = 0;

                    tempd = double.Parse(bits[count]); Det.B = (int)tempd; count++;
                    tempd = double.Parse(bits[count]); Det.A = (int)tempd; count++;
                    tempd = double.Parse(bits[count]); Det.n = (int)tempd; count++;

                    Det.output = new double[Det.n];
                    Det.coord = new double[Det.n][];

                    if (Det.A == 1)
                    {
                        for (i = 0; i < Det.n; i++)
                        { Det.coord[i] = new double[2]; }
                        for (i = 0; i < Det.n; i++)
                        {
                            for (j = 0; j < 2; j++)
                            { Det.coord[i][j] = double.Parse(bits[count]); count++; }
                        }
                    }
                    else
                    {
                        for (i = 0; i < Det.n; i++)
                        { Det.coord[i] = new double[3]; }
                        for (i = 0; i < Det.n; i++)
                        {
                            for (j = 0; j < 3; j++)
                            { Det.coord[i][j] = double.Parse(bits[count]); count++; }
                        }
                    }
                    reader.Close();
                }
            }
            else
            {//Exception - file open  
            }


            // compute and save density and flux
            for (j = 0; j < nt; j++)
            {
                Det.density[j] = 0;
                for (i = 0; i < ns; i++)
                {
                    Det.flux[i][j] = (flux[i][j][0] + flux[i][j][1] + flux[i][j][2]) / 3;
                    Det.density[j] += Det.flux[i][j];
                }
                Det.density[j] *= dtheta;
            }

            // compute and save density and flux
            for (j = 0; j < nt; j++)
            {
                Det.density[j] = 0;
                for (i = 0; i < ns; i++)
                {
                    Det.flux[i][j] = (flux[i][j][0] + flux[i][j][1] + flux[i][j][2]) / 3;
                    Det.density[j] += Det.flux[i][j];
                }
                Det.density[j] *= dtheta;
            }


            // compute radiance at each node
            for (i = 0; i < nt; i++)
            {                
                for (j = 0; j < ns; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        Det.radiance[t[i][k]][j] = flux[j][i][k];
                    }
                }
            }

            // compute fluence at each node
            for (i = 0; i < np; i++)
            {
                Det.fluence[i] = 0;
                for (j = 0; j < ns; j++)
                {
                    Det.fluence[i] += Det.radiance[i][j];                    
                }
                Det.fluence[i] *= dtheta;
            }

            writer = new StreamWriter("fluence.txt");
            for (i = 0; i < np; i++)
            {
                writer.Write("{0}\t", Det.fluence[i]);
            }
            writer.Close();


            writer = new StreamWriter("flux.txt");
            for (i = 0; i < ns; i++)
            {
                for (j = 0; j < nt; j++)
                { writer.Write("{0}\t", Det.flux[i][j]); }
            }
            writer.Close();

            writer = new StreamWriter("density.txt");
            for (j = 0; j < nt; j++)
            {
                writer.Write("{0}\t", Det.density[j]);
            }
            writer.Close();

            // given "det.coord", compute and save "output"
            // Case 1.1: boundary outgoing angular-weighted measurement with weight "s dot n"
            // Case 1.2: boundary outgoing angular-resolved measurement
            // Case 2.1: outgoing angular-weighted measurement with weight "s dot n"
            // Case 2.2: outgoing angular-resolved measurement
            // Case 2.3: angular-averaged measurement
            // Case 2.4: angular-resolved measurement

            if (Det.B == 0) // internal measurements
            {
                distance = new double[nt];
                for (i = 0; i < Det.n; i++)
                {   // spatial interpolation
                    for (j = 0; j < nt; j++)
                    {
                        distance[j] = 0;
                        tempd = 0;
                        for (k = 0; k < 2; k++)
                        {
                            tempd = Det.coord[i][k] - smesh.c[j][k];
                            distance[j] += tempd * tempd;
                        }
                    }
                    x = Det.coord[i][0]; y = Det.coord[i][1];
                    for (j = 0; j < nt; j++)
                    {
                        tri = MathFunctions.FindMin(nt, distance);
                        distance[tri] = 1e10;
                        x1 = p[t[tri][0] - 1][0]; y1 = p[t[tri][0] - 1][1];
                        x2 = p[t[tri][1] - 1][0]; y2 = p[t[tri][1] - 1][1];
                        x3 = p[t[tri][2] - 1][0]; y3 = p[t[tri][2] - 1][1];
                        area[0] = MathFunctions.Area(x, y, x2, y2, x3, y3);
                        area[1] = MathFunctions.Area(x1, y1, x, y, x3, y3);
                        area[2] = MathFunctions.Area(x1, y1, x2, y2, x, y);
                        tempd = area[0] + area[1] + area[2];
                        if (Math.Abs(tempd - a[tri]) / a[tri] < 1e-2)
                        {
                            for (k = 0; k < 3; k++)
                            { w_s[k] = area[k] / tempd; }
                            goto stop;
                        }
                    }
                stop: ;

                    Det.output[i] = 0;
                    if (Det.A == 1)//angular-averaged measurement (density)
                    {
                        for (k = 0; k < 3; k++)
                        {
                            temp = 0;
                            for (j = 0; j < ns; j++)
                            {
                                temp += flux[j][tri][k];
                            }
                            Det.output[i] += temp * w_s[k];
                        }
                        Det.output[i] *= dtheta;
                    }
                    else// angular-resolved measurement (flux)
                    {
                        MathFunctions.Intepolation_a(Det.coord[i][2], dtheta, ns, w_a, w_a2, 1.0);//angular interpolation
                        for (k = 0; k < 3; k++)
                        {
                            temp = 0;
                            for (j = 0; j < 2; j++)
                            {
                                temp += flux[w_a[j]][tri][k] * w_a2[j];
                            }
                            Det.output[i] += temp * w_s[k];
                        }
                    }
                }

            }
            else // boundary measurement
            {
                distance = new double[ne];
                for (i = 0; i < Det.n; i++)
                {   // nodal interpolation at the boundary
                    for (j = 0; j < ne; j++)
                    {
                        distance[j] = 0;
                        tempd = 0;
                        for (k = 0; k < 2; k++)
                        {
                            tempd = Det.coord[i][k] - smesh.ec[j][k];
                            distance[j] += tempd * tempd;
                        }
                    }
                    x = Det.coord[i][0]; y = Det.coord[i][1];
                    edge = MathFunctions.FindMin(ne, distance);
                    x1 = p[e[edge][1]][0]; y1 = p[e[edge][1]][1];
                    x2 = p[e[edge][2]][0]; y2 = p[e[edge][2]][1];
                    l[0] = MathFunctions.Length(x, y, x2, y2);
                    l[1] = MathFunctions.Length(x1, y1, x, y);
                    tempd = l[0] + l[1];
                    for (k = 0; k < 2; k++)
                    { w_s[k] = l[k] / tempd; }

                    tri = e[edge][0];

                    Det.output[i] = 0;
                    if (vacuum == 1)// vacuum B.C.
                    {
                        if (Det.A == 1)
                        {
                            for (j = 0; j < ns; j++)// angular integration with weights "s dot n" on the angular set with " sn>0"
                            {
                                sn = theta[j][ 0] * n[edge][0] + theta[j][1] * n[edge][1];
                                if (sn > 0)
                                {
                                    for (k = 0; k < 2; k++)// boundary interpolation
                                    {
                                        Det.output[i] += sn * w_s[k] * flux[j][tri][e2[edge][k]];
                                    }
                                }
                            }
                            Det.output[i] *= dtheta;
                        }
                        else
                        {
                            MathFunctions.Intepolation_a(Det.coord[i][2], dtheta, ns, w_a, w_a2, 1.0);
                            for (k = 0; k < 2; k++)// boundary interpolation
                            {
                                temp = 0;
                                for (j = 0; j < 2; j++)// angular interpolation
                                {
                                    temp += flux[w_a[j]][tri][e2[edge][k]] * w_a2[j];
                                }
                                Det.output[i] += temp * w_s[k];
                            }
                        }
                    }
                    else
                    {
                        if (Det.A == 1)
                        {
                            for (j = 0; j < ns; j++)
                            {
                                sn = theta[j][ 0] * n[edge][0] + theta[j][ 1] * n[edge][1];
                                if (sn > 0)// s dot n
                                {
                                    for (k = 0; k < 2; k++)// boundary interpolation
                                    {
                                        temp = 0;
                                        for (m = 0; m < 2; m++) // boundary coupling
                                        {   // contribution 1: scattered flux
                                            temp += flux[so[edge][j][m]][tri][e2[edge][k]] * so2[edge][j][m];
                                            // contribution 2: reflected boundary source
                                            temp += q[ro[edge][j][m]][edge][k] * ro2[edge][j][m];
                                        }
                                        Det.output[i] += sn * temp * w_s[k];
                                    }
                                }
                            }
                            Det.output[i] *= dtheta;
                        }
                        else
                        {
                            MathFunctions.Intepolation_a(Det.coord[i][2], dtheta, ns, w_a, w_a2, 1.0);// angular weights
                            for (k = 0; k < 2; k++)// boundary interpolation
                            {
                                temp2 = 0;
                                for (j = 0; j < 2; j++)// angular interpolation
                                {
                                    temp = 0;
                                    ang = w_a[j];
                                    for (m = 0; m < 2; m++)//boundary coupling
                                    {   // scattered flux
                                        temp += flux[so[edge][ang][m]][tri][e2[edge][k]] * so2[edge][ang][m];
                                        // reflected boundary source
                                        temp += q[ro[edge][ang][m]][edge][k] * ro2[edge][ang][m];
                                    }
                                    temp2 += temp * w_a2[j];
                                }
                                Det.output[i] += temp2 * w_s[k];
                            }
                        }
                    }
                }

            }

            writer = new StreamWriter("output.txt");
            for (j = 0; j < Det.n; j++)
            { writer.Write("{0}\t", Det.output[j]); }
            writer.Close();

        }

        

    }
}

