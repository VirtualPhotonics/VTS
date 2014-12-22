using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleEllipsoidTissue class.
    /// </summary>
    public class SingleEllipsoidTissueInput : TissueInput, ITissueInput
    {
        private ITissueRegion _ellipsoidRegion;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// allows definition of single ellipsoid tissue
        /// </summary>
        /// <param name="ellipsoidRegion">ellipsoid region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public SingleEllipsoidTissueInput(ITissueRegion ellipsoidRegion, ITissueRegion[] layerRegions)
        {
            TissueType = "SingleEllipsoid";
            _ellipsoidRegion = ellipsoidRegion;
            _layerRegions = layerRegions;
        }

        /// <summary>
        /// SingleEllipsoidTissueInput default constructor provides homogeneous tissue with single ellipsoid
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public SingleEllipsoidTissueInput()
            : this(
                new EllipsoidTissueRegion(
                    new Position(0, 0, 1),
                    0.5,
                    0.5,
                    0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                ),
                new ITissueRegion[] 
                { 
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0))
                })
        {
        }

        /// <summary>
        /// regions of tissue (layers and ellipsoid)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return _layerRegions.Concat(_ellipsoidRegion).ToArray(); } }
        /// <summary>
        /// tissue ellipsoid region
        /// </summary>
        public ITissueRegion EllipsoidRegion { get { return _ellipsoidRegion; } set { _ellipsoidRegion = value; } }
        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions { get { return _layerRegions; } set { _layerRegions = value; } }

        /// <summary>
        ///// Required factory method to create the corresponding 
        ///// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="pft">Phase Function Type</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns></returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold)
        {
            var t = new SingleInclusionTissue(EllipsoidRegion, LayerRegions);

            t.Initialize(awt, pft, russianRouletteWeightThreshold);

            return t;
        }
    }

    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleEllipsoidTissue class.
    /// </summary>
    public class MultiEllipsoidTissueInput : TissueInput, ITissueInput
    {
        private ITissueRegion[] _ellipsoidRegions;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// allows definition of single ellipsoid tissue
        /// </summary>
        /// <param name="ellipsoidRegions">ellipsoid region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public MultiEllipsoidTissueInput(ITissueRegion[] ellipsoidRegions, ITissueRegion[] layerRegions)
        {
            TissueType = "MultiEllipsoid";
            _ellipsoidRegions = ellipsoidRegions;
            _layerRegions = layerRegions;
        }

        /// <summary>
        /// SingleEllipsoidTissueInput default constructor provides homogeneous tissue with single ellipsoid
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public MultiEllipsoidTissueInput()
            : this(
                new ITissueRegion[]
                {
                    new EllipsoidTissueRegion(
                        new Position(10, 0, 10), 
                        5.0, 
                        1.0, 
                        5.0,
                        new OpticalProperties(0.1, 1.0, 0.8, 1.4)),
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 40), 
                        5.0, 
                        0, 
                        5.0,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                },
                new ITissueRegion[] 
                { 
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 50.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0))
                })
        {
        }

        /// <summary>
        /// regions of tissue (layers and ellipsoid)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return _layerRegions.Concat(_ellipsoidRegions).ToArray(); } }
        /// <summary>
        /// tissue ellipsoid region
        /// </summary>
        public ITissueRegion[] EllipsoidRegions { get { return _ellipsoidRegions; } set { _ellipsoidRegions = value; } }
        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions { get { return _layerRegions; } set { _layerRegions = value; } }
        
        /// <summary>
        ///// Required factory method to create the corresponding 
        ///// ITissue based on the ITissueInput data
        /// </summary>
        /// <param name="awt">Absorption Weighting Type</param>
        /// <param name="pft">Phase Function Type</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette Weight Threshold</param>
        /// <returns></returns>
        public ITissue CreateTissue(AbsorptionWeightingType awt, PhaseFunctionType pft, double russianRouletteWeightThreshold)
        {
            throw new NotImplementedException();

            //var t = new SingleInclusionTissue(EllipsoidRegions, LayerRegions); // todo: add implementation

            //t.Initialize(awt, pft, russianRouletteWeightThreshold);

            //return t;
        }
    }

    /// <summary>
    /// Implements ITissue.  Defines a tissue geometry comprised of an
    /// inclusion embedded within a layered slab.
    /// </summary>
    public class SingleInclusionTissue : MultiLayerTissue, ITissue
    {
        private ITissueRegion _inclusionRegion;
        private int _inclusionRegionIndex;
        private int _layerRegionIndexOfInclusion;

        /// <summary>
        /// Creates an instance of a SingleInclusionTissue
        /// </summary>
        /// <param name="inclusionRegion">The single inclusion (must be contained completely within a layer region)</param>
        /// <param name="layerRegions">The tissue layers</param>
        public SingleInclusionTissue(
            ITissueRegion inclusionRegion,
            IList<ITissueRegion> layerRegions)
            : base(layerRegions)
        {
            // overwrite the Regions property in the TissueBase class (will be called last in the most derived class)
            Regions = layerRegions.Concat(inclusionRegion).ToArray();

            _inclusionRegion = inclusionRegion;
            _inclusionRegionIndex = layerRegions.Count; // index is, by convention, after the layer region indices
            _layerRegionIndexOfInclusion = Enumerable.Range(0, layerRegions.Count)
                .FirstOrDefault(i => ((LayerTissueRegion)layerRegions[i]).ContainsPosition(_inclusionRegion.Center));
        }

        /// <summary>
        /// Creates a default instance of a SingleInclusionTissue
        /// </summary>
        public SingleInclusionTissue()
            : this(
                new EllipsoidTissueRegion(),
                new MultiLayerTissueInput().Regions) { }
        /// <summary>
        /// method to get tissue region index of photon's current position
        /// </summary>
        /// <param name="position">photon Position</param>
        /// <returns>integer tissue region index</returns>
        public int GetRegionIndex(Position position)
        {
            // if it's in the inclusion, return "3", otherwise, call the layer method to determine
            return _inclusionRegion.ContainsPosition(position) ? _inclusionRegionIndex : base.GetRegionIndex(position);
        }

        // todo: DC - worried that this is "uncombined" with GetDistanceToBoundary() from an efficiency standpoint
        // note, however that there are two overloads currently for RayIntersectBoundary, one that does extra work to calc distances
        /// <summary>
        /// method to get index of neighbor tissue region when photon on boundary of two regions
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>index of neighbor index</returns>
        public int GetNeighborRegionIndex(Photon photon)
        {
            // first, check what region the photon is in
            int regionIndex = photon.CurrentRegionIndex;

            // if we're outside the layer containing the inclusion, then just call the base method
            //if ((regionIndex != _inclusionRegionIndex) && (regionIndex != _layerRegionIndexOfInclusion))
            //{
            //    return base.GetNeighborRegionIndex(photon);
            //}

            // if we're in the layer region of the inclusion, could be on boundary of inclusion
            // or boundary of layer
            if ((regionIndex == _layerRegionIndexOfInclusion) && !Regions[_layerRegionIndexOfInclusion].OnBoundary(photon.DP.Position) )
            {
                return _inclusionRegionIndex;
            }

            if (regionIndex == _inclusionRegionIndex)
            {
                return _layerRegionIndexOfInclusion;
            }

            //// if we're actually inside the inclusion
            //if (_inclusionRegion.ContainsPosition(photon.DP.Position))
            //{
            //    // then the neighbor region is the layer containing the current photon position
            //    return layerRegionIndex;
            //}

            //// otherwise, it depends on which direction the photon's pointing from within the layer

            //// if the ray intersects the inclusion, the neighbor is the inclusion
            //if( _inclusionRegion.RayIntersectBoundary(photon) )
            //{
            //    return _inclusionRegionIndex;
            //}

            // otherwise we can do this with the base class method
            return base.GetNeighborRegionIndex(photon);
        }
        /// <summary>
        /// method to get distance from current photon position and direction to boundary of region
        /// </summary>
        /// <param name="photon">Photon</param>
        /// <returns>distance to boundary</returns>
        public double GetDistanceToBoundary(Photon photon)
        {
            // first, check what region the photon is in
            int regionIndex = photon.CurrentRegionIndex;

            if ((regionIndex == _layerRegionIndexOfInclusion) || (regionIndex == _inclusionRegionIndex))
            {
                // check if current track will hit the inclusion boundary, returning the correct distance
                double distanceToBoundary;
                if (_inclusionRegion.RayIntersectBoundary(photon, out distanceToBoundary))
                {
                    return distanceToBoundary;
                }
                else // otherwise, check that a projected track will hit the inclusion boundary
                {          
                    var projectedPhoton = new Photon();
                    projectedPhoton.DP = new PhotonDataPoint(photon.DP.Position, photon.DP.Direction, photon.DP.Weight, 
                        photon.DP.TotalTime, photon.DP.StateFlag);
                    projectedPhoton.S = 100;
                    if (_inclusionRegion.RayIntersectBoundary(projectedPhoton, out distanceToBoundary))
                    {
                        return distanceToBoundary;
                    }
                }
            }

            // if not hitting the inclusion, call the base (layer) method
            return base.GetDistanceToBoundary(photon);
        }
        /// <summary>
        /// method that provides reflected direction when phton reflects off boundary
        /// </summary>
        /// <param name="currentPosition">Position</param>
        /// <param name="currentDirection">Direction</param>
        /// <returns>new Direction</returns>
        public Direction GetReflectedDirection(
            Position currentPosition,
            Direction currentDirection)
        {
            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(currentPosition))
            {
                return base.GetReflectedDirection(currentPosition, currentDirection);
            }
            else
            {
                return currentDirection;
            }
            //throw new NotImplementedException(); // hopefully, this won't happen when the tissue inclusion is index-matched
        }
        /// <summary>
        /// method that provides refracted direction when phton refracts off boundary
        /// </summary>
        /// <param name="currentPosition">Position</param>
        /// <param name="currentDirection">Direction</param>
        /// <returns>new Direction</returns>
        public Direction GetRefractedDirection(
            Position currentPosition,
            Direction currentDirection,
            double nCurrent,
            double nNext,
            double cosThetaSnell)
        {
            // needs to call MultiLayerTissue when crossing top and bottom layer
            if (base.OnDomainBoundary(currentPosition))
            {
                return base.GetRefractedDirection(currentPosition, currentDirection, nCurrent, nNext, cosThetaSnell);
            }
            else
            {
                return currentDirection;
            }
            //throw new NotImplementedException(); // hopefully, this won't happen when the tissue inclusion is index-matched
        }
    }
}
