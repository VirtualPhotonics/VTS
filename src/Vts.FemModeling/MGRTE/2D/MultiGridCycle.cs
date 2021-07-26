using System;
using Vts.FemModeling.MGRTE._2D.DataStructures;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// This class contains numerical algorithms required to solve MG-RTE. 
    /// </summary>
    public class MultiGridCycle
    {

        private double Pi = GlobalConstants.Pi;

        public void Boundary(int ne, int nt, int[][] t, int[][] p2, double[][] p, int[][] e, int[][] e2, int[][] so2, double[][] n, int[] ori)
        // Purpose: this function is to compute "e", "e2", "so2", "n", "sn" and "ori" with the data structure as follow:
        //
        //          e[ne][4]:   for the 2nd index,1st is the corresponding triangle containing boundary,
        //                      2nd and 3rd are global nodal index of boundary nodes, and 4th
        //                      is global index of the other non-boundary node in the triangle.
        //
        //          e2[ne][2]:  the local order of the boundary nodes in the corresponding triangle with
        //                      1st entry for node 1 (e[ne][1]) and 2nd entry for node 2 (e[ne][2]).
        //
        //          so2[nt][3]: the corresponding triangle and edge of the boundary
        //                      Example:    if boundary "10" is edge "2" of triangle "20",
        //                                  then so2[20][2]=10, and "-1" otherwise.
        //
        //          n[ne][2]:   x and y coordinate of the outgoing normal.
        //
        //          ori[ne]:    orientation of the boundary edge w.r.t. the triangle, which is useful at ONLY one place in "bc_reflection".
        {
            int i, j, k, tri;
            double nx, ny, temp;

            for (i = 0; i < nt; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    so2[i][j] = -1;
                }
            }
            for (i = 0; i < ne; i++)
            {
                tri = FindTri2(p2[e[i][1]], p2[e[i][2]]);
                e[i][0] = tri;

                // find the local index for boundary nodes in the corresponding triangle
                for (j = 0; j < 2; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        if (e[i][j + 1] == t[tri][k])
                        {
                            e2[i][j] = k; goto stop;
                        }
                    }
                stop: ;
                }
                // find the non-boundary node and the boundary index in the corresponding triangle
                for (j = 0; j < 3; j++)
                {
                    if (t[tri][j] != e[i][1] && t[tri][j] != e[i][2])
                    {
                        e[i][3] = t[tri][j];    //non-edge node
                        so2[tri][j] = i;        //boundary index
                        goto stop2;
                    }
                }
            stop2: ;

                // The following is to compute outgoing vector "n" on the boundary
                nx = -(p[e[i][1]][1] - p[e[i][2]][1]);
                ny = p[e[i][1]][0] - p[e[i][2]][0];
                ori[i] = 1;
                if (nx * (p[e[i][3]][0] - p[e[i][2]][0]) + ny * (p[e[i][3]][1] - p[e[i][2]][1]) > 0)// the normal is incoming.
                {
                    nx = -nx; ny = -ny; ori[i] = 0;
                }
                temp = Math.Sqrt(nx * nx + ny * ny);
                n[i][0] = nx / temp; n[i][1] = ny / temp;
            }
        }


        public void BoundReflection(int ns, double[][] theta, SpatialMesh smesh, double nTissue, double nExt, BoundaryCoupling b)
        // Purpose: this fucntion is to find the coupling relation between directions on the boundary
        //          due to reflection and refraction in the presence of refraction index mismatch at the boundary.
        //          For the data structure of "b", see "struct boundarycoupling" in "solver".
        {
            int i, j;
            int[] sign = new int[2] { -1, 1 };
            double dx, dy, sn, dtheta = 2 * Pi / ns, ratio_reflection, ratio_refraction;
            double[] temp = new double[2];
            double theta0, theta_i, theta_m, theta_m2;

            for (i = 0; i < smesh.Ne; i++)
            {
                dx = smesh.P[smesh.E[i][1]][0] - smesh.P[smesh.E[i][2]][0];
                dy = smesh.P[smesh.E[i][1]][1] - smesh.P[smesh.E[i][2]][1];
                if (smesh.Ori[i] == 0)
                {
                    dx = -dx; dy = -dy; // to make sure that (dx,dy) goes clockwisely.
                }
                for (j = 0; j < ns; j++)
                {
                    sn = theta[j][0] * smesh.N[i][0] + theta[j][1] * smesh.N[i][1];     // "s" dot "n" for the angle "s"
                    if (sn < 0)
                    {
                        theta0 = Pi - Math.Acos(sn);
                        ratio_reflection = Reflection(theta0, nTissue, nExt);
                        Refraction(temp, theta0, nExt, nTissue);
                        ratio_refraction = temp[0]; theta_i = temp[1];

                        if (theta[j][0] * dx + theta[j][1] * dy > 0)        // the ONLY place for clockwise (dx,dy)
                        {
                            theta_m = Mod2pi(theta[j][2] + (Pi - 2 * theta0));
                            theta_m2 = Mod2pi(theta[j][2] + (theta_i - theta0));
                        }
                        else
                        {
                            theta_m = Mod2pi(theta[j][2] - (Pi - 2 * theta0));
                            theta_m2 = Mod2pi(theta[j][2] - (theta_i - theta0));
                        }
                        // contribution to incoming flux through internal reflection
                        MathFunctions.Intepolation_a(theta_m, dtheta, ns, b.Ri[i][j], b.Ri2[i][j], ratio_reflection);
                        // contribution from boundary source to incoming flux through refraction
                        MathFunctions.Intepolation_a(theta_m2, dtheta, ns, b.Si[i][j], b.Si2[i][j], ratio_refraction);
                    }
                    else
                    {
                        theta0 = Math.Acos(sn);
                        ratio_reflection = Reflection(theta0, nExt, nTissue);
                        Refraction(temp, theta0, nTissue, nExt);
                        ratio_refraction = temp[0]; theta_i = temp[1];

                        if (theta[j][0] * dx + theta[j][1] * dy > 0)    // the ONLY place for clockwise (dx,dy)
                        {
                            theta_m = Mod2pi(theta[j][2] - (Pi - 2 * theta0));
                            theta_m2 = Mod2pi(theta[j][2] - (theta_i - theta0));
                        }
                        else
                        {
                            theta_m = Mod2pi(theta[j][2] + (Pi - 2 * theta0));
                            theta_m2 = Mod2pi(theta[j][2] + (theta_i - theta0));
                        }
                        // contribution from boundary source to outgoing flux through reflection
                        MathFunctions.Intepolation_a(theta_m, dtheta, ns, b.Ro[i][j], b.Ro2[i][j], ratio_reflection);
                        // contribution to outgoing flux after refraction
                        MathFunctions.Intepolation_a(theta_m2, dtheta, ns, b.So[i][j], b.So2[i][j], ratio_refraction);
                    }
                }
            }
        }


        public void CtoF(int nt_c, int ns_c, double[][][] flux, double[][][] dcflux, int[][] smap, double[][][][] cf)
        // Purpose: this function describes coarse-to-fine operator simultaneouly in angle and space,
        //          i.e., the combination of "ctof_a" and "ctof_s".
        {
            int i, j, k, m, n;
            for (i = 0; i < ns_c - 1; i++)
            {
                for (j = 0; j < nt_c; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        for (m = 1; m <= smap[j][0]; m++)
                        {
                            for (n = 0; n < 3; n++)
                            {
                                flux[2 * i][smap[j][m]][n] += dcflux[i][j][k] * cf[j][k][m - 1][n];
                                flux[2 * i + 1][smap[j][m]][n] += 0.5 * (dcflux[i][j][k] + dcflux[i + 1][j][k]) * cf[j][k][m - 1][n];
                            }
                        }
                    }
                }
            }
            i = ns_c - 1;
            for (j = 0; j < nt_c; j++)
            {
                for (k = 0; k < 3; k++)
                {
                    for (m = 1; m <= smap[j][0]; m++)
                    {
                        for (n = 0; n < 3; n++)
                        {
                            flux[2 * i][smap[j][m]][n] += dcflux[i][j][k] * cf[j][k][m - 1][n];
                            flux[2 * i + 1][smap[j][m]][n] += 0.5 * (dcflux[i][j][k] + dcflux[0][j][k]) * cf[j][k][m - 1][n];
                        }
                    }
                }
            }
        }


        public void CtoF_a(int nt, int ns_c, double[][][] flux, double[][][] dcflux)
        // Purpose: this function describes angular coarse-to-fine operator by linear interpolation.
        {
            int i, j, k;
            for (i = 0; i < nt; i++)
            {
                for (j = 0; j < ns_c - 1; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        flux[2 * j][i][k] += dcflux[j][i][k];
                        flux[2 * j + 1][i][k] += (dcflux[j][i][k] + dcflux[j + 1][i][k]) / 2;
                    }
                }
                j = ns_c - 1;
                for (k = 0; k < 3; k++)
                {
                    flux[2 * j][i][k] += dcflux[j][i][k];
                    flux[2 * j + 1][i][k] += (dcflux[j][i][k] + dcflux[0][i][k]) / 2;
                }
            }
        }


        public void CtoF_s(int nt_c, int ns, double[][][] flux, double[][][] dcflux, int[][] smap, double[][][][] cf)
        // Purpose: this function describes spatial coarse-to-fine operator by linear interpolation for piecewise linear DG.
        {
            int i, j, k, m, n;
            for (i = 0; i < ns; i++)
            {
                for (j = 0; j < nt_c; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        for (m = 1; m <= smap[j][0]; m++)
                        {
                            for (n = 0; n < 3; n++)
                            {
                                flux[i][smap[j][m]][n] += dcflux[i][j][k] * cf[j][k][m - 1][n];
                            }
                        }
                    }
                }
            }
        }


        private void CtoF_s2(int nt_c, double[][] f, double[][] cf2, int[][] smap, double[][][][] cf)
        // Purpose: this function describes spatial coarse-to-fine operator by linear interpolation for piecewise linear DG.
        {
            int j, k, m, n;
            for (j = 0; j < nt_c; j++)
            {
                for (k = 0; k < 3; k++)
                {
                    for (m = 1; m <= smap[j][0]; m++)
                    {
                        for (n = 0; n < 3; n++)
                        {
                            f[smap[j][m]][n] = cf2[j][k] * cf[j][k][m - 1][n];
                        }
                    }
                }
            }
        }


        public void Defect(AngularMesh amesh, SpatialMesh smesh, int Ns, double[][][] RHS, double[][] ua, double[][] us, double[][][] flux,
        BoundaryCoupling bb, double[][][] q, double[][][] res, int vacuum)

        // Purpose: this function is to compute the residual with vacuum or reflection boundary condition.
        //          see "relaxation" for more details.
        {
            int i, j, k, m, ii, jj, bi, aMeshLevel, edge = 0;

            double[,] left = new double[3, 3];
            double[] right = new double[3];
            double[] temp = new double[3];
            double tempd;
            double dettri, cosi, sini, a, b, c, source_corr;
            double[,] bv = new double[3, 3] { { 1.0 / 3, 1.0 / 6, 1.0 / 6 }, { 1.0 / 6, 1.0 / 3, 1.0 / 6 }, { 1.0 / 6, 1.0 / 6, 1.0 / 3 } };
            double[,] matrix1 = new double[3, 3];
            double[,] matrix2 = new double[3, 3];
            int[,] index = new int[3, 2];


            index[0, 0] = 1; index[0, 1] = 2;
            index[1, 0] = 2; index[1, 1] = 0;
            index[2, 0] = 0; index[2, 1] = 1;

            aMeshLevel = Ns / amesh.Ns;

            if (vacuum == 0)
            {
                for (i = 0; i < amesh.Ns; i++)
                {
                    bi = i * aMeshLevel;
                    for (j = 0; j < smesh.Nt; j++)
                    {
                        dettri = 2 * smesh.A[j];
                        cosi = amesh.Ang[i][0]; sini = amesh.Ang[i][1];
                        a = cosi * (smesh.P[smesh.T[j][2]][1] - smesh.P[smesh.T[j][0]][1]) + sini * (smesh.P[smesh.T[j][0]][0] - smesh.P[smesh.T[j][2]][0]);
                        b = cosi * (smesh.P[smesh.T[j][0]][1] - smesh.P[smesh.T[j][1]][1]) + sini * (smesh.P[smesh.T[j][1]][0] - smesh.P[smesh.T[j][0]][0]);
                        MatrixConvec(a, b, matrix1);

                        a = ua[j][0] + us[j][0];
                        b = ua[j][1] + us[j][1];
                        c = ua[j][2] + us[j][2];
                        MatrixAbsorb(a, b, c, dettri, matrix2);

                        for (ii = 0; ii < 3; ii++)
                        {
                            for (jj = 0; jj < 3; jj++)
                            { left[ii, jj] = matrix1[ii, jj] + matrix2[ii, jj]; }
                        }

                        for (ii = 0; ii < 3; ii++)
                        {
                            temp[ii] = 0;
                            for (k = 0; k < amesh.Ns; k++)
                            { temp[ii] += amesh.W[i][k][smesh.Region[smesh.T[j][ii]]] * flux[k][j][ii]; }
                        }

                        source_corr = smesh.A[j] / 12;
                        SourceAssign(us[j], temp, right, RHS[i][j], dettri, source_corr);

                        for (k = 0; k < 3; k++)
                        {
                            if (smesh.Bd2[bi][j][k] > 0)
                            {
                                for (ii = 0; ii < 2; ii++)
                                {
                                    for (jj = 0; jj < 2; jj++)
                                    { left[index[k, ii], index[k, jj]] += smesh.Bd2[bi][j][k] * bv[index[k, ii], index[k, jj]]; }
                                }
                            }
                            else if (smesh.Bd2[bi][j][k] < 0)
                            {
                                if (smesh.So2[j][k] > -1)// upwind flux from the internal reflection and boundary source
                                {
                                    edge = smesh.So2[j][k];

                                    temp[0] = 0;
                                    temp[1] = 0;
                                    for (m = 0; m < 2; m++)
                                    {
                                        for (ii = 0; ii < 2; ii++)
                                        {
                                            temp[ii] += flux[bb.Ri[edge][i][m]][j][smesh.E2[edge][ii]] * bb.Ri2[edge][i][m];
                                        }
                                        for (ii = 0; ii < 2; ii++)
                                        {
                                            temp[ii] += q[bb.Si[edge][i][m]][edge][ii] * bb.Si2[edge][i][m];
                                        }
                                    }

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += temp[jj] * bv[smesh.E2[edge][ii], smesh.E2[edge][jj]];
                                        }
                                        right[smesh.E2[edge][ii]] += -smesh.Bd2[bi][j][k] * tempd;
                                    }
                                }
                                else // upwind flux from the adjacent triangle
                                {
                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += flux[i][smesh.Bd[bi][j][3 * k]][smesh.Bd[bi][j][3 * k + jj + 1]] * bv[index[k, ii], index[k, jj]];
                                        }
                                        right[index[k, ii]] += -smesh.Bd2[bi][j][k] * tempd;
                                    }
                                }
                            }
                        }
                        // compute "right-left*flux" and then find the corresponding nodal values of "res".
                        tempd = 0;
                        for (ii = 0; ii < 3; ii++)
                        {
                            temp[ii] = (right[ii] - left[ii, 0] * flux[i][j][0] - left[ii, 1] * flux[i][j][1] - left[ii, 2] * flux[i][j][2]) / source_corr;
                            tempd += temp[ii];
                        }
                        tempd = tempd / 4;
                        for (ii = 0; ii < 3; ii++)
                        { res[i][j][ii] = temp[ii] - tempd; }
                    }
                }
            }
            else
            {
                for (i = 0; i < amesh.Ns; i++)
                {
                    bi = i * aMeshLevel;
                    for (j = 0; j < smesh.Nt; j++)
                    {
                        dettri = 2 * smesh.A[j];
                        cosi = amesh.Ang[i][0]; sini = amesh.Ang[i][1];
                        a = cosi * (smesh.P[smesh.T[j][2]][1] - smesh.P[smesh.T[j][0]][1]) + sini * (smesh.P[smesh.T[j][0]][0] - smesh.P[smesh.T[j][2]][0]);
                        b = cosi * (smesh.P[smesh.T[j][0]][1] - smesh.P[smesh.T[j][1]][1]) + sini * (smesh.P[smesh.T[j][1]][0] - smesh.P[smesh.T[j][0]][0]);
                        MatrixConvec(a, b, matrix1);

                        a = ua[j][0] + us[j][0];
                        b = ua[j][1] + us[j][1];
                        c = ua[j][2] + us[j][2];
                        MatrixAbsorb(a, b, c, dettri, matrix2);

                        for (ii = 0; ii < 3; ii++)
                        {
                            for (jj = 0; jj < 3; jj++)
                            { left[ii, jj] = matrix1[ii, jj] + matrix2[ii, jj]; }
                        }

                        for (ii = 0; ii < 3; ii++)
                        {
                            temp[ii] = 0;
                            for (k = 0; k < amesh.Ns; k++)
                            { temp[ii] += amesh.W[i][k][smesh.Region[smesh.T[j][ii]]] * flux[k][j][ii]; }
                        }

                        source_corr = smesh.A[j] / 12;
                        SourceAssign(us[j], temp, right, RHS[i][j], dettri, source_corr);


                        for (k = 0; k < 3; k++)
                        {
                            if (smesh.Bd2[bi][j][k] > 0)
                            {
                                for (ii = 0; ii < 2; ii++)
                                {
                                    for (jj = 0; jj < 2; jj++)
                                    { left[index[k, ii], index[k, jj]] += smesh.Bd2[bi][j][k] * bv[index[k, ii], index[k, jj]]; }
                                }
                            }
                            else if (smesh.Bd2[bi][j][k] < 0)
                            {
                                if (smesh.So2[j][k] > -1)
                                // "tri" is at the domain boundary: upwind flux is from boundary source only.
                                {
                                    edge = smesh.So2[j][k];

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += q[i][edge][jj] * bv[smesh.E2[edge][ii], smesh.E2[edge][jj]];
                                        }
                                        right[smesh.E2[edge][ii]] += -smesh.Bd2[bi][j][k] * tempd;
                                    }
                                }
                                else
                                {
                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += flux[i][smesh.Bd[bi][j][3 * k]][smesh.Bd[bi][j][3 * k + jj + 1]] * bv[index[k, ii], index[k, jj]];
                                        }
                                        right[index[k, ii]] += -smesh.Bd2[bi][j][k] * tempd;
                                    }
                                }
                            }
                        }
                        // compute "right-left*flux" and then find the corresponding nodal values of "res".
                        tempd = 0;
                        for (ii = 0; ii < 3; ii++)
                        {
                            temp[ii] = (right[ii] - left[ii, 0] * flux[i][j][0] - left[ii, 1] * flux[i][j][1] - left[ii, 2] * flux[i][j][2]) / source_corr;
                            tempd += temp[ii];
                        }
                        tempd = tempd / 4;
                        for (ii = 0; ii < 3; ii++)
                        { res[i][j][ii] = temp[ii] - tempd; }
                    }
                }
            }
        }


        public void EdgeTri(int nt, double[] theta, double[][] p, int[][] p2, int[][] t, int[][] bd, double[][] bd2, int[][] so2)
        // Purpose: this function is to compute edge integrals for each triangle.
        //
        //          Let "n" be the normal of some edge of the triangle and "s" be the angular direction with
        //          the assumption for conforming mesh that one edge is shared by at most two triangles.
        //
        //          Case 1 (s dot n>0): the edge flux is outgoing from the triangle.
        //          Case 2 (s dot n<0):
        //              Case 2.1 (internal triangle): the edge flux is incoming from the adjacent triangle.
        //              Case 2.2 (boundary triangle): the edge flux is incoming from the boundary source.
        //
        //          For each angular direction "s" on each mesh "sMeshLevel", the edge integrals can be assembled from the following:
        //
        //          bd[nt][9]:  in Case 2.1, it saves the adjacent triangle of the current triangle, the local order of shared nodes
        //                      in the adjacent triangle in the order of edges: index "0" to "2" for edge "1" in the current triangle,
        //                      index "3" to "5" for edge "2" and index "6" to "8" for edge "3".
        //                      Example:    assume if the upwind flux for edge "2" of triangle "50" comes from triangle "10" with
        //                                  the local order "1" and "3", then bd[50][3]=10, bd[50][4]=1 and bd[50][5]=3.
        //                      in Case 2.2, the corresponding boundary source is found through the function "boundary".
        //
        //          bd2[nt][3]: the values of "s dot n * L (length of the edge)" by the order of edges of the current triangle.
        //
        // Note:    we define "23" as the 1st edge, "31" as the 2nd and "12" as the 3rd.
        {
            int i, j;
            double a = theta[0], b = theta[1]; // a=cos(theta), b=sin(theta)
            double x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0, dx, dy, convConvConvTol = 1e-6;
            for (i = 0; i < nt; i++)
            {
                x1 = p[t[i][0]][0]; y1 = p[t[i][0]][1];
                x2 = p[t[i][1]][0]; y2 = p[t[i][1]][1];
                x3 = p[t[i][2]][0]; y3 = p[t[i][2]][1];

                // 1st edge "23"
                dx = y3 - y2; dy = x2 - x3;
                if (dx * (x1 - x3) + dy * (y1 - y3) > 0) // to make sure the normal of edge "23" is outgoing w.r.t. the triangle.
                { dx = -dx; dy = -dy; }

                bd2[i][0] = a * dx + b * dy;// s dot n * L
                if (Math.Abs(bd2[i][0]) < convConvConvTol)
                { bd2[i][0] = 0; }
                if (bd2[i][0] < 0 && so2[i][0] == -1)
                // "bd2[i][n]<0 or s dot n <0" means that this edge has upwind flux from the other adjacent triangle or the boundary source.
                // "so2[i][0]==-1" indicates that this triangle is not at the boundary.
                {
                    bd[i][0] = FindTri(i, p2[t[i][1]], p2[t[i][2]]);// to find the adjacent triangle
                    if (bd[i][0] > -1)
                    {
                        for (j = 0; j < 3; j++)
                        {
                            if (t[bd[i][0]][j] == t[i][1])// to find the local order in the adjacent triangle of node "2" of the current triangle
                            { bd[i][1] = j; goto stop3; }
                        }
                    stop3: ;
                        for (j = 0; j < 3; j++)
                        {
                            if (t[bd[i][0]][j] == t[i][2])// to find the local order in the adjacent triangle of node "3" of the current triangle
                            { bd[i][2] = j; goto stop4; }
                        }
                    stop4: ;
                    }
                }

                // 2nd edge "31"
                dx = y1 - y3; dy = x3 - x1;
                if (dx * (x2 - x3) + dy * (y2 - y3) > 0)
                { dx = -dx; dy = -dy; }

                bd2[i][1] = a * dx + b * dy;
                if (Math.Abs(bd2[i][1]) < convConvConvTol)
                { bd2[i][1] = 0; }
                if (bd2[i][1] < 0 && so2[i][1] == -1)
                {
                    bd[i][3] = FindTri(i, p2[t[i][0]], p2[t[i][2]]);
                    if (bd[i][3] > -1)
                    {
                        for (j = 0; j < 3; j++)
                        {
                            if (t[bd[i][3]][j] == t[i][2])
                            { bd[i][4] = j; goto stop5; }
                        }
                    stop5: ;
                        for (j = 0; j < 3; j++)
                        {
                            if (t[bd[i][3]][j] == t[i][0])
                            { bd[i][5] = j; goto stop6; }
                        }
                    stop6: ;
                    }
                }

                // 3rd edge "12"
                dx = y2 - y1; dy = x1 - x2;
                if (dx * (x3 - x1) + dy * (y3 - y1) > 0)
                { dx = -dx; dy = -dy; }

                bd2[i][2] = a * dx + b * dy;
                if (Math.Abs(bd2[i][2]) < convConvConvTol)
                { bd2[i][2] = 0; }
                if (bd2[i][2] < 0 && so2[i][2] == -1)
                {
                    bd[i][6] = FindTri(i, p2[t[i][0]], p2[t[i][1]]);
                    if (bd[i][6] > -1)
                    {
                        for (j = 0; j < 3; j++)
                        {
                            if (t[bd[i][6]][j] == t[i][0])
                            { bd[i][7] = j; goto stop1; }
                        }
                    stop1: ;
                        for (j = 0; j < 3; j++)
                        {
                            if (t[bd[i][6]][j] == t[i][1])
                            { bd[i][8] = j; goto stop2; }
                        }
                    stop2: ;
                    }
                }
            }
        }





        private int FindTri(int tri, int[] pt1, int[] pt2)
        // Purpose: this function is find the adjacent triangle of the current triangle "tri"
        // Note: the edge can not be at the boundary of the domain.
        {
            int i, j, tri2 = -1, flag = 0;
            for (i = 1; i <= pt1[0]; i++)
            {
                for (j = 1; j <= pt2[0]; j++)
                {
                    if (pt1[i] == pt2[j])
                    {
                        if (pt1[i] != tri)
                        {
                            flag = 1;
                            tri2 = pt1[i];
                            goto stop;
                        }
                    }
                }
            }
            if (flag == 0)
            { Console.WriteLine("nonconforming mesh!!!\n"); }
        stop: return tri2;
        }


        private int FindTri2(int[] pt1, int[] pt2)
        // Purpose: this function is to find the triangle containing the boundary.
        // Note: "pt1" and "pt2" have to be boundary nodes of the same boundary.
        {
            int i, j, tri = -1;
            for (i = 1; i <= pt1[0]; i++)
            {
                for (j = 1; j <= pt2[0]; j++)
                {
                    if (pt1[i] == pt2[j])
                    {
                        tri = pt1[i];
                        goto stop;
                    }
                }
            }
        stop: return tri;
        }


        public void FtoC(int nt_c, int ns_c, double[][][] flux, double[][][] cflux, int[][] smap, double[][][][] fc)
        // Purpose: this function describes fine-to-coarse operator simultaneouly in angle and space,
        //          i.e., the combination of "ftoc_a" and "ftoc_s".
        {
            int i, j, k, m, n;
            for (i = 0; i < ns_c; i++)
            {
                for (j = 0; j < nt_c; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        cflux[i][j][k] = 0;
                        for (m = 1; m <= smap[j][0]; m++)
                        {
                            for (n = 0; n < 3; n++)
                            {
                                cflux[i][j][k] += flux[2 * i][smap[j][m]][n] * fc[j][k][m - 1][n];
                            }
                        }
                    }
                }
            }
        }


        public void FtoC_a(int nt, int ns_c, double[][][] flux, double[][][] cflux)
        // Purpose: this function describes angular fine-to-coarse operator by simple restriction.
        {
            int i, j, k;
            for (i = 0; i < nt; i++)
            {
                for (j = 0; j < ns_c; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        cflux[j][i][k] = flux[2 * j][i][k];

                    }
                }
            }
        }


        public void FtoC_s(int nt_c, int ns, double[][][] flux, double[][][] cflux, int[][] smap, double[][][][] fc)
        // Purpose: this function describes spatial fine-to-coarse operator by L2 projection for piecewise linear DG.
        {
            int i, j, k, m, n;
            for (i = 0; i < ns; i++)
            {
                for (j = 0; j < nt_c; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        cflux[i][j][k] = 0;
                        for (m = 1; m <= smap[j][0]; m++)
                        {
                            for (n = 0; n < 3; n++)
                            {
                                cflux[i][j][k] += flux[i][smap[j][m]][n] * fc[j][k][m - 1][n];

                            }
                        }
                    }
                }
            }
        }


        public void FtoC_s2(int nt_c, double[][] f, double[][] cf, int[][] smap, double[][][][] fc)
        // Purpose: this function describes spatial fine-to-coarse operator by L2 projection for piecewise linear DG.
        {
            int j, k, m, n;
            for (j = 0; j < nt_c; j++)
            {
                for (k = 0; k < 3; k++)
                {
                    cf[j][k] = 0;
                    for (m = 1; m <= smap[j][0]; m++)
                    {
                        for (n = 0; n < 3; n++)
                        {
                            cf[j][k] += f[smap[j][m]][n] * fc[j][k][m - 1][n];
                        }
                    }
                }
            }
        }


        private void MatrixAbsorb(double a, double b, double c, double dettri, double[,] matrix)
        //Purpose of this matrix to assign absorption terms to the matrix
        {
            matrix[0, 0] = dettri / 60 * (3 * a + b + c);
            matrix[0, 1] = dettri / 120 * (2 * a + 2 * b + c);
            matrix[0, 2] = dettri / 120 * (2 * a + b + 2 * c);
            matrix[1, 0] = matrix[0, 1];
            matrix[1, 1] = dettri / 60 * (a + 3 * b + c);
            matrix[1, 2] = dettri / 120 * (a + 2 * b + 2 * c);
            matrix[2, 0] = matrix[0, 2];
            matrix[2, 1] = matrix[1, 2];
            matrix[2, 2] = dettri / 60 * (a + b + 3 * c);
        }


        private void MatrixConvec(double a, double b, double[,] matrix)
        //Purpose of this matrix to assign convection terms to the matrix
        {
            matrix[0, 0] = (a + b) / 6;
            matrix[0, 1] = matrix[0, 0];
            matrix[0, 2] = matrix[0, 0];
            matrix[1, 0] = -a / 6;
            matrix[1, 1] = matrix[1, 0];
            matrix[1, 2] = matrix[1, 0];
            matrix[2, 0] = -b / 6;
            matrix[2, 1] = matrix[2, 0];
            matrix[2, 2] = matrix[2, 0];
        }


        private void MatrixSolver(double[,] A, double[] B, double[] sol)
        // Purpose: this function is to solve a 3 by 3 nonsingular linear system by Gaussian elimination with pivoting.
        {
            int[] ind = new int[3] { 0, 1, 2 };
            int i, j, temp2;
            double temp;

            if (A[0, 0] == 0)
            {
                if (A[1, 0] == 0)
                {
                    ind[0] = 2; ind[2] = 0;
                }
                else
                {
                    ind[0] = 1; ind[1] = 0;
                }
            }

            temp = A[ind[0], 0];
            B[ind[0]] = B[ind[0]] / temp;
            for (j = 0; j < 3; j++)
            { A[ind[0], j] = A[ind[0], j] / temp; }

            for (i = 1; i < 3; i++)
            {
                if (A[ind[i], 0] == 0)
                { }
                else
                {
                    temp = A[ind[i], 0];
                    B[ind[i]] = B[ind[i]] / temp - B[ind[0]];
                    for (j = 0; j < 3; j++)
                    {
                        A[ind[i], j] = A[ind[i], j] / temp - A[ind[0], j];
                    }
                }
            }

            if (A[ind[1], 1] == 0.0)
            { temp2 = ind[1]; ind[1] = ind[2]; ind[2] = temp2; }
            temp = A[ind[1], 1];
            B[ind[1]] = B[ind[1]] / temp;
            for (j = 1; j < 3; j++)
            { A[ind[1], j] = A[ind[1], j] / temp; }

            if (A[ind[2], 1] == 0.0)
            { }
            else
            {
                temp = A[ind[2], 1];
                B[ind[2]] = B[ind[2]] / temp - B[ind[1]];
                for (j = 1; j < 3; j++)
                {
                    A[ind[2], j] = A[ind[2], j] / temp - A[ind[1], j];
                }
            }

            sol[2] = B[ind[2]] / A[ind[2], 2];
            sol[1] = (B[ind[1]] - sol[2] * A[ind[1], 2]) / A[ind[1], 1];
            sol[0] = (B[ind[0]] - sol[2] * A[ind[0], 2] - sol[1] * A[ind[0], 1]) / A[ind[0], 0];
        }


        public double MgCycle(AngularMesh[] amesh, SpatialMesh[] smesh, BoundaryCoupling[] b, double[][][][] q,
            double[][][][] RHS, double[][][] ua, double[][][] us, double[][][][] flux, double[][][][] d,
            int n1, int n2, int aMeshLevel, int aMeshLevel0, int sMeshLevel, int sMeshLevel0, int NS, int vacuum, int mgMethod)

        // Purpose: this function contains the multigrid methods with V-cycle.
        //     AMG: angular multigrid method.
        //     SMG: spatial multigrid method.
        //     MG1: simultaneou angular and spatial multigrid method.
        //     MG2: angular multigrid first, then spatial multigrid.
        //     MG3: spatial multigrid first, then angular multigrid.
        //     MG4: alternative angular and spatial multigrid.
        //          MG4_a: angular multigrid first; 
        //          MG4_s: spatial multigrid first.
        {
            int i, ns, nt, da, ds, level = -1;
            double res = 1e10;

            nt = smesh[sMeshLevel].Nt;
            ns = amesh[aMeshLevel].Ns;
            ds = sMeshLevel - sMeshLevel0;
            da = aMeshLevel - aMeshLevel0;

            switch (mgMethod)
            {
                case 1: //AMG:
                    level = da;
                    break;
                case 2: //SMG:
                    level = ds;
                    break;
                case 3: //MG1:
                    level = Math.Max(da, ds);
                    break;
                case 4: //MG2:
                    level = ds + da;
                    break;
                case 5: //MG3:
                    level = ds + da;
                    break;
                case 6: //MG4_a:
                    level = ds + da;
                    break;
                case 7: //MG4_s:
                    level = ds + da;
                    break;
            }

            for (i = 0; i < n1; i++)
            {
                Relaxation(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], vacuum);
            }

            switch (mgMethod)
            {
                case 1://AMG:
                    {
                        if (aMeshLevel == aMeshLevel0)
                        { }
                        else
                        {
                            Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                            res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                            FtoC_a(nt, amesh[aMeshLevel - 1].Ns, d[level], RHS[level - 1]);
                            MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel, aMeshLevel0, sMeshLevel - 1, sMeshLevel0, NS, vacuum, mgMethod);
                            CtoF_a(nt, amesh[aMeshLevel - 1].Ns, flux[level], flux[level - 1]);
                        }
                    }
                    break;
                case 2://SMG:
                    {
                        if (sMeshLevel == sMeshLevel0)
                        { }
                        else
                        {
                            Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                            res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                            FtoC_s(smesh[sMeshLevel - 1].Nt, ns, d[level], RHS[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Fc);
                            MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel - 1, aMeshLevel0, sMeshLevel, sMeshLevel0, NS, vacuum, mgMethod);
                            CtoF_s(smesh[sMeshLevel - 1].Nt, ns, flux[level], flux[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Cf);
                        }
                    }
                    break;
                case 3://MG1:
                    {
                        if (aMeshLevel == aMeshLevel0)
                        {
                            if (sMeshLevel == sMeshLevel0)
                            { }
                            else
                            {
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_s(smesh[sMeshLevel - 1].Nt, ns, d[level], RHS[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel, aMeshLevel0, sMeshLevel - 1, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_s(smesh[sMeshLevel - 1].Nt, ns, flux[level], flux[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Cf);
                            }
                        }
                        else
                        {
                            if (sMeshLevel == sMeshLevel0)
                            {
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_a(nt, amesh[aMeshLevel - 1].Ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel - 1, aMeshLevel0, sMeshLevel, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_a(nt, amesh[aMeshLevel - 1].Ns, flux[level], flux[level - 1]);
                            }
                            else
                            {
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC(smesh[sMeshLevel - 1].Nt, amesh[aMeshLevel - 1].Ns, d[level], RHS[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel - 1, aMeshLevel0, sMeshLevel, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF(smesh[sMeshLevel - 1].Nt, amesh[aMeshLevel - 1].Ns, flux[level], flux[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Cf);
                            }
                        }
                    }
                    break;
                case 4://MG2:
                    {
                        if (aMeshLevel == aMeshLevel0)
                        {
                            if (sMeshLevel == sMeshLevel0)
                            { }
                            else
                            {
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_s(smesh[sMeshLevel - 1].Nt, ns, d[level], RHS[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel, aMeshLevel0, sMeshLevel - 1, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_s(smesh[sMeshLevel - 1].Nt, ns, flux[level], flux[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Cf);
                            }
                        }
                        else
                        {
                            Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                            res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                            FtoC_a(nt, amesh[aMeshLevel - 1].Ns, d[level], RHS[level - 1]);
                            MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel - 1, aMeshLevel0, sMeshLevel, sMeshLevel0, NS, vacuum, mgMethod);
                            CtoF_a(nt, amesh[aMeshLevel - 1].Ns, flux[level], flux[level - 1]);
                        }
                    }
                    break;
                case 5://MG3:
                    {
                        if (sMeshLevel == sMeshLevel0)
                        {
                            if (aMeshLevel == aMeshLevel0)
                            { }
                            else
                            {
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_a(nt, amesh[aMeshLevel - 1].Ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel - 1, aMeshLevel0, sMeshLevel, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_a(nt, amesh[aMeshLevel - 1].Ns, flux[level], flux[level - 1]);
                            }
                        }
                        else
                        {
                            Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                            res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                            FtoC_s(smesh[sMeshLevel - 1].Nt, ns, d[level], RHS[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Fc);
                            MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel, aMeshLevel0, sMeshLevel - 1, sMeshLevel0, NS, vacuum, mgMethod);
                            CtoF_s(smesh[sMeshLevel - 1].Nt, ns, flux[level], flux[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Cf);
                        }
                    }
                    break;
                case 6://MG4_a:
                    {
                        if (aMeshLevel == aMeshLevel0)
                        {
                            if (sMeshLevel == sMeshLevel0)
                            { }
                            else
                            {
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_s(smesh[sMeshLevel - 1].Nt, ns, d[level], RHS[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel, aMeshLevel0, sMeshLevel - 1, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_s(smesh[sMeshLevel - 1].Nt, ns, flux[level], flux[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Cf);
                            }
                        }
                        else
                        {
                            if (sMeshLevel == sMeshLevel0)
                            {
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_a(nt, amesh[aMeshLevel - 1].Ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel - 1, aMeshLevel0, sMeshLevel, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_a(nt, amesh[aMeshLevel - 1].Ns, flux[level], flux[level - 1]);
                            }
                            else
                            {
                                mgMethod = 7;//MG4_s
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_a(nt, amesh[aMeshLevel - 1].Ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel - 1, aMeshLevel0, sMeshLevel, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_a(nt, amesh[aMeshLevel - 1].Ns, flux[level], flux[level - 1]);
                            }
                        }
                    }
                    break;
                case 7://MG4_s:
                    {
                        if (aMeshLevel == aMeshLevel0)
                        {
                            if (sMeshLevel == sMeshLevel0)
                            { }
                            else
                            {
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_s(smesh[sMeshLevel - 1].Nt, ns, d[level], RHS[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel, aMeshLevel0, sMeshLevel - 1, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_s(smesh[sMeshLevel - 1].Nt, ns, flux[level], flux[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Cf);
                            }
                        }
                        else
                        {
                            if (sMeshLevel == sMeshLevel0)
                            {
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_a(nt, amesh[aMeshLevel - 1].Ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel - 1, aMeshLevel0, sMeshLevel, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_a(nt, amesh[aMeshLevel - 1].Ns, flux[level], flux[level - 1]);
                            }
                            else
                            {
                                mgMethod = 6;
                                Defect(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[sMeshLevel].A);
                                FtoC_s(smesh[sMeshLevel - 1].Nt, ns, d[level], RHS[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, aMeshLevel, aMeshLevel0, sMeshLevel - 1, sMeshLevel0, NS, vacuum, mgMethod);
                                CtoF_s(smesh[sMeshLevel - 1].Nt, ns, flux[level], flux[level - 1], smesh[sMeshLevel].Smap, smesh[sMeshLevel].Cf);
                            }
                        }
                    }
                    break;
            }

            for (i = 0; i < n2; i++)
            {
                Relaxation(amesh[aMeshLevel], smesh[sMeshLevel], NS, RHS[level], ua[sMeshLevel], us[sMeshLevel], flux[level], b[level], q[level], vacuum);
            }

            return res;
        }

        private double Mod2pi(double x)
        // Purpose: this function is to transfer angle "x" into [0 2*Pi)
        {
            double y;
            if (x < 2 * Pi)
            {
                if (x < 0)
                {
                    y = x + 2 * Pi;
                }
                else
                {
                    y = x;
                }
            }
            else
            {
                y = x - 2 * Pi;
            }
            return y;
        }

        private double Reflection(double theta_i, double ni, double no)
        // Purpose: this function is to find the reflection energy ratio for the reflected angle "theta_i" by tracing-back computation.
        {
            double r, theta_t, temp1, temp2;
            if (Math.Abs(theta_i) < 1e-6)
            {
                temp1 = (ni - no) / (ni + no);
                r = temp1 * temp1;
            }
            else
            {
                temp1 = Math.Sin(theta_i) * ni / no;
                if (temp1 < 1)
                {
                    theta_t = Math.Asin(temp1);
                    temp1 = Math.Sin(theta_i - theta_t) / Math.Sin(theta_i + theta_t);
                    temp2 = Math.Tan(theta_i - theta_t) / Math.Tan(theta_i + theta_t);
                    r = 0.5 * (temp1 * temp1 + temp2 * temp2);
                }
                else
                {
                    r = 1;
                }
            }
            return r;
        }


        private void Refraction(double[] temp, double theta_r, double ni, double no)
        // Purpose: this function is to find the refraction energy ratio for the refracted angle "theta_r" by tracing-back computation.
        {
            double r, theta_i, temp1, temp2;
            if (Math.Abs(theta_r) < 1e-6)
            {
                temp1 = (ni - no) / (ni + no);
                r = 1 - temp1 * temp1;
                theta_i = 0;
            }
            else
            {
                temp1 = Math.Sin(theta_r) * ni / no;
                if (temp1 < 1)
                {
                    theta_i = Math.Asin(temp1);
                    temp1 = Math.Sin(theta_i - theta_r) / Math.Sin(theta_i + theta_r);
                    temp2 = Math.Tan(theta_i - theta_r) / Math.Tan(theta_i + theta_r);
                    r = 1 - 0.5 * (temp1 * temp1 + temp2 * temp2);
                }
                else
                {
                    theta_i = Pi / 2;
                    r = 0;
                }
            }
            temp[0] = r; temp[1] = theta_i;
        }

        private void Relaxation(AngularMesh amesh, SpatialMesh smesh, int Ns, double[][][] RHS, double[][] ua, double[][] us, double[][][] flux,
        BoundaryCoupling bb, double[][][] q, int vacuum)

        // Purpose: this function is improved source-iteration (ISI) with vacuum or reflection boundary condition.
        {
            int i, j, k, m, ii, jj, tri, bi, edge = -1;
            double[,] left = new double[3, 3];
            double[] right = new double[3];
            double[] temp = new double[3];
            double tempd;
            double dettri, cosi, sini, a, b, c, sourceCorr;
            double[,] bv = new double[3, 3] { { 1.0 / 3, 1.0 / 6, 1.0 / 6 }, { 1.0 / 6, 1.0 / 3, 1.0 / 6 }, { 1.0 / 6, 1.0 / 6, 1.0 / 3 } };
            double[,] matrix1 = new double[3, 3];
            double[,] matrix2 = new double[3, 3];

            int[,] index = new int[3, 2];

            int ns = amesh.Ns;
            int aMeshLevel = Ns / ns;
            int nt = smesh.Nt;

            index[0, 0] = 1; index[0, 1] = 2;
            index[1, 0] = 2; index[1, 1] = 0;
            index[2, 0] = 0; index[2, 1] = 1;


            if (vacuum == 0)// reflection boundary condition
            {
                for (i = 0; i < ns; i++)
                {
                    bi = i * aMeshLevel;
                    // "bi" is the angular index of the coarse angle on the fine angular mesh
                    // since sweep ordering is saved on the finest angular mesh for each spatial mesh for simplicity.
                    for (j = 0; j < nt; j++)
                    {
                        tri = smesh.So[bi][j];// the current sweep triangle by sweep ordering
                        dettri = 2 * smesh.A[tri];
                        cosi = amesh.Ang[i][0]; sini = amesh.Ang[i][1];

                        // matrix1: convection term
                        a = cosi * (smesh.P[smesh.T[tri][2]][1] - smesh.P[smesh.T[tri][0]][1]) + sini * (smesh.P[smesh.T[tri][0]][0] - smesh.P[smesh.T[tri][2]][0]);
                        b = cosi * (smesh.P[smesh.T[tri][0]][1] - smesh.P[smesh.T[tri][1]][1]) + sini * (smesh.P[smesh.T[tri][1]][0] - smesh.P[smesh.T[tri][0]][0]);
                        MatrixConvec(a, b, matrix1);

                        // matrix2: absorption term
                        a = ua[tri][0] + (1 - amesh.W[i][i][smesh.Region[smesh.T[j][0]]]) * us[tri][0];
                        b = ua[tri][1] + (1 - amesh.W[i][i][smesh.Region[smesh.T[j][1]]]) * us[tri][1];
                        c = ua[tri][2] + (1 - amesh.W[i][i][smesh.Region[smesh.T[j][2]]]) * us[tri][2];
                        MatrixAbsorb(a, b, c, dettri, matrix2);

                        // left[][]: convection+absorption
                        for (ii = 0; ii < 3; ii++)
                        {
                            for (jj = 0; jj < 3; jj++)
                            { left[ii, jj] = matrix1[ii, jj] + matrix2[ii, jj]; }
                        }

                        // temp: scattering contribution to the direction "i" from all directions except "i".
                        for (ii = 0; ii < 3; ii++)
                        {
                            temp[ii] = 0;
                            for (k = 0; k < ns; k++)
                            {
                                temp[ii] += amesh.W[i][k][smesh.Region[smesh.T[j][ii]]] * flux[k][tri][ii];
                            }
                            temp[ii] += -amesh.W[i][i][smesh.Region[smesh.T[j][ii]]] * flux[i][tri][ii];
                        }

                        sourceCorr = smesh.A[tri] / 12;
                        SourceAssign(us[tri], temp, right, RHS[i][tri], dettri, sourceCorr);


                        // add edge contributions to left, or add upwind fluxes to right from boundary source or the adjacent triangle
                        for (k = 0; k < 3; k++)
                        {   // boundary outgoing flux (s dot n>0): add boundary contributions to left
                            if (smesh.Bd2[bi][tri][k] > 0)
                            {
                                for (ii = 0; ii < 2; ii++)
                                {
                                    for (jj = 0; jj < 2; jj++)
                                    { left[index[k, ii], index[k, jj]] += smesh.Bd2[bi][tri][k] * bv[index[k, ii], index[k, jj]]; }
                                }
                            }
                            // boundary incoming flux (s dot n<0): add upwind fluxes to right
                            else if (smesh.Bd2[bi][tri][k] < 0)
                            {
                                if (smesh.So2[tri][k] > -1)
                                // "tri" is at the domain boundary: upwind flux is from internal reflection and boundary source.
                                {
                                    edge = smesh.So2[tri][k];

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        temp[ii] = 0;
                                        for (m = 0; m < 2; m++)
                                        {
                                            //temp[ii]+=flux[ri[edge][i][m]][tri][index[k][ii]]*ri2[edge][i][m];
                                            temp[ii] += flux[bb.Ri[edge][i][m]][tri][smesh.E2[edge][ii]] * bb.Ri2[edge][i][m];
                                            temp[ii] += q[bb.Si[edge][i][m]][edge][ii] * bb.Si2[edge][i][m];
                                        }
                                    }
                                    // note: we distinguish boundary source from internal source for reflection boundary condition
                                    //       due to refraction index mismatch at the boundary.

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += temp[jj] * bv[smesh.E2[edge][ii], smesh.E2[edge][jj]];
                                        }
                                        right[smesh.E2[edge][ii]] += -smesh.Bd2[bi][tri][k] * tempd;
                                    }
                                }
                                else
                                // "tri" is not at the domain boundary: upwind flux is from the adjacent triangle.
                                {
                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += flux[i][smesh.Bd[bi][tri][3 * k]][smesh.Bd[bi][tri][3 * k + jj + 1]] * bv[index[k, ii], index[k, jj]];
                                        }
                                        right[index[k, ii]] += -smesh.Bd2[bi][tri][k] * tempd;
                                    }
                                }
                            }
                        }
                        MatrixSolver(left, right, flux[i][tri]);// update the nodal values at "tri"
                    }
                }
            }
            else // vacuum boundary condition
            {
                for (i = 0; i < ns; i++)
                {
                    bi = i * aMeshLevel;
                    for (j = 0; j < nt; j++)
                    {
                        tri = smesh.So[bi][j];
                        dettri = 2 * smesh.A[tri];
                        cosi = amesh.Ang[i][0]; sini = amesh.Ang[i][1];
                        a = cosi * (smesh.P[smesh.T[tri][2]][1] - smesh.P[smesh.T[tri][0]][1]) + sini * (smesh.P[smesh.T[tri][0]][0] - smesh.P[smesh.T[tri][2]][0]);
                        b = cosi * (smesh.P[smesh.T[tri][0]][1] - smesh.P[smesh.T[tri][1]][1]) + sini * (smesh.P[smesh.T[tri][1]][0] - smesh.P[smesh.T[tri][0]][0]);
                        MatrixConvec(a, b, matrix1);

                        a = ua[tri][0] + (1 - amesh.W[i][i][smesh.Region[smesh.T[j][0]]]) * us[tri][0];
                        b = ua[tri][1] + (1 - amesh.W[i][i][smesh.Region[smesh.T[j][1]]]) * us[tri][1];
                        c = ua[tri][2] + (1 - amesh.W[i][i][smesh.Region[smesh.T[j][2]]]) * us[tri][2];
                        MatrixAbsorb(a, b, c, dettri, matrix2);

                        for (ii = 0; ii < 3; ii++)
                        {
                            for (jj = 0; jj < 3; jj++)
                            { left[ii, jj] = matrix1[ii, jj] + matrix2[ii, jj]; }
                        }

                        for (ii = 0; ii < 3; ii++)
                        {
                            temp[ii] = 0;
                            for (k = 0; k < ns; k++)
                            {
                                temp[ii] += amesh.W[i][k][smesh.Region[smesh.T[j][ii]]] * flux[k][tri][ii];
                            }
                            temp[ii] += -amesh.W[i][i][smesh.Region[smesh.T[j][ii]]] * flux[i][tri][ii];
                        }

                        sourceCorr = smesh.A[tri] / 12;
                        SourceAssign(us[tri], temp, right, RHS[i][tri], dettri, sourceCorr);


                        for (k = 0; k < 3; k++)
                        {
                            if (smesh.Bd2[bi][tri][k] > 0)
                            {
                                for (ii = 0; ii < 2; ii++)
                                {
                                    for (jj = 0; jj < 2; jj++)
                                    { left[index[k, ii], index[k, jj]] += smesh.Bd2[bi][tri][k] * bv[index[k, ii], index[k, jj]]; }
                                }
                            }
                            else if (smesh.Bd2[bi][tri][k] < 0)
                            {
                                if (smesh.So2[tri][k] > -1)
                                // "tri" is at the domain boundary: upwind flux is from boundary source only.
                                {
                                    edge = smesh.So2[tri][k];

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += q[i][edge][jj] * bv[smesh.E2[edge][ii], smesh.E2[edge][jj]];
                                        }
                                        right[smesh.E2[edge][ii]] += -smesh.Bd2[bi][tri][k] * tempd;
                                    }
                                }
                                else
                                {
                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += flux[i][smesh.Bd[bi][tri][3 * k]][smesh.Bd[bi][tri][3 * k + jj + 1]] * bv[index[k, ii], index[k, jj]];
                                        }
                                        right[index[k, ii]] += -smesh.Bd2[bi][tri][k] * tempd;
                                    }
                                }
                            }
                        }
                        MatrixSolver(left, right, flux[i][tri]);
                    }
                }
            }
        }


        public double Residual(int nt, int ns, double[][][] d, double[] a)
        // Purpose: this function is to compute the residual.
        {
            double res = 0, temp;
            int i, j, k;

            for (i = 0; i < nt; i++)
            {
                temp = 0;
                for (j = 0; j < ns; j++)
                {
                    for (k = 0; k < 3; k++)
                    { temp += Math.Abs(d[j][i][k]); }
                }
                res += temp * a[i];
            }
            return res * 2 * Pi / ns / 3;
        }


        private void SourceAssign(double[] us, double[] temp, double[] right, double[] RHS, double dettri, double source_corr)
        // Note: point (delta) source needs correction.
        // right[][]: scattering+light source
        {
            double a, b, c, d, e, f;

            a = us[0];
            b = us[1];
            c = us[2];
            d = temp[0];
            e = temp[1];
            f = temp[2];

            right[0] = dettri / 120 * (2 * a * (3 * d + e + f) + b * (2 * d + 2 * e + f) + c * (2 * d + e + 2 * f))
            + (2 * RHS[0] + RHS[1] + RHS[2]) * source_corr;
            right[1] = dettri / 120 * (a * (2 * d + 2 * e + f) + 2 * b * (d + 3 * e + f) + c * (d + 2 * e + 2 * f))
            + (RHS[0] + 2 * RHS[1] + RHS[2]) * source_corr;
            right[2] = dettri / 120 * (a * (2 * d + e + 2 * f) + b * (d + 2 * e + 2 * f) + 2 * c * (d + e + 3 * f))
            + (RHS[0] + RHS[1] + 2 * RHS[2]) * source_corr;
        }


        public void SpatialMapping(SpatialMesh cmesh, SpatialMesh fmesh, int[][] smap)
        // Purpose: this function is to compute "cf" and "fc".
        //
        //          cf[2][n][2]: coarse-to-fine spatial mapping of nodal values computed by linear interpolation
        //          fc[2][n][2]: fine-to-coarse spatial mapping of nodal values computed by L2 projection
        //
        //    Note: 1D spatial mapping is straightforward due to the simple geometry,
        //          in particular, it is the same with respect to different intervals.
        {
            double[][] p;
            double[][] c_f;
            double[][] c_c;
            double[] a;
            double[] distance;
            double area1, area2, area3, x, y, x1, y1, x2, y2, x3, y3;
            int[][] t;
            int nt_f, nt_c, i, j, tri, flag;

            p = cmesh.P;
            t = cmesh.T;
            c_c = cmesh.C;
            c_f = fmesh.C;
            a = cmesh.A;
            nt_f = fmesh.Nt;
            nt_c = cmesh.Nt;

            distance = new double[nt_c];

            for (i = 0; i < nt_c; i++)
            { smap[i][0] = 0; }

            for (i = 0; i < nt_f; i++)
            {
                flag = 0;
                x = c_f[i][0]; y = c_f[i][1];
                for (j = 0; j < nt_c; j++)
                { distance[j] = (x - c_c[j][0]) * (x - c_c[j][0]) + (y - c_c[j][1]) * (y - c_c[j][1]); }
                for (j = 0; j < nt_c; j++)
                {
                    tri = MathFunctions.FindMin(nt_c, distance);// find the triangle with minimal distance between (x,y) and centers of coarse triangles.
                    distance[tri] = 1e10;
                    x1 = p[t[tri][0]][0]; y1 = p[t[tri][0]][1];
                    x2 = p[t[tri][1]][0]; y2 = p[t[tri][1]][1];
                    x3 = p[t[tri][2]][0]; y3 = p[t[tri][2]][1];

                    area1 = MathFunctions.Area(x, y, x2, y2, x3, y3);
                    area2 = MathFunctions.Area(x1, y1, x, y, x3, y3);
                    area3 = MathFunctions.Area(x1, y1, x2, y2, x, y);
                    if (Math.Abs(area1 + area2 + area3 - a[tri]) / a[tri] < 1e-2)// If true, then the fine triangle "i" is in the coarse triangle "j".
                    {
                        flag = 1;
                        smap[tri][0] += 1;
                        smap[tri][smap[tri][0]] = i;
                        goto stop;
                    }
                }
                if (flag == 0)
                { Console.WriteLine("spatial mapping has a problem\n"); }
            stop: ;
            }
        }

        public void SpatialMapping2(SpatialMesh cmesh, SpatialMesh fmesh, int[][] smap, double[][][][] cf, double[][][][] fc, int tempsize)

        // Purpose: this function is to compute "cf" and "fc".
        //
        //          cf[nt_c][3][smap[nt_c][0]][3]: coarse-to-fine spatial mapping of nodal values computed by linear interpolation
        //          fc[nt_c][3][smap[nt_c][0]][3]: fine-to-coarse spatial mapping of nodal values computed by L2 projection
        //
        //          the 1st indexs the coarse triangles;
        //          the 2nd indexes 3 coarse nodes in the coarse triangle;
        //          the 3rd indexes the fine triangles contained in the coarse triangle;
        //          the 4th indexes 3 fine nodes in the fine triangle.
        {
            int[][] ind_p;
            int np, nt_c, flag, i, j, k, m;
            int[][] temp;
            int[][] tf;
            int[][] t;
            double[][] temp2;
            double tempd, x1, x2, x3, y1, y2, y3, x, y;
            double[][] pf;
            double[][] p;
            double[] xf = new double[3];
            double[] yf = new double[3];
            double[] a;
            double[] af;

            tf = fmesh.T; pf = fmesh.P; af = fmesh.A;
            nt_c = cmesh.Nt; p = cmesh.P; t = cmesh.T; a = cmesh.A;

            temp = new int[3 * tempsize][];
            for (i = 0; i < 3 * tempsize; i++)
            {
                temp[i] = new int[2]; ;
                for (j = 0; j < 2; j++)
                { temp[i][j] = -1; }
            }

            temp2 = new double[3 * tempsize][];
            for (i = 0; i < 3 * tempsize; i++)
            {
                temp2[i] = new double[3];
                for (j = 0; j < 3; j++)
                { temp2[i][j] = 0; }
            }

            ind_p = new int[tempsize][];
            for (j = 0; j < tempsize; j++)
            {
                ind_p[j] = new int[3];
                for (k = 0; k < 3; k++)
                { ind_p[j][k] = -1; }
            }

            for (i = 0; i < nt_c; i++)
            {
                // compute "ind_p" and "temp"
                // temp[][]:    find discrete (without repeat) nodes of the fine triangles contained in the coarse triangle
                //              and save it sequentially, i.e., order it in sequence as 1,2,3... and so on.
                //              the 1st indexes fine nodes;
                //              for the 2nd index, 1st entry is the global nodal index on fine mesh, 2nd entry is the repeatence of this node.

                // ind_p[][3]:  the local nodal correspondence in fine triangles contained in the coarse triangle
                //              Example:    if the 2nd node of the 3rd fine triangle is the 4th node as in "temp",
                //                          then ind_p[3][2]=4.

                temp[0][0] = tf[smap[i][1]][0];
                temp[0][1] = 1;
                np = 1; ind_p[0][0] = 0;
                j = 0;
                {
                    for (k = 1; k < 3; k++)
                    {
                        flag = 0;// flag indicates whether it is a new node that has not been found yet.
                        for (m = 0; m < np; m++)
                        {
                            if (tf[smap[i][j + 1]][k] == temp[m][0])// existing node
                            {
                                ind_p[j][k] = m;
                                temp[m][1] += 1;
                                flag = 1;
                                goto stop;
                            }
                        }
                    stop: ;
                        if (flag == 0)// new node
                        {
                            ind_p[j][k] = np;
                            temp[np][0] = tf[smap[i][j + 1]][k];
                            temp[np][1] = 1;
                            np += 1;
                        }
                    }
                }

                for (j = 1; j < smap[i][0]; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        flag = 0;
                        for (m = 0; m < np; m++)
                        {
                            if (tf[smap[i][j + 1]][k] == temp[m][0])
                            {
                                ind_p[j][k] = m;
                                temp[m][1] += 1;
                                flag = 1;
                                goto stop2;
                            }
                        }
                    stop2: ;
                        if (flag == 0)
                        {
                            ind_p[j][k] = np;
                            temp[np][0] = tf[smap[i][j + 1]][k];
                            temp[np][1] = 1;
                            np += 1;
                        }
                    }
                }

                // compute temp2
                // temp2[][]: the nodal weights of non-repeat fine nodes in the coarse triangle.
                x1 = p[t[i][0]][0]; y1 = p[t[i][0]][1];
                x2 = p[t[i][1]][0]; y2 = p[t[i][1]][1];
                x3 = p[t[i][2]][0]; y3 = p[t[i][2]][1];
                tempd = MathFunctions.Area(x1, y1, x2, y2, x3, y3);
                for (j = 0; j < np; j++)
                {
                    x = pf[temp[j][0]][0]; y = pf[temp[j][0]][1];
                    temp2[j][1] = MathFunctions.Area(x1, y1, x, y, x3, y3) / tempd;
                    temp2[j][2] = MathFunctions.Area(x1, y1, x2, y2, x, y) / tempd;
                    temp2[j][0] = 1 - temp2[j][1] - temp2[j][2];
                }

                // compute "cf"
                for (j = 0; j < smap[i][0]; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        for (m = 0; m < 3; m++)
                        {
                            cf[i][m][j][k] = temp2[ind_p[j][k]][m];
                        }
                    }
                }

                // compute "fc"
                for (j = 0; j < smap[i][0]; j++)
                {
                    for (k = 0; k < 3; k++)
                    {
                        xf[k] = temp2[ind_p[j][k]][1];
                        yf[k] = temp2[ind_p[j][k]][2];
                    }

                    fc[i][1][j][0] = 2 * xf[0] + xf[1] + xf[2] - 1;
                    fc[i][2][j][0] = 2 * yf[0] + yf[1] + yf[2] - 1;
                    fc[i][0][j][0] = 1 - fc[i][1][j][0] - fc[i][2][j][0];

                    fc[i][1][j][1] = xf[0] + 2 * xf[1] + xf[2] - 1;
                    fc[i][2][j][1] = yf[0] + 2 * yf[1] + yf[2] - 1;
                    fc[i][0][j][1] = 1 - fc[i][1][j][1] - fc[i][2][j][1];

                    fc[i][1][j][2] = xf[0] + xf[1] + 2 * xf[2] - 1;
                    fc[i][2][j][2] = yf[0] + yf[1] + 2 * yf[2] - 1;
                    fc[i][0][j][2] = 1 - fc[i][1][j][2] - fc[i][2][j][2];

                    tempd = af[smap[i][j + 1]] / a[i];
                    for (k = 0; k < 3; k++)
                    {
                        for (m = 0; m < 3; m++)
                        { fc[i][m][j][k] *= tempd; }
                    }
                }
            }
        }

    }
}