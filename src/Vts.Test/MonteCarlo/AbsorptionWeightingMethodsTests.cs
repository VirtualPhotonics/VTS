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
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                });
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            IDictionary<string, IPhaseFunction> regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            foreach (var tissueRegion in tissueInput.Regions)
            {
                if (!regionPhaseFunctions.ContainsKey(tissueRegion.PhaseFunctionKey))
                {
                    regionPhaseFunctions.Add(tissueRegion.PhaseFunctionKey,
                        PhaseFunctionFactory.GetPhaseFunction(tissueRegion, 
                            tissueInput, new MersenneTwister(0)));
                }
            }
            // specify Analog random walk process: PhotonStateType.Absorbed dictates weight result
            var tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Analog,
                regionPhaseFunctions,
                0.0);
            var currentRegionIndex = 1;
            var rng = new MersenneTwister(0);
            var detectorInput = new ATotalDetectorInput();
            var detector = DetectorFactory.GetDetector(detectorInput, tissue, rng);
            var absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, detector);
            var weight = absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.That(Math.Abs(weight), Is.LessThan(1e-6)); // should be 0.0
            // turn on PhotonStateType
            dp.StateFlag = PhotonStateType.Absorbed;
            weight = absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.That(Math.Abs(weight - 1.0), Is.LessThan(1e-6)); // weight should be 1.0

            // specify Discrete random walk process: previousDp weight = or != dp weight dictates resultsing weight
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Discrete,
                regionPhaseFunctions,
                0.0);
            currentRegionIndex = 1;
            absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, detector);
            weight = absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.That(Math.Abs(weight - 0.001996), Is.LessThan(1e-6)); // should be 1.0 * 0.01/5.01
            // make previousDp weight = 0.9
            previousDp.Weight = 0.9;
            weight = absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.That(Math.Abs(weight), Is.LessThan(1e-6)); // weight should be 0.0
            previousDp.Weight = 1.0;  // set back weight for next test

            // specify Continuous random walk process: previousDp weight = or != dp weight dictates resultsing weight
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Continuous,
                regionPhaseFunctions,
                0.0);
            currentRegionIndex = 1;
            absorptionWeightingMethod = AbsorptionWeightingMethods.GetVolumeAbsorptionWeightingMethod(tissue, detector);
            weight = absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.That(Math.Abs(weight - 0.1), Is.LessThan(1e-6)); // should be previousWeight - weight
            // make previousDp weight = 0.9
            previousDp.Weight = 0.9;
            weight = absorptionWeightingMethod(previousDp, dp, currentRegionIndex);
            Assert.That(Math.Abs(weight), Is.LessThan(1e-6)); // weight should be 0.0
        }

        /// <summary>
        /// test for pMC weight factor for "terminal" detectors (e.g. reflectance)
        /// execute tests for Analog, Discrete, Continuous random walk processes
        /// </summary>
        [Test]
        public void Validate_GetpMCTerminalAbsorptionWeightingMethod_func_is_correct()
        {
            var tissueInput = new MultiLayerTissueInput(
            [
                new LayerTissueRegion(
                    new DoubleRange(double.NegativeInfinity, 0.0),
                    new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                    "HenyeyGreensteinKey1"),
                new LayerTissueRegion(
                    new DoubleRange(0.0, 100.0),
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                    "HenyeyGreensteinKey2"),
                new LayerTissueRegion(
                    new DoubleRange(100.0, double.PositiveInfinity),
                    new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                    "HenyeyGreensteinKey3")
            ]);
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
                
            IDictionary<string, IPhaseFunction> regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            foreach (var tissueRegion in tissueInput.Regions)
            {
                if (!regionPhaseFunctions.ContainsKey(tissueRegion.PhaseFunctionKey))
                {
                    regionPhaseFunctions.Add(tissueRegion.PhaseFunctionKey,
                        PhaseFunctionFactory.GetPhaseFunction(tissueRegion,
                            tissueInput, new MersenneTwister(0)));
                }
            }
            // specify Analog random walk process: PhotonStateType.Absorbed dictates weight result
            var tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Analog,
                regionPhaseFunctions,
                0.0);
            var numberOfCollisions = new List<long> {0, 10, 0};
            var pathLengths = new List<double> {0, 100, 0};
            var referenceOps = new List<OpticalProperties>
            {
                new(0.0, 1e-10, 1.0, 1.0),
                new(0.01, 1.0, 0.8, 1.4),
                new(0.0, 1e-10, 1.0, 1.0),
            };
            var perturbedOps = new List<OpticalProperties>
            {
                new(0.0, 1e-10, 1.0, 1.0),
                new(0.1, 1.0, 0.8, 1.4),
                new(0.0, 1e-10, 1.0, 1.0),
            };
            var perturbedRegionsIndices = new List<int> {1};
            var detector = new pMCROfRhoDetector();
            Assert.Throws<NotImplementedException>(() =>
                AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(
                    tissue, detector));

            // specify Discrete random walk process:
            // numberOfCollisions>0 and reference mus>0 dictates results
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Discrete,
                regionPhaseFunctions,
                0.0);
            var absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(
                tissue, detector);
            var weightFactor = absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.That(Math.Abs(weightFactor - 0.000123), Is.LessThan(1e-6));
            // set reference mus to 0 to test else of code
            referenceOps[1].Mus = 0; 
            weightFactor = absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            // the weight factor when Mus=0 should be 0 because exponential weight has -(100*5)
            // in exponent
            Assert.That(Math.Abs(weightFactor), Is.LessThan(1e-6));
            // set reference mus back for next test
            referenceOps[1].Mus = 5.0;

            // specify Continuous random walk process
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Continuous,
                regionPhaseFunctions,
                0.0);
            absorbAction = AbsorptionWeightingMethods.GetpMCTerminationAbsorptionWeightingMethod(
                tissue, detector);
            weightFactor = absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.That(Math.Abs(weightFactor - 0.000123), Is.LessThan(1e-6));
            // set reference mus to 0
            referenceOps[1].Mus = 0;
            weightFactor = absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.That(Math.Abs(weightFactor), Is.LessThan(1e-6)); // should be 0
        }

        /// <summary>
        /// Test for dMC weight factor for "terminal" detectors (e.g. reflectance)
        /// execute tests for Analog and (Discrete or Continuous: same code) random walk processes
        /// </summary>
        [Test]
        public void Validate_GetdMCTerminalAbsorptionWeightingMethod_func_is_correct()
        {
            var tissueInput = new MultiLayerTissueInput(
            [
                new LayerTissueRegion(
                    new DoubleRange(double.NegativeInfinity, 0.0),
                    new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                    "HenyeyGreensteinKey1"),
                new LayerTissueRegion(
                    new DoubleRange(0.0, 100.0),
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                    "HenyeyGreensteinKey2"),
                new LayerTissueRegion(
                    new DoubleRange(100.0, double.PositiveInfinity),
                    new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                    "HenyeyGreensteinKey3")
            ]);
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());

            IDictionary<string, IPhaseFunction> regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            foreach (var tissueRegion in tissueInput.Regions)
            {
                if (!regionPhaseFunctions.ContainsKey(tissueRegion.PhaseFunctionKey))
                {
                    regionPhaseFunctions.Add(tissueRegion.PhaseFunctionKey,
                        PhaseFunctionFactory.GetPhaseFunction(tissueRegion,
                            tissueInput, new MersenneTwister(0)));
                }
            }

            // specify Analog random walk process: PhotonStateType.Absorbed dictates weight result
            var tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Analog,
                regionPhaseFunctions,
                0.0);
            var numberOfCollisions = new List<long> { 0, 10, 0 };
            var pathLengths = new List<double> { 0, 100, 0 };
            var referenceOps = new List<OpticalProperties>
            {
                new(0.0, 1e-10, 1.0, 1.0),
                new(0.01, 1.0, 0.8, 1.4),
                new(0.0, 1e-10, 1.0, 1.0),
            };
            var perturbedOps = new List<OpticalProperties>
            {
                new(0.0, 1e-10, 1.0, 1.0),
                new(0.1, 1.0, 0.8, 1.4),
                new(0.0, 1e-10, 1.0, 1.0),
            };
            var perturbedRegionsIndices = new List<int> { 1 };
            var detector = new pMCROfRhoDetector();
            // check both derivatives
            Assert.Throws<NotImplementedException>(() =>
                AbsorptionWeightingMethods.GetdMCTerminationAbsorptionWeightingMethod(
                    tissue, detector, DifferentialMonteCarloType.DMua), "Analog cannot be used for dMC estimates.");
            Assert.Throws<NotImplementedException>(() =>
                AbsorptionWeightingMethods.GetdMCTerminationAbsorptionWeightingMethod(
                    tissue, detector, DifferentialMonteCarloType.DMus), "Analog cannot be used for dMC estimates.");

            // specify Discrete random walk process and dMua derivative
            // numberOfCollisions>0 and reference mus>0 dictates results
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Discrete,
                regionPhaseFunctions,
                0.0);
            var absorbAction = AbsorptionWeightingMethods.GetdMCTerminationAbsorptionWeightingMethod(
                tissue, detector, DifferentialMonteCarloType.DMua);
            var weightFactor = absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.That(Math.Abs(weightFactor + 0.012340), Is.LessThan(1e-6));
            // set reference mus to 0 to test else of code
            referenceOps[1].Mus = 0;
            weightFactor = absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            // the weight factor when Mus=0 should be 0 because exponential weight has -(100*5)
            // in exponent
            Assert.That(Math.Abs(weightFactor), Is.LessThan(1e-6)); 
            // set reference mus back for next test
            referenceOps[1].Mus = 5.0;

            // specify Continuous random walk process and dMus derivative
            tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Continuous,
                regionPhaseFunctions,
                0.0);
            absorbAction = AbsorptionWeightingMethods.GetdMCTerminationAbsorptionWeightingMethod(
                tissue, detector, DifferentialMonteCarloType.DMus);
            weightFactor = absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.That(Math.Abs(weightFactor + 0.012094), Is.LessThan(1e-6));
            // set reference mus to 0
            referenceOps[1].Mus = 0;
            weightFactor = absorbAction(
                numberOfCollisions,
                pathLengths,
                perturbedOps,
                referenceOps,
                perturbedRegionsIndices);
            Assert.That(Math.Abs(weightFactor), Is.LessThan(1e-6)); // should be 0
        }
    }
}

