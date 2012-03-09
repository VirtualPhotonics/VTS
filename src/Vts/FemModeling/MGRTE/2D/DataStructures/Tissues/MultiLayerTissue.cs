using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Implements ITissue.  Defines tissue geometries comprised of layers
    /// (including homogenous with air layers above and below).  Layers are infinite along
    /// x- and y- axes.
    /// </summary>
    public class MultiLayerTissue 
    {
        private IList<InclusionLayerRegion> _layerRegions;

        /// <summary>
        /// Creates an instance of a MultiLayerTissue
        /// </summary>
        /// <param name="regions">list of tissue regions comprising tissue</param>
        public MultiLayerTissue(
            IList<ITissueRegion> regions)
        {
            _layerRegions = regions.Select(region => (InclusionLayerRegion) region).ToArray();
        }

      
        /// <summary>
        /// Creates a default instance of a MultiLayerTissue based on a homogeneous medium slab geometry
        /// and discrete absorption weighting
        /// </summary>
        public MultiLayerTissue() 
            : this(new MultiLayerTissueInput().Regions){}
       
        
    }
}
