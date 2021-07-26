using System;
using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Initialization class
    /// </summary>
    public class Initialization
    {        
        public static void Initial(
            ref AngularMesh[] amesh,
            ref SpatialMesh[] smesh,
            ref double[][][][] flux,
            ref double[][][][] d,
            ref double[][][][] RHS,
            ref double[][][][] q,
            ref int[][] noflevel,
            ref BoundaryCoupling[] b,
            int level,
            int mgMethod,
            int vacuum,
            double nTissue,
            double nExt,
            int aMeshLevel,
            int aMeshLevel0,
            int sMeshLevel,
            int sMeshLevel0,
            double[][][] ua,
            double[][][] us,
            MultiGridCycle Mgrid)
        {


            //Purpose: in this function, we load the following four ".txt" files:
            //             1) "amesh.txt": "ns", "a" and "w" for angular mesh
            //             3) "ua.txt": absorption coefficient
            //             4) "us.txt": scattering coefficient

            //         we first load the mesh files:
            //             1.1. "amesh.txt": "ns", "a" and "w" for angular mesh

            //         and then we compute the following for spatial mesh:
            //             2.1. "smap", "cf" and "fc"

            //         and finally malloc the following for multigrid algorithm:
            //             3.1. "noflevel"
            //             3.2. "ua", "us", "flux", "RHS", "d" and "q"
            //             3.3.  "b"

            int tempsize = 20, tempsize2 = 20;
            int i, j, k, m, n, nt, np, ne, ns, da = aMeshLevel - aMeshLevel0, ds = sMeshLevel - sMeshLevel0;
            double x1, x2, x3, y1, y2, y3;
            int[][] t;
            int[][] e;
            int[][] p2;
            int[][] smap;
            double[][] p;


            // 2.1. compute "c", "ec", "a" and "p2"
            //      p2[np][p2[np][0]+1]: triangles adjacent to one node
            //      For the 2nd index, the 1st element is the total number of triangles adjacent to this node,
            //      the corresponding triangles are saved from the 2nd.
            //      Example:    assume triangles [5 16 28 67] are adjacent to the node 10, then
            //                  p2[10][0]=4, p2[10][1]=5, p2[10][2]=16, p2[10][3]=28 and p2[10][4]=67.
            for (i = 0; i <= sMeshLevel; i++)
            {
                p = smesh[i].P; t = smesh[i].T; nt = smesh[i].Nt; np = smesh[i].Np; e = smesh[i].E; ne = smesh[i].Ne;

                smesh[i].C = new double[nt][];
                for (j = 0; j < nt; j++)
                {
                    smesh[i].C[j] = new double[2];
                    for (k = 0; k < 2; k++)
                    {
                        smesh[i].C[j][k] = (p[t[j][0]][k] + p[t[j][1]][k] + p[t[j][2]][k]) / 3;
                    }// center of triangle
                }

                smesh[i].Ec = new double[ne][];
                for (j = 0; j < ne; j++)
                {
                    smesh[i].Ec[j] = new double[2]; ;
                    for (k = 0; k < 2; k++)
                    { smesh[i].Ec[j][k] = (p[e[j][1]][k] + p[e[j][2]][k]) / 2; }// center of edge
                }

                smesh[i].A = new double[nt]; ;
                for (j = 0; j < nt; j++)
                {
                    x1 = p[t[j][0]][0]; y1 = p[t[j][0]][1];
                    x2 = p[t[j][1]][0]; y2 = p[t[j][1]][1];
                    x3 = p[t[j][2]][0]; y3 = p[t[j][2]][1];
                    smesh[i].A[j] = MathFunctions.Area(x1, y1, x2, y2, x3, y3);//area of triangle
                }

                p2 = new int[np][];
                for (j = 0; j < np; j++)
                {
                    p2[j] = new int[tempsize];
                    // tempsize is the initial length of the second index of "p2", and it may cause problem if it is too small.
                    p2[j][0] = 0;
                    for (k = 1; k < tempsize; k++)
                    {
                        p2[j][k] = -1;
                    }
                }

                for (j = 0; j < nt; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        p2[t[j][k]][0] += 1;
                        p2[t[j][k]][p2[t[j][k]][0]] = j;
                    }
                }

                for (j = 0; j < np; j++)
                {
                    if (p2[j][0] + 1 > tempsize)
                    {
                        Console.WriteLine("WARNING: tempsize for p2 is too small!!\n");
                        goto stop;
                    }
                }

                smesh[i].P2 = new int[np][];
                for (j = 0; j < np; j++)
                {
                    smesh[i].P2[j] = new int[(p2[j][0] + 1)];
                    for (k = 0; k <= p2[j][0]; k++)
                    { smesh[i].P2[j][k] = p2[j][k]; }
                }
            }

            // 2.2. compute "smap", "cf" and "fc"
            //      For the data structure of "smap", see "spatialmapping";
            //      For the data structure of "cf" and "fc", see "spatialmapping2".
            //      Note: those arrays are not defined on the coarsest spatial mesh (i=0),
            //      since they are always saved on the fine level instead of the coarse level.
            for (i = 1; i <= sMeshLevel; i++)
            {
                smap = new int[smesh[i - 1].Nt][];
                for (j = 0; j < smesh[i - 1].Nt; j++)
                {
                    smap[j] = new int[tempsize2];
                    // tempsize2 is the initial length of the second index of "smap", and it may cause problem if it is too small.
                    smap[j][0] = 0;
                    for (k = 1; k < tempsize2; k++)
                    { smap[j][k] = -1; }
                }
                Mgrid.SpatialMapping(smesh[i - 1], smesh[i], smap);

                for (j = 0; j < smesh[i - 1].Nt; j++)
                {
                    if (smap[j][0] > tempsize2 - 1)
                    {
                        Console.WriteLine("WARNING: tempsize2 for smap is too small!!\n");
                        goto stop;
                    }
                }

                smesh[i].Smap = new int[smesh[i - 1].Nt][];
                for (j = 0; j < smesh[i - 1].Nt; j++)
                {
                    smesh[i].Smap[j] = new int[smap[j][0] + 1];
                    for (k = 0; k <= smap[j][0]; k++)
                    { smesh[i].Smap[j][k] = smap[j][k]; }
                }


                smesh[i].Cf = new double[smesh[i - 1].Nt][][][];
                for (j = 0; j < smesh[i - 1].Nt; j++)
                {
                    smesh[i].Cf[j] = new double[3][][];
                    for (k = 0; k < 3; k++)
                    {
                        smesh[i].Cf[j][k] = new double[smesh[i].Smap[j][0]][];
                        for (m = 0; m < smesh[i].Smap[j][0]; m++)
                        { smesh[i].Cf[j][k][m] = new double[3]; ; }
                    }
                }
                smesh[i].Fc = new double[smesh[i - 1].Nt][][][];
                for (j = 0; j < smesh[i - 1].Nt; j++)
                {
                    smesh[i].Fc[j] = new double[3][][];
                    for (k = 0; k < 3; k++)
                    {
                        smesh[i].Fc[j][k] = new double[smesh[i].Smap[j][0]][];
                        for (m = 0; m < smesh[i].Smap[j][0]; m++)
                        { smesh[i].Fc[j][k][m] = new double[3]; ; }
                    }
                }
                Mgrid.SpatialMapping2(smesh[i - 1], smesh[i], smesh[i].Smap, smesh[i].Cf, smesh[i].Fc, tempsize2);
            }

            // 2.3. compute "e", "e2", "so2", "n" and "ori"
            //      For the data structure of "eo", "e2","so2", "n" and "ori", see "boundary".
            for (i = 0; i <= sMeshLevel; i++)
            {
                smesh[i].E2 = new int[smesh[i].Ne][];
                for (j = 0; j < smesh[i].Ne; j++)
                { smesh[i].E2[j] = new int[2]; }
                smesh[i].So2 = new int[smesh[i].Nt][];
                for (j = 0; j < smesh[i].Nt; j++)
                { smesh[i].So2[j] = new int[3]; }
                smesh[i].N = new double[smesh[i].Ne][];
                for (j = 0; j < smesh[i].Ne; j++)
                { smesh[i].N[j] = new double[2]; }
                smesh[i].Ori = new int[smesh[i].Ne];

                Mgrid.Boundary(smesh[i].Ne, smesh[i].Nt, smesh[i].T, smesh[i].P2, smesh[i].P, smesh[i].E, smesh[i].E2, smesh[i].So2, smesh[i].N, smesh[i].Ori);
            }

            // 2.4. compute "bd" and "bd2"
            //      For the data structure of "bd" and "bd2", see "edgeterm".
            for (i = 0; i <= sMeshLevel; i++)
            {
                smesh[i].Bd = new int[amesh[aMeshLevel].Ns][][];
                for (j = 0; j < amesh[aMeshLevel].Ns; j++)
                {
                    smesh[i].Bd[j] = new int[smesh[i].Nt][]; ;
                    for (k = 0; k < smesh[i].Nt; k++)
                    {
                        smesh[i].Bd[j][k] = new int[9];
                        for (m = 0; m < 9; m++)
                        { smesh[i].Bd[j][k][m] = -1; }
                    }
                }
            }
            for (i = 0; i <= sMeshLevel; i++)
            {
                smesh[i].Bd2 = new double[amesh[aMeshLevel].Ns][][];
                for (j = 0; j < amesh[aMeshLevel].Ns; j++)
                {
                    smesh[i].Bd2[j] = new double[smesh[i].Nt][];
                    for (k = 0; k < smesh[i].Nt; k++)
                    { smesh[i].Bd2[j][k] = new double[3]; }
                }
            }
            for (i = 0; i <= sMeshLevel; i++)
            {
                for (j = 0; j < amesh[aMeshLevel].Ns; j++)
                {
                    Mgrid.EdgeTri(smesh[i].Nt, amesh[aMeshLevel].Ang[j], smesh[i].P, smesh[i].P2, smesh[i].T, smesh[i].Bd[j], smesh[i].Bd2[j], smesh[i].So2);
                }
            }

            // 3.1. compute "noflevel"
            //      level: the mesh layers for multgrid; the bigger value represents finer mesh.
            //      noflevel[level][2]: the corresponding angular mesh level and spatial mesh level for each multigrid mesh level
            //      Example:    assume noflevel[i][0]=3 and noflevel[i][1]=2, then
            //                  the spatial mesh level is "3" and the angular mesh level is "2" on the multgrid mesh level "i".
            switch (mgMethod)
            {
                case 1: //AMG
                    for (i = 0; i <= level; i++)
                    { noflevel[i] = new int[2]; }
                    for (i = 0; i <= da; i++)
                    {
                        noflevel[i][0] = sMeshLevel;
                        noflevel[i][1] = i + aMeshLevel0;
                    }
                    break;
                case 2: //SMG
                    for (i = 0; i <= level; i++)
                    { noflevel[i] = new int[2]; }
                    for (i = 0; i <= ds; i++)
                    {
                        noflevel[i][0] = i + sMeshLevel0;
                        noflevel[i][1] = aMeshLevel;
                    }
                    break;
                case 3: //MG1
                    for (i = 0; i <= level; i++)
                    { noflevel[i] = new int[2]; }
                    if (ds > da)
                    {
                        for (i = 0; i < ds - da; i++)
                        {
                            noflevel[i][0] = i + sMeshLevel0;
                            noflevel[i][1] = aMeshLevel0;
                        }
                        for (i = 0; i <= da; i++)
                        {
                            noflevel[i + ds - da][0] = i + ds - da + sMeshLevel0;
                            noflevel[i + ds - da][1] = i + aMeshLevel0;
                        }
                    }
                    else
                    {
                        for (i = 0; i < da - ds; i++)
                        {
                            noflevel[i][0] = sMeshLevel0;
                            noflevel[i][1] = i + aMeshLevel0;
                        }
                        for (i = 0; i <= ds; i++)
                        {
                            noflevel[i + da - ds][0] = i + sMeshLevel0;
                            noflevel[i + da - ds][1] = i + da - ds + aMeshLevel0;
                        }
                    }
                    break;
                case 4: //MG2
                    for (i = 0; i <= level; i++)
                    { noflevel[i] = new int[2]; }
                    for (i = sMeshLevel0; i <= sMeshLevel; i++)
                    {
                        noflevel[i - sMeshLevel0][0] = i;
                        noflevel[i - sMeshLevel0][1] = aMeshLevel0;
                    }
                    for (i = aMeshLevel0 + 1; i <= aMeshLevel; i++)
                    {
                        noflevel[i - aMeshLevel0 + sMeshLevel - sMeshLevel0][0] = sMeshLevel;
                        noflevel[i - aMeshLevel0 + sMeshLevel - sMeshLevel0][1] = i;
                    }
                    break;
                case 5: //MG3
                    for (i = 0; i <= level; i++)
                    { noflevel[i] = new int[2]; }
                    for (i = aMeshLevel0; i <= aMeshLevel; i++)
                    {
                        noflevel[i - aMeshLevel0][0] = sMeshLevel0;
                        noflevel[i - aMeshLevel0][1] = i;
                    }
                    for (i = sMeshLevel0 + 1; i <= sMeshLevel; i++)
                    {
                        noflevel[i - sMeshLevel0 + aMeshLevel - aMeshLevel0][0] = i;
                        noflevel[i - sMeshLevel0 + aMeshLevel - aMeshLevel0][1] = aMeshLevel;
                    }
                    break;
                case 6: //MG4_a
                    for (i = 0; i <= level; i++)
                    { noflevel[i] = new int[2]; }
                    if (ds >= da)
                    {
                        for (i = 0; i <= ds - da; i++)
                        {
                            noflevel[i][0] = i + sMeshLevel0;
                            noflevel[i][1] = aMeshLevel0;
                        }
                        for (i = 1; i <= da; i++)
                        {
                            noflevel[ds - da + 2 * i - 1][0] = sMeshLevel0 + ds - da + i;
                            noflevel[ds - da + 2 * i - 1][1] = aMeshLevel0 + i - 1;
                            noflevel[ds - da + 2 * i][0] = sMeshLevel0 + ds - da + i;
                            noflevel[ds - da + 2 * i][1] = aMeshLevel0 + i;
                        }
                    }
                    else
                    {
                        for (i = 0; i <= da - ds; i++)
                        {
                            noflevel[i][0] = sMeshLevel0;
                            noflevel[i][1] = i + aMeshLevel0;
                        }
                        for (i = 1; i <= ds; i++)
                        {
                            noflevel[da - ds + 2 * i - 1][0] = sMeshLevel0 + i;
                            noflevel[da - ds + 2 * i - 1][1] = aMeshLevel0 + da - ds + i - 1;
                            noflevel[da - ds + 2 * i][0] = sMeshLevel0 + i;
                            noflevel[da - ds + 2 * i][1] = aMeshLevel0 + da - ds + i;
                        }
                    }
                    break;
                case 7:  //MG4_s
                    for (i = 0; i <= level; i++)
                    { noflevel[i] = new int[2]; }
                    if (ds >= da)
                    {
                        for (i = 0; i <= ds - da; i++)
                        {
                            noflevel[i][0] = i + sMeshLevel0;
                            noflevel[i][1] = aMeshLevel0;
                        }
                        for (i = 1; i <= da; i++)
                        {
                            noflevel[ds - da + 2 * i - 1][0] = sMeshLevel0 + ds - da + i - 1;
                            noflevel[ds - da + 2 * i - 1][1] = aMeshLevel0 + i;
                            noflevel[ds - da + 2 * i][0] = sMeshLevel0 + ds - da + i;
                            noflevel[ds - da + 2 * i][1] = aMeshLevel0 + i;
                        }
                    }
                    else
                    {
                        for (i = 0; i <= da - ds; i++)
                        {
                            noflevel[i][0] = sMeshLevel0;
                            noflevel[i][1] = i + aMeshLevel0;
                        }
                        for (i = 1; i <= ds; i++)
                        {
                            noflevel[da - ds + 2 * i - 1][0] = sMeshLevel0 + i - 1;
                            noflevel[da - ds + 2 * i - 1][1] = aMeshLevel0 + da - ds + i;
                            noflevel[da - ds + 2 * i][0] = sMeshLevel0 + i;
                            noflevel[da - ds + 2 * i][1] = aMeshLevel0 + da - ds + i;
                        }
                    }
                    break;
            }

            // 3.2. malloc "ua", "us", "flux", "RHS", "d" and "q"
            //      ua[level][nt][2]:absorption coefficient
            //      us[level][nt][2]:scattering coefficient
            //      flux[level][ns][nt][2]: photon flux
            //      RHS[level][ns][nt][2]: source term
            //      d[level][ns][nt][2]: residual
            //      q[level][2][ns]: boundary source
            {


                for (i = sMeshLevel - 1; i >= 0; i--)
                {
                    ua[i] = new double[smesh[i].Nt][];
                    us[i] = new double[smesh[i].Nt][];
                    for (j = 0; j < smesh[i].Nt; j++)
                    {
                        ua[i][j] = new double[3];
                        us[i][j] = new double[3];
                    }
                    Mgrid.FtoC_s2(smesh[i].Nt, ua[i + 1], ua[i], smesh[i + 1].Smap, smesh[i + 1].Fc);
                    Mgrid.FtoC_s2(smesh[i].Nt, us[i + 1], us[i], smesh[i + 1].Smap, smesh[i + 1].Fc);
                }


                for (n = 0; n <= level; n++)
                {
                    nt = smesh[noflevel[n][0]].Nt;
                    ns = amesh[noflevel[n][1]].Ns;
                    RHS[n] = new double[ns][][];
                    for (i = 0; i < ns; i++)
                    {
                        RHS[n][i] = new double[nt][];
                        for (j = 0; j < nt; j++)
                        {
                            RHS[n][i][j] = new double[3];
                            for (k = 0; k < 3; k++)
                            {
                                RHS[n][i][j][k] = 0;
                            }
                        }
                    }
                }

                for (n = 0; n <= level; n++)
                {
                    nt = smesh[noflevel[n][0]].Nt;
                    ns = amesh[noflevel[n][1]].Ns;
                    d[n] = new double[ns][][];
                    for (i = 0; i < ns; i++)
                    {
                        d[n][i] = new double[nt][];
                        for (j = 0; j < nt; j++)
                        {
                            d[n][i][j] = new double[3];
                            for (k = 0; k < 3; k++)
                            {
                                d[n][i][j][k] = 0;
                            }
                        }
                    }
                }
                for (n = 0; n <= level; n++)
                {
                    nt = smesh[noflevel[n][0]].Nt;
                    ns = amesh[noflevel[n][1]].Ns;
                    flux[n] = new double[ns][][];
                    for (i = 0; i < ns; i++)
                    {
                        flux[n][i] = new double[nt][];
                        for (j = 0; j < nt; j++)
                        {
                            flux[n][i][j] = new double[3];
                            for (k = 0; k < 3; k++)
                            {
                                flux[n][i][j][k] = 0;
                            }
                        }
                    }
                }

                for (n = 0; n <= level; n++)
                {
                    ne = smesh[noflevel[n][0]].Ne;
                    ns = amesh[noflevel[n][1]].Ns;
                    q[n] = new double[ns][][];
                    for (i = 0; i < ns; i++)
                    {
                        q[n][i] = new double[ne][];
                        for (j = 0; j < ne; j++)
                        {
                            q[n][i][j] = new double[2];
                            for (k = 0; k < 2; k++)
                            { q[n][i][j][k] = 0; }
                        }
                    }
                }
            }


            // 3.3. compute "b"
            //      For the data structure of "b", see "boundarycoupling".
            if (vacuum == 0)// we need "b" only in the presence of refraction index mismatch at the domain boundary.
            {
                for (i = 0; i <= level; i++)
                {
                    ne = smesh[noflevel[i][0]].Ne;
                    ns = amesh[noflevel[i][1]].Ns;

                    b[i].Ri = new int[ne][][];
                    for (j = 0; j < ne; j++)
                    {
                        b[i].Ri[j] = new int[ns][];
                        for (k = 0; k < ns; k++)
                            b[i].Ri[j][k] = new int[2];
                    }

                    b[i].Si = new int[ne][][];
                    for (j = 0; j < ne; j++)
                    {
                        b[i].Si[j] = new int[ns][];
                        for (k = 0; k < ns; k++)
                            b[i].Si[j][k] = new int[2];
                    }

                    b[i].Ro = new int[ne][][];
                    for (j = 0; j < ne; j++)
                    {
                        b[i].Ro[j] = new int[ns][];
                        for (k = 0; k < ns; k++)
                            b[i].Ro[j][k] = new int[2];
                    }


                    b[i].So = new int[ne][][];
                    for (j = 0; j < ne; j++)
                    {
                        b[i].So[j] = new int[ns][];
                        for (k = 0; k < ns; k++)
                            b[i].So[j][k] = new int[2];
                    }

                    b[i].Ri2 = new double[ne][][];
                    for (j = 0; j < ne; j++)
                    {
                        b[i].Ri2[j] = new double[ns][];
                        for (k = 0; k < ns; k++)
                            b[i].Ri2[j][k] = new double[2];
                    }

                    b[i].Ro2 = new double[ne][][];
                    for (j = 0; j < ne; j++)
                    {
                        b[i].Ro2[j] = new double[ns][];
                        for (k = 0; k < ns; k++)
                            b[i].Ro2[j][k] = new double[2];
                    }

                    b[i].Si2 = new double[ne][][];
                    for (j = 0; j < ne; j++)
                    {
                        b[i].Si2[j] = new double[ns][];
                        for (k = 0; k < ns; k++)
                            b[i].Si2[j][k] = new double[2];
                    }

                    b[i].So2 = new double[ne][][];
                    for (j = 0; j < ne; j++)
                    {
                        b[i].So2[j] = new double[ns][];
                        for (k = 0; k < ns; k++)
                            b[i].So2[j][k] = new double[2];
                    }

                    Mgrid.BoundReflection(ns, amesh[noflevel[i][1]].Ang, smesh[noflevel[i][0]], nTissue, nExt, b[i]);
                }
            }
        stop: ;
        }
    }
}