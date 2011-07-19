using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Detectors;

namespace Vts.IO
{
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
					typeof (TDiffuseDetector),
					typeof (TOfAngleDetector),
					typeof (TOfRhoAndAngleDetector),
					typeof (TOfRhoDetector),
                };

            _types = knownTypesArray.ToDictionary(type => type.ToString());
        }

        public static IDictionary<string, Type> CurrentKnownTypes { get { return _types; } }

        public static void Add(Type t)
        {
            if(!_types.ContainsKey(t.ToString()))
            {
                _types.Add(t.ToString(), t);
            }
        }
    }
}
