using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;

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
            var knownTypesArray = new Type[] {	
	                // nothing here anymore...not using XML DataContractSerializer
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
