using System.Collections.Generic;

using System.Runtime.Serialization;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Implements IInclusionInput.  Defines input to MultiLayerTissue class for inclusion.
    /// </summary>
    public class MultiLayerInclusionInput : IInclusionInput
    {
        private IInclusionRegion[] _regions;

        /// <summary>
        /// constructor for Multi-layer inclusion input
        /// </summary>
        /// <param name="regions">list of tissue regions comprising tissue</param>
        public MultiLayerInclusionInput(IInclusionRegion[] regions)
        {
            _regions = regions;
        }

        /// <summary>
        /// MultiLayerTissue default constructor 
        /// </summary>
        public MultiLayerInclusionInput()
            : this(
                new IInclusionRegion[]
                { 
                    new LayerInclusionRegion(
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        new Position(0,0,0),
                        0.0),
                    new LayerInclusionRegion(
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                        new Position(0,0,0.05),
                        0.01),
                    new LayerInclusionRegion(
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        new Position(0,0,0),
                        0.0),
                })
        {
        }
        /// <summary>
        /// tissue identifier
        /// </summary>
        [IgnoreDataMember]
        public InclusionType InclusionType { get { return InclusionType.MultiLayerCircular; } }

        
        /// <summary>
        /// list of tissue regions comprising tissue
        /// </summary>
        public IInclusionRegion[] Regions { get { return _regions; } set { _regions = value; } }
    }
}
