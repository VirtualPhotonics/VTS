using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for FluorescenceEmissionAOfRhoAndZSource 
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
    public class FluorescenceEmissionAOfRhoAndZSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of FluorescenceEmissionAOfRhoAndZSourceInput class
        /// </summary>
        /// <param name="inputFolder">Folder where AOfRhoAndZ resides</param>
        /// <param name="infile">Infile for simulation that generated AOfRhoAndZ</param>
        /// <param name="initialTissueRegionIndex">Tissue region of fluorescence</param>
        /// <param name="samplingMethod">sample initial position: CDF from AE or Uniform</param>
        public FluorescenceEmissionAOfRhoAndZSourceInput(
            string inputFolder, string infile, int initialTissueRegionIndex, 
            SourcePositionSamplingType samplingMethod)
        {
            SourceType = "FluorescenceEmissionAOfRhoAndZ";
            InputFolder = inputFolder;
            Infile = infile;
            InitialTissueRegionIndex = initialTissueRegionIndex;
            SamplingMethod = samplingMethod;
        }

        /// <summary>
        /// Initializes the default constructor of FluorescenceEmissionAOfRhoAndZSourceInput class
        /// </summary>
        public FluorescenceEmissionAOfRhoAndZSourceInput()
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
        /// Infile that generated AOfRhoAndZ
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

            return new FluorescenceEmissionAOfRhoAndZSource(
                this.InputFolder,
                this.Infile,
                this.InitialTissueRegionIndex,
                this.SamplingMethod) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements FluorescenceEmissionAOfRhoAndZSource with AOfRhoAndZ created from prior
    /// simulation and initial tissue region index.
    /// </summary>
    public class FluorescenceEmissionAOfRhoAndZSource : FluorescenceEmissionSourceBase
    {
        /// <summary>
        /// class that holds all Source arrays for proper initiation
        /// </summary>
        public AOfRhoAndZLoader Loader { get; set; }
        /// <summary>
        /// Sampling method flag
        /// </summary>
        public SourcePositionSamplingType SamplingMethod { get; set; }

        /// <summary>
        /// key into dictionary of indices
        /// </summary>
        public static int IndexCount { get; set; }

        /// <summary>
        /// Returns an instance of  Fluorescence Emission AOfRhoAndZ Source with
        /// a Lambertian angular distribution.
        /// </summary>
        /// <param name="inputFolder">folder where A(rho,z) resides</param>
        /// <param name="infile">infile that generated A(rho,z)</param>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        /// <param name="samplingMethod">SourcePositionSamplingType(CDF,Uniform)</param>
        public FluorescenceEmissionAOfRhoAndZSource(
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
            Loader = new AOfRhoAndZLoader(inputFolder, infile, initialTissueRegionIndex);
        }

        /// <summary>
        /// method to determine final fluorescent source photon position and weight
        /// </summary>
        /// <param name="rng">random number generator</param>
        /// <param name="weight">return weight</param>
        /// <returns>photon position</returns>
        protected override Position GetFinalPositionAndWeight(Random rng, out double weight)
        {
            double xMidpoint, yMidpoint, zMidpoint;
            double r1, r2;
            switch (SamplingMethod)
            {
                case SourcePositionSamplingType.CDF:
                    // determine position from CDF determined in AOfRhoAndZLoader
                    // due to ordering of indices CDF will be increasing with each increment
                    var rho = rng.NextDouble();
                    // omit final bins from fluorescent source so Count-2
                    for (var i = 0; i < Loader.Rho.Count - 2; i++)
                    {
                        for (var k = 0; k < Loader.Z.Count - 2; k++)
                        {
                            if (Loader.MapOfRhoAndZ[i, k] != 1) continue;
                            if (rho >= Loader.CDFOfRhoAndZ[i, k]) continue;
                            // following code assumes tissue region is cylindrical in nature
                            // therefore, if (rho,z) in region, (x=rho,0,z) is in region
                            // algorithm adopted from Jacques' mcfluor.c
                            r1 = i * Loader.Rho.Delta;
                            r2 = (i + 1) * Loader.Rho.Delta;
                            xMidpoint = (2.0 / 3.0) * (r2 * r2 + r2 * r1 + r1 * r1) / (r1 + r2);
                            yMidpoint = 0.0; 
                            zMidpoint = Loader.Z.Start + k * Loader.Z.Delta + Loader.Z.Delta / 2; 
                            weight = Loader.TotalAbsorbedEnergy;
                            return new Position(xMidpoint, yMidpoint, zMidpoint);
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
                    var iRho = indices[0];
                    var iZ = indices[1];
                    // following code assumes tissue region is cylindrical in nature
                    // therefore, if (rho,z) in region, (x=rho,0,z) is in region
                    // algorithm adopted from Jacques' mcfluor.c
                    r1 = iRho * Loader.Rho.Delta;
                    r2 = (iRho + 1) * Loader.Rho.Delta;
                    xMidpoint = (2.0 / 3.0) * (r2 * r2 + r2 * r1 + r1 * r1) / (r1 + r2); 
                    yMidpoint = 0.0;
                    zMidpoint = Loader.Z.Start + iZ * Loader.Z.Delta + Loader.Z.Delta / 2;
                    // undo normalization performed when AOfRhoAndZDetector saved
                    var normalizationFactor = 2.0 * Math.PI * Loader.Rho.Delta * Loader.Z.Delta;
                    var rhoZNorm = (Loader.Rho.Start + (iRho + 0.5) * Loader.Rho.Delta) * normalizationFactor;
                    weight = Loader.AOfRhoAndZ[iRho, iZ] * rhoZNorm;
                    IndexCount += 1;
                    return new Position(xMidpoint, yMidpoint, zMidpoint);
                default:
                    throw new ArgumentOutOfRangeException(SamplingMethod.ToString());
            }
        }

    }

}
