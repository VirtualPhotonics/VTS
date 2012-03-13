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
    public class MultiLayerInclusion 
    {
        private IList<LayerInclusionRegion> _layerRegions;

        /// <summary>
        /// Creates an instance of a MultiLayerTissue
        /// </summary>
        /// <param name="regions">list of tissue regions comprising tissue</param>
        public MultiLayerInclusion(
            IList<IInclusionRegion> regions)
        {
            _layerRegions = regions.Select(region => (LayerInclusionRegion) region).ToArray();
        }

      
        /// <summary>
        /// Creates a default instance of a MultiLayerTissue 
        /// </summary>
        public MultiLayerInclusion() 
            : this(new MultiLayerInclusionInput().Regions){}
    }
}
