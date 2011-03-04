using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vts.Modeling.ForwardSolvers.MGRTE._2D.DataStructures;

namespace Vts.Modeling.ForwardSolvers.MGRTE._2D
{
    class MultiGridCycle
    {
        public double Pi = GlobalConstants.Pi;
        
        public double Area(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return 0.5 * Math.Abs((y2 - y1) * (x3 - x1) - (y3 - y1) * (x2 - x1));
        }

        public double Length(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));
        }

        public double MgCycle(AngularMesh[] amesh, SpatialMesh[] smesh, BoundaryCoupling[] b, double[][][][] q, 
            double[][][][] RHS, double[][][] ua, double[][][] us, double[][][][] flux, double[][][][] d,
            int n1, int n2, int alevel, int alevel0, int slevel, int slevel0, int NS, int vacuum, int whichmg)

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
 
            nt = smesh[slevel].nt;
            ns = amesh[alevel].ns;
            ds = slevel - slevel0; 
            da = alevel - alevel0;

            switch (whichmg)
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
                Relaxation(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], vacuum);
            }

            switch (whichmg)
            {
                case 1://AMG:
                    {
                        if (alevel == alevel0)
                        { }
                        else
                        {
                            Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                            res = Residual(nt, ns, d[level], smesh[slevel].a);
                            FtoC_a(nt, amesh[alevel - 1].ns, d[level], RHS[level - 1]);
                            MgCycle(amesh, smesh,  b, q, RHS, ua, us, flux, d, n1, n2, alevel, alevel0, slevel -  1, slevel0, NS, vacuum, whichmg);
                            CtoF_a(nt, amesh[alevel - 1].ns, flux[level], flux[level - 1]);
                        }
                    }
                    break;
                case 2://SMG:
                    {
                        if (slevel == slevel0)
                        { }
                        else
                        {
                            Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                            res = Residual(nt, ns, d[level], smesh[slevel].a);
                            FtoC_s(smesh[slevel - 1].nt, ns, d[level], RHS[level - 1], smesh[slevel].smap, smesh[slevel].fc);
                            MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel - 1, alevel0, slevel, slevel0, NS, vacuum, whichmg);
                            CtoF_s(smesh[slevel - 1].nt, ns, flux[level], flux[level - 1], smesh[slevel].smap, smesh[slevel].cf);
                        }
                    }
                    break;
                case 3://MG1:
                    {
                        if (alevel == alevel0)
                        {
                            if (slevel == slevel0)
                            { }
                            else
                            {
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_s(smesh[slevel - 1].nt, ns, d[level], RHS[level - 1], smesh[slevel].smap, smesh[slevel].fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel, alevel0, slevel - 1, slevel0, NS, vacuum, whichmg);
                                CtoF_s(smesh[slevel - 1].nt, ns, flux[level], flux[level - 1], smesh[slevel].smap, smesh[slevel].cf);
                            }
                        }
                        else
                        {
                            if (slevel == slevel0)
                            {
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_a(nt, amesh[alevel - 1].ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel - 1, alevel0, slevel, slevel0, NS, vacuum, whichmg);
                                CtoF_a(nt, amesh[alevel - 1].ns, flux[level], flux[level - 1]);
                            }
                            else
                            {
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC(smesh[slevel - 1].nt, amesh[alevel - 1].ns, d[level], RHS[level - 1], smesh[slevel].smap, smesh[slevel].fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel - 1, alevel0, slevel, slevel0, NS, vacuum, whichmg);
                                CtoF(smesh[slevel - 1].nt, amesh[alevel - 1].ns, flux[level], flux[level - 1], smesh[slevel].smap, smesh[slevel].cf);
                            }
                        }
                    }
                    break;
                case 4://MG2:
                    {
                        if (alevel == alevel0)
                        {
                            if (slevel == slevel0)
                            { }
                            else
                            {
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_s(smesh[slevel - 1].nt, ns, d[level], RHS[level - 1], smesh[slevel].smap, smesh[slevel].fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel, alevel0, slevel - 1, slevel0, NS, vacuum, whichmg);
                                CtoF_s(smesh[slevel - 1].nt, ns, flux[level], flux[level - 1], smesh[slevel].smap, smesh[slevel].cf);
                            }
                        }
                        else
                        {
                            Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                            res = Residual(nt, ns, d[level], smesh[slevel].a);
                            FtoC_a(nt, amesh[alevel - 1].ns, d[level], RHS[level - 1]);
                            MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel - 1, alevel0, slevel, slevel0, NS, vacuum, whichmg);
                            CtoF_a(nt, amesh[alevel - 1].ns, flux[level], flux[level - 1]);
                        }
                    }
                    break;
                case 5://MG3:
                    {
                        if (slevel == slevel0)
                        {
                            if (alevel == alevel0)
                            { }
                            else
                            {
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_a(nt, amesh[alevel - 1].ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel - 1, alevel0, slevel, slevel0, NS, vacuum, whichmg);
                                CtoF_a(nt, amesh[alevel - 1].ns, flux[level], flux[level - 1]);
                            }
                        }
                        else
                        {
                            Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                            res = Residual(nt, ns, d[level], smesh[slevel].a);
                            FtoC_s(smesh[slevel - 1].nt, ns, d[level], RHS[level - 1], smesh[slevel].smap, smesh[slevel].fc);
                            MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel, alevel0, slevel - 1, slevel0, NS, vacuum, whichmg);
                            CtoF_s(smesh[slevel - 1].nt, ns, flux[level], flux[level - 1], smesh[slevel].smap, smesh[slevel].cf);
                        }
                    }
                    break;
                case 6://MG4_a:
                    {
                        if (alevel == alevel0)
                        {
                            if (slevel == slevel0)
                            { }
                            else
                            {
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_s(smesh[slevel - 1].nt, ns, d[level], RHS[level - 1], smesh[slevel].smap, smesh[slevel].fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel, alevel0, slevel - 1, slevel0, NS, vacuum, whichmg);
                                CtoF_s(smesh[slevel - 1].nt, ns, flux[level], flux[level - 1], smesh[slevel].smap, smesh[slevel].cf);
                            }
                        }
                        else
                        {
                            if (slevel == slevel0)
                            {
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_a(nt, amesh[alevel - 1].ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel - 1, alevel0, slevel, slevel0, NS, vacuum, whichmg);
                                CtoF_a(nt, amesh[alevel - 1].ns, flux[level], flux[level - 1]);
                            }
                            else
                            {
                                whichmg = 7;//MG4_s
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_a(nt, amesh[alevel - 1].ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel - 1, alevel0, slevel, slevel0, NS, vacuum, whichmg);
                                CtoF_a(nt, amesh[alevel - 1].ns, flux[level], flux[level - 1]);
                            }
                        }
                    }
                    break;
                case 7://MG4_s:
                    {
                        if (alevel == alevel0)
                        {
                            if (slevel == slevel0)
                            { }
                            else
                            {
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_s(smesh[slevel - 1].nt, ns, d[level], RHS[level - 1], smesh[slevel].smap, smesh[slevel].fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel, alevel0, slevel - 1, slevel0, NS, vacuum, whichmg);
                                CtoF_s(smesh[slevel - 1].nt, ns, flux[level], flux[level - 1], smesh[slevel].smap, smesh[slevel].cf);
                            }
                        }
                        else
                        {
                            if (slevel == slevel0)
                            {
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_a(nt, amesh[alevel - 1].ns, d[level], RHS[level - 1]);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel - 1, alevel0, slevel, slevel0, NS, vacuum, whichmg);
                                CtoF_a(nt, amesh[alevel - 1].ns, flux[level], flux[level - 1]);
                            }
                            else
                            {
                                whichmg = 6;
                                Defect(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], d[level], vacuum);
                                res = Residual(nt, ns, d[level], smesh[slevel].a);
                                FtoC_s(smesh[slevel - 1].nt, ns, d[level], RHS[level - 1], smesh[slevel].smap, smesh[slevel].fc);
                                MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, n1, n2, alevel, alevel0, slevel - 1, slevel0, NS, vacuum, whichmg);
                                CtoF_s(smesh[slevel - 1].nt, ns, flux[level], flux[level - 1], smesh[slevel].smap, smesh[slevel].cf);
                            }
                        }
                    }
                    break;
            }


            for (i = 0; i < n2; i++)
            { Relaxation(amesh[alevel], smesh[slevel], NS, RHS[level], ua[slevel], us[slevel], flux[level], b[level], q[level], vacuum); }

            return res;            
        }


        public double Residual(int nt, int ns, double[][][] d, double []a)
        // Purpose: this function is to compute the residual.
        {
            double res = 0,temp;
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


        public void Defect(AngularMesh amesh, SpatialMesh smesh, int Ns, double[][][] RHS, double[][] ua, double[][] us, double[][][] flux,
        BoundaryCoupling bb, double[][][] q, double[][][] res, int vacuum)

        // Purpose: this function is to compute the residual with vacuum or reflection boundary condition.
        //          see "relaxation" for more details.
        {
            int i, j, k, m, ii, jj, bi, alevel, edge = 0;

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

            alevel = Ns / amesh.ns;

            if (vacuum == 0)
            {
                for (i = 0; i < amesh.ns; i++)
                {
                    bi = i * alevel;
                    for (j = 0; j < smesh.nt; j++)
                    {
                        dettri = 2 * smesh.a[j];
                        cosi = amesh.a[i][0]; sini = amesh.a[i][1];
                        a = cosi * (smesh.p[smesh.t[j][2]][1] - smesh.p[smesh.t[j][0]][1]) + sini * (smesh.p[smesh.t[j][0]][0] - smesh.p[smesh.t[j][2]][0]);
                        b = cosi * (smesh.p[smesh.t[j][0]][1] - smesh.p[smesh.t[j][1]][1]) + sini * (smesh.p[smesh.t[j][1]][0] - smesh.p[smesh.t[j][0]][0]);
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
                            for (k = 0; k < amesh.ns; k++)
                            { temp[ii] += amesh.w[i, k] * flux[k][j][ii]; }
                        }

                        source_corr = smesh.a[j] / 12;
                        SourceAssign(us[j], temp, right, RHS[i][j], dettri, source_corr);                       

                        for (k = 0; k < 3; k++)
                        {
                            if (smesh.bd2[bi][j][k] > 0)
                            {
                                for (ii = 0; ii < 2; ii++)
                                {
                                    for (jj = 0; jj < 2; jj++)
                                    { left[index[k, ii], index[k, jj]] += smesh.bd2[bi][j][k] * bv[index[k, ii], index[k, jj]]; }
                                }
                            }
                            else if (smesh.bd2[bi][j][k] < 0)
                            {
                                if (smesh.so2[j][k] > -1)// upwind flux from the internal reflection and boundary source
                                {
                                    edge = smesh.so2[j][k];

                                    temp[0] = 0;
                                    temp[1] = 0;
                                    for (m = 0; m < 2; m++)
                                    {
                                        for (ii = 0; ii < 2; ii++)
                                        {
                                            temp[ii] += flux[bb.ri[edge][i][m]][j][smesh.e2[edge][ii]] * bb.ri2[edge][i][m];
                                        }
                                        for (ii = 0; ii < 2; ii++)
                                        {
                                            temp[ii] += q[bb.si[edge][i][m]][edge][ii] * bb.si2[edge][i][m];
                                        }
                                    }

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += temp[jj] * bv[smesh.e2[edge][ii], smesh.e2[edge][jj]];
                                        }
                                        right[smesh.e2[edge][ii]] += -smesh.bd2[bi][j][k] * tempd;
                                    }
                                }
                                else // upwind flux from the adjacent triangle
                                {
                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += flux[i][smesh.bd[bi][j][3 * k]][smesh.bd[bi][j][3 * k + jj + 1]] * bv[index[k, ii], index[k, jj]];
                                        }
                                        right[index[k, ii]] += -smesh.bd2[bi][j][k] * tempd;
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
                for (i = 0; i < amesh.ns; i++)
                {
                    bi = i * alevel;
                    for (j = 0; j < smesh.nt; j++)
                    {
                        dettri = 2 * smesh.a[j];
                        cosi = amesh.a[i][0]; sini = amesh.a[i][1];
                        a = cosi * (smesh.p[smesh.t[j][2]][1] - smesh.p[smesh.t[j][0]][1]) + sini * (smesh.p[smesh.t[j][0]][0] - smesh.p[smesh.t[j][2]][0]);
                        b = cosi * (smesh.p[smesh.t[j][0]][1] - smesh.p[smesh.t[j][1]][1]) + sini * (smesh.p[smesh.t[j][1]][0] - smesh.p[smesh.t[j][0]][0]);
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
                            for (k = 0; k < amesh.ns; k++)
                            { temp[ii] += amesh.w[i, k] * flux[k][j][ii]; }
                        }

                        source_corr = smesh.a[j] / 12;
                        SourceAssign(us[j], temp, right, RHS[i][j], dettri, source_corr);


                        for (k = 0; k < 3; k++)
                        {
                            if (smesh.bd2[bi][j][k] > 0)
                            {
                                for (ii = 0; ii < 2; ii++)
                                {
                                    for (jj = 0; jj < 2; jj++)
                                    { left[index[k, ii], index[k, jj]] += smesh.bd2[bi][j][k] * bv[index[k, ii], index[k, jj]]; }
                                }
                            }
                            else if (smesh.bd2[bi][j][k] < 0)
                            {
                                if (smesh.so2[j][k] > -1)
                                // "tri" is at the domain boundary: upwind flux is from boundary source only.
                                {
                                    edge = smesh.so2[j][k];

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += q[i][edge][jj] * bv[smesh.e2[edge][ii], smesh.e2[edge][jj]];
                                        }
                                        right[smesh.e2[edge][ii]] += -smesh.bd2[bi][j][k] * tempd;
                                    }
                                }
                                else
                                {
                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += flux[i][smesh.bd[bi][j][3 * k]][smesh.bd[bi][j][3 * k + jj + 1]] * bv[index[k, ii], index[k, jj]];
                                        }
                                        right[index[k, ii]] += -smesh.bd2[bi][j][k] * tempd;
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


        public void Relaxation(AngularMesh amesh, SpatialMesh smesh, int Ns, double[][][] RHS, double[][] ua, double[][] us, double[][][] flux,
        BoundaryCoupling bb, double[][][] q, int vacuum)

        // Purpose: this function is improved source-iteration (ISI) with vacuum or reflection boundary condition.
        {
            int i, j, k, m, ii, jj, ns, nt, tri, bi, alevel, edge = 0;
            double[,] left = new double[3, 3];
            double[] right = new double[3];
            double[] temp = new double[3];
            double tempd;
            double dettri, cosi, sini, a, b, c, source_corr;
            double[,] bv = new double[3, 3] { { 1.0 / 3, 1.0 / 6, 1.0 / 6 }, { 1.0 / 6, 1.0 / 3, 1.0 / 6 }, { 1.0 / 6, 1.0 / 6, 1.0 / 3 } };
            double[,] matrix1 = new double[3, 3];
            double[,] matrix2 = new double[3, 3];

            int[,] index = new int[3, 2];

            ns = amesh.ns;
            alevel = Ns / ns;
            nt = smesh.nt;

            index[0, 0] = 1; index[0, 1] = 2;
            index[1, 0] = 2; index[1, 1] = 0;
            index[2, 0] = 0; index[2, 1] = 1;


            if (vacuum == 0)// reflection boundary condition
            {
                for (i = 0; i < ns; i++)
                {
                    bi = i * alevel;
                    // "bi" is the angular index of the coarse angle on the fine angular mesh
                    // since sweep ordering is saved on the finest angular mesh for each spatial mesh for simplicity.
                    for (j = 0; j < nt; j++)
                    {
                        tri = smesh.so[bi][j];// the current sweep triangle by sweep ordering
                        dettri = 2 * smesh.a[tri];
                        cosi = amesh.a[i][0]; sini = amesh.a[i][1];
                        a = cosi * (smesh.p[smesh.t[tri][2]][1] - smesh.p[smesh.t[tri][0]][1]) + sini * (smesh.p[smesh.t[tri][0]][0] - smesh.p[smesh.t[tri][2]][0]);
                        b = cosi * (smesh.p[smesh.t[tri][0]][1] - smesh.p[smesh.t[tri][1]][1]) + sini * (smesh.p[smesh.t[tri][1]][0] - smesh.p[smesh.t[tri][0]][0]);                        
                        MatrixConvec(a, b, matrix1);

                        // matrix2: absorption term
                        a = ua[tri][0] + (1 - amesh.w[i, i]) * us[tri][0];
                        b = ua[tri][1] + (1 - amesh.w[i, i]) * us[tri][1];
                        c = ua[tri][2] + (1 - amesh.w[i, i]) * us[tri][2];
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
                                temp[ii] += amesh.w[i, k] * flux[k][tri][ii];
                            }
                            temp[ii] += -amesh.w[i, i] * flux[i][tri][ii];
                        }

                        source_corr = smesh.a[tri] / 12;
                        SourceAssign(us[tri], temp, right, RHS[i][tri], dettri, source_corr);
                       

                        // add edge contributions to left, or add upwind fluxes to right from boundary source or the adjacent triangle
                        for (k = 0; k < 3; k++)
                        {   // boundary outgoing flux (s dot n>0): add boundary contributions to left
                            if (smesh.bd2[bi][tri][k] > 0)
                            {
                                for (ii = 0; ii < 2; ii++)
                                {
                                    for (jj = 0; jj < 2; jj++)
                                    { left[index[k, ii], index[k, jj]] += smesh.bd2[bi][tri][k] * bv[index[k, ii], index[k, jj]]; }
                                }
                            }
                            // boundary incoming flux (s dot n<0): add upwind fluxes to right
                            else if (smesh.bd2[bi][tri][k] < 0)
                            {
                                if (smesh.so2[tri][k] > -1)
                                // "tri" is at the domain boundary: upwind flux is from internal reflection and boundary source.
                                {
                                    edge = smesh.so2[tri][k];

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        temp[ii] = 0;
                                        for (m = 0; m < 2; m++)
                                        {
                                            //temp[ii]+=flux[ri[edge][i][m]][tri][index[k][ii]]*ri2[edge][i][m];
                                            temp[ii] += flux[bb.ri[edge][i][m]][tri][smesh.e2[edge][ii]] * bb.ri2[edge][i][m];
                                            temp[ii] += q[bb.si[edge][i][m]][edge][ii] * bb.si2[edge][i][m];
                                        }
                                    }
                                    // note: we distinguish boundary source from internal source for reflection boundary condition
                                    //       due to refraction index mismatch at the boundary.

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += temp[jj] * bv[smesh.e2[edge][ii], smesh.e2[edge][jj]];
                                        }
                                        right[smesh.e2[edge][ii]] += -smesh.bd2[bi][tri][k] * tempd;
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
                                            tempd += flux[i][smesh.bd[bi][tri][3 * k]][smesh.bd[bi][tri][3 * k + jj + 1]] * bv[index[k, ii], index[k, jj]];
                                        }
                                        right[index[k, ii]] += -smesh.bd2[bi][tri][k] * tempd;
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
                    bi = i * alevel;
                    for (j = 0; j < nt; j++)
                    {
                        tri = smesh.so[bi][j];
                        dettri = 2 * smesh.a[tri];
                        cosi = amesh.a[i][0]; sini = amesh.a[i][1];
                        a = cosi * (smesh.p[smesh.t[tri][2]][1] - smesh.p[smesh.t[tri][0]][1]) + sini * (smesh.p[smesh.t[tri][0]][0] - smesh.p[smesh.t[tri][2]][0]);
                        b = cosi * (smesh.p[smesh.t[tri][0]][1] - smesh.p[smesh.t[tri][1]][1]) + sini * (smesh.p[smesh.t[tri][1]][0] - smesh.p[smesh.t[tri][0]][0]);
                        MatrixConvec(a, b, matrix1);

                        a = ua[tri][0] + (1 - amesh.w[i, i]) * us[tri][0];
                        b = ua[tri][1] + (1 - amesh.w[i, i]) * us[tri][1];
                        c = ua[tri][2] + (1 - amesh.w[i, i]) * us[tri][2];
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
                                temp[ii] += amesh.w[i, k] * flux[k][tri][ii];
                            }
                            temp[ii] += -amesh.w[i, i] * flux[i][tri][ii];
                        }

                        source_corr = smesh.a[tri] / 12;
                        SourceAssign(us[tri],temp, right, RHS[i][tri], dettri, source_corr);
                        

                        for (k = 0; k < 3; k++)
                        {
                            if (smesh.bd2[bi][tri][k] > 0)
                            {
                                for (ii = 0; ii < 2; ii++)
                                {
                                    for (jj = 0; jj < 2; jj++)
                                    { left[index[k, ii], index[k, jj]] += smesh.bd2[bi][tri][k] * bv[index[k, ii], index[k, jj]]; }
                                }
                            }
                            else if (smesh.bd2[bi][tri][k] < 0)
                            {
                                if (smesh.so2[tri][k] > -1)
                                // "tri" is at the domain boundary: upwind flux is from boundary source only.
                                {
                                    edge = smesh.so2[tri][k];

                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += q[i][edge][jj] * bv[smesh.e2[edge][ii], smesh.e2[edge][jj]];
                                        }
                                        right[smesh.e2[edge][ii]] += -smesh.bd2[bi][tri][k] * tempd;
                                    }
                                }
                                else
                                {
                                    for (ii = 0; ii < 2; ii++)
                                    {
                                        tempd = 0;
                                        for (jj = 0; jj < 2; jj++)
                                        {
                                            tempd += flux[i][smesh.bd[bi][tri][3 * k]][smesh.bd[bi][tri][3 * k + jj + 1]] * bv[index[k, ii], index[k, jj]];
                                        }
                                        right[index[k, ii]] += -smesh.bd2[bi][tri][k] * tempd;
                                    }
                                }
                            }
                        }
                        MatrixSolver(left, right, flux[i][tri]);
                    }
                }
            }
        }

        public void MatrixConvec(double a, double b, double[,] matrix)
        {
            //Purpose of this matrix to assign convection terms
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

        public void MatrixAbsorb(double a, double b, double c, double dettri, double[,] matrix)
        {
            //Purpose of this matrix to assign absorption terms
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

        public void SourceAssign( double []us, double []temp, double []right, double []RHS, double dettri, double source_corr)
        {
            double a, b, c, d, e, f;

            a = us[0];
            b = us[1];
            c = us[2];
            d = temp[0]; 
            e = temp[1]; 
            f = temp[2];

            // right[][]: scattering+light source
            // Note: point (delta) source needs correction.
            right[0] = dettri / 120 * (2 * a * (3 * d + e + f) + b * (2 * d + 2 * e + f) + c * (2 * d + e + 2 * f))
            + (2 * RHS[0] + RHS[1] + RHS[2]) * source_corr;
            right[1] = dettri / 120 * (a * (2 * d + 2 * e + f) + 2 * b * (d + 3 * e + f) + c * (d + 2 * e + 2 * f))
            + (RHS[0] + 2 * RHS[1] + RHS[2]) * source_corr;
            right[2] = dettri / 120 * (a * (2 * d + e + 2 * f) + b * (d + 2 * e + 2 * f) + 2 * c * (d + e + 3 * f))
            + (RHS[0] + RHS[1] + 2 * RHS[2]) * source_corr;
        }


        

        public void MatrixSolver(double[,] A, double[] B, double[] sol)
        // Purpose: this function is to solve a 3 by 3 nonsingular linear system by Gaussian elimination with pivoting.
        {
            int[] ind = new int[3] { 0, 1, 2 };
            int i, j, temp2;
            double temp;

            if (A[0, 0] == 0)
            {
                if (A[1, 0] == 0)
                { ind[0] = 2; ind[2] = 0; }
                else
                { ind[0] = 1; ind[1] = 0; }
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
                    { A[ind[i], j] = A[ind[i], j] / temp - A[ind[0], j]; }
                }
            }

            if (A[ind[1], 1] == 0)
            { temp2 = ind[1]; ind[1] = ind[2]; ind[2] = temp2; }
            temp = A[ind[1], 1];
            B[ind[1]] = B[ind[1]] / temp;
            for (j = 1; j < 3; j++)
            { A[ind[1], j] = A[ind[1], j] / temp; }

            if (A[ind[2], 1] == 0)
            { }
            else
            {
                temp = A[ind[2], 1];
                B[ind[2]] = B[ind[2]] / temp - B[ind[1]];
                for (j = 1; j < 3; j++)
                { A[ind[2], j] = A[ind[2], j] / temp - A[ind[1], j]; }
            }

            sol[2] = B[ind[2]] / A[ind[2], 2];
            sol[1] = (B[ind[1]] - sol[2] * A[ind[1], 2]) / A[ind[1], 1];
            sol[0] = (B[ind[0]] - sol[2] * A[ind[0], 2] - sol[1] * A[ind[0], 1]) / A[ind[0], 0];
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

        public void CtoF_s2(int nt_c, double[][] f, double[][] cf2, int[][] smap, double[][][][] cf)
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

            p = cmesh.p;
            t = cmesh.t;
            c_c = cmesh.c;
            c_f = fmesh.c;
            a = cmesh.a;
            nt_f = fmesh.nt;
            nt_c = cmesh.nt;

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
                    tri = FindMin(nt_c, distance);// find the triangle with minimal distance between (x,y) and centers of coarse triangles.
                    distance[tri] = 1e10;
                    x1 = p[t[tri][0]][0]; y1 = p[t[tri][0]][1];
                    x2 = p[t[tri][1]][0]; y2 = p[t[tri][1]][1];
                    x3 = p[t[tri][2]][0]; y3 = p[t[tri][2]][1];

                    area1 = Area(x, y, x2, y2, x3, y3);
                    area2 = Area(x1, y1, x, y, x3, y3);
                    area3 = Area(x1, y1, x2, y2, x, y);
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

            tf = fmesh.t; pf = fmesh.p; af = fmesh.a;
            nt_c = cmesh.nt; p = cmesh.p; t = cmesh.t; a = cmesh.a;

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
                tempd = Area(x1, y1, x2, y2, x3, y3);
                for (j = 0; j < np; j++)
                {
                    x = pf[temp[j][0]][0]; y = pf[temp[j][0]][1];
                    temp2[j][1] = Area(x1, y1, x, y, x3, y3) / tempd;
                    temp2[j][2] = Area(x1, y1, x2, y2, x, y) / tempd;
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


        public int FindMin(int n, double[] d)
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

    }
}
