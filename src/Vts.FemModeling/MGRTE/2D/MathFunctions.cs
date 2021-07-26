using System;
using Vts.Common;
using Vts.FemModeling.MGRTE._2D.DataStructures;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Common Math functions
    /// </summary>
    public static class MathFunctions
    {
        /// <summary>
        /// Calculate area of a traingle
        /// </summary>
        /// <param name="x1">x coordinate 1</param>
        /// <param name="y1">y coordinate 1</param>
        /// <param name="x2">x coordinate 2</param>
        /// <param name="y2">y coordinate 2</param>
        /// <param name="x3">x coordinate 3</param>
        /// <param name="y3">y coordinate 3</param>
        /// <returns>area</returns>
        public static double Area(double x1, double y1, double x2, double y2, double x3, double y3)
        // Purpose: Calculate the area of a triangle
        {
            return 0.5 * Math.Abs((y2 - y1) * (x3 - x1) - (y3 - y1) * (x2 - x1));
        }

        /// <summary>
        /// find the minimum from the vector d with the size n.
        /// </summary>
        /// <param name="n">size</param>
        /// <param name="d">vector</param>
        /// <returns></returns>
        public static int FindMin(int n, double[] d)       
        {
            int i;
            int nmin= 0; 
            double dmin = d[nmin];
            for (i = 1; i < n; i++)
            {
                if (d[i] < dmin)
                { nmin = i; dmin = d[i]; }
            }
            return nmin;
        }

        /// <summary>
        /// find two linearly intepolated angles "b" and weights "b2" for the angle "theta_m"
        /// </summary>
        /// <param name="theta_m">theta modified</param>
        /// <param name="dtheta"></param>
        /// <param name="ns"></param>
        /// <param name="b"></param>
        /// <param name="b2"></param>
        /// <param name="constant"></param>
        public static void Intepolation_a(double theta_m, double dtheta, int ns, int[] b, double[] b2, double constant)
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

        /// <summary>
        /// calculate the distance between two points
        /// </summary>
        /// <param name="x1">x coordinate 1</param>
        /// <param name="y1">y coordinate 1</param>
        /// <param name="x2">x coordinate 2</param>
        /// <param name="y2">y coordinate 2</param>
        /// <returns></returns>
        public static double Length(double x1, double y1, double x2, double y2)       
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
        /// <param name="region"></param>
        public static void Weight_2D(double[][][] w, double[][] theta, int nAngle, double g, int region)
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

            w[0][0][region] = 2.0 / 6.0 * (2.0 * f[0] + f[1]) * dx;
            w[0][n - 1][region] = 2.0 / 6.0 * (f[n - 2] + 2.0 * f[n - 1]) * dx;

            for (i = 1; i < n - 1; i++)
            {
                w[0][i][region] = 1.0 / 6.0 * (f[i - 1] + 2.0 * f[i]) * dx + 1.0 / 6.0 * (2.0 * f[i] + f[i + 1]) * dx;
                w[0][2 * n - 2 - i][region] = w[0][i][region];
            }

            norm = 0;
            for (i = 0; i < nAngle; i++)
                norm += w[0][i][region];

            //Normalize
            for (i = 0; i < nAngle; i++)
                w[0][i][region] = w[0][i][region] / norm;



            for (i = 1; i < nAngle; i++)
                for (j = 0; j < nAngle; j++)
                {
                    nj = (j + i) % nAngle;
                    if (nj == 0)
                    {
                        nj = 0;
                    }
                    w[i][nj][region] = w[0][j][region];
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
        /// <param name="dAng">angular array</param>
        /// <param name="n">number of angles</param>
        /// <param name="g">anisotropy factor</param>

        public static void HGPhaseFunction_2D(double[] f, double[] dAng, int n, double g)
        {
            int i;

            for (i = 0; i < n; i++)
                f[i] = 1 / (2 * 3.14159265) * (1 - g * g) / (1 + g * g - 2 * g * Math.Cos(dAng[i]));
        }

        /// <summary>
        /// compute sweep ordering
        /// </summary>
        /// <param name="smesh">spatial mesh</param>
        /// <param name="amesh">angular mesh</param>
        /// <param name="sMeshLevel">spatial mesh level</param>
        /// <param name="aMeshLevel">angular mesh level</param>
        public static void SweepOrdering(ref SpatialMesh[] smesh, AngularMesh[] amesh, int sMeshLevel, int aMeshLevel)
        {
            int i, j, k;
            double[][] centOfTri;
            double[] temp;
            int[] idxtemp;           
            for (i = 0; i <= sMeshLevel; i++)
            {
                centOfTri = new double[smesh[i].Nt][];
                for (j = 0; j < smesh[i].Nt; j++)
                {
                    centOfTri[j] = new double[2];
                    for (k = 0; k < 2; k++)
                        centOfTri[j][k] = (
                            smesh[i].P[smesh[i].T[j][0]][k]
                            + smesh[i].P[smesh[i].T[j][1]][k]
                            + smesh[i].P[smesh[i].T[j][2]][k]) / 3;

                }

                temp = new double [smesh[i].Nt];
                idxtemp = new int[smesh[i].Nt];
                smesh[i].So = new int[amesh[aMeshLevel].Ns][];
                for (j = 0; j < amesh[aMeshLevel].Ns; j++)                        
                    smesh[i].So[j] = new int[smesh[i].Nt];

                for (j = 0; j < amesh[aMeshLevel].Ns; j++)
                {
                    for (k = 0; k < smesh[i].Nt; k++)
                    {
                        temp[k] = Math.Cos(amesh[aMeshLevel].Ang[j][2]) * centOfTri[k][0] + Math.Sin(amesh[aMeshLevel].Ang[j][2]) * centOfTri[k][1];
                        idxtemp[k] = k;
                    }
                    QuickSort(temp, idxtemp, 0, smesh[i].Nt - 1);
                    for (k = 0; k < smesh[i].Nt; k++)                    
                         smesh[i].So[j][k] = idxtemp[k];
                }
            }
        }

        /// <summary>
        /// convert triangular mesh to a square mesh
        /// </summary>
        /// <param name="smesh">spatial mesh</param>
        /// <param name="x">x coordinates</param>
        /// <param name="y">y coordinates</param>
        /// <param name="uxy">grid location</param>
        /// <param name="inten">average intensity of each traingle</param>
        /// <param name="nxy">total points</param>
        public static void SquareTriMeshToGrid(ref SpatialMesh smesh, ref double[] x, ref double[] y, ref double[][] uxy, double[] inten, int nxy)
        {
            int i, j, k;
            int np, nt;
            double dx, dy;
            int[][] tn;
            double[][] a12;
            double[][] a13;
            double[] a2 = new double[2];
            double[] a3 = new double[2];
            double[] b2 = new double[2];
            double[] b3 = new double[2];
            double d2, d3;
            double[] r1p = new double[2];

            double xmin, xmax, zmin, zmax;
            double temp;
            double tiny = 2.2204e-12;

            double[] xminArray;
            double[] xmaxArray;
            double[] zminArray;
            double[] zmaxArray;

            int[] idxxminArray;
            int[] idxxmaxArray;
            int[] idxzminArray;
            int[] idxzmaxArray;

            // Assign large number for min and small number for max
            xmin = 1e10; xmax = -1e10; zmin = 1e10; zmax = -1e10;

            np = smesh.Np;
            nt = smesh.Nt;

            for (i = 0; i < np; i++)
            {
                xmin = Math.Min(smesh.P[i][0], xmin);
                xmax = Math.Max(smesh.P[i][0], xmax);
                zmin = Math.Min(smesh.P[i][1], zmin);
                zmax = Math.Max(smesh.P[i][1], zmax);
            }

            dx = (xmax - xmin) / (nxy - 1);
            dy = (zmax - zmin) / (nxy - 1);           

            for (i = 0; i < nxy; i++)
            {
                x[i] = xmin + i * dx;
                y[i] = zmin + i * dy;
            }

            xminArray = new double[nt];
            xmaxArray = new double[nt];
            zminArray = new double[nt];
            zmaxArray = new double[nt];

            idxxminArray = new int[nt];
            idxxmaxArray = new int[nt];
            idxzminArray = new int[nt];
            idxzmaxArray = new int[nt];

            for (i = 0; i < nt; i++)
            {
                xmin = 1e10; xmax = -1e10; zmin = 1e10; zmax = -1e10;

                xmin = Math.Min(smesh.P[smesh.T[i][0]][0], xmin);
                xmin = Math.Min(smesh.P[smesh.T[i][1]][0], xmin);
                xmin = Math.Min(smesh.P[smesh.T[i][2]][0], xmin);

                xmax = Math.Max(smesh.P[smesh.T[i][0]][0], xmax);
                xmax = Math.Max(smesh.P[smesh.T[i][1]][0], xmax);
                xmax = Math.Max(smesh.P[smesh.T[i][2]][0], xmax);

                zmin = Math.Min(smesh.P[smesh.T[i][0]][1], zmin);
                zmin = Math.Min(smesh.P[smesh.T[i][1]][1], zmin);
                zmin = Math.Min(smesh.P[smesh.T[i][2]][1], zmin);

                zmax = Math.Max(smesh.P[smesh.T[i][0]][1], zmax);
                zmax = Math.Max(smesh.P[smesh.T[i][1]][1], zmax);
                zmax = Math.Max(smesh.P[smesh.T[i][2]][1], zmax);

                xminArray[i] = xmin;
                xmaxArray[i] = xmax;
                zminArray[i] = zmin;
                zmaxArray[i] = zmax;

                idxxminArray[i] = i;
                idxxmaxArray[i] = i;
                idxzminArray[i] = i;
                idxzmaxArray[i] = i;
            }

            QuickSort(xminArray, idxxminArray, 0, nt - 1);
            QuickSort(xmaxArray, idxxmaxArray, 0, nt - 1);
            QuickSort(zminArray, idxzminArray, 0, nt - 1);
            QuickSort(zmaxArray, idxzmaxArray, 0, nt - 1);

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
                    while (y[j] < zminArray[i])
                    {
                        j++;
                        if (j >= nxy)
                            break;
                    }
                }
                zminArray[i] = j;
            }

            j = nxy - 1;
            for (i = nt - 1; i >= 0; i--)
            {
                if (j >= 0)
                {
                    while (y[j] > zmaxArray[i])
                    {
                        j--;
                        if (j < 0)
                            break;
                    }
                }
                zmaxArray[i] = j;
            }

            RearrangeArray(ref xminArray, idxxminArray, nt);
            RearrangeArray(ref xmaxArray, idxxmaxArray, nt);
            RearrangeArray(ref zminArray, idxzminArray, nt);
            RearrangeArray(ref zmaxArray, idxzmaxArray, nt);


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
                if ((xminArray[i] <= xmaxArray[i]) && (zminArray[i] <= zmaxArray[i]))
                {
                    for (j = 0; j < 2; j++)
                    {
                        a2[j] = smesh.P[smesh.T[i][1]][j] - smesh.P[smesh.T[i][0]][j];
                        a3[j] = smesh.P[smesh.T[i][2]][j] - smesh.P[smesh.T[i][0]][j];
                    }

                    temp = a2[0] * a3[1] - a2[1] * a3[0];
                    b2[0] = a3[1] / temp;
                    b2[1] = -a3[0] / temp;
                    b3[0] = -a2[1] / temp;
                    b3[1] = a2[0] / temp;

                    for (j = (int)xminArray[i]; j <= (int)xmaxArray[i]; j++)
                    {
                        for (k = (int)zminArray[i]; k <= (int)zmaxArray[i]; k++)
                        {
                            if (tn[k][j] == -1)
                            {
                                r1p[0] = x[j] - smesh.P[smesh.T[i][0]][0];
                                r1p[1] = y[k] - smesh.P[smesh.T[i][0]][1];
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
                    tt1 = (1 - a12[i][j] - a13[i][j]) * inten[(int)smesh.T[tn[i][j]][0]];
                    tt2 = a12[i][j] * inten[(int)smesh.T[tn[i][j]][1]];
                    tt3 = a13[i][j] * inten[(int)smesh.T[tn[i][j]][2]];
                    uxy[i][j] = tt1 + tt2 + tt3;

                }
        }

        private static void QuickSort(double[] a, int[] idx, int n_left, int n_right)
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

        private static void Swap(ref double x, ref double y)
        {
            double temp = y;
            y = x;
            x = temp;
        }

        private static void Swap(ref int x, ref int y)
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
        private static void RearrangeArray(ref double[] a, int[] idx, int n)
        {
            int i;
            double[] temp = new double[n];

            for (i = 0; i < n; i++)
                temp[idx[i]] = a[i];
            a = temp;
        }


        /// <summary>
        /// Create an angular mesh
        /// </summary>
        /// <param name="amesh"></param>
        /// <param name="aLevel"></param>
        /// <param name="tissueInput"> </param>
        public static void CreateAnglularMesh(ref AngularMesh[] amesh, int aLevel, MultiEllipsoidTissueInput tissueInput)
        {
            int i, j, k, l;
            int nLayerRegions = tissueInput.LayerRegions.Length;
            int nInclusionRegions = tissueInput.EllipsoidRegions.Length;
            int nRegions = nLayerRegions + nInclusionRegions - 2;


            for (i = 0; i <= aLevel; i++)
            {
                amesh[i].Ns = (int) Math.Pow(2.0, (double) (i + 1));
                amesh[i].Ang = new double[amesh[i].Ns][];
                amesh[i].W = new double[amesh[i].Ns][][];
                for (j = 0; j < amesh[i].Ns; j++)
                {
                    amesh[i].Ang[j] = new double[3];
                    amesh[i].W[j] = new double[amesh[i].Ns][];
                    for (k = 0; k < amesh[i].Ns; k++)
                        amesh[i].W[j][k] = new double[nRegions];
                }

                for (j = 0; j < amesh[i].Ns; j++)
                {
                    for (l = 0; l < nLayerRegions - 2; l++)
                        Weight_2D(amesh[i].W, amesh[i].Ang, amesh[i].Ns, tissueInput.LayerRegions[l + 1].RegionOP.G, l);

                    for (l = 0; l < nInclusionRegions; l++)
                        Weight_2D(amesh[i].W, amesh[i].Ang, amesh[i].Ns, tissueInput.EllipsoidRegions[l].RegionOP.G,
                                  l + nLayerRegions - 2);
                }
            }
        }

        /// <summary>
        /// Assign region values to smesh[].Region variable
        /// </summary>
        /// <param name="smesh">spatial mesh</param>
        /// <param name="np">number of nodes</param>
        /// <param name="tissueInput">tissue input</param>
        public static void AssignRegions(ref SpatialMesh[] smesh, int np, MultiEllipsoidTissueInput tissueInput)
        {
            int i, j, k;
            double x, z;

            int nLayers = tissueInput.LayerRegions.Length;
            int nInclusions = tissueInput.EllipsoidRegions.Length;

            for (i = 0; i <= np; i++)
                for (j = 0; j < smesh[i].Np; j++)
                {
                    x = smesh[i].P[j][0];
                    z = smesh[i].P[j][1];

                    for (k = 1; k < nLayers - 1; k++)
                    {
                        if ((z >= ((LayerTissueRegion)tissueInput.LayerRegions[k]).ZRange.Start) &&
                           (z <= ((LayerTissueRegion)tissueInput.LayerRegions[k]).ZRange.Stop))
                            smesh[i].Region[j] = k - 1;

                    }

                    for (k = 0; k < nInclusions; k++)
                    {
                        double dx = ((EllipsoidTissueRegion)tissueInput.EllipsoidRegions[k]).Dx;
                        double dz = ((EllipsoidTissueRegion)tissueInput.EllipsoidRegions[k]).Dz;

                        Position center = ((EllipsoidTissueRegion)tissueInput.EllipsoidRegions[k]).Center;

                        double func = ((x - center.X) * (x - center.X) / (dx * dx)) + ((z - center.Z) * (z - center.Z) / (dz * dz));
                        if (func <= 1)
                            smesh[i].Region[j] = nLayers - 2 + k;

                    }
                }
        }

        /// <summary>
        /// Set Mus in nodes
        /// </summary>
        /// <param name="ua">mua </param>
        /// <param name="smesh">spatial mesh</param>
        /// <param name="input">simulation input</param>
        public static void SetMua(ref double[][][] ua, SpatialMesh[] smesh, SimulationInput input)
        {
            int j, k, l;
            int nt = smesh[input.MeshDataInput.SMeshLevel].Nt;
            var tissueInput = ((MultiEllipsoidTissueInput) input.TissueInput);
            int nLayerRegions = tissueInput.LayerRegions.Length;
            int nInclusionRegions = tissueInput.EllipsoidRegions.Length;
            int sMeshLevel = input.MeshDataInput.SMeshLevel;

            ua[sMeshLevel] = new double[nt][];
            for (j = 0; j < nt; j++)
            {
                ua[sMeshLevel][j] = new double[3];
                for (k = 0; k < 3; k++)
                {
                     for (l = 0; l < nLayerRegions - 2; l++)
                     {
                         if (smesh[sMeshLevel].Region[smesh[sMeshLevel].T[j][0]] == l) 
                             ua[sMeshLevel][j][k] = tissueInput.LayerRegions[l+1].RegionOP.Mua;
                     }

                     for (l = 0; l < nInclusionRegions; l++)
                     {
                         if (smesh[sMeshLevel].Region[smesh[sMeshLevel].T[j][0]] == l + nLayerRegions - 2) 
                             ua[sMeshLevel][j][k] = tissueInput.EllipsoidRegions[l].RegionOP.Mua;
                     }
                }
            }
        }
        
        /// <summary>
        /// Set Mus in nodes
        /// </summary>
        /// <param name="us">mus </param>
        /// <param name="smesh">spatial mesh</param>
        /// <param name="input">simulation input</param>
        public static void SetMus(ref double[][][] us, SpatialMesh[] smesh, SimulationInput input)
        {
            int j, k, l;
            int nt = smesh[input.MeshDataInput.SMeshLevel].Nt;
            var tissueInput = ((MultiEllipsoidTissueInput)input.TissueInput);
            int nLayerRegions = tissueInput.LayerRegions.Length;
            int nInclusionRegions = tissueInput.EllipsoidRegions.Length;
            int sMeshLevel = input.MeshDataInput.SMeshLevel;

            us[sMeshLevel] = new double[nt][];
            for (j = 0; j < nt; j++)
            {
                us[sMeshLevel][j] = new double[3];
                for (k = 0; k < 3; k++)
                {
                    for (l = 0; l < nLayerRegions - 2; l++)
                    {
                        if (smesh[sMeshLevel].Region[smesh[sMeshLevel].T[j][0]] == l)
                            us[sMeshLevel][j][k] = tissueInput.LayerRegions[l + 1].RegionOP.Mus;
                    }

                    for (l = 0; l < nInclusionRegions; l++)
                    {
                        if (smesh[sMeshLevel].Region[smesh[sMeshLevel].T[j][0]] == l + nLayerRegions - 2)
                            us[sMeshLevel][j][k] = tissueInput.EllipsoidRegions[l].RegionOP.Mus;
                    }
                }
            }
        }


        /// <summary>
        /// Create a squarte mesh for given spatial mesh level
        /// </summary>
        /// <param name="smesh">spatial mesh</param>
        /// <param name="sMeshLevel">spatial mesh levels</param>
        /// <param name="length">length</param>
        public static void CreateSquareMesh(ref SpatialMesh[] smesh, int sMeshLevel, double length)
        {
            int i;
            int np, nt, ne;

            //SQUARE MESH
            np = 5;
            ne = 4;
            nt = 4;

            double[][] pts = new double[np][];
            for (i = 0; i < np; i++)
                pts[i] = new double[4];

            int[][] edge = new int[ne][];
            for (i = 0; i < ne; i++)
                edge[i] = new int[4];

            int[][] tri = new int[nt][];
            for (i = 0; i < nt; i++)
                tri[i] = new int[3];

            //create the basic square mesh
            BasicSquareMesh(pts, edge, tri, length);

            AssignSpatialMesh(ref smesh, pts, edge, tri, np, ne, nt, 0);

            //string str = "PET" + 0;
            //str = str + ".txt";
            //WritePTEData(str, pts, edge, tri, np, ne, nt);

            if (sMeshLevel > 0)
                CreateMultigrid(ref smesh, pts, edge, tri, np, ne, nt, sMeshLevel);
        }

        /// <summary>
        /// Define Basic Square Mesh
        /// </summary>
        /// <param name="pts">points data</param>
        /// <param name="edge">edge data</param>
        /// <param name="tri">triangle data</param>
        /// <param name="length">length</param>
        private static void BasicSquareMesh(
            double[][] pts,
            int[][] edge,
            int[][] tri,
            double length)
        {
            //Boundaries: For X: x =-0.5; x =0.5  & For Z: z=0; z=1;  
            pts[0][0] = -0.5 *length;  pts[0][1] = 0;            pts[0][2] = 0; pts[0][3] = 1;
            pts[1][0] = 0.5 * length;  pts[1][1] = 0;            pts[1][2] = 1; pts[1][3] = 1;
            pts[2][0] = 0.5 * length;  pts[2][1] = length;       pts[2][2] = 2; pts[2][3] = 1;
            pts[3][0] = -0.5 *length;  pts[3][1] = length;       pts[3][2] = 3; pts[3][3] = 1;
            pts[4][0] = 0;             pts[4][1] = 0.5 * length; pts[4][2] = 4; pts[4][3] = 0;

            edge[0][0] = -1; edge[0][1] = (int)pts[0][2]; edge[0][2] = (int)pts[1][2]; edge[0][3] = -1;
            edge[1][0] = -1; edge[1][1] = (int)pts[1][2]; edge[1][2] = (int)pts[2][2]; edge[1][3] = -1;
            edge[2][0] = -1; edge[2][1] = (int)pts[2][2]; edge[2][2] = (int)pts[3][2]; edge[2][3] = -1;
            edge[3][0] = -1; edge[3][1] = (int)pts[3][2]; edge[3][2] = (int)pts[0][2]; edge[3][3] = -1;

            tri[0][0] = (int)pts[0][2]; tri[0][1] = (int)pts[1][2]; tri[0][2] = (int)pts[4][2];
            tri[1][0] = (int)pts[1][2]; tri[1][1] = (int)pts[2][2]; tri[1][2] = (int)pts[4][2];
            tri[2][0] = (int)pts[2][2]; tri[2][1] = (int)pts[3][2]; tri[2][2] = (int)pts[4][2];
            tri[3][0] = (int)pts[3][2]; tri[3][1] = (int)pts[0][2]; tri[3][2] = (int)pts[4][2];
        }

        /// <summary>
        /// Create Multigrid based on spatial mesh level
        /// </summary>
        /// <param name="smesh"></param>
        /// <param name="p">points data</param>
        /// <param name="e">edge data</param>
        /// <param name="t">triangle data</param>
        /// <param name="np">number of points</param>
        /// <param name="ne">number of boundary edges</param>
        /// <param name="nt">number of triangles</param>
        /// <param name="sMeshLevel">spatial mesh levels</param>
        public static void CreateMultigrid(ref SpatialMesh[] smesh, double[][] p, int[][] e, int[][] t, int np, int ne, int nt, int sMeshLevel)
        {
            int i, j, nt2, ne2, np2, npt, trint;

            //Input arrays
            double[][] oldp = p;
            int[][] oldt = t;
            int[][] olde = e;

            //char[] str[80];
            //char[] level[4];	       

            for (j = 1; j <= sMeshLevel; j++)
            {   
                //npt gets the total number of point after sub division
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

                double[][] newp = new double[np2][];
                for (i = 0; i < np2; i++)
                    newp[i] = new double[4];

                int[][] newe = new int[ne2][];
                for (i = 0; i < ne2; i++)
                    newe[i] = new int[4];

                int[][] newt = new int[nt2][];
                for (i = 0; i < nt2; i++)
                    newt[i] = new int[3];

                NewPET(newp, oldp, newe, newt, oldt, ptemp, np, nt, trint);

                //Update np, nt and ne
                np = np2;
                ne = ne2;
                nt = nt2;

                //Assign output arrays to input arrays
                oldp = newp;
                olde = newe;
                oldt = newt;                

                AssignSpatialMesh(ref smesh, oldp, olde, oldt, np, ne, nt, j);
            }

            for (j = 0; j <= sMeshLevel; j++)
                smesh[j].Region = new int[smesh[j].Np];
            


        }
                
        private static void FindPTemp(double[][] ptemp, double[][] p, int[][] t, int nt)
        {
            int i;
            int p0, p1, p2;

            for (i = 0; i < nt; i++)
            {
                //x cordinates
                ptemp[3 * i][0] = 0.5 * (p[t[i][0]][0] + p[t[i][1]][0]);
                ptemp[3 * i + 1][0] = 0.5 * (p[t[i][0]][0] + p[t[i][2]][0]);
                ptemp[3 * i + 2][0] = 0.5 * (p[t[i][1]][0] + p[t[i][2]][0]);

                //y cordinates
                ptemp[3 * i][1] = 0.5 * (p[t[i][0]][1] + p[t[i][1]][1]);
                ptemp[3 * i + 1][1] = 0.5 * (p[t[i][0]][1] + p[t[i][2]][1]);
                ptemp[3 * i + 2][1] = 0.5 * (p[t[i][1]][1] + p[t[i][2]][1]);

                //P index - initialize to negative values
                ptemp[3 * i][2] = -1;
                ptemp[3 * i + 1][2] = -1;
                ptemp[3 * i + 2][2] = -1;

                //find the edge vertex of a triangle
                p0 = (int)p[t[i][0]][3];
                p1 = (int)p[t[i][1]][3];
                p2 = (int)p[t[i][2]][3];

                if (p0 + p1 == 2)
                {
                    ptemp[3 * i][3] = 1;
                    ptemp[3 * i][4] = p[t[i][0]][2];
                    ptemp[3 * i][5] = p[t[i][1]][2];
                }
                else if (p0 + p2 == 2)
                {
                    ptemp[3 * i + 1][3] = 1;
                    ptemp[3 * i + 1][4] = p[t[i][0]][2];
                    ptemp[3 * i + 1][5] = p[t[i][2]][2];
                }
                else if (p1 + p2 == 2)
                {
                    ptemp[3 * i + 2][3] = 1;
                    ptemp[3 * i + 2][4] = p[t[i][1]][2];
                    ptemp[3 * i + 2][5] = p[t[i][2]][2];
                }
            }

        }

        private static void ReindexingPTemp(double[][] ptemp, ref int count, int trint, int np)
        {
            int i, j;

            count = np;
            for (i = 0; i < trint - 1; i++)
            {
                for (j = i + 1; j < trint; j++)
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
            for (i = 0; i < trint; i++)
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
        private static void NewPET(double[][] newp, double[][] oldp, int[][] newe, int[][] newt, int[][] oldt, double[][] ptemp, int np, int nt, int trint)
        {
            int i, count;

            //Assign P
            for (i = 0; i < np; i++)
            {
                count = (int)oldp[i][2];
                newp[count][0] = oldp[i][0];
                newp[count][1] = oldp[i][1];
                newp[count][2] = oldp[i][2];
                newp[count][3] = oldp[i][3];
            }

            for (i = 0; i < trint; i++)
            {
                count = (int)ptemp[i][2];
                newp[count][0] = ptemp[i][0];
                newp[count][1] = ptemp[i][1];
                newp[count][2] = ptemp[i][2];
                newp[count][3] = ptemp[i][3];
            }


            //Assign E
            count = 0;
            for (i = 0; i < trint; i++)
            {
                if (ptemp[i][3] == 1)
                {
                    newe[count][0] = -1;
                    newe[count][1] = (int)ptemp[i][4];
                    newe[count][2] = (int)ptemp[i][2];
                    newe[count][3] = -1;
                    count++;
                    newe[count][0] = -1;
                    newe[count][1] = (int)ptemp[i][2];
                    newe[count][2] = (int)ptemp[i][5];
                    newe[count][3] = -1;
                    count++;
                }
            }



            double x0, x1, x2, x3, x4, x5;
            double y0, y1, y2, y3, y4, y5;

            double temp;

            //Assign T
            for (i = 0; i < nt; i++)
            {

                x0 = oldp[oldt[i][0]][0];
                y0 = oldp[oldt[i][0]][1];

                x1 = oldp[oldt[i][1]][0];
                y1 = oldp[oldt[i][1]][1];

                x2 = oldp[oldt[i][2]][0];
                y2 = oldp[oldt[i][2]][1];

                x3 = ptemp[3 * i][0];
                y3 = ptemp[3 * i][1];

                x4 = ptemp[3 * i + 1][0];
                y4 = ptemp[3 * i + 1][1];

                x5 = ptemp[3 * i + 2][0];
                y5 = ptemp[3 * i + 2][1];

                //Nodes in the triangle should be arranged counter clock wise
                // http://softsurfer.com/Archive/algorithm_0101/algorithm_0101.htm
                //(x1-x0)*(y2-y0)-(x2-x0)*(y1-y0) > 0 counter clockwise

                //Triangle 1
                temp = (x3 - x0) * (y4 - y0) - (x4 - x0) * (y3 - y0);
                if (temp > 0)
                {
                    newt[4 * i][0] = oldt[i][0];
                    newt[4 * i][1] = (int)ptemp[3 * i][2];
                    newt[4 * i][2] = (int)ptemp[3 * i + 1][2];
                }
                else
                {
                    newt[4 * i][0] = oldt[i][0];
                    newt[4 * i][1] = (int)ptemp[3 * i + 1][2];
                    newt[4 * i][2] = (int)ptemp[3 * i][2];
                }

                //Triangle 2
                temp = (x3 - x1) * (y5 - y1) - (x5 - x1) * (y3 - y1);
                if (temp > 0)
                {
                    newt[4 * i + 1][0] = oldt[i][1];
                    newt[4 * i + 1][1] = (int)ptemp[3 * i][2];
                    newt[4 * i + 1][2] = (int)ptemp[3 * i + 2][2];
                }
                else
                {
                    newt[4 * i + 1][0] = oldt[i][1];
                    newt[4 * i + 1][1] = (int)ptemp[3 * i + 2][2];
                    newt[4 * i + 1][2] = (int)ptemp[3 * i][2];
                }

                //Triangle 3
                temp = (x4 - x2) * (y5 - y2) - (x5 - x2) * (y4 - y2);
                if (temp > 0)
                {
                    newt[4 * i + 2][0] = oldt[i][2];
                    newt[4 * i + 2][1] = (int)ptemp[3 * i + 1][2];
                    newt[4 * i + 2][2] = (int)ptemp[3 * i + 2][2];
                }
                else
                {
                    newt[4 * i + 2][0] = oldt[i][2];
                    newt[4 * i + 2][1] = (int)ptemp[3 * i + 2][2];
                    newt[4 * i + 2][2] = (int)ptemp[3 * i + 1][2];
                }

                //Triangle 4 (Inner Triangle)
                temp = (x4 - x3) * (y5 - y3) - (x5 - x3) * (y4 - y3);
                if (temp > 0)
                {
                    newt[4 * i + 3][0] = (int)ptemp[3 * i + 1][2];
                    newt[4 * i + 3][1] = (int)ptemp[3 * i + 2][2];
                    newt[4 * i + 3][2] = (int)ptemp[3 * i][2];
                }
                else
                {
                    newt[4 * i + 3][0] = (int)ptemp[3 * i + 1][2];
                    newt[4 * i + 3][1] = (int)ptemp[3 * i][2];
                    newt[4 * i + 3][2] = (int)ptemp[3 * i + 2][2];
                }
            }
        }
        

        private static void AssignSpatialMesh(ref SpatialMesh[] smesh, double[][] p, int[][] e, int[][] t, int np, int ne, int nt, int level)
        {
            int i, j;

            smesh[level].Np = np;
            smesh[level].Ne = ne;
            smesh[level].Nt = nt;

            smesh[level].P = new double[np][];
            for (i = 0; i < np; i++)
            {
                smesh[level].P[i] = new double[2];
                for (j = 0; j < 2; j++)
                    smesh[level].P[i][j] = p[i][j];
            }

            smesh[level].T = new int[nt][];
            for (i = 0; i < nt; i++)
            {
                smesh[level].T[i] = new int[3];
                for (j = 0; j < 3; j++)
                    smesh[level].T[i][j] = t[i][j];
            }


            smesh[level].E = new int[ne][];
            for (i = 0; i < ne; i++)
            {
                smesh[level].E[i] = new int[4];
                for (j = 0; j < 4; j++)
                    smesh[level].E[i][j] = e[i][j];
            }

        } 
    }
}
