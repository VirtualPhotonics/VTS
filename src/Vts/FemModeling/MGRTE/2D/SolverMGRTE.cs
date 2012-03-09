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
    /// <summary>
    /// MG TRE Solver
    /// </summary>
    public class SolverMGRTE
    {        
        /// <summary>
        /// Execute MG RTE solver
        /// </summary>
        /// <param name="input">Simulation inputs</param>
        /// <returns>measurements</returns>
        public static Measurement ExecuteMGRTE(SimulationInput input)
        {
            
            int nMismatch;  
            int i, j, k, m, n, ns, nt1, nt2, ns1, ns2, da, ds;
            int nf = 0;
            int level;
            double res = 0, res0 = 1, rho = 1.0;
            
           
            ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(SolverMGRTE));   

            // step 1: initialization

            /* Read the initial time. */
            DateTime startTime1 = DateTime.Now;

            if (Math.Abs(input.TissueInput.Regions[0].RegionOP.N - input.TissueInput.Regions[1].RegionOP.N) 
                / input.TissueInput.Regions[0].RegionOP.N < 0.01) // refraction index mismatch at the boundary
                nMismatch = 1; 
            else
                nMismatch = 0; 
                 

            //  step 2: compute "level"
            //  level: the indicator of mesh levels in multigrid          

            ds = input.MeshDataInput.SMeshLevel - input.SimulationParameterInput.StartingSmeshLevel;
            da = input.MeshDataInput.AMeshLevel - input.SimulationParameterInput.StartingAmeshLevel;
                        

            switch (input.SimulationParameterInput.MethodMg)
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
            AngularMesh[] amesh = new AngularMesh[input.MeshDataInput.AMeshLevel + 1];
            SpatialMesh[] smesh = new SpatialMesh[input.MeshDataInput.SMeshLevel + 1];
            BoundaryCoupling[] b = new BoundaryCoupling[level + 1];

            int[][] noflevel = new int[level + 1][];
            double[][][] ua = new double[input.MeshDataInput.SMeshLevel + 1][][];
            double[][][] us = new double[input.MeshDataInput.SMeshLevel + 1][][];
            double[][][][] RHS = new double[level + 1][][][];
            double[][][][] d = new double[level + 1][][][];
            double[][][][] flux = new double[level + 1][][][];
            double[][][][] q = new double[level + 1][][][];

            MultiGridCycle Mgrid = new MultiGridCycle();
            OutputCalculation Rteout = new OutputCalculation();
 
            //Avoid g value equal to 1
            //if (input.TissueInput. >= 1.0)
            //    input.TissueInput. = 1 - 1e-5;

            //if (input.TissueInput. >= 1.0)
            //    input.TissueInput. = 1 - 1e-5;

            
            //Create spatial and angular mesh
            MathFunctions.CreateAnglularMesh(ref amesh, input.MeshDataInput.AMeshLevel, input.TissueInput.Regions[1].RegionOP.G);      
            MathFunctions.CreateSquareMesh(ref smesh, input.MeshDataInput.SMeshLevel, input.MeshDataInput.SideLength);

            MathFunctions.SweepOrdering(ref smesh, amesh, input.MeshDataInput.SMeshLevel, input.MeshDataInput.AMeshLevel);
            MathFunctions.SetMus(ref us, smesh, input);
            MathFunctions.SetMua(ref ua, smesh, input);

            // load optical property, angular mesh, and spatial mesh files
            Initialization.Initial(
                ref amesh, ref smesh, ref flux, ref d, 
                ref RHS, ref q, ref noflevel, ref b,
                level, input.SimulationParameterInput.MethodMg,nMismatch,input.SimulationParameterInput.NExternal,
                input.SimulationParameterInput.NExternal,input.MeshDataInput.AMeshLevel, input.SimulationParameterInput.StartingAmeshLevel,
                input.MeshDataInput.SMeshLevel, input.SimulationParameterInput.StartingSmeshLevel, ua, us, Mgrid);           

           

            //todo: Assign an external source 
            IExtSource extsource = FemSourceFactory.GetExtSource(new ExtPointSourceInput(new DoubleRange(0, 0), new DoubleRange(Math.PI, 2.0 * Math.PI)));
            extsource.AssignMeshForExtSource(amesh, input.MeshDataInput.AMeshLevel, smesh, input.MeshDataInput.SMeshLevel, level, q);

            //Assign an internal source
            //IIntSource intsource = FemSourceFactory.GetIntSource(new Int2DPointSourceInput(new DoubleRange(0, 0.5), new DoubleRange(0, 2 * Math.PI)));
            //intsource.AssignMeshForIntSource(amesh, input.MeshDataInput.AMeshLevel, smesh, input.MeshDataInput.SMeshLevel, level,RHS);

           
            /* Read the end time. */
            DateTime stopTime1 = DateTime.Now;
            /* Compute and print the duration of this first task. */
            TimeSpan duration1 = stopTime1 - startTime1;
            
            logger.Info(() => "Initlalization for RTE_2D takes " + duration1.TotalSeconds + " seconds\n"); 

            //step 2: RTE solver
            DateTime startTime2 = DateTime.Now;

            ns = amesh[input.MeshDataInput.AMeshLevel].Ns;
            

            if (input.SimulationParameterInput.FullMg == 1)
            {
                nt2 = smesh[noflevel[level][0]].Nt;
                ns2 = amesh[noflevel[level][1]].Ns;
                for (n = level - 1; n >= 0; n--)
                {
                    nt1 = smesh[noflevel[n][0]].Nt;
                    ns1 = amesh[noflevel[n][1]].Ns;
                    if (nt1 == nt2)
                    {
                        Mgrid.FtoC_a(nt1, ns1, RHS[n + 1], RHS[n]);
                    }
                    else
                    {
                        if (ns1 == ns2)
                        {
                            Mgrid.FtoC_s(nt1, ns1, RHS[n + 1], RHS[n], smesh[noflevel[n][0] + 1].Smap, smesh[noflevel[n][0] + 1].Fc);
                        }
                        else
                        {
                            Mgrid.FtoC(nt1, ns1, RHS[n + 1], RHS[n], smesh[noflevel[n][0] + 1].Smap, smesh[noflevel[n][0] + 1].Fc);
                        }

                    }
                    nt2 = nt1; ns2 = ns1;
                }

                nt1 = smesh[noflevel[0][0]].Nt;
                ns1 = amesh[noflevel[0][1]].Ns;

                for (n = 0; n < level; n++)
                {
                    if (input.SimulationParameterInput.MethodMg == 6)
                    {
                        if (((level - n) % 2) == 0)
                        {
                            for (i = 0; i < input.SimulationParameterInput.NCycle; i++)
                            {
                                res = Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, input.SimulationParameterInput.NPreIterations, input.SimulationParameterInput.NPostIterations, 
                                    noflevel[n][1], input.SimulationParameterInput.StartingAmeshLevel, noflevel[n][0], input.SimulationParameterInput.StartingSmeshLevel, ns, nMismatch, 6);
                            }
                        }
                        else
                        {
                            for (i = 0; i < input.SimulationParameterInput.NCycle; i++)
                            {
                                Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, input.SimulationParameterInput.NPreIterations, input.SimulationParameterInput.NPostIterations, 
                                    noflevel[n][1], input.SimulationParameterInput.StartingAmeshLevel, noflevel[n][0], input.SimulationParameterInput.StartingSmeshLevel, ns, nMismatch, 7);
                            }
                        }
                    }
                    else
                    {
                        if (input.SimulationParameterInput.MethodMg== 7)
                        {
                            if (((level - n) % 2) == 0)
                            {
                                for (i = 0; i < input.SimulationParameterInput.NCycle; i++)
                                {
                                    Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, input.SimulationParameterInput.NPreIterations, input.SimulationParameterInput.NPostIterations, 
                                        noflevel[n][1], input.SimulationParameterInput.StartingAmeshLevel, noflevel[n][0], input.SimulationParameterInput.StartingSmeshLevel, ns, nMismatch, 7);
                                }
                            }
                            else
                            {
                                for (i = 0; i < input.SimulationParameterInput.NCycle; i++)
                                {
                                    Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, input.SimulationParameterInput.NPreIterations, input.SimulationParameterInput.NPostIterations, 
                                        noflevel[n][1], input.SimulationParameterInput.StartingAmeshLevel, noflevel[n][0], input.SimulationParameterInput.StartingSmeshLevel, ns, nMismatch, 6);
                                }
                            }
                        }
                        else
                        {
                            for (i = 0; i < input.SimulationParameterInput.NCycle; i++)
                            {
                                Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, input.SimulationParameterInput.NPreIterations, input.SimulationParameterInput.NPostIterations, 
                                    noflevel[n][1], input.SimulationParameterInput.StartingAmeshLevel, noflevel[n][0], input.SimulationParameterInput.StartingSmeshLevel, ns, nMismatch, 
                                    input.SimulationParameterInput.MethodMg);
                            }
                        }
                    }

                    nt2 = smesh[noflevel[n + 1][0]].Nt;
                    ns2 = amesh[noflevel[n + 1][1]].Ns;
                    if (nt1 == nt2)
                    {
                        Mgrid.CtoF_a(nt1, ns1, flux[n + 1], flux[n]);
                    }
                    else
                    {
                        if (ns1 == ns2)
                        {
                            Mgrid.CtoF_s(nt1, ns1, flux[n + 1], flux[n], smesh[noflevel[n][0] + 1].Smap, smesh[noflevel[n][0] + 1].Cf);
                        }
                        else
                        {
                            Mgrid.CtoF(nt1, ns1, flux[n + 1], flux[n], smesh[noflevel[n][0] + 1].Smap, smesh[noflevel[n][0] + 1].Cf);
                        }
                    }
                    nt1 = nt2; ns1 = ns2;
                    for (m = 0; m <= n; m++)
                    {
                        for (i = 0; i < amesh[noflevel[m][1]].Ns; i++)
                        {
                            for (j = 0; j < smesh[noflevel[m][0]].Nt; j++)
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

            while (n < input.SimulationParameterInput.NIterations)
            {
                n++;
                res = Mgrid.MgCycle(amesh, smesh, b, q, RHS, ua, us, flux, d, input.SimulationParameterInput.NPreIterations, input.SimulationParameterInput.NPostIterations, input.MeshDataInput.AMeshLevel, 
                    input.SimulationParameterInput.StartingAmeshLevel, input.MeshDataInput.SMeshLevel, input.SimulationParameterInput.StartingSmeshLevel, ns, nMismatch, input.SimulationParameterInput.MethodMg);
                for (m = 0; m < level; m++)
                {
                    for (i = 0; i < amesh[noflevel[m][1]].Ns; i++)
                    {
                        for (j = 0; j < smesh[noflevel[m][0]].Nt; j++)
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

                    if (res < input.SimulationParameterInput.ConvTolerance)
                    {
                        rho = Math.Pow(rho, 1.0 / (n - 1));
                        nf = n;
                        n = input.SimulationParameterInput.NIterations;
                    }
                }
                else
                {
                    logger.Info(() => "Iteration: " + n + ", Current tolerance: " + res + "\n");
                    res0 = res;
                    if (res < input.SimulationParameterInput.ConvTolerance)                    
                        n = input.SimulationParameterInput.NIterations;                    
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
            Measurement measurement = Rteout.RteOutput(flux[level], q[level], amesh[input.MeshDataInput.AMeshLevel], smesh[input.MeshDataInput.SMeshLevel], b[level], nMismatch);

            return measurement;
        }
        
    }
}
