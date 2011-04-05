using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vts.Modeling.ForwardSolvers.MGRTE._2D.DataStructures;
using Vts.Modeling.ForwardSolvers.MGRTE._2D.IO;
using Vts.Common;

namespace Vts.Modeling.ForwardSolvers.MGRTE._2D
{
    public class SolverMGRTE
    {
        /*Multigrid*/
        //AMG = 1;
        //SMG = 2;
        //MG1 = 3;
        //MG2 = 4;
        //MG3 = 5;
        //MG4_a = 6;
        //MG4_s = 7;

        /*Source type*/
        //NO_SOURCE = 0;                            
        //POINT_SOURCE = 1;                         
        //POINT_SOURCE_ISO = 2;                     
        //NODAL_VALUED_SOURCE_ISO = 3;            
        
        //public static void Main()
        //{
        //    string parametersFile = "parameters.txt";

        //    // 0.9. load parameters from "parameters.txt"
        //    Parameters para = ParametersIO.ReadParametersFromFile(parametersFile);

        //    // Purpose: this is the main function for RTE_2D.
        //    // Note: we assume the spatial domain has "nt" intervals,
        //    //       starting from "0" to "nt-1" with increasing "x" coordinate;
        //    //       the top boundary with bigger "x" is labeled as "1" and the bottom as "0";
        //    //       in each interval, the node with the smaller "x" is labeled as "0" and the node with the bigger "x" is labeled as "1".
        //    ExecuteMGRTE(para);

        //    Console.ReadLine();
        //}
                

        public static void ExecuteMGRTE(Parameters para)
        {

            int level = -1;
            int vacuum;
            int i, j, k, m, n, ns, nt1, nt2, ns1, ns2, da, ds, nf = 0;
            double res = 0, res0 = 1, rho = 1.0;

            // step 1: initialization

            /* Read the initial time. */
            DateTime startTime1 = DateTime.Now;

            if (Math.Abs(para.Index_i - para.Index_o) / para.Index_i < 0.01) // refraction index mismatch at the boundary
            { 
                vacuum = 1; 
            }
            else
            { 
                vacuum = 0; 
            }

            // 1.2. compute "level"
            //      level: the indicator of mesh levels in multigrid
            ds = para.Slevel - para.Slevel0;
            da = para.Alevel - para.Alevel0;

            

            switch (para.Whichmg)
            {
                case 1:
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

            AngularMesh[] amesh = new AngularMesh[para.Alevel + 1];
            SpatialMesh[] smesh = new SpatialMesh[para.Slevel + 1];
            BoundaryCoupling[] b = new BoundaryCoupling[level + 1];

            int[][] noflevel = new int[level + 1][];
            double[][][] ua = new double[para.Slevel + 1][][];
            double[][][] us = new double[para.Slevel + 1][][];
            double[][][][] RHS = new double[level + 1][][][];
            double[][][][] d = new double[level + 1][][][];
            double[][][][] flux = new double[level + 1][][][];
            double[][][][] q = new double[level + 1][][][];

            MultiGridCycle Mgrid = new MultiGridCycle();
            Output Rteout = new Output();
            Source Insource = new Source();

            ReadFiles.ReadAmesh(ref amesh, para.Alevel);
            ReadFiles.ReadSmesh(ref smesh, para.Slevel, para.Alevel, amesh);
            ReadFiles.ReadMua(ref ua, para.Slevel, smesh[para.Slevel].nt);
            ReadFiles.ReadMus(ref us, para.Slevel, smesh[para.Slevel].nt);

            // load optical property, angular mesh, and spatial mesh files
            Initialization.Initial(
                ref amesh, ref smesh, ref flux, ref d, 
                ref RHS, ref q, ref noflevel, ref b,
                level, para.Whichmg,vacuum,para.Index_i,
                para.Index_o,para.Alevel,para.Alevel0, 
                para.Slevel,para.Slevel0, ua,us,Mgrid);           


            // initialize internal and boundary sources 
            Insource.Inputsource(para.Alevel, amesh, para.Slevel, smesh, level, RHS, q);

            /* Read the end time. */
            DateTime stopTime1 = DateTime.Now;
            /* Compute and print the duration of this first task. */
            TimeSpan duration1 = stopTime1 - startTime1;


            Console.WriteLine("Initlalization for RTE_2D takes {0} seconds.\n", duration1.TotalSeconds);

            //step 2: RTE solver
            DateTime startTime2 = DateTime.Now;
            ns = amesh[para.Alevel].ns;

            if (para.Fmg == 1)
            {
                nt2 = smesh[noflevel[level][0]].nt;
                ns2 = amesh[noflevel[level][1]].ns;
                for (n = level - 1; n >= 0; n--)
                {
                    nt1 = smesh[noflevel[n][0]].nt;
                    ns1 = amesh[noflevel[n][1]].ns;
                    if (nt1 == nt2)
                    {
                        Mgrid.FtoC_a(nt1, ns1, RHS[n + 1], RHS[n]);
                    }
                    else
                    {
                        if (ns1 == ns2)
                        {
                            Mgrid.FtoC_s(nt1, ns1, RHS[n + 1], RHS[n], smesh[noflevel[n][0] + 1].smap, smesh[noflevel[n][0] + 1].fc);
                        }
                        else
                        {
                            Mgrid.FtoC(nt1, ns1, RHS[n + 1], RHS[n], smesh[noflevel[n][0] + 1].smap, smesh[noflevel[n][0] + 1].fc);
                        }

                    }
                    nt2 = nt1; ns2 = ns1;
                }

                nt1 = smesh[noflevel[0][0]].nt;
                ns1 = amesh[noflevel[0][1]].ns;

                for (n = 0; n < level; n++)
                {
                    if (para.Whichmg == 6)
                    {
                        if (((level - n) % 2) == 0)
                        {
                            for (i = 0; i < para.N3; i++)
                            {
                                res = Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, para.N1, para.N2, noflevel[n][1], para.Alevel0, noflevel[n][0], para.Slevel0, ns, vacuum, 6);
                            }
                        }
                        else
                        {
                            for (i = 0; i < para.N3; i++)
                            {
                                Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, para.N1, para.N2, noflevel[n][1], para.Alevel0, noflevel[n][0], para.Slevel0, ns, vacuum, 7);
                            }
                        }
                    }
                    else
                    {
                        if (para.Whichmg == 7)
                        {
                            if (((level - n) % 2) == 0)
                            {
                                for (i = 0; i < para.N3; i++)
                                {
                                    Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, para.N1, para.N2, noflevel[n][1], para.Alevel0, noflevel[n][0], para.Slevel0, ns, vacuum, 7);
                                }
                            }
                            else
                            {
                                for (i = 0; i < para.N3; i++)
                                {
                                    Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, para.N1, para.N2, noflevel[n][1], para.Alevel0, noflevel[n][0], para.Slevel0, ns, vacuum, 6);
                                }
                            }
                        }
                        else
                        {
                            for (i = 0; i < para.N3; i++)
                            {
                                Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, para.N1, para.N2, noflevel[n][1], para.Alevel0, noflevel[n][0], para.Slevel0, ns, vacuum, para.Whichmg);
                            }
                        }
                    }

