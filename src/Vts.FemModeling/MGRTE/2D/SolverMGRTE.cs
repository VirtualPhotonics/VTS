using System;
using Vts.Common.Logging;
using Vts.FemModeling.MGRTE._2D.DataStructures;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

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
            int i, j, k, m, n;
            int level;
            double res = 0, res0 = 1, rho = 1.0;
            int ds = input.MeshDataInput.SMeshLevel - input.SimulationOptionsInput.StartingSmeshLevel;
            int da = input.MeshDataInput.AMeshLevel - input.SimulationOptionsInput.StartingAmeshLevel;
            
            
            /* Read the initial time. */
            DateTime startTime1 = DateTime.Now;
            ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof (SolverMGRTE));
            
            //  step 1: compute "level"
            //  level: the indicator of mesh levels in multigrid  
            switch (input.SimulationOptionsInput.MethodMg)
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
            var amesh = new AngularMesh[input.MeshDataInput.AMeshLevel + 1];
            var smesh = new SpatialMesh[input.MeshDataInput.SMeshLevel + 1];
            var b = new BoundaryCoupling[level + 1];

            var noflevel = new int[level + 1][];
            var ua = new double[input.MeshDataInput.SMeshLevel + 1][][];
            var us = new double[input.MeshDataInput.SMeshLevel + 1][][];
            var rhs = new double[level + 1][][][];
            var d = new double[level + 1][][][];
            var flux = new double[level + 1][][][];
            var q = new double[level + 1][][][];

            var mgrid = new MultiGridCycle();
            var rteout = new OutputCalculation();

            var tissueInput = (MultiEllipsoidTissueInput)input.TissueInput;
            int incRegions = tissueInput.EllipsoidRegions.Length;
            int tisRegions = tissueInput.LayerRegions.Length;

            double depth = 0.0;
            for (i = 1; i < tissueInput.LayerRegions.Length - 1; i++)
                depth += ((LayerTissueRegion)(tissueInput.LayerRegions[i])).ZRange.Stop - ((LayerTissueRegion)(tissueInput.LayerRegions[i])).ZRange.Start;

            input.MeshDataInput.SideLength = depth;
            int totRegions = incRegions + tisRegions ;

            //   MG-RTE does not converge when g = 1.0;
            for (i = 0; i < totRegions; i++)
                if (input.TissueInput.Regions[i].RegionOP.G >= 1.0)
                    input.TissueInput.Regions[i].RegionOP.G = 1.0 - 1e-5;

            // Check refractive index mismatch
            if (Math.Abs(input.TissueInput.Regions[0].RegionOP.N - input.TissueInput.Regions[1].RegionOP.N)
                / input.TissueInput.Regions[0].RegionOP.N < 0.01) // refraction index mismatch at the boundary
                nMismatch = 1;
            else
                nMismatch = 0;
            
            
            //Create spatial and angular mesh
            MathFunctions.CreateSquareMesh(ref smesh, input.MeshDataInput.SMeshLevel, depth);
            MathFunctions.AssignRegions(ref smesh, input.MeshDataInput.SMeshLevel, tissueInput);
            MathFunctions.CreateAnglularMesh(ref amesh, input.MeshDataInput.AMeshLevel, tissueInput);      
            

            MathFunctions.SweepOrdering(ref smesh, amesh, input.MeshDataInput.SMeshLevel, input.MeshDataInput.AMeshLevel);
            MathFunctions.SetMus(ref us, smesh, input);
            MathFunctions.SetMua(ref ua, smesh, input);

            // load optical property, angular mesh, and spatial mesh files
            Initialization.Initial(
                ref amesh, ref smesh, ref flux, ref d, 
                ref rhs, ref q, ref noflevel, ref b,
                level, input.SimulationOptionsInput.MethodMg,nMismatch,input.SimulationOptionsInput.NExternal,
                input.SimulationOptionsInput.NExternal,input.MeshDataInput.AMeshLevel, input.SimulationOptionsInput.StartingAmeshLevel,
                input.MeshDataInput.SMeshLevel, input.SimulationOptionsInput.StartingSmeshLevel, ua, us, mgrid);
            
            //Assign external source if available
            if (input.ExtSourceInput != null)
            {
                IExtSource extsource = FemSourceFactory.GetExtSource(input.ExtSourceInput);
                extsource.AssignMeshForExtSource(amesh, input.MeshDataInput.AMeshLevel, smesh, input.MeshDataInput.SMeshLevel, level, q);
            }

            //Assign internal source if available
            if (input.IntSourceInput != null)
            {
                //Assign an internal source
                IIntSource intsource = FemSourceFactory.GetIntSource(input.IntSourceInput);
                intsource.AssignMeshForIntSource(amesh, input.MeshDataInput.AMeshLevel, smesh, input.MeshDataInput.SMeshLevel, level, rhs);
            }
           
            /* Read the end time. */
            DateTime stopTime1 = DateTime.Now;
            /* Compute and print the duration of this first task. */
            TimeSpan duration1 = stopTime1 - startTime1;
            
            logger.Info(() => "Initlalization for RTE_2D takes " + duration1.TotalSeconds + " seconds\n"); 

            //step 2: RTE solver
            DateTime startTime2 = DateTime.Now;

            int ns = amesh[input.MeshDataInput.AMeshLevel].Ns;
            

            if (input.SimulationOptionsInput.FullMg == 1)
            {
                int nt1, ns1;
                int nt2 = smesh[noflevel[level][0]].Nt;
                int ns2 = amesh[noflevel[level][1]].Ns;
                
                for (n = level - 1; n >= 0; n--)
                {
                    nt1 = smesh[noflevel[n][0]].Nt;
                    ns1 = amesh[noflevel[n][1]].Ns;
                    if (nt1 == nt2)
                    {
                        mgrid.FtoC_a(nt1, ns1, rhs[n + 1], rhs[n]);
                    }
                    else
                    {
                        if (ns1 == ns2)
                        {
                            mgrid.FtoC_s(nt1, ns1, rhs[n + 1], rhs[n], smesh[noflevel[n][0] + 1].Smap, smesh[noflevel[n][0] + 1].Fc);
                        }
                        else
                        {
                            mgrid.FtoC(nt1, ns1, rhs[n + 1], rhs[n], smesh[noflevel[n][0] + 1].Smap, smesh[noflevel[n][0] + 1].Fc);
                        }

                    }
                    nt2 = nt1; ns2 = ns1;
                }

                nt1 = smesh[noflevel[0][0]].Nt;
                ns1 = amesh[noflevel[0][1]].Ns;

                for (n = 0; n < level; n++)
                {
                    if (input.SimulationOptionsInput.MethodMg == 6)
                    {
                        if (((level - n) % 2) == 0)
                        {
                            for (i = 0; i < input.SimulationOptionsInput.NCycle; i++)
                            {
                                res = mgrid.MgCycle(amesh, smesh, b, q, rhs, ua, us, flux, d, input.SimulationOptionsInput.NPreIterations, input.SimulationOptionsInput.NPostIterations, 
                                    noflevel[n][1], input.SimulationOptionsInput.StartingAmeshLevel, noflevel[n][0], input.SimulationOptionsInput.StartingSmeshLevel, ns, nMismatch, 6);
                            }
                        }
                        else
                        {
                            for (i = 0; i < input.SimulationOptionsInput.NCycle; i++)
                            {
                                mgrid.MgCycle(amesh, smesh, b, q, rhs, ua, us, flux, d, input.SimulationOptionsInput.NPreIterations, input.SimulationOptionsInput.NPostIterations, 
                                    noflevel[n][1], input.SimulationOptionsInput.StartingAmeshLevel, noflevel[n][0], input.SimulationOptionsInput.StartingSmeshLevel, ns, nMismatch, 7);
                            }
                        }
                    }
                    else
                    {
                        if (input.SimulationOptionsInput.MethodMg== 7)
                        {
                            if (((level - n) % 2) == 0)
                            {
                                for (i = 0; i < input.SimulationOptionsInput.NCycle; i++)
                                {
                                    mgrid.MgCycle(amesh, smesh, b, q, rhs, ua, us, flux, d, input.SimulationOptionsInput.NPreIterations, input.SimulationOptionsInput.NPostIterations, 
                                        noflevel[n][1], input.SimulationOptionsInput.StartingAmeshLevel, noflevel[n][0], input.SimulationOptionsInput.StartingSmeshLevel, ns, nMismatch, 7);
                                }
                            }
                            else
                            {
                                for (i = 0; i < input.SimulationOptionsInput.NCycle; i++)
                                {
                                    mgrid.MgCycle(amesh, smesh, b, q, rhs, ua, us, flux, d, input.SimulationOptionsInput.NPreIterations, input.SimulationOptionsInput.NPostIterations, 
                                        noflevel[n][1], input.SimulationOptionsInput.StartingAmeshLevel, noflevel[n][0], input.SimulationOptionsInput.StartingSmeshLevel, ns, nMismatch, 6);
                                }
                            }
                        }
                        else
                        {
                            for (i = 0; i < input.SimulationOptionsInput.NCycle; i++)
                            {
                                mgrid.MgCycle(amesh, smesh, b, q, rhs, ua, us, flux, d, input.SimulationOptionsInput.NPreIterations, input.SimulationOptionsInput.NPostIterations, 
                                    noflevel[n][1], input.SimulationOptionsInput.StartingAmeshLevel, noflevel[n][0], input.SimulationOptionsInput.StartingSmeshLevel, ns, nMismatch, 
                                    input.SimulationOptionsInput.MethodMg);
                            }
                        }
                    }

                    nt2 = smesh[noflevel[n + 1][0]].Nt;
                    ns2 = amesh[noflevel[n + 1][1]].Ns;
                    if (nt1 == nt2)
                    {
                        mgrid.CtoF_a(nt1, ns1, flux[n + 1], flux[n]);
                    }
                    else
                    {
                        if (ns1 == ns2)
                        {
                            mgrid.CtoF_s(nt1, ns1, flux[n + 1], flux[n], smesh[noflevel[n][0] + 1].Smap, smesh[noflevel[n][0] + 1].Cf);
                        }
                        else
                        {
                            mgrid.CtoF(nt1, ns1, flux[n + 1], flux[n], smesh[noflevel[n][0] + 1].Smap, smesh[noflevel[n][0] + 1].Cf);
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

            while (n < input.SimulationOptionsInput.NIterations)
            {
                n++;
                res = mgrid.MgCycle(amesh, smesh, b, q, rhs, ua, us, flux, d, input.SimulationOptionsInput.NPreIterations, input.SimulationOptionsInput.NPostIterations, input.MeshDataInput.AMeshLevel, 
                    input.SimulationOptionsInput.StartingAmeshLevel, input.MeshDataInput.SMeshLevel, input.SimulationOptionsInput.StartingSmeshLevel, ns, nMismatch, input.SimulationOptionsInput.MethodMg);
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

                    if (res < input.SimulationOptionsInput.ConvTolerance)
                    {
                        rho = Math.Pow(rho, 1.0 / (n - 1));
                        n = input.SimulationOptionsInput.NIterations;
                    }
                }
                else
                {
                    logger.Info(() => "Iteration: " + n + ", Current tolerance: " + res + "\n");
                    res0 = res;
                    if (res < input.SimulationOptionsInput.ConvTolerance)                    
                        n = input.SimulationOptionsInput.NIterations;                    
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
            Measurement measurement = rteout.RteOutput(flux[level], q[level], amesh[input.MeshDataInput.AMeshLevel], smesh[input.MeshDataInput.SMeshLevel], b[level], nMismatch);

            return measurement;
        }
        
    }
}
