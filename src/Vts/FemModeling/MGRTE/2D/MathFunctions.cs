using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D
{
    public static class MathFunctions
    {

        public static double Area(double x1, double y1, double x2, double y2, double x3, double y3)
        // Purpose: Calculate the area of a triangle
        {
            return 0.5 * Math.Abs((y2 - y1) * (x3 - x1) - (y3 - y1) * (x2 - x1));
        }

        public static int FindMin(int n, double[] d)
        // Purpose: this function is to find the minimum from the vector d with the size n.
        {
            int i;
            double dmin;
            int nmin;
            nmin = 0; dmin = d[nmin];
            for (i = 1; i < n; i++)
            {
                if (d[i] < dmin)
                { nmin = i; dmin = d[i]; }
            }
            return nmin;
        }


        public static void Intepolation_a(double theta_m, double dtheta, int ns, int[] b, double[] b2, double constant)
        // Purpose: this function is to find two linearly intepolated angles "b" and weights "b2" for the angle "theta_m"
        {
            int theta1, theta2;
            double w1, w2;

            theta1 = (int)Math.Floor(theta_m / dtheta) + 1;
            w2 = (theta_m - (theta1 - 1) * dtheta) / dtheta;
            w1 = 1.0 - w2;
            if (theta1 == ns)
            { theta2 = 1; }
            else
            { theta2 = theta1 + 1; }
            b[0] = theta1 - 1; b[1] = theta2 - 1;
            b2[0] = w1 * constant; b2[1] = w2 * constant;

        }

        public static double Length(double x1, double y1, double x2, double y2)
        //Purpose: this function calculate the distance between two points
        {
            return Math.Sqrt((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));
        }

        /// <summary>
        /// This function provides angular weights and coordinates on 2D unit circular phase space.
        /// </summary>
        /// <param name="w">Weight</param>
        /// <param name="theta">Angular array</param>
        /// <param name="nAngle">number of angles</param>
        /// <param name="g">anisotropy factor</param>
        public static void Weight_2D(double[][] w, double[][] theta, int nAngle, double g)
        {
            int i, j, n, nj;
            double dx, norm;
            double[] f;
            double[] x;

            n = (nAngle + 2) / 2;
            dx = 1 / (double)(n - 1) * 3.14159265;

            f = new double[n];
            x = new double[n];

            for (i = 0; i < n; i++)
                x[i] = dx * i;

            HGPhaseFunction_2D(f, x, n, g);

            w[0][0] = 2.0 / 6.0 * (2.0 * f[0] + f[1]) * dx;
            w[0][n - 1] = 2.0 / 6.0 * (f[n - 2] + 2.0 * f[n - 1]) * dx;

            for (i = 1; i < n - 1; i++)
            {
                w[0][i] = 1.0 / 6.0 * (f[i - 1] + 2.0 * f[i]) * dx + 1.0 / 6.0 * (2.0 * f[i] + f[i + 1]) * dx;
                w[0][2 * n - 2 - i] = w[0][i];
            }

            norm = 0;
            for (i = 0; i < nAngle; i++)
                norm += w[0][i];

            //Normalize
            for (i = 0; i < nAngle; i++)
                w[0][i] = w[0][i] / norm;



            for (i = 1; i < nAngle; i++)
                for (j = 0; j < nAngle; j++)
                {
                    nj = (j + i) % nAngle;
                    if (nj == 0)
                    {
                        nj = 0;
                    }
                    w[i][nj] = w[0][j];
                }

            for (i = 0; i < nAngle; i++)
            {
                theta[i][2] = 2 * 3.14159265 * i / nAngle;
                theta[i][0] = Math.Cos(theta[i][2]);
                theta[i][1] = Math.Sin(theta[i][2]);
            }
        }

        /// <summary>
        /// This function describes phase function or scattering kernel and is set to be Henyey-Greenstein phase function
        /// </summary>
        /// <param name="f">function output</param>
        /// <param name="theta">angular array</param>
        /// <param name="n">number of angles</param>
        /// <param name="g">anisotropy factor</param>

        public static void HGPhaseFunction_2D(double[] f, double[] dAng, int n, double g)
        {
            int i;

            for (i = 0; i < n; i++)
                f[i] = 1 / (2 * 3.14159265) * (1 - g * g) / (1 + g * g - 2 * g * Math.Cos(dAng[i]));
        }


        public static void SweepOrdering(ref SpatialMesh[] smesh, int nAngle, int sLevel)
        {
            int i, j, k;
            double[][] centOfTri;


            for (i = 0; i < sLevel; i++)
            {
                centOfTri = new double[smesh[i].nt][];
                for (j = 0; j < smesh[i].nt; j++)
                {
                    centOfTri[j] = new double[2];
                    for (k = 0; k < 2; k++)
                        centOfTri[j][k] = (
                            smesh[i].p[smesh[i].t[j][0]][k]
                            + smesh[i].p[smesh[i].t[j][1]][k]
                            + smesh[i].p[smesh[i].t[j][2]][k]) / 3;

                }
            }
        }

        public static void SquareTriMeshToGrid(ref SpatialMesh[] smesh, double[] inten, double[][] uxy, int nxy, int sLevel)
        {
            int i, j, k;
            int np, nt;
            double dx, dy;
            double[] x;
            double[] y;
            int[][] tn;
            double[][] a12;
            double[][] a13;
            double[] a2 = new double[2];
            double[] a3 = new double[2];
            double[] b2 = new double[2];
            double[] b3 = new double[2];
            double d2, d3;
            double[] r1p = new double[2];

            double xmin, xmax, ymin, ymax;
            double temp;
            double tiny = 2.2204e-12;

            double[] xminArray;
            double[] xmaxArray;
            double[] yminArray;
            double[] ymaxArray;

            int[] idxxminArray;
            int[] idxxmaxArray;
            int[] idxyminArray;
            int[] idxymaxArray;

            xmin = 1e10; xmax = -1e10; ymin = 1e10; ymax = -1e10;

            np = smesh[sLevel].np;
            nt = smesh[sLevel].nt;

            for (i = 0; i < np; i++)
            {
                xmin = Math.Min(smesh[sLevel].p[i][0], xmin);
                xmax = Math.Max(smesh[sLevel].p[i][0], xmax);
                ymin = Math.Min(smesh[sLevel].p[i][1], ymin);
                ymax = Math.Max(smesh[sLevel].p[i][1], ymax);
            }



            dx = (xmax - xmin) / (nxy - 1);
            dy = (ymax - ymin) / (nxy - 1);

            x = new double[nxy];
            y = new double[nxy];

            for (i = 0; i < nxy; i++)
            {
                x[i] = xmin + i * dx;
                y[i] = ymin + i * dy;
            }

            xminArray = new double[nt];
            xmaxArray = new double[nt];
            yminArray = new double[nt];
            ymaxArray = new double[nt];

            idxxminArray = new int[nt];
            idxxmaxArray = new int[nt];
            idxyminArray = new int[nt];
            idxymaxArray = new int[nt];

            for (i = 0; i < nt; i++)
            {
                xmin = 1e10; xmax = -1e10; ymin = 1e10; ymax = -1e10;

                xmin = Math.Min(smesh[sLevel].p[smesh[sLevel].t[i][0]][0], xmin);
                xmin = Math.Min(smesh[sLevel].p[smesh[sLevel].t[i][1]][0], xmin);
                xmin = Math.Min(smesh[sLevel].p[smesh[sLevel].t[i][2]][0], xmin);

                xmax = Math.Max(smesh[sLevel].p[smesh[sLevel].t[i][0]][0], xmax);
                xmax = Math.Max(smesh[sLevel].p[smesh[sLevel].t[i][1]][0], xmax);
                xmax = Math.Max(smesh[sLevel].p[smesh[sLevel].t[i][2]][0], xmax);

                ymin = Math.Min(smesh[sLevel].p[smesh[sLevel].t[i][0]][1], ymin);
                ymin = Math.Min(smesh[sLevel].p[smesh[sLevel].t[i][1]][1], ymin);
                ymin = Math.Min(smesh[sLevel].p[smesh[sLevel].t[i][2]][1], ymin);

                ymax = Math.Max(smesh[sLevel].p[smesh[sLevel].t[i][0]][1], ymax);
                ymax = Math.Max(smesh[sLevel].p[smesh[sLevel].t[i][1]][1], ymax);
                ymax = Math.Max(smesh[sLevel].p[smesh[sLevel].t[i][2]][1], ymax);

                xminArray[i] = xmin;
                xmaxArray[i] = xmax;
                yminArray[i] = ymin;
                ymaxArray[i] = ymax;

                idxxminArray[i] = i;
                idxxmaxArray[i] = i;
                idxyminArray[i] = i;
                idxymaxArray[i] = i;
            }

            QuickSort(xminArray, idxxminArray, 0, nt - 1);
            QuickSort(xmaxArray, idxxmaxArray, 0, nt - 1);
            QuickSort(yminArray, idxyminArray, 0, nt - 1);
            QuickSort(ymaxArray, idxymaxArray, 0, nt - 1);

            j = 0;
            for (i = 0; i < nt; i++)
            {
                if (j < nxy)
                {
                    while (x[j] < xminArray[i])
                    {
                        j++;
                        if (j >= nxy)
                            break;
                    }
                }
                xminArray[i] = j;
            }

            j = nxy - 1;
            for (i = nt - 1; i >= 0; i--)
            {
                if (j >= 0)
                {
                    while (x[j] > xmaxArray[i])
                    {
                        j--;
                        if (j < 0)
                            break;
                    }
                }
                xmaxArray[i] = j;
            }


            j = 0;
            for (i = 0; i < nt; i++)
            {
                if (j < nxy)
                {
                    while (y[j] < yminArray[i])
                    {
                        j++;
                        if (j >= nxy)
                            break;
                    }
                }
                yminArray[i] = j;
            }

            j = nxy - 1;
            for (i = nt - 1; i >= 0; i--)
            {
                if (j >= 0)
                {
                    while (y[j] > ymaxArray[i])
                    {
                        j--;
                        if (j < 0)
                            break;
                    }
                }
                ymaxArray[i] = j;
            }

            RearrangeArray(ref xminArray, idxxminArray, nt);
            RearrangeArray(ref xmaxArray, idxxmaxArray, nt);
            RearrangeArray(ref yminArray, idxyminArray, nt);
            RearrangeArray(ref ymaxArray, idxymaxArray, nt);

            //double[] temp1 = {0.0000, 1.0000,	1.0000,	2.0000,	3.0000,	3.0000,	2.0000,	3.0000,	2.0000,	1.0000,	1.0000,	0.0000,	0.0000,	0.0000,	1.0000,	0.0000};
            //double[] temp2 = {1.0000, 2.0000,	2.0000,	3.0000,	3.0000,	3.0000,	2.0000,	3.0000,	3.0000,	2.0000,	2.0000,	1.0000,	0.0000,	0.0000,	1.0000,	0.0000};
            //double[] temp3 = {3.0000, 3.0000,	2.0000,	3.0000,	2.0000,	1.0000,	1.0000,	0.0000,	0.0000,	0.0000,	1.0000,	0.0000,	0.0000,	1.0000,	1.0000,	2.0000};
            //double[] temp4 = {3.0000, 3.0000,	2.0000,	3.0000,	3.0000,	2.0000,	2.0000,	1.0000,	0.0000,	0.0000,	1.0000,	0.0000,	1.0000,	2.0000,	2.0000,	3.0000};

            //xminArray = temp1;
            //xmaxArray = temp2;
            //yminArray = temp3;
            //ymaxArray = temp4;

            tn = new int[nxy][];
            a12 = new double[nxy][];
            a13 = new double[nxy][];
            for (i = 0; i < nxy; i++)
            {
                tn[i] = new int[nxy];
                a12[i] = new double[nxy];
                a13[i] = new double[nxy];
            }

            //Set tn to a non positive number
            for (i = 0; i < nxy; i++)
                for (j = 0; j < nxy; j++)
                {
                    tn[i][j] = -1;
                }


            for (i = 0; i < nt; i++)
            {
                if ((xminArray[i] <= xmaxArray[i]) && (yminArray[i] <= ymaxArray[i]))
                {
                    for (j = 0; j < 2; j++)
                    {
                        a2[j] = smesh[sLevel].p[smesh[sLevel].t[i][1]][j] - smesh[sLevel].p[smesh[sLevel].t[i][0]][j];
                        a3[j] = smesh[sLevel].p[smesh[sLevel].t[i][2]][j] - smesh[sLevel].p[smesh[sLevel].t[i][0]][j];
                    }

                    temp = a2[0] * a3[1] - a2[1] * a3[0];
                    b2[0] = a3[1] / temp;
                    b2[1] = -a3[0] / temp;
                    b3[0] = -a2[1] / temp;
                    b3[1] = a2[0] / temp;

                    for (j = (int)xminArray[i]; j <= (int)xmaxArray[i]; j++)
                    {
                        for (k = (int)yminArray[i]; k <= (int)ymaxArray[i]; k++)
                        {
                            if (tn[k][j] == -1)
                            {
                                r1p[0] = x[j] - smesh[sLevel].p[smesh[sLevel].t[i][0]][0];
                                r1p[1] = y[k] - smesh[sLevel].p[smesh[sLevel].t[i][0]][1];
                                d2 = b2[0] * r1p[0] + b2[1] * r1p[1];
                                if ((d2 >= -tiny) && (d2 <= 1 + tiny))
                                {
                                    d3 = b3[0] * r1p[0] + b3[1] * r1p[1];
                                    if ((d3 >= -tiny) && (d2 + d3 <= 1 + tiny))
                                    {
                                        tn[k][j] = i;
                                        a12[k][j] = d2;
                                        a13[k][j] = d3;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            double tt1, tt2, tt3;


            for (i = 0; i < nxy; i++)
                for (j = 0; j < nxy; j++)
                {
                    tt1 = (1 - a12[i][j] - a13[i][j]) * inten[(int)smesh[sLevel].t[tn[i][j]][0]];
                    tt2 = a12[i][j] * inten[(int)smesh[sLevel].t[tn[i][j]][1]];
                    tt3 = a13[i][j] * inten[(int)smesh[sLevel].t[tn[i][j]][2]];
                    uxy[i][j] = tt1 + tt2 + tt3;

                }
        }

        public static void QuickSort(double[] a, int[] idx, int n_left, int n_right)
        {
            int i, j;
            double pivot;

            if (n_left >= n_right)
                return;
            i = n_left;
            j = n_right;

            // Choose the last element as pivot
            pivot = a[j];
            while (i < j)
            {
                while (i < j && a[i] <= pivot) i++;
                while (i < j && a[j] >= pivot) j--;
                if (i < j)
                {
                    Swap(ref a[i], ref a[j]);
                    Swap(ref idx[i], ref idx[j]);
                }
            }
            if (i != n_right)
            {
                Swap(ref a[i], ref a[n_right]);
                Swap(ref idx[i], ref idx[n_right]);
            }

            // sort sub-array recursively
            QuickSort(a, idx, n_left, i - 1);
            QuickSort(a, idx, i + 1, n_right);
        }

        public static void Swap(ref double x, ref double y)
        {
            double temp = y;
            y = x;
            x = temp;
        }

        public static void Swap(ref int x, ref int y)
        {
            int temp = y;
            y = x;
            x = temp;
        }

        /// <summary>
        /// Rearrange an array based on its index array
        /// </summary>
        /// <param name="a">data array</param>
        /// <param name="idx">index of data array</param>
        /// <param name="n">number of elements in the array</param>
        public static void RearrangeArray(ref double[] a, int[] idx, int n)
        {
            int i;
            double[] temp = new double[n];

            for (i = 0; i < n; i++)
                temp[idx[i]] = a[i];
            a = temp;
        }


    }
}
