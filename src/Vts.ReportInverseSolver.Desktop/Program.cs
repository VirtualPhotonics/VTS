using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Vts.IO;
using Vts.Factories;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.Optimizers;
using Vts.Extensions;
using Vts.Common;

namespace Vts.ReportInverseSolver.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            //resources path definition:
            var projectName = "Vts.ReportInverseSolver.Desktop";
            var inputPath = @"..\..\Resources\";
            string currentAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            inputPath = currentAssemblyDirectoryName + "\\" + inputPath;
            
            //choose data & run report for R(r,t)
            double[] dts = { 0.005, 0.025 };//ps
            double[] riseMarkers = { 80.0, 50.0 };// % peak value
            double[] tailMarkers = { 20.0, 1.0, 0.1 };// % peak value
            string stDevMode = "A";// U = Uniform, A = Absolute, R = Relative
            InverseFitType[] IFTs = { InverseFitType.MuaMusp, InverseFitType.Mua, InverseFitType.Musp };//recovered op
            foreach (var dt in dts)
            {
                foreach (var riseMarker in riseMarkers)
                {
                    foreach (var tailMarker in tailMarkers)
                    {
                        foreach (var IFT in IFTs)
                        {
                            ReportInverseSolverRofRhoAndT(dt, riseMarker, tailMarker, stDevMode, IFT, projectName, inputPath);  
                        }
                    }
                }
            }
            Console.WriteLine(" -------------- THE END for RofRhoAndT --------------");
            Console.ReadLine();
        }

      
        private static void ReportInverseSolverRofRhoAndT(double dt,
                                                          double riseMarker,
                                                          double tailMarker,
                                                          string stDevMode,
                                                          InverseFitType IFT,
                                                          string projectName,
                                                          string inputPath)
        {
            bool stepByStep = false;
            Console.WriteLine("#############################################");
            Console.WriteLine("##### REPORT INVERSE SOLVER: RofRhoAndT #####");
            Console.WriteLine("#############################################");
            //path definition
            string spaceDomainFolder = "Real";
            string timeDomainFolder = "TimeDomain";
            string problemFolder = "dt" + (dt * 1000).ToString() + "markers" + riseMarker.ToString() +
                                    tailMarker.ToString();
            problemFolder = problemFolder.Replace(".", "p"); 
            //fs definition
            var forwardSolverTypes = new ForwardSolverType[]
                      {
                          //ForwardSolverType.PointSDA,
                          //ForwardSolverType.MonteCarlo,
                          ForwardSolverType.Nurbs,
                          //ForwardSolverType.DistributedPointSDA,
                          //ForwardSolverType.DistributedGaussianSDA,
                          //ForwardSolverType.DeltaPOne,
                      };
            //optimizer definition
            var optimizerTypes = new OptimizerType[]
                      {
                          OptimizerType.MPFitLevenbergMarquardt,
                      };
            //optical properties definition
            var g = 0.8;
            var n = 1.4;
            //guess
            var guessMuas = new double[] { 0.001, 0.01, 0.1};//[mm-1]
            var guessMusps = new double[] { 0.5, 1, 1.5, 2 };//[mm-1]
            var guessOps =
                      from musp in guessMusps
                      from mua in guessMuas
                      select new OpticalProperties(mua, musp, g, n);
            //real
            var realMuas = new double[] { 0.001, 0.01, 0.03, 0.1, 0.3 };//[mm-1]
            var realMusps = new double[] { 0.5, 0.7, 1.0, 1.2, 1.5, 2.0 };//[mm-1]
            var realOps =
                      from musp in realMusps
                      from mua in realMuas
                      select new OpticalProperties(mua, musp, g, n);

            // s-d separations, match folders
            double[] rhos = new double[] { 0.375, 1.125, 2.125, 4.875, 9.875, 14.875, 19.875 };//[mm]

            foreach (var fST in forwardSolverTypes)
            {
                
                //initialize forward solver
                var fs = SolverFactory.GetForwardSolver(fST);
                Console.WriteLine("Forward Solver Type: {0}", fST.ToString());
                foreach (var oT in optimizerTypes)
                {
                    var optimizer = SolverFactory.GetOptimizer(oT);
                    Console.WriteLine("Optimizer Type: {0}", oT.ToString());
                    foreach (var rho in rhos)
                    {
                        string rhoFolder = rho.ToString();
                        Console.WriteLine("=================================================");
                        Console.WriteLine("SOURCE DETECTOR SEPARETION: R = {0} mm", rhoFolder);
                        if (stepByStep) { Console.WriteLine("Press enter to continue"); }
                        Console.WriteLine("=================================================");
                        if (stepByStep) { Console.ReadLine(); }
                        rhoFolder = rhoFolder.Replace(".", "p");
                        rhoFolder = "rho" + rhoFolder;
                        double[] constantVals = { rho };
                        
                        foreach (var rOp in realOps)
                        {
                            //output 
                            double bestMua = 0.0;
                            double meanMua = 0.0;
                            double guessBestMua = 0.0;
                            double bestMusp = 0.0;
                            double meanMusp = 0.0;
                            double guessBestMusp = 0.0;
                            double bestChiSquared = 10000000000000.0;//initialize very large to avoid if first
                            double meanChiSquared = 0.0;
                            DateTime start = new DateTime();//processing start time
                            DateTime end = new DateTime();//processing finish time
                            double elapsedSeconds;//processing time
                            
                            //set filename based on real optical properties
                            var filename = "musp" + rOp.Musp.ToString() + "mua" + rOp.Mua.ToString();
                            filename = filename.Replace(".", "p");
                            Console.WriteLine("Looking for file {0}", filename);

                            if (File.Exists(inputPath + spaceDomainFolder + "/" + timeDomainFolder + "/" + problemFolder + "/" + rhoFolder + "/" + filename + "Range"))
                            {
                                Console.WriteLine("The file has been found for rho = {0} mm.", rho);
                                //read binary files
                                var timeRange = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                                                      ("Resources/" + spaceDomainFolder + "/" + timeDomainFolder + "/" + problemFolder + "/" + rhoFolder + "/" + filename + "Range", projectName, 2);
                                int numberOfPoints = Convert.ToInt32((timeRange[1] - timeRange[0]) / dt) + 1;
                                var T = new DoubleRange(timeRange[0], timeRange[1], numberOfPoints).AsEnumerable().ToArray();
                                var R = (IEnumerable<double>)FileIO.ReadArrayFromBinaryInResources<double>
                                                      ("Resources/" + spaceDomainFolder + "/" + timeDomainFolder + "/" + problemFolder + "/" + rhoFolder + "/" + filename + "R", projectName, numberOfPoints);
                                var S = GetStandardDeviationValues("Resources/" + spaceDomainFolder + "/" + timeDomainFolder + "/" + problemFolder + "/" + rhoFolder + "/" + filename + "S",
                                                                   projectName, stDevMode, numberOfPoints, R.ToArray());
                                start = DateTime.Now;
                                foreach (var gOp in guessOps)
                                {
                                    if (IFT == InverseFitType.Mua) { gOp.Musp = rOp.Musp; }
                                    if (IFT == InverseFitType.Musp) { gOp.Mua = rOp.Mua; } 
                                    //solve inverse problem
                                    double[] fit = ComputationFactory.ConstructAndExecuteVectorizedOptimizer(
                                                                   fs, optimizer, SolutionDomainType.RofRhoAndT,
                                                                   IndependentVariableAxis.T, T, R, S, gOp,
                                                                   IFT, constantVals);
                                    OpticalProperties fOp = new OpticalProperties(fit[0], fit[1], g, n);
                                    //calculate chi squared and change values if it improved
                                    double chiSquared = EvaluateChiSquared(R.ToArray(), fs.RofRhoAndT(fOp.AsEnumerable(), rho.AsEnumerable(), T).ToArray(), S.ToArray());
                                    if (chiSquared < bestChiSquared)
                                    {
                                        guessBestMua = gOp.Mua;
                                        bestMua = fit[0];
                                        guessBestMusp = gOp.Musp;
                                        bestMusp = fit[1];
                                        bestChiSquared = chiSquared;
                                    }
                                    meanMua += fit[0];
                                    meanMusp += fit[1];
                                    meanChiSquared += chiSquared;
                                }
                                end = DateTime.Now;
                                meanMua /= guessOps.Count();
                                meanMusp /= guessOps.Count();
                                meanChiSquared /= guessOps.Count();
                                elapsedSeconds = (end - start).TotalSeconds;
                                
                                MakeDirectoryIfNonExistent(spaceDomainFolder, timeDomainFolder, problemFolder, fST.ToString(), oT.ToString(), IFT.ToString(), rhoFolder);
                                //write results to array
                                double[] inverseProblemValues = FillInverseSolverValuesArray(bestMua, meanMua, guessBestMua,
                                                                                             bestMusp, meanMusp, guessBestMusp,
                                                                                             bestChiSquared, meanChiSquared, 
                                                                                             elapsedSeconds, numberOfPoints);
                                // write array to binary
                                LocalWriteArrayToBinary<double>(inverseProblemValues, @"Output/" + spaceDomainFolder + "/" +
                                                                timeDomainFolder + "/" + problemFolder + "/" + fST.ToString() + "/" +
                                                                oT.ToString() + "/" + IFT.ToString() + "/" + rhoFolder + "/" + filename, FileMode.Create);
                                
                                Console.WriteLine("Real MUA = {0} - best MUA = {1} - mean MUA = {2}", rOp.Mua, bestMua, meanMua);
                                Console.WriteLine("Real MUSp = {0} - best MUSp = {1} - mean MUSp = {2}", rOp.Musp, bestMusp, meanMusp);
                                if (stepByStep) { Console.ReadLine(); }
                            }
                            else
                            {
                                Console.WriteLine("The file has not been found.");
                            }
                            
                            Console.Clear();
                        }
                    }
                }
            }
        }

        private static double[] FillInverseSolverValuesArray(double bestMua, double meanMua,
                                                       double guessBestMua, double bestMusp,
                                                      double meanMusp, double guessBestMusp,
                                               double bestChiSquared, double meanChiSquared,
                                                     double elapsedSeconds, int numberOfPoints)
        {
            double[] inverseProblemValues = new double[10];

            inverseProblemValues[0] = bestMua;
            inverseProblemValues[1] = meanMua;
            inverseProblemValues[2] = guessBestMua;
            inverseProblemValues[3] = bestMusp;
            inverseProblemValues[4] = meanMusp;
            inverseProblemValues[5] = guessBestMusp;
            inverseProblemValues[6] = bestChiSquared;
            inverseProblemValues[7] = meanChiSquared;
            inverseProblemValues[8] = elapsedSeconds;
            inverseProblemValues[9] = numberOfPoints;

            return inverseProblemValues; ;
        }

        private static IEnumerable<double> GetStandardDeviationValues(string path, string projectName, string stDevMode, int numberOfPoints, double[] R)
        {
            double[] S = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                                                      (path, projectName, numberOfPoints);
            if (stDevMode == "U")
            {
                for (int i = 0; i < S.Length; i++)
                {
                    S[i] = 1.0;
                }
            }
            else if (stDevMode == "R")
            {
                for (int i = 0; i < S.Length; i++)
                {
                    S[i] = S[i] / R[i];   
                }
            }
            return S;
        }

        private static void MakeDirectoryIfNonExistent(string sDTFolder, string tDTFolder, string problemFolder, string fsFolder, string optimFolder, string IFTFolder, string constantValueFolder)
        {
            if (!(Directory.Exists(@"Output/" + sDTFolder + "/" + tDTFolder + "/" + problemFolder + "/" + fsFolder + "/" + optimFolder + "/" + constantValueFolder)))
            {
                Directory.CreateDirectory(@"Output/" + sDTFolder + "/" + tDTFolder + "/" + problemFolder + "/" + fsFolder + "/" + optimFolder + "/" + IFTFolder + "/" + constantValueFolder);
            }
        }

        private static void LocalWriteArrayToBinary<T>(Array dataIN, string filename, FileMode mode) where T : struct
        {
            // Create a file to write binary data 
            using (Stream s = StreamFinder.GetFileStream(filename, mode))
            {
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter<T>().WriteToBinary(bw, dataIN);
                }
            }
        }

        private static double EvaluateChiSquared(double[] measuredR, double[] modelR, double[] measuredS)
        {
            double chiSquared = 0.0;
            for (int i = 0; i < measuredR.Length; i++)
            {
                chiSquared += (measuredR[i] - modelR[i]) * (measuredR[i] - modelR[i]) / (measuredS[i] * measuredS[i]);
            }
            return chiSquared;
        }
    }
}
