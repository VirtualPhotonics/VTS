using System;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class AbsorptionWeightingMethodsTests
    {
        /// <summary>
        /// all methods in this class are private, there are public Func's
        /// that call the methods and these tests test the Func's
        /// execute tests for Analog, Discrete and Continuous random walk specifications
        /// </summary>
        [Test]
        public void Validate_GetVolumeAbsorptionWeightingMethod_func_is_correct()
        {
            Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;
            var previousDp = new PhotonDataPoint(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                1.0,
                10.0,
                PhotonStateType.Alive);
            var dp = new PhotonDataPoint(
                new Position (0,0,0),
                new Direction(0, 0, 1),
                0.9,
                10.0,
                PhotonStateType.Alive);
            var tissueInput = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });

            // specify Analog random walk process: PhotonStateType.Absorbed dictates weight result
            var tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Analog,
                PhaseFunctionType.HenyeyGreenstein,
                0.0);
            var currentRegionIndex = 1;
            var rng = new MersenneTwister(0);
            var detectorInput = new ATotalDetectorInput();
            var detector = DetectorFactory.GetDetector(detectorInput, tissue, rng);
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, detector);
            var weight = _absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.Less(Math.Abs(weight), 1e-6); // should be 0.0
            // turn on PhotonStateType
            dp.StateFlag = PhotonStateType.Absorbed;
            weight = _absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.Less(Math.Abs(weight - 1.0), 1e-6); // weight should be 1.0

            // specify Discrete random walk process: previousDp weight = or != dp weight dictates resultsing weight
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein,
                0.0);
            currentRegionIndex = 1;
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, detector);
            weight = _absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.Less(Math.Abs(weight - 0.001996), 1e-6); // should be 1.0 * 0.01/5.01
            // make previousDp weight = 0.9
            previousDp.Weight = 0.9;
            weight = _absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.Less(Math.Abs(weight), 1e-6); // weight should be 0.0
            previousDp.Weight = 1.0;  // set back weight for next test

            // specify Continuous random walk process: previousDp weight = or != dp weight dictates resultsing weight
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Continuous,
                PhaseFunctionType.HenyeyGreenstein,
                0.0);
            currentRegionIndex = 1;
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, detector);
            weight = _absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.Less(Math.Abs(weight - 0.1), 1e-6); // should be previousWeight - weight
            // make previousDp weight = 0.9
            previousDp.Weight = 0.9;
            weight = _absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.Less(Math.Abs(weight), 1e-6); // weight should be 0.0
        }
        /// <summary>
        /// test for pMC weight factor for "terminal" detectors (e.g. reflectance)
        /// execute tests for Analog, Discrete, Continuous random walk processes
        /// </summary>
        [Test]
        public void Validate_GetpMCTerminalAbsorptionWeightingMethod_func_is_correct()
        {
            Func<IList<long>, IList<double>, IList<OpticalProperties>, IList<OpticalProperties>, IList<int>, double> _absorbAction;
            var tissueInput = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });

            // specify Analog random walk process: PhotonStateType.Absorbed dictates weight result
            var tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Analog,
                PhaseFunctionType.HenyeyGreenstein,
                0.0);
            var numberOfCollisions = new List<long>() {0, 10, 0};
            var pathLengths = new List<double>() {0, 100, 0};
            var referenceOps = new List<OpticalProperties>()
            {
                new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
            };
            var perturbedOps = new List<OpticalProperties>()
            {
                new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                new OpticalProperties(0.1, 1.0, 0.8, 1.4),
                new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
            };
            var perturbedRegionsIndices = new List<int>() {1};
            var detector = new pMCROfRhoDetector();
            Assert.Throws<NotImplementedException>(() =>
                AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(
                    tissue, detector));

            // specify Discrete random walk process:
            // numberOfCollisions>0 and reference mus>0 dictates results
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein,
                0.0);
            _absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(
                tissue, detector);
            var weightFactor = _absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.Less(Math.Abs(weightFactor - 0.000123), 1e-6);
            // set reference mus to 0
            referenceOps[1].Mus = 0; 
            weightFactor = _absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.Less(Math.Abs(weightFactor), 1e-6); // should be 0
            // set reference mus back for next test
            referenceOps[1].Mus = 5.0;

            // specify Continuous random walk process
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Continuous,
                PhaseFunctionType.HenyeyGreenstein,
                0.0);
            _absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(
                tissue, detector);
            weightFactor = _absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.Less(Math.Abs(weightFactor - 0.000123), 1e-6);
            // set reference mus to 0
            referenceOps[1].Mus = 0;
            weightFactor = _absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.Less(Math.Abs(weightFactor), 1e-6); // should be 0
        }
    }
}

