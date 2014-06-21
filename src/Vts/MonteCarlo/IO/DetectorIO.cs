using System;
using MathNet.Numerics;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.IO
{
    /// <summary>
    /// Class that handles IO for IDetectors.
    /// </summary>
    public static class DetectorIO
    {
        /// <summary>
        /// Writes Detector txt for scalar detectors, writes Detector txt and 
        /// binary for 1D and larger detectors.  Detector.Name is used for filename.
        /// </summary>
        /// <param name="detector">IDetector being written.</param>
        /// <param name="folderPath">location of written file.</param>
        public static void WriteDetectorToFile(IDetector detector, string folderPath)
        {
            try
            {
                // allow null folderPath in case writing to isolated storage
                string filePath = folderPath;
                if (folderPath == "")
                {
                    filePath = detector.Name;
                }
                else
                {
                    filePath = folderPath + @"/" + detector.Name;

                    // uses isolated storage for Silverlight, desktop folder otherwise
                    FileIO.CreateDirectory(folderPath);
                }

                if (detector is IDetector<double>)
                {
                    var d = detector as IDetector<double>;
                    FileIO.WriteToJson((RDiffuseDetector)d, filePath + ".txt");
                }
                if (detector is IDetector<double[]>)
                {
                    var d = detector as IDetector<double[]>;
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    // output of 2nd moment 
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<double[,]>)
                {
                    var d = detector as IDetector<double[,]>;
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<double[, ,]>)
                {
                    var d = detector as IDetector<double[, ,]>;
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<double[, , ,]>)
                {
                    var d = detector as IDetector<double[, , ,]>;
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<double[, , , ,]>)
                {
                    var d = detector as IDetector<double[, , , ,]>;
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex>)
                {
                    //var d = detector as IDetector<Complex>;
                    //FileIO.WriteToJson(d, filePath + ".txt");
                }
                if (detector is IDetector<Complex[]>)
                {
                    var d = detector as IDetector<Complex[]>;
                    //FileIO.WriteToJson(d, filePath + ".txt");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex[,]>)
                {
                    var d = detector as IDetector<Complex[,]>;
                    //FileIO.WriteToJson(d, filePath + ".txt");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex[, ,]>)
                {
                    var d = detector as IDetector<Complex[, ,]>;
                    //FileIO.WriteToJson(d, filePath + ".txt");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex[, , ,]>)
                {
                    var d = detector as IDetector<Complex[, , ,]>;
                    //FileIO.WriteToJson(d, filePath + ".txt");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex[, , , ,]>)
                {
                    var d = detector as IDetector<Complex[, , , ,]>;
                    //FileIO.WriteToJson(d, filePath + ".txt");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
				
				switch (detector.TallyType) {
					case TallyType.RDiffuse:					
                        FileIO.WriteToJson((RDiffuseDetector)detector, filePath + ".txt");
						break;
					case TallyType.TDiffuse:
	                    FileIO.WriteToJson((TDiffuseDetector)detector, filePath + ".txt");
						break;

                    case TallyType.ATotal:
	                    FileIO.WriteToJson((ATotalDetector)detector, filePath + ".txt");
						break;

                    // "1D" detectors
                    case TallyType.ROfRho:
	                    FileIO.WriteToJson((ROfRhoDetector)detector, filePath + ".txt");
						break;

                    case TallyType.pMCROfRho:
	                    FileIO.WriteToJson((pMCROfRhoDetector)detector, filePath + ".txt");
						break;

                    case TallyType.dMCdROfRhodMua:
				        FileIO.WriteToJson((dMCdROfRhodMuaDetector) detector, filePath + ".txt");
                        break;

                    case TallyType.dMCdROfRhodMus:
                        FileIO.WriteToJson((dMCdROfRhodMusDetector)detector, filePath + ".txt");
                        break;

                    case TallyType.TOfRho:
	                    FileIO.WriteToJson((TOfRhoDetector)detector, filePath + ".txt");
						break;

                    case TallyType.ROfAngle:
	                    FileIO.WriteToJson((ROfAngleDetector)detector, filePath + ".txt");
						break;

                    case TallyType.ROfFx:
	                    FileIO.WriteToJson((ROfFxDetector)detector, filePath + ".txt");
						break;

                    case TallyType.pMCROfFx:
	                    FileIO.WriteToJson((pMCROfFxDetector)detector, filePath + ".txt");
						break;

                    case TallyType.TOfAngle:
	                    FileIO.WriteToJson((TOfAngleDetector)detector, filePath + ".txt");
						break;

                    // "2D" detectors
                    case TallyType.ROfRhoAndTime:
	                    FileIO.WriteToJson((ROfRhoAndTimeDetector)detector, filePath + ".txt");
						break;

                    case TallyType.pMCROfRhoAndTime:
	                    FileIO.WriteToJson((pMCROfRhoAndTimeDetector)detector, filePath + ".txt");
						break;

                    case TallyType.pMCROfFxAndTime:
	                    FileIO.WriteToJson((pMCROfFxAndTimeDetector)detector, filePath + ".txt");
						break;

                    case TallyType.ROfRhoAndAngle:
	                    FileIO.WriteToJson((ROfRhoAndAngleDetector)detector, filePath + ".txt");
						break;

                    case TallyType.TOfRhoAndAngle:
	                    FileIO.WriteToJson((TOfRhoAndAngleDetector)detector, filePath + ".txt");
						break;

                    case TallyType.ROfRhoAndOmega:
	                    FileIO.WriteToJson((ROfRhoAndOmegaDetector)detector, filePath + ".txt");
						break;
					
                    case TallyType.ROfFxAndTime:
	                    FileIO.WriteToJson((ROfFxAndTimeDetector)detector, filePath + ".txt");
						break;

                    case TallyType.ROfXAndY:
	                    FileIO.WriteToJson((ROfXAndYDetector)detector, filePath + ".txt");
						break;

                    case TallyType.FluenceOfRhoAndZ:
	                    FileIO.WriteToJson((FluenceOfRhoAndZDetector)detector, filePath + ".txt");
						break;

                    case TallyType.FluenceOfXAndYAndZ:
	                    FileIO.WriteToJson((FluenceOfXAndYAndZDetector)detector, filePath + ".txt");
						break;

                    case TallyType.AOfRhoAndZ:
	                    FileIO.WriteToJson((AOfRhoAndZDetector)detector, filePath + ".txt");
						break;


                    // "3D" detectors
                    case TallyType.FluenceOfRhoAndZAndTime:
				        FileIO.WriteToJson((FluenceOfRhoAndZAndTimeDetector) detector, filePath + ".txt");
						break;

                    case TallyType.ReflectedMTOfRhoAndSubregionHist:
	                    FileIO.WriteToJson((ReflectedMTOfRhoAndSubregionHistDetector)detector, filePath + ".txt");
                        var dmt = detector as ReflectedMTOfRhoAndSubregionHistDetector;
                        FileIO.WriteArrayToBinary<double>(dmt.FractionalMT, filePath + "_FractionalMT", false);
						break;

                    case TallyType.ReflectedTimeOfRhoAndSubregionHist:
				        FileIO.WriteToJson((ReflectedTimeOfRhoAndSubregionHistDetector) detector, filePath + ".txt");
                        var d = detector as ReflectedTimeOfRhoAndSubregionHistDetector;
                        FileIO.WriteArrayToBinary<double>(d.FractionalTime, filePath + "_FractionalTime", false);
						break;

                    // "5D" detectors
                    case TallyType.RadianceOfXAndYAndZAndThetaAndPhi:
	                    FileIO.WriteToJson((RadianceOfXAndYAndZAndThetaAndPhiDetector)detector, filePath + ".txt");
						break;
					default:
	                    FileIO.WriteToJson(detector, filePath + ".txt");
						break;
					}
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem writing detector information to file.\n\nDetails:\n\n" + e + "\n");
            }
        }
        /// <summary>
        /// Reads Detector from File with given fileName.
        /// </summary>
        /// <param name="tallyType">TallyType of IDetector being read</param>
        /// <param name="fileName">filename string of file to be read</param>
        /// <param name="folderPath">path string where file resides</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFile(TallyType tallyType, string fileName, string folderPath)
        {
            try
            {
                // allow null filePaths in case writing to isolated storage
                string filePath;
                if (folderPath == "")
                {
                    filePath = fileName;
                }
                else
                {
                    filePath = folderPath + @"/" + fileName;
                }
                switch (tallyType)
                {
                    // "0D" detectors
                    case TallyType.RDiffuse:
                        return FileIO.ReadFromJson<RDiffuseDetector>(filePath + ".txt");

                    case TallyType.TDiffuse:
                        return FileIO.ReadFromJson<TDiffuseDetector>(filePath + ".txt");

                    case TallyType.ATotal:
                        return FileIO.ReadFromJson<ATotalDetector>(filePath + ".txt");

                    // "1D" detectors
                    case TallyType.ROfRho:
                        var rOfRhoDetector = FileIO.ReadFromJson<ROfRhoDetector>(filePath + ".txt");
                        var rOfRhoDetectorDims = new int[] { rOfRhoDetector.Rho.Count - 1 };
                        rOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, rOfRhoDetectorDims);
                        return rOfRhoDetector;

                    case TallyType.pMCROfRho:
                        var pMCROfRhoDetector = FileIO.ReadFromJson<pMCROfRhoDetector>(filePath + ".txt");
                        var pMCROfRhoDetectorDims = new int[] { pMCROfRhoDetector.Rho.Count - 1 };
                        pMCROfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, pMCROfRhoDetectorDims);
                        return pMCROfRhoDetector;

                    case TallyType.dMCdROfRhodMua:
                        var dMCdROfRhodMuaDetector = FileIO.ReadFromJson<dMCdROfRhodMuaDetector>(filePath + ".txt");
                        var dMCdROfRhodMuaDetectorDims = new int[] { dMCdROfRhodMuaDetector.Rho.Count - 1 };
                        dMCdROfRhodMuaDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, dMCdROfRhodMuaDetectorDims);
                        return dMCdROfRhodMuaDetector;

                    case TallyType.dMCdROfRhodMus:
                        var dMCdROfRhodMusDetector = FileIO.ReadFromJson<dMCdROfRhodMusDetector>(filePath + ".txt");
                        var dMCdROfRhodMusDetectorDims = new int[] { dMCdROfRhodMusDetector.Rho.Count - 1 };
                        dMCdROfRhodMusDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, dMCdROfRhodMusDetectorDims);
                        return dMCdROfRhodMusDetector;

                    case TallyType.TOfRho:
                        var tOfRhoDetector = FileIO.ReadFromJson<TOfRhoDetector>(filePath + ".txt");
                        var tOfRhoDetectorDims = new int[] { tOfRhoDetector.Rho.Count - 1 };
                        tOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, tOfRhoDetectorDims);
                        return tOfRhoDetector;

                    case TallyType.ROfAngle:
                        var rOfAngleDetector = FileIO.ReadFromJson<ROfAngleDetector>(filePath + ".txt");
                        var rOfAngleDetectorDims = new int[] { rOfAngleDetector.Angle.Count - 1 };
                        rOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, rOfAngleDetectorDims);
                        return rOfAngleDetector;

                    case TallyType.ROfFx:
                        var rOfFxDetector = FileIO.ReadFromJson<ROfFxDetector>(filePath + ".txt");
                        var rOfFxDetectorDims = new int[] { rOfFxDetector.Fx.Count };
                        rOfFxDetector.Mean = (Complex[])FileIO.ReadArrayFromBinary<Complex>(filePath, rOfFxDetectorDims);
                        return rOfFxDetector;

                    case TallyType.pMCROfFx:
                        var pMCROfFxDetector = FileIO.ReadFromJson<pMCROfFxDetector>(filePath + ".txt");
                        var pMCROfFxDetectorDims = new int[] { pMCROfFxDetector.Fx.Count };
                        pMCROfFxDetector.Mean = (Complex[])FileIO.ReadArrayFromBinary<Complex>(filePath, pMCROfFxDetectorDims);
                        return pMCROfFxDetector;

                    case TallyType.TOfAngle:
                        var tOfAngleDetector = FileIO.ReadFromJson<TOfAngleDetector>(filePath + ".txt");
                        var tOfAngleDetectorDims = new int[] { tOfAngleDetector.Angle.Count - 1 };
                        tOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, tOfAngleDetectorDims);
                        return tOfAngleDetector;

                    // "2D" detectors
                    case TallyType.ROfRhoAndTime:
                        var rOfRhoAndTimeDetector = FileIO.ReadFromJson<ROfRhoAndTimeDetector>(filePath + ".txt");
                        var rOfRhoAndTimeDetectorDims = new int[] { rOfRhoAndTimeDetector.Rho.Count - 1, rOfRhoAndTimeDetector.Time.Count - 1 };
                        rOfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, rOfRhoAndTimeDetectorDims);
                        return rOfRhoAndTimeDetector;

                    case TallyType.pMCROfRhoAndTime:
                        var pMCROfRhoAndTimeDetector =
                            FileIO.ReadFromJson<pMCROfRhoAndTimeDetector>(filePath + ".txt");
                        var pMCROfRhoAndTimeDetectorDims = new int[] { pMCROfRhoAndTimeDetector.Rho.Count - 1, pMCROfRhoAndTimeDetector.Time.Count - 1 };
                        pMCROfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, pMCROfRhoAndTimeDetectorDims);
                        return pMCROfRhoAndTimeDetector;

                    case TallyType.pMCROfFxAndTime:
                        var pMCROfFxAndTimeDetector =
                            FileIO.ReadFromJson<pMCROfFxAndTimeDetector>(filePath + ".txt");
                        var pMCROfFxAndTimeDetectorDims = new int[] { pMCROfFxAndTimeDetector.Fx.Count - 1, pMCROfFxAndTimeDetector.Time.Count - 1 };
                        pMCROfFxAndTimeDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinary<Complex>(filePath, pMCROfFxAndTimeDetectorDims);
                        return pMCROfFxAndTimeDetector;

                    case TallyType.ROfRhoAndAngle:
                        var rOfRhoAndAngleDetector = FileIO.ReadFromJson<ROfRhoAndAngleDetector>(filePath + ".txt");
                        var rOfRhoAndAngleDetectorDims = new int[] { rOfRhoAndAngleDetector.Rho.Count - 1, rOfRhoAndAngleDetector.Angle.Count - 1 };
                        rOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, rOfRhoAndAngleDetectorDims);
                        return rOfRhoAndAngleDetector;

                    case TallyType.TOfRhoAndAngle:
                        var tOfRhoAndAngleDetector = FileIO.ReadFromJson<TOfRhoAndAngleDetector>(filePath + ".txt");
                        var tOfRhoAndAngleDetectorDims = new int[] { tOfRhoAndAngleDetector.Rho.Count - 1, tOfRhoAndAngleDetector.Angle.Count - 1 };
                        tOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, tOfRhoAndAngleDetectorDims);
                        return tOfRhoAndAngleDetector;

                    case TallyType.ROfRhoAndOmega:
                        var rOfRhoAndOmegaDetector = FileIO.ReadFromJson<ROfRhoAndOmegaDetector>(filePath + ".txt");
                        var rOfRhoAndOmegaDetectorDims = new int[] { rOfRhoAndOmegaDetector.Rho.Count - 1, rOfRhoAndOmegaDetector.Omega.Count };
                        rOfRhoAndOmegaDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinary<Complex>(filePath, rOfRhoAndOmegaDetectorDims);
                        return rOfRhoAndOmegaDetector;

                    case TallyType.ROfFxAndTime:
                        var rOfFxAndTimeDetector = FileIO.ReadFromJson<ROfFxAndTimeDetector>(filePath + ".txt");
                        var rOfFxAndTimeDetectorDims = new int[] { rOfFxAndTimeDetector.Fx.Count, rOfFxAndTimeDetector.Time.Count - 1 };
                        rOfFxAndTimeDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinary<Complex>(filePath, rOfFxAndTimeDetectorDims);
                        return rOfFxAndTimeDetector;

                    case TallyType.ROfXAndY:
                        var rOfXAndYDetector = FileIO.ReadFromJson<ROfXAndYDetector>(filePath + ".txt");
                        var rOfXAndYDetectorDims = new int[] { rOfXAndYDetector.X.Count - 1, rOfXAndYDetector.Y.Count - 1 };
                        rOfXAndYDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, rOfXAndYDetectorDims);
                        return rOfXAndYDetector;

                    case TallyType.FluenceOfRhoAndZ:
                        var fluenceOfRhoAndZDetector = FileIO.ReadFromJson<FluenceOfRhoAndZDetector>(filePath + ".txt");
                        var fluenceOfRhoAndZDetectorDims = new int[] { fluenceOfRhoAndZDetector.Rho.Count - 1, fluenceOfRhoAndZDetector.Z.Count - 1 };
                        fluenceOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, fluenceOfRhoAndZDetectorDims);
                        return fluenceOfRhoAndZDetector;

                    case TallyType.FluenceOfXAndYAndZ:
                        var fluenceOfXAndYAndZDetector = FileIO.ReadFromJson<FluenceOfXAndYAndZDetector>(filePath + ".txt");
                        var fluenceOfXAndYAndZDetectorDims = new int[] { fluenceOfXAndYAndZDetector.X.Count - 1, fluenceOfXAndYAndZDetector.Y.Count - 1, fluenceOfXAndYAndZDetector.Z.Count -1 };
                        fluenceOfXAndYAndZDetector.Mean = (double[,,])FileIO.ReadArrayFromBinary<double>(filePath, fluenceOfXAndYAndZDetectorDims);
                        return fluenceOfXAndYAndZDetector;

                    case TallyType.AOfRhoAndZ:
                        var aOfRhoAndZDetector = FileIO.ReadFromJson<AOfRhoAndZDetector>(filePath + ".txt");
                        var aOfRhoAndZDetectorDims = new int[] { aOfRhoAndZDetector.Rho.Count - 1, aOfRhoAndZDetector.Z.Count - 1 };
                        aOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, aOfRhoAndZDetectorDims);
                        return aOfRhoAndZDetector;


                    // "3D" detectors
                    case TallyType.FluenceOfRhoAndZAndTime:
                        var fluenceOfRhoAndZAndTimeDetector =
                            FileIO.ReadFromJson<FluenceOfRhoAndZAndTimeDetector>(filePath + ".txt");
                        var fluenceOfRhoAndZAndTimeDetectorDims = new int[] { 
                            fluenceOfRhoAndZAndTimeDetector.Rho.Count - 1, 
                            fluenceOfRhoAndZAndTimeDetector.Z.Count - 1,
                            fluenceOfRhoAndZAndTimeDetector.Time.Count - 1 };
                        fluenceOfRhoAndZAndTimeDetector.Mean = (double[, ,])FileIO.ReadArrayFromBinary<double>(filePath, fluenceOfRhoAndZAndTimeDetectorDims);
                        return fluenceOfRhoAndZAndTimeDetector;

                    case TallyType.ReflectedMTOfRhoAndSubregionHist:
                        var reflectedMTOfRhoAndSubregionHistDetector =
                            FileIO.ReadFromJson<ReflectedMTOfRhoAndSubregionHistDetector>(filePath + ".txt");
                        var reflectedMTOfRhoAndSubregionHistDetectorDims = new int[] {
                            reflectedMTOfRhoAndSubregionHistDetector.Rho.Count - 1, 
                            reflectedMTOfRhoAndSubregionHistDetector.MTBins.Count - 1 };                                                 
                        reflectedMTOfRhoAndSubregionHistDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinary<double>(filePath, reflectedMTOfRhoAndSubregionHistDetectorDims);
                        return reflectedMTOfRhoAndSubregionHistDetector;

                    case TallyType.ReflectedTimeOfRhoAndSubregionHist:
                        var reflectedTimeOfRhoAndSubregionHistDetector =
                            FileIO.ReadFromJson<ReflectedTimeOfRhoAndSubregionHistDetector>(filePath + ".txt");
                        var reflectedTimeOfRhoAndSubregionHistDetectorDims = new int[] {
                            reflectedTimeOfRhoAndSubregionHistDetector.Rho.Count - 1, 
                            reflectedTimeOfRhoAndSubregionHistDetector.SubregionIndices.Count,
                            reflectedTimeOfRhoAndSubregionHistDetector.Time.Count - 1 };
                        reflectedTimeOfRhoAndSubregionHistDetector.Mean =
                            (double[, ,])FileIO.ReadArrayFromBinary<double>(filePath, reflectedTimeOfRhoAndSubregionHistDetectorDims);
                        return reflectedTimeOfRhoAndSubregionHistDetector;

                    // "5D" detectors
                    case TallyType.RadianceOfXAndYAndZAndThetaAndPhi:
                        var radianceOfXAndYAndZAndThetaAndPhiDetector =
                            FileIO.ReadFromJson<RadianceOfXAndYAndZAndThetaAndPhiDetector>(filePath + ".txt");
                        var radianceOfXAndYAndZAndThetaAndPhiDims = new int[] { 
                            radianceOfXAndYAndZAndThetaAndPhiDetector.X.Count - 1, 
                            radianceOfXAndYAndZAndThetaAndPhiDetector.Y.Count - 1,
                            radianceOfXAndYAndZAndThetaAndPhiDetector.Z.Count - 1,
                            radianceOfXAndYAndZAndThetaAndPhiDetector.Theta.Count - 1,
                            radianceOfXAndYAndZAndThetaAndPhiDetector.Phi.Count - 1};
                        radianceOfXAndYAndZAndThetaAndPhiDetector.Mean = (double[,,,,])FileIO.ReadArrayFromBinary<double>(filePath, radianceOfXAndYAndZAndThetaAndPhiDims);
                        return radianceOfXAndYAndZAndThetaAndPhiDetector;

                    default:
                        throw new ArgumentOutOfRangeException("tallyType");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading detector information from file.\n\nDetails:\n\n" + e + "\n");
            }

            return null;
        }
        /// <summary>
        /// Reads Detector from file with default fileName (TallyType.ToString).
        /// </summary>
        /// <param name="tallyType">TallyType of IDetector being read</param>
        /// <param name="folderPath">path string of folder where file to be read resides</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFile(TallyType tallyType, string folderPath)
        {
            return ReadDetectorFromFile(tallyType, tallyType.ToString(), folderPath);
        }
        /// <summary>
        /// Reads Detector from a file in resources using given fileName.
        /// </summary>
        /// <param name="tallyType">TallyType of IDetector being read</param>
        /// <param name="fileName">filename string of file to be read</param>
        /// <param name="folderPath">path string of folder where file to be read resides</param>
        /// <param name="projectName">project name string where file resides in resources</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFileInResources(TallyType tallyType, string fileName, string folderPath, string projectName)
        {
            try
            {
                string filePath = folderPath + fileName;
                switch (tallyType)
                {
                    // "0D" detectors
                    case TallyType.RDiffuse:
                        return FileIO.ReadFromJsonInResources<RDiffuseDetector>(filePath + ".txt", projectName);

                    case TallyType.TDiffuse:
                        return FileIO.ReadFromJsonInResources<TDiffuseDetector>(filePath + ".txt", projectName);

                    case TallyType.ATotal:
                        return FileIO.ReadFromJsonInResources<ATotalDetector>(filePath + ".txt", projectName);

                    // "1D" detectors
                    case TallyType.ROfRho:
                        var rOfRhoDetector = FileIO.ReadFromJsonInResources<ROfRhoDetector>(filePath + ".txt", projectName);
                        var rOfRhoDetectorDims = new int[] { rOfRhoDetector.Rho.Count - 1 };
                        rOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoDetectorDims);
                        return rOfRhoDetector;

                    case TallyType.pMCROfRho:
                        var pMuaMusROfRhoDetector = FileIO.ReadFromJsonInResources<pMCROfRhoDetector>(filePath + ".txt", projectName);
                        var pMCROfRhoDetectorDims = new int[] { pMuaMusROfRhoDetector.Rho.Count - 1 };
                        pMuaMusROfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, pMCROfRhoDetectorDims);
                        return pMuaMusROfRhoDetector;

                    case TallyType.dMCdROfRhodMua:
                        var dMCdROfRhodMuaDetector = FileIO.ReadFromJsonInResources<dMCdROfRhodMuaDetector>(filePath + ".txt", projectName);
                        var dMCdROfRhodMuaDetectorDims = new int[] { dMCdROfRhodMuaDetector.Rho.Count - 1 };
                        dMCdROfRhodMuaDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, dMCdROfRhodMuaDetectorDims);
                        return dMCdROfRhodMuaDetector;

                    case TallyType.dMCdROfRhodMus:
                        var dMCdROfRhodMusDetector = FileIO.ReadFromJsonInResources<dMCdROfRhodMusDetector>(filePath + ".txt", projectName);
                        var dMCdROfRhodMusDetectorDims = new int[] { dMCdROfRhodMusDetector.Rho.Count - 1 };
                        dMCdROfRhodMusDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, dMCdROfRhodMusDetectorDims);
                        return dMCdROfRhodMusDetector;

                    case TallyType.TOfRho:
                        var tOfRhoDetector = FileIO.ReadFromJsonInResources<TOfRhoDetector>(filePath + ".txt", projectName);
                        tOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                        return tOfRhoDetector;

                    case TallyType.ROfAngle:
                        var rOfAngleDetector = FileIO.ReadFromJsonInResources<ROfAngleDetector>(filePath + ".txt", projectName);
                        var rOfAngleDetectorDims = new int[] { rOfAngleDetector.Angle.Count - 1 };
                        rOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfAngleDetectorDims);
                        return rOfAngleDetector;

                    case TallyType.ROfFx:
                        var rOfFxDetector = FileIO.ReadFromJsonInResources<ROfFxDetector>(filePath + ".txt", projectName);
                        var rOfFxDetectorDims = new int[] { rOfFxDetector.Fx.Count };
                        rOfFxDetector.Mean = (Complex[])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, rOfFxDetectorDims);
                        return rOfFxDetector;

                    case TallyType.pMCROfFx:
                        var pMuaMusROfFxDetector = FileIO.ReadFromJsonInResources<pMCROfFxDetector>(filePath + ".txt", projectName);
                        var pMCROfFxDetectorDims = new int[] { pMuaMusROfFxDetector.Fx.Count };
                        pMuaMusROfFxDetector.Mean = (Complex[])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, pMCROfFxDetectorDims);
                        return pMuaMusROfFxDetector;

                    case TallyType.TOfAngle:
                        var tOfAngleDetector = FileIO.ReadFromJsonInResources<TOfAngleDetector>(filePath + ".txt", projectName);
                        var tOfAngleDetectorDims = new int[] { tOfAngleDetector.Angle.Count - 1 };
                        tOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, tOfAngleDetectorDims);
                        return tOfAngleDetector;

                    // "2D" detectors
                    case TallyType.ROfRhoAndTime:
                        var rOfRhoAndTimeDetector = FileIO.ReadFromJsonInResources<ROfRhoAndTimeDetector>(filePath + ".txt", projectName);
                        var rOfRhoAndTimeDetectorDims = new int[] { rOfRhoAndTimeDetector.Rho.Count, rOfRhoAndTimeDetector.Time.Count - 1 };
                        rOfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoAndTimeDetectorDims);
                        return rOfRhoAndTimeDetector;

                    case TallyType.pMCROfRhoAndTime:
                        var pMuaMusROfRhoAndTimeDetector = FileIO.ReadFromJsonInResources<pMCROfRhoAndTimeDetector>(filePath + ".txt", projectName);
                        var pMCROfRhoAndTimeDetectorDims = new int[] { pMuaMusROfRhoAndTimeDetector.Rho.Count, pMuaMusROfRhoAndTimeDetector.Time.Count - 1 };
                        pMuaMusROfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, pMCROfRhoAndTimeDetectorDims);
                        return pMuaMusROfRhoAndTimeDetector;

                    case TallyType.pMCROfFxAndTime:
                        var pMuaMusROfFxAndTimeDetector = FileIO.ReadFromJsonInResources<pMCROfFxAndTimeDetector>(filePath + ".txt", projectName);
                        var pMCROfFxAndTimeDetectorDims = new int[] { pMuaMusROfFxAndTimeDetector.Fx.Count, pMuaMusROfFxAndTimeDetector.Time.Count - 1 };
                        pMuaMusROfFxAndTimeDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, pMCROfFxAndTimeDetectorDims);
                        return pMuaMusROfFxAndTimeDetector;

                    case TallyType.ROfRhoAndAngle:
                        var rOfRhoAndAngleDetector =
                            FileIO.ReadFromJsonInResources<ROfRhoAndAngleDetector>(filePath + ".txt", projectName);
                        var rOfRhoAndAngleDetectorDims = new int[] { rOfRhoAndAngleDetector.Rho.Count - 1, rOfRhoAndAngleDetector.Angle.Count - 1 };
                        rOfRhoAndAngleDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoAndAngleDetectorDims);
                        return rOfRhoAndAngleDetector;

                    case TallyType.TOfRhoAndAngle:
                        var tOfRhoAndAngleDetector =
                            FileIO.ReadFromJsonInResources<TOfRhoAndAngleDetector>(filePath + ".txt", projectName);
                        var tOfRhoAndAngleDetectorDims = new int[] { tOfRhoAndAngleDetector.Rho.Count - 1, tOfRhoAndAngleDetector.Angle.Count - 1 };
                        tOfRhoAndAngleDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, tOfRhoAndAngleDetectorDims);
                        return tOfRhoAndAngleDetector;

                    case TallyType.ROfRhoAndOmega:
                        var rOfRhoAndOmegaDetector =
                            FileIO.ReadFromJsonInResources<ROfRhoAndOmegaDetector>(filePath + ".txt", projectName);
                        var rOfRhoAndOmegaDetectorDims = new int[] { rOfRhoAndOmegaDetector.Rho.Count - 1, rOfRhoAndOmegaDetector.Omega.Count};
                        rOfRhoAndOmegaDetector.Mean =
                            (Complex[,])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, rOfRhoAndOmegaDetectorDims);
                        return rOfRhoAndOmegaDetector;

                    case TallyType.ROfFxAndTime:
                        var rOfFxAndTimeDetector = FileIO.ReadFromJsonInResources<ROfFxAndTimeDetector>(filePath + ".txt", projectName);
                        var rOfFxAndTimeDetectorDims = new int[] { rOfFxAndTimeDetector.Fx.Count, rOfFxAndTimeDetector.Time.Count - 1 };
                        rOfFxAndTimeDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, rOfFxAndTimeDetectorDims);
                        return rOfFxAndTimeDetector;

                    case TallyType.ROfXAndY:
                        var rOfXAndYDetector = FileIO.ReadFromJsonInResources<ROfXAndYDetector>(filePath + ".txt",
                                                                                               projectName);
                        var rOfXAndYDetectorDims = new int[] { rOfXAndYDetector.X.Count, rOfXAndYDetector.Y.Count };
                        rOfXAndYDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfXAndYDetectorDims);
                        return rOfXAndYDetector;

                    case TallyType.FluenceOfRhoAndZ:
                        var fluenceOfRhoAndZDetector =
                            FileIO.ReadFromJsonInResources<FluenceOfRhoAndZDetector>(filePath + ".txt", projectName);
                        var fluenceOfRhoAndZDetectorDims = new int[] { fluenceOfRhoAndZDetector.Rho.Count, fluenceOfRhoAndZDetector.Z.Count };
                        fluenceOfRhoAndZDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, fluenceOfRhoAndZDetectorDims);
                        return fluenceOfRhoAndZDetector;

                    case TallyType.AOfRhoAndZ:
                        var aOfRhoAndZDetector = FileIO.ReadFromJsonInResources<AOfRhoAndZDetector>(filePath + ".txt",
                                                                                                   projectName);
                        var aOfRhoAndZDetectorDims = new int[] { aOfRhoAndZDetector.Rho.Count, aOfRhoAndZDetector.Z.Count };
                        aOfRhoAndZDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, aOfRhoAndZDetectorDims);
                        return aOfRhoAndZDetector;


                    // "3D" detectors
                    case TallyType.FluenceOfRhoAndZAndTime:
                        var fluenceOfRhoAndZAndTimeDetector =
                            FileIO.ReadFromJsonInResources<FluenceOfRhoAndZAndTimeDetector>(filePath + ".txt",  projectName);
                        var fluenceOfRhoAndZAndTimeDetectorDims =
                            new int[] { fluenceOfRhoAndZAndTimeDetector.Rho.Count, 
                                        fluenceOfRhoAndZAndTimeDetector.Z.Count,
                                        fluenceOfRhoAndZAndTimeDetector.Time.Count };
                        fluenceOfRhoAndZAndTimeDetector.Mean =
                            (double[, ,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, fluenceOfRhoAndZAndTimeDetectorDims);
                        return fluenceOfRhoAndZAndTimeDetector;
                    case TallyType.ReflectedMTOfRhoAndSubregionHist:
                        var reflectedMTOfRhoAndSubRegionHistDetector =
                            FileIO.ReadFromJsonInResources<ReflectedMTOfRhoAndSubregionHistDetector>(filePath + ".txt", projectName);
                        var ReflectedMTOfRhoAndSubregionHistDims =
                            new int[] { reflectedMTOfRhoAndSubRegionHistDetector.Rho.Count,  
                                        reflectedMTOfRhoAndSubRegionHistDetector.MTBins.Count };
                        reflectedMTOfRhoAndSubRegionHistDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, ReflectedMTOfRhoAndSubregionHistDims);
                        return reflectedMTOfRhoAndSubRegionHistDetector;
                    case TallyType.ReflectedTimeOfRhoAndSubregionHist:
                        var reflectedTimeOfRhoAndSubregionHistDetector =
                            FileIO.ReadFromJsonInResources<ReflectedTimeOfRhoAndSubregionHistDetector>(filePath + ".txt", projectName);
                        var reflectedTimeOfRhoAndSubregionHistDims =
                            new int[] { reflectedTimeOfRhoAndSubregionHistDetector.Rho.Count, 
                                        reflectedTimeOfRhoAndSubregionHistDetector.SubregionIndices.Count, 
                                        reflectedTimeOfRhoAndSubregionHistDetector.Time.Count };
                        reflectedTimeOfRhoAndSubregionHistDetector.Mean =
                            (double[, ,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, reflectedTimeOfRhoAndSubregionHistDims);
                        return reflectedTimeOfRhoAndSubregionHistDetector;

                    default:
                        throw new ArgumentOutOfRangeException("tallyType");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading detector information from resource file.\n\nDetails:\n\n" + e + "\n");
            }

            return null;
        }
        /// <summary>
        /// Reads Detector from file in resources using default name (TallyType.ToString).
        /// </summary>
        /// <param name="tallyType">TallyType of IDetector to be read</param>
        /// <param name="folderPath">path string of folder where file to be read resides</param>
        /// <param name="projectName">project name string where the file resides in resources</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFileInResources(TallyType tallyType, string folderPath, string projectName)
        {
            return ReadDetectorFromFileInResources(tallyType, tallyType.ToString(), folderPath, projectName);
        }
    }
}
