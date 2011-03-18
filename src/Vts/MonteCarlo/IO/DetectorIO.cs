using System;
using System.Collections.Generic;
using System.Numerics;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.IO
{
    public static class DetectorIO
    {
        public static void WriteDetectorToFile(IDetector detector, string folderPath)
        {
            string filePath = folderPath + @"/" + detector.TallyType;

            if (detector is IDetector<double>)
            {
                var d = detector as IDetector<double>;
                FileIO.WriteToXML(d, filePath);
            }
            if (detector is IDetector<double[]>)
            {
                var d = detector as IDetector<double[]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<double>(d.Mean, filePath);
            }
            if (detector is IDetector<double[,]>)
            {
                var d = detector as IDetector<double[,]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<double>(d.Mean, filePath);
            }
            if (detector is IDetector<double[, ,]>)
            {
                var d = detector as IDetector<double[, ,]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<double>(d.Mean, filePath);
            }
            if (detector is IDetector<double[, , ,]>)
            {
                var d = detector as IDetector<double[, , ,]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<double>(d.Mean, filePath);
            }
            if (detector is IDetector<double[, , , ,]>)
            {
                var d = detector as IDetector<double[, , , ,]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<double>(d.Mean, filePath);
            }
            if (detector is IDetector<Complex>)
            {
                var d = detector as IDetector<Complex>;
                FileIO.WriteToXML(d, filePath);
            }
            if (detector is IDetector<Complex[]>)
            {
                var d = detector as IDetector<Complex[]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
            }
            if (detector is IDetector<Complex[,]>)
            {
                var d = detector as IDetector<Complex[,]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
            }
            if (detector is IDetector<Complex[, ,]>)
            {
                var d = detector as IDetector<Complex[, ,]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
            }
            if (detector is IDetector<Complex[, , ,]>)
            {
                var d = detector as IDetector<Complex[, , ,]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
            }
            if (detector is IDetector<Complex[, , , ,]>)
            {
                var d = detector as IDetector<Complex[, , , ,]>;
                FileIO.WriteToXML(d, filePath);
                FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
            }
        }

        public static IDetector ReadDetectorFromFile(TallyType tallyType, string folderPath)
        {
            string filePath = folderPath + @"/" + tallyType;
            switch (tallyType)
            {
                // "0D" detectors
                case TallyType.RDiffuse:
                    return FileIO.ReadFromXML<RDiffuseDetector>(filePath);

                case TallyType.TDiffuse:
                    return FileIO.ReadFromXML<TDiffuseDetector>(filePath);

                case TallyType.ATotal:
                    return FileIO.ReadFromXML<ATotalDetector>(filePath);

                // "1D" detectors
                case TallyType.ROfRho:
                    var rOfRhoDetector = FileIO.ReadFromXML<ROfRhoDetector>(filePath);
                    rOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                    return rOfRhoDetector;

                case TallyType.pMuaMusInROfRho:
                    var pMuaMusInROfRhoDetector = FileIO.ReadFromXML<pMCMuaMusROfRhoDetector>(filePath);
                    pMuaMusInROfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                    return pMuaMusInROfRhoDetector;

                case TallyType.TOfRho:
                    var tOfRhoDetector = FileIO.ReadFromXML<TOfRhoDetector>(filePath);
                    tOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                    return tOfRhoDetector;

                case TallyType.ROfAngle:
                    var rOfAngleDetector = FileIO.ReadFromXML<ROfAngleDetector>(filePath);
                    rOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                    return rOfAngleDetector;

                case TallyType.TOfAngle:
                    var tOfAngleDetector = FileIO.ReadFromXML<TOfAngleDetector>(filePath);
                    tOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                    return tOfAngleDetector;

                // "2D" detectors
                case TallyType.ROfRhoAndTime:
                    var rOfRhoAndTimeDetector = FileIO.ReadFromXML<ROfRhoAndTimeDetector>(filePath);
                    rOfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return rOfRhoAndTimeDetector;

                case TallyType.pMuaMusInROfRhoAndTime:
                    var pMuaMusInROfRhoAndTimeDetector = FileIO.ReadFromXML<pMCMuaMusROfRhoAndTimeDetector>(filePath);
                    pMuaMusInROfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return pMuaMusInROfRhoAndTimeDetector;

                case TallyType.ROfRhoAndAngle:
                    var rOfRhoAndAngleDetector = FileIO.ReadFromXML<ROfRhoAndAngleDetector>(filePath);
                    rOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return rOfRhoAndAngleDetector;

                case TallyType.TOfRhoAndAngle:
                    var tOfRhoAndAngleDetector = FileIO.ReadFromXML<TOfRhoAndAngleDetector>(filePath);
                    tOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return tOfRhoAndAngleDetector;

                case TallyType.ROfRhoAndOmega:
                    var rOfRhoAndOmegaDetector = FileIO.ReadFromXML<ROfRhoAndOmegaDetector>(filePath);
                    rOfRhoAndOmegaDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return rOfRhoAndOmegaDetector;

                case TallyType.ROfXAndY:
                    var rOfXAndYDetector = FileIO.ReadFromXML<ROfXAndYDetector>(filePath);
                    rOfXAndYDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return rOfXAndYDetector;

                case TallyType.FluenceOfRhoAndZ:
                    var fluenceOfRhoAndZDetector = FileIO.ReadFromXML<FluenceOfRhoAndZDetector>(filePath);
                    fluenceOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return fluenceOfRhoAndZDetector;

                case TallyType.AOfRhoAndZ:
                    var aOfRhoAndZDetector = FileIO.ReadFromXML<AOfRhoAndZDetector>(filePath);
                    aOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return aOfRhoAndZDetector;

                case TallyType.MomentumTransferOfRhoAndZ:
                    var momentumTransferOfRhoAndZDetector = FileIO.ReadFromXML<MomentumTransferOfRhoAndZDetector>(filePath);
                    momentumTransferOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return momentumTransferOfRhoAndZDetector;

                // "3D" detectors
                case TallyType.FluenceOfRhoAndZAndTime:
                    var fluenceOfRhoAndZAndTimeDetector = FileIO.ReadFromXML<FluenceOfRhoAndZAndTimeDetector>(filePath);
                    fluenceOfRhoAndZAndTimeDetector.Mean = (double[, ,])FileIO.ReadArrayFromBinary<double>(filePath);
                    return fluenceOfRhoAndZAndTimeDetector;

                default:
                    throw new ArgumentOutOfRangeException("tallyType");
            }
        }

        public static IDetector ReadDetectorFromFileInResources(TallyType tallyType, string folderPath, string projectName)
        {
            string filePath = folderPath + tallyType;
            switch (tallyType)
            {
                // "0D" detectors
                case TallyType.RDiffuse:
                    return FileIO.ReadFromXMLInResources<RDiffuseDetector>(filePath, projectName);

                case TallyType.TDiffuse:
                    return FileIO.ReadFromXMLInResources<TDiffuseDetector>(filePath, projectName);

                case TallyType.ATotal:
                    return FileIO.ReadFromXMLInResources<ATotalDetector>(filePath, projectName);

                // "1D" detectors
                case TallyType.ROfRho:
                    var rOfRhoDetector = FileIO.ReadFromXMLInResources<ROfRhoDetector>(filePath, projectName);
                    rOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return rOfRhoDetector;

                case TallyType.pMuaMusInROfRho:
                    var pMuaMusInROfRhoDetector = FileIO.ReadFromXMLInResources<pMCMuaMusROfRhoDetector>(filePath, projectName);
                    pMuaMusInROfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return pMuaMusInROfRhoDetector;

                case TallyType.TOfRho:
                    var tOfRhoDetector = FileIO.ReadFromXMLInResources<TOfRhoDetector>(filePath, projectName);
                    tOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return tOfRhoDetector;

                case TallyType.ROfAngle:
                    var rOfAngleDetector = FileIO.ReadFromXMLInResources<ROfAngleDetector>(filePath, projectName);
                    rOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return rOfAngleDetector;

                case TallyType.TOfAngle:
                    var tOfAngleDetector = FileIO.ReadFromXMLInResources<TOfAngleDetector>(filePath, projectName);
                    tOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return tOfAngleDetector;

                // "2D" detectors
                case TallyType.ROfRhoAndTime:
                    var rOfRhoAndTimeDetector = FileIO.ReadFromXMLInResources<ROfRhoAndTimeDetector>(filePath, projectName);
                    rOfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return rOfRhoAndTimeDetector;

                case TallyType.pMuaMusInROfRhoAndTime:
                    var pMuaMusInROfRhoAndTimeDetector = FileIO.ReadFromXMLInResources<pMCMuaMusROfRhoAndTimeDetector>(filePath, projectName);
                    pMuaMusInROfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return pMuaMusInROfRhoAndTimeDetector;

                case TallyType.ROfRhoAndAngle:
                    var rOfRhoAndAngleDetector = FileIO.ReadFromXMLInResources<ROfRhoAndAngleDetector>(filePath, projectName);
                    rOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return rOfRhoAndAngleDetector;

                case TallyType.TOfRhoAndAngle:
                    var tOfRhoAndAngleDetector = FileIO.ReadFromXMLInResources<TOfRhoAndAngleDetector>(filePath, projectName);
                    tOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return tOfRhoAndAngleDetector;

                case TallyType.ROfRhoAndOmega:
                    var rOfRhoAndOmegaDetector = FileIO.ReadFromXMLInResources<ROfRhoAndOmegaDetector>(filePath, projectName);
                    rOfRhoAndOmegaDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return rOfRhoAndOmegaDetector;

                case TallyType.ROfXAndY:
                    var rOfXAndYDetector = FileIO.ReadFromXMLInResources<ROfXAndYDetector>(filePath, projectName);
                    rOfXAndYDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return rOfXAndYDetector;

                case TallyType.FluenceOfRhoAndZ:
                    var fluenceOfRhoAndZDetector = FileIO.ReadFromXMLInResources<FluenceOfRhoAndZDetector>(filePath, projectName);
                    fluenceOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return fluenceOfRhoAndZDetector;

                case TallyType.AOfRhoAndZ:
                    var aOfRhoAndZDetector = FileIO.ReadFromXMLInResources<AOfRhoAndZDetector>(filePath, projectName);
                    aOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return aOfRhoAndZDetector;

                case TallyType.MomentumTransferOfRhoAndZ:
                    var momentumTransferOfRhoAndZDetector = FileIO.ReadFromXMLInResources<MomentumTransferOfRhoAndZDetector>(filePath, projectName);
                    momentumTransferOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return momentumTransferOfRhoAndZDetector;

                // "3D" detectors
                case TallyType.FluenceOfRhoAndZAndTime:
                    var fluenceOfRhoAndZAndTimeDetector = FileIO.ReadFromXMLInResources<FluenceOfRhoAndZAndTimeDetector>(filePath, projectName);
                    fluenceOfRhoAndZAndTimeDetector.Mean = (double[, ,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                    return fluenceOfRhoAndZAndTimeDetector;

                default:
                    throw new ArgumentOutOfRangeException("tallyType");
            }
        }
    }
}
