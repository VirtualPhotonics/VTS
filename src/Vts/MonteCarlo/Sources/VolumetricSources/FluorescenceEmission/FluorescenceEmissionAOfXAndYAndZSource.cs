﻿using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for FluorescenceEmissionAOfXAndYAndZSource 
    /// implementation.  This source reads the Cartesian coordinate absorbed energy results of a
    /// prior simulation and uses it to generate an emission source.  There are two methods
    /// used to sample the absorbed energy: CDF or Uniform.
    /// Using the CDF method, a probability density function (PDF) and resulting cumulative
    /// function (CDF) is determined from the absorbed energy map.  Then the number of photons
    /// launched from each fluorescent voxel is determined probabilistically from sampling the CDF.
    /// And the weight of each photon is set to the total absorbed energy of the fluorescing region
    /// because if N photons are launched the results are normalized by N producing back the total
    /// absorbed energy.
    /// Using the Uniform method, the same number of photons is sent out from each voxel with weight equal
    /// to the absorbed energy of that voxel.  HOWEVER, the results of a multiple voxel source
    /// CANNOT be combined because the initial weight is different  So if the fluorescing region
    /// has more than 1 voxel, each voxel must be simulated with a different simulation (possibly
    /// using the parallel MC).
    /// </summary>
    public class FluorescenceEmissionAOfXAndYAndZSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of FluorescenceEmissionAOfXAndYAndZSourceInput class
        /// </summary>
        /// <param name="inputFolder">Folder where AOfXAndYAndZ resides</param>
        /// <param name="infile">Infile for simulation that generated AOfXAndYAndZ</param>
        /// <param name="initialTissueRegionIndex">Tissue region of fluorescence</param>
        /// <param name="samplingMethod">sample initial position: CDF from AE or Uniform</param>
        public FluorescenceEmissionAOfXAndYAndZSourceInput(
            string inputFolder, string infile, int initialTissueRegionIndex, 
            SourcePositionSamplingType samplingMethod)
        {
            SourceType = "FluorescenceEmissionAOfXAndYAndZ";
            InputFolder = inputFolder;
            Infile = infile;
            InitialTissueRegionIndex = initialTissueRegionIndex;
            SamplingMethod = samplingMethod;
        }

        /// <summary>
        /// Initializes the default constructor of FluorescenceEmissionAOfXAndYAndZSourceInput class
        /// </summary>
        public FluorescenceEmissionAOfXAndYAndZSourceInput()
            : this("", "",  0, SourcePositionSamplingType.CDF) { }

        /// <summary>
        ///  fluorescence emission source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// Input folder where AE(x,y,z) resides
        /// </summary>
        public string InputFolder { get; set; }
        /// <summary>
        /// Infile that generated AOfXAndYAndZ
        /// </summary>
        public string Infile { get; set; }
        /// <summary>
        /// Initial tissue region index = tissue region index of fluorescence
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }
        /// <summary>
        /// Sampling method for location and associated weight
        /// </summary>
        public SourcePositionSamplingType SamplingMethod { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <returns>instantiated source</returns>
        public ISource CreateSource(Random rng = null)
        {
            rng = rng ?? new Random();

            return new FluorescenceEmissionAOfXAndYAndZSource(
                this.InputFolder,
                this.Infile,
                this.InitialTissueRegionIndex,
                this.SamplingMethod) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements FluorescenceEmissionAOfXAndYAndZSource with AOfXAndYAndZ created from prior
    /// simulation and initial tissue region index.
    /// </summary>
    public class FluorescenceEmissionAOfXAndYAndZSource : FluorescenceEmissionSourceBase
    {
        /// <summary>
        /// class that holds all Source arrays for proper initiation
        /// </summary>
        public AOfXAndYAndZLoader Loader { get; set; }
        /// <summary>
        /// Sampling method flag
        /// </summary>
        public SourcePositionSamplingType SamplingMethod { get; set; }

        /// <summary>
        /// key into dictionary of indices
        /// </summary>
        public static int IndexCount { get; set; }

        /// <summary>
        /// Returns an instance of  Fluorescence Emission AOfXAndYAndZ Source with
        /// a Lambertian angular distribution.
        /// </summary>
        /// <param name="inputFolder">folder where A(x,y,z) resides</param>
        /// <param name="infile">infile that generated A(x,y,z)</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        /// <param name="samplingMethod">SourcePositionSamplingtype(CDF,Uniform)</param>
        public FluorescenceEmissionAOfXAndYAndZSource(
            string inputFolder,
            string infile,
            int initialTissueRegionIndex,
            SourcePositionSamplingType samplingMethod)
            : base(
                inputFolder,
                infile,
                initialTissueRegionIndex)
        {
            SamplingMethod = samplingMethod;
            // Loader loads absorbed energy results in grid MINUS final bins
            // which contain results of tallies beyond final bin
            Loader = new AOfXAndYAndZLoader(inputFolder, infile, initialTissueRegionIndex);
        }
        /// <summary>
        /// Method to determine source photon position and weight
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <param name="weight">return photon weight</param>
        /// <returns>photon position</returns>
        protected override Position GetFinalPositionAndWeight(Random rng, out double weight)
        {
            double xMidpoint, yMidpoint, zMidpoint;
            switch (SamplingMethod)
            {
                case SourcePositionSamplingType.CDF:
                    // determine position from CDF determined in AOfXAndYAndZLoader
                    // due to ordering of indices CDF will be increasing with each increment
                    var rho = rng.NextDouble();
                    // omit final bins from fluorescent source so Count-2
                    for (var i = 0; i < Loader.X.Count - 3; i++)
                    {
                        for (var j = 0; j < Loader.Y.Count - 3; j++)
                        {
                            for (var k = 0; k < Loader.Z.Count - 2; k++)
                            {
                                if (Loader.MapOfXAndYAndZ[i, j, k] != 1) continue;
                                if (rho >= Loader.CDFOfXAndYAndZ[i, j, k]) continue;
                                xMidpoint = Loader.X.Start + i * Loader.X.Delta + Loader.X.Delta / 2;
                                yMidpoint = Loader.Y.Start + j * Loader.Y.Delta + Loader.Y.Delta / 2;
                                zMidpoint = Loader.Z.Start + k * Loader.Z.Delta + Loader.Z.Delta / 2;
                                weight = Loader.TotalAbsorbedEnergy;
                                return new Position(xMidpoint, yMidpoint, zMidpoint);
                            }
                        }
                    }
                    weight = 1.0;
                    return null;
                case SourcePositionSamplingType.Uniform:
                    // rotate through indices by starting over if reached end
                    if (IndexCount > Loader.FluorescentRegionIndicesInOrder.Count - 1)
                    {
                        IndexCount = 0;
                    }
                    var indices = Loader.FluorescentRegionIndicesInOrder[IndexCount].ToArray();
                    var ix = indices[0];
                    var iy = indices[1];
                    var iz = indices[2];
                    xMidpoint = Loader.X.Start + ix * Loader.X.Delta + Loader.X.Delta / 2;
                    yMidpoint = Loader.Y.Start + iy * Loader.Y.Delta + Loader.Y.Delta / 2;
                    zMidpoint = Loader.Z.Start + iz * Loader.Z.Delta + Loader.Z.Delta / 2;
                    // undo normalization performed when AOfXAndYAndZDetector saved
                    var xyzNorm = Loader.X.Delta * Loader.Y.Delta * Loader.Z.Delta;
                    weight = Loader.AOfXAndYAndZ[ix, iy, iz] * xyzNorm;
                    IndexCount += 1;
                    return new Position(xMidpoint, yMidpoint, zMidpoint);
                default:
                    throw new ArgumentOutOfRangeException(SamplingMethod.ToString());
            }
        }


    }

}
