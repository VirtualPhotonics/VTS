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
                    typeof (RDiffuseDetector),
					typeof (TDiffuseDetector),
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
