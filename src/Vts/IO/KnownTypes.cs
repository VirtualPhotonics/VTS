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
            var knownTypesArray = new [] {
				
				    // in order of files in folder Vts.MonteCarlo.DataStructures.SourceInput				
                    typeof (CustomLineSourceInput),
                    typeof (DirectionalLineSourceInput),
					typeof (IsotropicLineSourceInput),
				
                    typeof (CustomPointSourceInput),
					typeof (DirectionalPointSourceInput),
                    typeof (IsotropicPointSourceInput),
				
					typeof (LambertianSurfaceEmittingCylindricalFiberSourceInput),
					typeof (CustomSurfaceEmittingSphericalSourceInput),
					typeof (LambertianSurfaceEmittingSphericalSourceInput),
					typeof (LambertianSurfaceEmittingTubularSourceInput),
				
					typeof (CustomCircularSourceInput),
					typeof (DirectionalCircularSourceInput),
					typeof (CustomEllipticalSourceInput),
					typeof (DirectionalEllipticalSourceInput),
					typeof (CustomRectangularSourceInput),
					typeof (DirectionalRectangularSourceInput),
				
					typeof (CustomVolumetricEllipsoidalSourceInput),
					typeof (IsotropicVolumetricEllipsoidalSourceInput),
					typeof (CustomVolumetricCuboidalSourceInput),
					typeof (IsotropicVolumetricCuboidalSourceInput),
				
					// tissue types
					typeof (MultiLayerTissueInput),
					typeof (MultiEllipsoidTissueInput),
					typeof (SingleEllipsoidTissueInput),
					typeof (LayerTissueRegion), 
					typeof (EllipsoidTissueRegion),				
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
