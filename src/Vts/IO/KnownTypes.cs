using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.IO
{
    /// <summary>
    /// Class created to work-around the issues with the DataContractSerializer in Mono
    /// </summary>
    public static class KnownTypes
    {
        private static readonly IDictionary<string,Type> _types;

        static KnownTypes()
        {
            var knownTypesArray = new[]
                {
					typeof (AOfRhoAndZDetector),
					typeof (ATotalDetector),
					typeof (FluenceOfRhoAndZDetector),
					typeof (FluenceOfRhoAndZAndTimeDetector),
					typeof (MomentumTransferOfRhoAndZDetector),
					typeof (pMCROfRhoDetector),
					typeof (pMCROfRhoAndTimeDetector),
					typeof (RadianceOfRhoAndZAndAngleDetector),
                    typeof (RDiffuseDetector),
					typeof (ROfAngleDetector),
					typeof (ROfRhoAndAngleDetector),
					typeof (ROfRhoAndOmegaDetector),
					typeof (ROfRhoAndTimeDetector),
					typeof (ROfRhoDetector),
					typeof (ROfXAndYDetector),
					typeof (RSpecularDetector),
					typeof (ROfFxDetector),
					typeof (TDiffuseDetector),
					typeof (TOfAngleDetector),
					typeof (TOfRhoAndAngleDetector),
					typeof (TOfRhoDetector),

					typeof (DirectionalCircularSourceInput),
					typeof (DirectionalPointSourceInput),
                    typeof (IsotropicPointSourceInput),
                    typeof (CustomPointSourceInput),
                    typeof (CustomLineSourceInput),
                    typeof (CustomCircularSourceInput),
                    typeof (DirectionalLineSourceInput),
                    typeof (FlatSourceProfile),
                    typeof (GaussianSourceProfile),
					// typeof (DirectionalPointSource), todo: add all sources...
                };

            _types = knownTypesArray.ToDictionary(type => type.ToString());
        }

        /// <summary>
        /// Gets a dictionary of the current known types
        /// </summary>
        public static IDictionary<string, Type> CurrentKnownTypes { get { return _types; } }

        /// <summary>
        /// Adds a new type to the list of known types
        /// </summary>
        /// <param name="t">The type to add</param>
        public static void Add(Type t)
        {
            if(!_types.ContainsKey(t.ToString()))
            {
                _types.Add(t.ToString(), t);
            }
        }
    }
}
