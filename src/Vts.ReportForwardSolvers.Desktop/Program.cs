using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Vts.Common;
using Vts.Extensions;
using Vts.Factories;
using Vts.IO;

namespace Vts.ReportForwardSolvers.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            //path
            var projectName = "Vts.ReportForwardSolvers.Desktop";
            var inputPath = @"..\..\Resources\";
            string currentAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            inputPath = currentAssemblyDirectoryName + "\\" + inputPath;
            // ops definition: the fs are used to predict reflectance for the specific domains for these optical properties.
            // for R(r,t) and for R(r) the locations are not selected, but are set to those locations obtained by Equal Frequency Discretizatio
            // of MC results. 
            var g = 0.8;
            var n = 1.4;
            var muas = new double[] { 0.001, 0.01, 0.03, 0.1, 0.3 };//[mm-1]
            var musps = new double[] { 0.5, 0.7, 1.0, 1.2, 1.5, 2.0 };//[mm-1]

            var Ops =
                      from musp in musps
                      from mua in muas
                      select new OpticalProperties(mua, musp, g, n);
            //FS
            var forwardSolverTypes = new ForwardSolverType[]
                      {
                          ForwardSolverType.Nurbs,
                          //ForwardSolverType.MonteCarlo,
                          //ForwardSolverType.PointSourceSDA,
                          //ForwardSolverType.DistributedPointSDA,
                          //ForwardSolverType.DistributedGaussianSDA,
                          //ForwardSolverType.DeltaPOne,
                      };
            //sDT
            var spatialDomainTypes = new SpatialDomainType[]
                     {
                         //SpatialDomainType.Real,
                         SpatialDomainType.SpatialFrequency,
                     };
            //tDT
            var timeDomainTypes = new TimeDomainType[]
                     {
                         TimeDomainType.SteadyState,
                         //TimeDomainType.TimeDomain,
                         //TimeDomainType.FrequencyDomain,   
                     };
            //execute
            foreach (var sDT in spatialDomainTypes)
            {
                foreach (var tDT in timeDomainTypes)
                {
                    foreach (var op in Ops)
                    {
                        ReportAllForwardSolvers(forwardSolverTypes, sDT, tDT, op, inputPath, projectName);
                    }
                }
            }
        }

        #region methods

        private static void ReportAllForwardSolvers(ForwardSolverType[] forwardSolverTypes,
                                                    SpatialDomainType sDT,
                                                    TimeDomainType tDT,
                                                    OpticalProperties op,
                                                    string inputPath,
                                                    string projectName)
        {
            if (tDT == TimeDomainType.SteadyState)
            {
                ReportSteadyStateForwardSolver(forwardSolverTypes, sDT, op, inputPath, projectName);
            }
            else
            {
                Report2DForwardSolver(forwardSolverTypes, sDT, tDT, op, inputPath, projectName);
            }
        }

        private static void ReportSteadyStateForwardSolver(ForwardSolverType[] fSTs,
                                                                   SpatialDomainType sDT,
                                                                   OpticalProperties op,
                                                                   string inputPath,
                                                                   string projectName)
        {
            var filename = "musp" + op.Musp.ToString() + "mua" + op.Mua.ToString();
            filename = filename.Replace(".", "p");
            Console.WriteLine("Looking for file {0} in spatial domain type {1}", filename, sDT.ToString());

            if (File.Exists(inputPath + sDT.ToString() + "/SteadyState/" + filename)|| sDT == SpatialDomainType.SpatialFrequency)
            {
                Console.WriteLine("The file {0} has been found.", filename);
                int sDim = GetSpatialNumberOfPoints(sDT);
                IEnumerable<double> spatialVariable;
                // if R(r) not uniform locations where we evaluate, but points defined in binary files (these points are average radial positions of photons collected in each bin)
                if (sDT == SpatialDomainType.Real)
                {
                    spatialVariable = (IEnumerable<double>)FileIO.ReadArrayFromBinaryInResources<double>
                                      ("Resources/" + sDT.ToString() + "/SteadyState/" + filename, projectName, sDim);
                }
                // if R(fx) uniform evaluation, sDim used to specify number of points.
                else
                {
                    spatialVariable = new DoubleRange(0.0, 1.0, sDim).AsEnumerable(); 
                }
                // after providing spatial locations where we want evaluation execute and store results
                foreach (var fST in fSTs)
                {
                    EvaluateAndWriteForwardSolverSteadyStateResults(fST, sDT, op, spatialVariable);
                }
            }
            else
            {
                Console.WriteLine("The file {0} has not been found.",filename);
            }
        }

        private static void Report2DForwardSolver(ForwardSolverType[] fSTs,
                                                   SpatialDomainType sDT,
                                                   TimeDomainType tDT,
                                                   OpticalProperties op,
                                                   string inputPath,
                                                   string projectName)
        {
            var filename = "musp" + op.Musp.ToString() + "mua" + op.Mua.ToString();
            filename = filename.Replace(".", "p");
            Console.WriteLine("Looking for file {0} in spatial domain type {1} and temporal domain type{2}",
                                                                   filename, sDT.ToString(), tDT.ToString());
            if (File.Exists(inputPath + sDT.ToString() + "/SteadyState/" + filename) ||
                File.Exists(inputPath + sDT.ToString() + "/" + tDT.ToString() + "/" + filename))
            {
                Console.WriteLine("The file {0} has been found.", filename);
                int sDim = GetSpatialNumberOfPoints(sDT);
                int tDim = GetTemporalNumberOfPoints(sDT, tDT);
                int[] dims = { sDim, tDim };


                var spatialVariable = (IEnumerable<double>)FileIO.ReadArrayFromBinaryInResources<double>
                                      ("Resources/" + sDT.ToString() + "/" + "SteadyState/" + filename, projectName, sDim);
                var temporalVariable = (double[,])FileIO.ReadArrayFromBinaryInResources<double>
                                      ("Resources/" + sDT.ToString() + "/" + tDT.ToString() + "/" + filename, projectName, dims);
                foreach (var fST in fSTs)
                {
                    EvaluateAndWriteForwardSolver2DResults(fST, sDT, tDT, op, spatialVariable, temporalVariable);
                }
            }
            else
            {
                Console.WriteLine("The file {0} has not been found", filename);
            }
        }

        private static void EvaluateAndWriteForwardSolverSteadyStateResults(ForwardSolverType fST,
                                                                            SpatialDomainType sDT,
                                                                            OpticalProperties op,
                                                                            IEnumerable<double> spatialVariable)
        {
            double[] reflectanceValues;

            var ReflectanceFunction = GetSteadyStateReflectanceFunction(fST, sDT);

            MakeDirectoryIfNonExistent(sDT.ToString(), "SteadyState", fST.ToString());

            reflectanceValues = ReflectanceFunction(op.AsEnumerable(), spatialVariable).ToArray();

            LocalWriteArrayToBinary(reflectanceValues,@"Output/" + sDT.ToString() +
                                              "/SteadyState/" + fST.ToString() + "/" +
                                              "musp" + op.Musp.ToString() + "mua" + op.Mua.ToString(),FileMode.Create);
        }

        private static void EvaluateAndWriteForwardSolver2DResults(ForwardSolverType fST,
                                                                   SpatialDomainType sDT,
                                                                   TimeDomainType tDT,
                                                                   OpticalProperties op,
                                                                   IEnumerable<double> spatialVariable,
                                                                   double[,] temporalVariable)
        {
            double[] reflectanceValues;
            var ReflectanceFunction = Get2DReflectanceFunction(fST, sDT, tDT);

            MakeDirectoryIfNonExistent(sDT.ToString(), tDT.ToString(), fST.ToString());

            var sV = spatialVariable.First();
            var tV = temporalVariable.Row(0);

            reflectanceValues = ReflectanceFunction(op.AsEnumerable(), sV.AsEnumerable(), tV).ToArray();

            LocalWriteArrayToBinary(reflectanceValues, @"Output/" + sDT.ToString() +
                                            "/" + tDT.ToString() + "/" + fST.ToString() + "/" +
                                            "musp" + op.Musp.ToString() + "mua" + op.Mua.ToString(),
                                            FileMode.Create);

            for (int spaceInd = 1; spaceInd < spatialVariable.Count(); spaceInd++)
            {
                sV = spatialVariable.ElementAt(spaceInd);
                tV = temporalVariable.Row(spaceInd);

                reflectanceValues = ReflectanceFunction(op.AsEnumerable(), sV.AsEnumerable(), tV).ToArray();

                LocalWriteArrayToBinary(reflectanceValues, @"Output/" + sDT.ToString() + "/" +
                                                tDT.ToString() + "/" + fST.ToString() + "/" +
                                                "musp" + op.Musp.ToString() + "mua" + op.Mua.ToString(),
                                                FileMode.Append);
            }
        }

        private static int GetSpatialNumberOfPoints(SpatialDomainType sDT)
        {
            int sDim;
            if (sDT == SpatialDomainType.Real)
            {
                sDim = 200;
            }
            else if (sDT == SpatialDomainType.SpatialFrequency)
            {
                sDim = 51;
            }
            else
            {
                throw new ArgumentException("Non valid spatial domain.");
            }
            return sDim;
        }

        private static int GetTemporalNumberOfPoints(SpatialDomainType sDT, TimeDomainType tDT)
        {
            int tDim;
            if (sDT == SpatialDomainType.Real)
            {
                if (tDT == TimeDomainType.TimeDomain)
                {
                    tDim = 201;
                }
                else if (tDT == TimeDomainType.FrequencyDomain)
                {
                    tDim = 201;
                }
                else
                {
                    throw new ArgumentException("Non valid temporal domain type.");
                }
            }
            else if (sDT == SpatialDomainType.SpatialFrequency)
            {
                if (tDT == TimeDomainType.TimeDomain)
                {
                    tDim = 201;
                }
                else if (tDT == TimeDomainType.FrequencyDomain)
                {
                    tDim = 201;
                }
                else
                {
                    throw new ArgumentException("Non valid temporal domain type.");
                }
            }
            else
            {
                throw new ArgumentException("Non valid spatial domain type.");
            }

            return tDim;
        }

        private static Func<IEnumerable<OpticalProperties>, IEnumerable<double>, IEnumerable<double>, IEnumerable<double>>
                       Get2DReflectanceFunction(ForwardSolverType fST, SpatialDomainType sD, TimeDomainType tD)
        {
            Func<IEnumerable<OpticalProperties>, IEnumerable<double>, IEnumerable<double>, IEnumerable<double>> ReflectanceFunction;

            switch (sD)
            {
                case SpatialDomainType.Real:
                    if (tD == TimeDomainType.TimeDomain)
                    {
                        ReflectanceFunction = SolverFactory.GetForwardSolver(fST).ROfRhoAndTime;
                    }
                    else if (tD == TimeDomainType.FrequencyDomain)
                    {
                        ReflectanceFunction = (op, rho, ft) => SolverFactory.GetForwardSolver(fST).ROfRhoAndFt(op, rho, ft).Select(rComplex => rComplex.Magnitude);
                    }
                    else 
                    {
                        throw new ArgumentException("Non valid temporal domain.");
                    }
                    break;
                case SpatialDomainType.SpatialFrequency:
                    if (tD == TimeDomainType.TimeDomain)
                    {
                        ReflectanceFunction = SolverFactory.GetForwardSolver(fST).ROfFxAndTime;
                    }
                    else if (tD == TimeDomainType.FrequencyDomain)
                    {
                        ReflectanceFunction = (op, fx, ft) => SolverFactory.GetForwardSolver(fST).ROfFxAndFt(op, fx, ft).Select(rComplex => rComplex.Magnitude);
                    }
                    else 
                    {
                        throw new ArgumentException("Non valid temporal domain.");
                    }
                    break;
                default:
                    throw new ArgumentException("Non valid spatial domain.");
            }
            return ReflectanceFunction;
        }

        private static Func<IEnumerable<OpticalProperties>, IEnumerable<double>, IEnumerable<double>>
                       GetSteadyStateReflectanceFunction(ForwardSolverType fST, SpatialDomainType sd)
        {
            Func<IEnumerable<OpticalProperties>, IEnumerable<double>, IEnumerable<double>> ReflectanceFunction;
            switch (sd)
            {
                case SpatialDomainType.Real:
                    ReflectanceFunction = SolverFactory.GetForwardSolver(fST).ROfRho;
                    break;
                case SpatialDomainType.SpatialFrequency:
                    ReflectanceFunction = SolverFactory.GetForwardSolver(fST).ROfFx;
                    break;
                default:
                    throw new ArgumentException("Non valid solution domain!");
            }
            return ReflectanceFunction;
        }

        private static void LocalWriteArrayToBinary(Array dataIN, string filename, FileMode mode)// where T : struct
        {
            // Create a file to write binary data 
            using (Stream s = StreamFinder.GetFileStream(filename, mode))
            {
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter().WriteToBinary(bw, dataIN);
                }
            }
        }

        private static void MakeDirectoryIfNonExistent(string sDTFolder, string tDTFolder, string fsFolder)
        {
            if (!(Directory.Exists(@"Output/" + sDTFolder + "/" + tDTFolder + "/" + fsFolder)))
            {
                Directory.CreateDirectory(@"Output/" + sDTFolder + "/" + tDTFolder + "/" + fsFolder);
            }
        }

        #endregion methods
    }
}

