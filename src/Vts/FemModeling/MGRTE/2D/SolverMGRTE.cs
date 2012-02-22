using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Vts.FemModeling.MGRTE._2D.DataStructures;
using Vts.FemModeling.MGRTE._2D.SourceInputs;
using Vts.Common;
using Vts.Common.Logging;


namespace Vts.FemModeling.MGRTE._2D
{
    public class SolverMGRTE
    {        

        public static Measurement ExecuteMGRTE(SimulationInputs parameters)
        {
            
            int vacuum;
            int i, j, k, m, n, ns, nt1, nt2, ns1, ns2, da, ds;
            int nf = 0;
            int level;
            double res = 0, res0 = 1, rho = 1.0;
            int AMeshLevel0, SMeshLevel0;

            ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(SolverMGRTE));   

            // step 1: initialization
           
            /* Read the initial time. */
            DateTime startTime1 = DateTime.Now;

            if (Math.Abs(parameters.NTissue - parameters.NExt) / parameters.NTissue < 0.01) // refraction index mismatch at the boundary
            { 
                vacuum = 1; 
            }
            else
            { 
                vacuum = 0; 
            }       
     
            //These values are suggested in the MG-RTE paper.
            parameters.NIterations = 100;
            parameters.NPreIteration = 3;
            parameters.NPostIteration = 3;
            parameters.NMgCycle = 1;
            parameters.FullMg = 1;

            AMeshLevel0 = 1;
            SMeshLevel0 = 1;

            // 1.2. compute "level"
            //      level: the indicator of mesh levels in multigrid          

            ds = parameters.SMeshLevel - SMeshLevel0;
            da = parameters.AMeshLevel - AMeshLevel0;          
                        

            switch (parameters.MgMethod)
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
                default:
                    level = -1;
                    break;
            }

            //Create Dynamic arrays based on above values
            AngularMesh[] amesh = new AngularMesh[parameters.AMeshLevel + 1];
            SpatialMesh[] smesh = new SpatialMesh[parameters.SMeshLevel + 1];
            BoundaryCoupling[] b = new BoundaryCoupling[level + 1];

            int[][] noflevel = new int[level + 1][];
            double[][][] ua = new double[parameters.SMeshLevel + 1][][];
            double[][][] us = new double[parameters.SMeshLevel + 1][][];
            double[][][][] RHS = new double[level + 1][][][];
            double[][][][] d = new double[level + 1][][][];
            double[][][][] flux = new double[level + 1][][][];
            double[][][][] q = new double[level + 1][][][];

            MultiGridCycle Mgrid = new MultiGridCycle();
            OutputCalculation Rteout = new OutputCalculation();
 
            //Avoid g value equal to 1
            if (parameters.MedG >= 1.0)
                parameters.MedG = 1 - 1e-5;

            if (parameters.InG >= 1.0)
                parameters.InG = 1 - 1e-5;

            //Create spatial and angular mesh
            MathFunctions.CreateAnglularMesh(ref amesh, parameters.AMeshLevel, parameters.MedG);      
            MathFunctions.CreateSquareMesh(ref smesh, parameters.SMeshLevel, parameters.Length);

            MathFunctions.SweepOrdering(ref smesh, amesh, parameters.SMeshLevel, parameters.AMeshLevel);
            MathFunctions.SetMus(ref us, smesh, parameters);
            MathFunctions.SetMua(ref ua, smesh, parameters);

            // load optical property, angular mesh, and spatial mesh files
            Initialization.Initial(
                ref amesh, ref smesh, ref flux, ref d, 
                ref RHS, ref q, ref noflevel, ref b,
                level, parameters.MgMethod,vacuum,parameters.NTissue,
                parameters.NExt,parameters.AMeshLevel, AMeshLevel0,
                parameters.SMeshLevel, SMeshLevel0, ua, us, Mgrid);           

           

            //todo: Assign an external source 
            IExtSource extsource = FemSourceFactory.GetExtSource(new ExtPointSourceInput(new DoubleRange(0, 0), new DoubleRange(Math.PI, 2.0 * Math.PI)));
            extsource.AssignMeshForExtSource(amesh, parameters.AMeshLevel, smesh, parameters.SMeshLevel, level, q);

