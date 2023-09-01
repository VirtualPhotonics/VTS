using System;
using System.Collections.Generic;
using System.Linq;
using Vts.IO;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate detector tally given TallyType.
    /// </summary>
    public class DetectorFactory
    {
        /// <summary>
        /// Method to instantiate all detectors given list of IDetectorInputs.  This method calls
        /// the method below that instantiates a single detector.
        /// </summary>
        /// <param name="detectorInputs">IEnumerable of IDetectorInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="rng">random number generator</param>
        /// <returns>List of IDetector</returns>
        public static IList<IDetector> GetDetectors(IEnumerable<IDetectorInput> detectorInputs, ITissue tissue, Random rng)
        {
            var detectors = detectorInputs?.Select(
                detectorInput => GetDetector(detectorInput, tissue, rng)).ToList();

            return detectors;
        }

        /// <summary>
        /// Method to instantiate a single IDetectorInput.  This method is called by
        /// the method below that instantiates a list of detectors.
        /// </summary>
        /// <param name="detectorInput">IEnumerable of IDetectorInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="rng">random number generator</param>
        /// <returns>IDetector</returns>
        public static IDetector GetDetector(IDetectorInput detectorInput, ITissue tissue, Random rng)
        {
            if (detectorInput == null)
            {
                return null;
            }

            var detector = detectorInput.CreateDetector();

            detector.Initialize(tissue, rng);

            return detector;
        }

        /// <summary>
        /// Method to register detector: currently not used
        /// </summary>
        /// <param name="detectorInputType">type of detector input</param>
        /// <param name="detectorType">type of detector</param>
        public static void RegisterDetector(Type detectorInputType, Type detectorType)
        {
            if (detectorInputType == null) return;

            // check that the detector input implements the IDetectorInput interface
            if (!typeof (IDetectorInput).IsAssignableFrom(detectorInputType))
                throw new ArgumentException("Cannot register detector input " +
                                            detectorInputType.AssemblyQualifiedName +
                                            " because it does not implement the Vts.MonteCarlo.IDetectorInput interface.");

            // check that the detector implements the IDetector interface
            if (!typeof(IDetector).IsAssignableFrom(detectorType))
                throw new ArgumentException("Cannot register detector " + detectorType.AssemblyQualifiedName +
                                            " because it does not implement the Vts.MonteCarlo.IDetector interface.");

            // also check that the input has a parameter-less constructor (assuming this in the following line)
            if (detectorInputType.GetConstructors().All(c => c.GetParameters().Any()))
            {
                throw new ArgumentException("Cannot register detector input " + detectorInputType.AssemblyQualifiedName +
                    " because it does not have a parameter-less (default) constructor.");
            }

            var detectorInput = (IDetectorInput) Activator.CreateInstance(detectorInputType);

            if (detectorInput == null) return;

            foreach (var knownConverter in VtsJsonSerializer.KnownConverters)
            {
                if (knownConverter.GetType() == typeof(ConventionBasedConverter<IDetectorInput>))
                {
                    var knownDetectorInput = (ConventionBasedConverter<IDetectorInput>)knownConverter;
                    knownDetectorInput.AddUserDefinedServices(detectorInputType, "TallyType", detectorInput.TallyType);
                }

                if (knownConverter.GetType() != typeof(ConventionBasedConverter<IDetector>)) continue;
                var knownDetector = (ConventionBasedConverter<IDetector>)knownConverter;
                knownDetector.AddUserDefinedServices(detectorType, "TallyType", detectorInput.TallyType);
            }
        }
    }
}
