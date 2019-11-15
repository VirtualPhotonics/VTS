using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for FluorescenceEmissionAOfXAndYAndZSource 
    /// implementation.  This source reads the Cartesian coordinate absorbed energy results of a
    /// prior simulation and uses it to generate an emission source.
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
        /// <param name="rng"></param>
        /// <returns></returns>
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
        double _totalWeight = 0.0;
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
        public static int IndexCount = 0;

        /// <summary>
        /// Returns an instance of  Fluorescence Emission AOfXAndYAndZ Source with
        /// a Lambertian angular distribution.
        /// </summary>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
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
            Loader = new AOfXAndYAndZLoader(inputFolder, infile, initialTissueRegionIndex);
        }

        protected override Position GetFinalPositionAndWeight(Random rng, out double weight)
        {
            Position finalPosition = null;
            double xMidpoint, yMidpoint, zMidpoint;
            switch (SamplingMethod)
            {
                case SourcePositionSamplingType.CDF:
                    // determine position from CDF determined in AOfXAndYAndZLoader
                    // due to ordering of indices CDF will be increasing with each increment
                    double rho = rng.NextDouble();
                    for (int i = 0; i < Loader.X.Count - 1; i++)
                    {
                        for (int j = 0; j < Loader.Y.Count - 1; j++)
                        {
                            for (int k = 0; k < Loader.Z.Count - 1; k++)
                            {
                                if (Loader.MapOfXAndYAndZ[i, j, k] == 1)
                                {
                                    if (rho < Loader.CDFOfXAndYAndZ[i, j, k])
                                    {
                                        xMidpoint = Loader.X.Start + i * Loader.X.Delta + Loader.X.Delta / 2;
                                        yMidpoint = Loader.Y.Start + j * Loader.Y.Delta + Loader.Y.Delta / 2;
                                        zMidpoint = Loader.Z.Start + k * Loader.Z.Delta + Loader.Z.Delta / 2;
                                        // the following outputs initial positions so that a plot can show distribution
                                        //Console.WriteLine(xMidpoint.ToString("") + " " +
                                        //                  yMidpoint.ToString("") + " " +
                                        //                  zMidpoint.ToString(""));
                                        weight = 1.0;
                                        return new Position(xMidpoint, yMidpoint, zMidpoint);
                                    }
                                }
                            }
                        }
                    }
                    weight = 1.0;
                    return finalPosition;
                case SourcePositionSamplingType.Uniform:
                    // rotate through indices by starting over if reached end
                    if (IndexCount > Loader.FluorescentRegionIndicesInOrder.Count - 1)
                    {
                        IndexCount = 0;
                        // the following output is to verify after each cycle through voxels total AE correct
                        //Console.WriteLine("totalWeight = " + _totalWeight.ToString(""));
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
                    _totalWeight = _totalWeight + weight;
                    IndexCount = IndexCount + 1;
                    return new Position(xMidpoint, yMidpoint, zMidpoint);
            }
            weight = 1.0;
            return finalPosition;
        }


    }

}