            //Assign an internal source
            //IIntSource intsource = FemSourceFactory.GetIntSource(new Int2DPointSourceInput(new DoubleRange(0, 0.5), new DoubleRange(0, 2 * Math.PI)));
            //intsource.AssignMeshForIntSource(amesh, parameters.AMeshLevel, smesh, parameters.SMeshLevel, level,RHS);

           
            /* Read the end time. */
            DateTime stopTime1 = DateTime.Now;
            /* Compute and print the duration of this first task. */
            TimeSpan duration1 = stopTime1 - startTime1;
            
            logger.Info(() => "Initlalization for RTE_2D takes " + duration1.TotalSeconds + " seconds\n"); 

            //step 2: RTE solver
            DateTime startTime2 = DateTime.Now;

            ns = amesh[parameters.AMeshLevel].ns;
            

            if (parameters.FullMg == 1)
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
                    if (parameters.MgMethod == 6)
                    {
                        if (((level - n) % 2) == 0)
                        {
                            for (i = 0; i < parameters.NMgCycle; i++)
                            {
                                res = Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, parameters.NPreIteration, parameters.NPostIteration, 
                                    noflevel[n][1], AMeshLevel0, noflevel[n][0], SMeshLevel0, ns, vacuum, 6);
                            }
                        }
                        else
                        {
                            for (i = 0; i < parameters.NMgCycle; i++)
                            {
                                Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, parameters.NPreIteration, parameters.NPostIteration, 
                                    noflevel[n][1], AMeshLevel0, noflevel[n][0], SMeshLevel0, ns, vacuum, 7);
                            }
                        }
                    }
                    else
                    {
                        if (parameters.MgMethod == 7)
                        {
                            if (((level - n) % 2) == 0)
                            {
                                for (i = 0; i < parameters.NMgCycle; i++)
                                {
                                    Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, parameters.NPreIteration, parameters.NPostIteration, 
                                        noflevel[n][1], AMeshLevel0, noflevel[n][0], SMeshLevel0, ns, vacuum, 7);
                                }
                            }
                            else
                            {
                                for (i = 0; i < parameters.NMgCycle; i++)
                                {
                                    Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, parameters.NPreIteration, parameters.NPostIteration, 
                                        noflevel[n][1], AMeshLevel0, noflevel[n][0], SMeshLevel0, ns, vacuum, 6);
                                }
                            }
                        }
                        else
                        {
                            for (i = 0; i < parameters.NMgCycle; i++)
                            {
                                Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, parameters.NPreIteration, parameters.NPostIteration, 
                                    noflevel[n][1], AMeshLevel0, noflevel[n][0], SMeshLevel0, ns, vacuum, parameters.MgMethod);
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

            while (n < parameters.NIterations)
            {
                n++;
                res = Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, parameters.NPreIteration, parameters.NPostIteration, parameters.AMeshLevel, 
                    AMeshLevel0, parameters.SMeshLevel, SMeshLevel0, ns, vacuum, parameters.MgMethod);
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
                    logger.Info(() => "Iteration: " + n + ", Current tolerance: " + res + "\n");  

                    if (res < parameters.ConvTol)
                    {
                        rho = Math.Pow(rho, 1.0 / (n - 1));
                        nf = n;
                        n = parameters.NIterations;
                    }
                }
                else
                {
                    logger.Info(() => "Iteration: " + n + ", Current tolerance: " + res + "\n");
                    res0 = res;
                    if (res < parameters.ConvTol)                    
                        n = parameters.NIterations;                    
                }            
                            
            }

            // 2.3. compute the residual
            //Mgrid.Defect(amesh[para.AMeshLevel], smesh[para.SMeshLevel], ns, RHS[level], ua[para.SMeshLevel], us[para.SMeshLevel], 
            //    flux[level], b[level], q[level], d[level], vacuum);
            //res = Mgrid.Residual(smesh[para.SMeshLevel].nt, amesh[para.AMeshLevel].ns, d[level], smesh[para.SMeshLevel].a);

            /* Read the start time. */
            DateTime stopTime2 = DateTime.Now;
            TimeSpan duration2 = stopTime2 - startTime2;
            TimeSpan duration3 = stopTime2 - startTime1;   

            logger.Info(() => "Iteration time: " + duration2.TotalSeconds + "seconds\n");
            logger.Info(() => "Total time: " + duration3.TotalSeconds + "seconds, Final residual: " + res + "\n");

            // step 3: postprocessing
            // 3.1. output
            Measurement measurement = Rteout.RteOutput(flux[level], q[level], amesh[parameters.AMeshLevel], smesh[parameters.SMeshLevel], b[level], vacuum);

            return measurement;
        }
        
    }
}