                    nt2 = smesh[noflevel[n + 1][0]].nt;
                    ns2 = amesh[noflevel[n + 1][1]].ns;
                    if (nt1 == nt2)
                    {
                        Mgrid.CtoF_a(nt1, ns1, flux[n + 1], flux[n]);
                    }
                    else
                    {
                        if (ns1 == ns2)
                        {
                            Mgrid.CtoF_s(nt1, ns1, flux[n + 1], flux[n], smesh[noflevel[n][0] + 1].smap, smesh[noflevel[n][0] + 1].cf);
                        }
                        else
                        {
                            Mgrid.CtoF(nt1, ns1, flux[n + 1], flux[n], smesh[noflevel[n][0] + 1].smap, smesh[noflevel[n][0] + 1].cf);
                        }
                    }
                    nt1 = nt2; ns1 = ns2;
                    for (m = 0; m <= n; m++)
                    {
                        for (i = 0; i < amesh[noflevel[m][1]].ns; i++)
                        {
                            for (j = 0; j < smesh[noflevel[m][0]].nt; j++)
                            {
                                for (k = 0; k < 3; k++)
                                {
                                    flux[m][i][j][k] = 0;
                                }
                            }
                        }
                    }
                }
            }

            // 2.2. multigrid solver on the finest mesh
            n = 0;
            Console.WriteLine("Iter\t\t Res\t Rho\t\t T (in seconds) ");

            while (n < para.N_max)
            {
                /* Read the start time. */
                startTime1 = DateTime.Now;

                n++;
                res = Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, para.N1, para.N2, para.Alevel, para.Alevel0, para.Slevel, para.Slevel0, ns, vacuum, para.Whichmg);
                for (m = 0; m < level; m++)
                {
                    for (i = 0; i < amesh[noflevel[m][1]].ns; i++)
                    {
                        for (j = 0; j < smesh[noflevel[m][0]].nt; j++)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                flux[m][i][j][k] = 0;
                            }
                        }
                    }
                }

                if (n > 1)
                {
                    rho *= res / res0;
                    Console.Write("{0}\t{1:e6}\t{2:N6}\t", n, res, res / res0);


                    if (res < para.Tol)
                    {
                        rho = Math.Pow(rho, 1.0 / (n - 1));
                        nf = n;
                        n = para.N_max;
                    }
                }
                else
                {
                    Console.Write("{0}\t{1:e6}\t{2:N6}\t", n, res, res / res0);
                }
                res0 = res;
                stopTime1 = DateTime.Now;
                duration1 = stopTime1 - startTime1;
                Console.Write("{0} \n", duration1.TotalSeconds);
            }

            // 2.3. compute the residual
            Mgrid.Defect(amesh[para.Alevel], smesh[para.Slevel], ns, RHS[level], ua[para.Slevel], us[para.Slevel], flux[level], b[level], q[level], d[level], vacuum);
            res = Mgrid.Residual(smesh[para.Slevel].nt, amesh[para.Alevel].ns, d[level], smesh[para.Slevel].a);

            /* Read the start time. */
            DateTime stopTime2 = DateTime.Now;
            TimeSpan duration2 = stopTime2 - startTime2;

            Console.WriteLine("\nT = {0}secs   res= {1}:\t nf= {2}:\t rho= {3:N6}", duration2.TotalSeconds, res, nf, rho);

            // step 3: postprocessing
            // 3.1. output
            Rteout.RteOutput(flux[level], q[level], amesh[para.Alevel], smesh[para.Slevel], b[level], vacuum);
        }

        

    }
}
