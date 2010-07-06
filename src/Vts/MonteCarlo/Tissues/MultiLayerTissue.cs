using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Implements ITissue.  Defines tissue geometries comprised of layers
    /// (including homogenous with air layers above and below).
    /// </summary>
    public class MultiLayerTissue : ITissue
    {
        private IList<ITissueRegion> _regions;

        /// <summary>
        /// LayeredTissue constructor 
        /// </summary>
        /// <remarks>air above and below tissue needs to be specified;</remarks>

        //public MultiLayerTissue(IList<LayerRegion> regions)
        //{
        //    _regions = regions.Select(region => (ITissueRegion)region).ToList();
        //}

        /// <summary>
        /// MultiLayerTissue default constructor provides homogeneous tissue
        /// </summary>
        public MultiLayerTissue()
            : this( new MultiLayerTissueInput(
                    new List<LayerRegion>
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(0.0, 10.0, 2),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(10.0, double.PositiveInfinity, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete)
                    })
                ) { }

        public MultiLayerTissue(MultiLayerTissueInput mlti)
        {
            this._regions = mlti.Regions;
        }

        public IList<ITissueRegion> Regions
        {
            get { return _regions; }
            private set { _regions = value ; }
        }

        public int GetRegionIndex(Position position)
        {
            // find the first layer satisfying 
            // this gets the sublist and then the index of the sublist -> not what I want
            //return _regions
               //.Where(region => region.ContainsPosition(position))
               //.Select((r, i) => i)
               //.First();
            // this is the long method but it works
            int index = -99;
            for (int i = 0; i < _regions.Count(); i++)
			{
			    if (_regions[i].ContainsPosition(position))
                    index = i;
			}
            return index;
        }

        /// <summary>
        /// Finds the distance to the next boundary and subsequently 
        /// calls photon.HitBoundaryAndAdjustTrackLength()
        /// </summary>
        /// <param name="photon"></param>
        public double GetDistanceToBoundary(Photon photon)
        {
            if (photon.DP.Direction.Uz == 0.0) return double.PositiveInfinity;

            // going "up" in negative z-direction?
            bool goingUp = photon.DP.Direction.Uz < 0.0;

            // get current and adjacent regions
            int currentRegionIndex = photon.CurrentRegionIndex;
            LayerRegion currentRegion = (LayerRegion)_regions[currentRegionIndex];

            // calculate distance to boundary based on z-projection of photon trajectory
            double distanceToBoundary =
                goingUp
                    ?
                        (currentRegion.ZRange.Start - photon.DP.Position.Z) / photon.DP.Direction.Uz
                    :
                        (currentRegion.ZRange.Stop - photon.DP.Position.Z) / photon.DP.Direction.Uz;


            return distanceToBoundary;
        }
        public bool OnDomainBoundary(Photon photon)
        {
            // this code assumes that the first and last layer is air
            return ((photon.DP.Position.Z < 1e-10) || 
                (Math.Abs(photon.DP.Position.Z - ((LayerRegion)_regions[_regions.Count() - 2]).ZRange.Stop) < 1e-10));
        }
        public int GetNeighborRegionIndex(Photon photon)
        {
            if (photon.DP.Direction.Uz == 0.0)
                throw new Exception("GetNeighborRegionIndex called and Photon not on boundary");
            else
            {
                if (photon.DP.Direction.Uz > 0.0)
                    return Math.Min(photon.CurrentRegionIndex + 1, _regions.Count - 1);
                else
                    return Math.Min(photon.CurrentRegionIndex - 1, 0);
            }
        }
        public double GetAngleRelativeToBoundaryNormal(Photon photon)
        {
            return Math.Abs(photon.DP.Direction.Uz); // abs will work for upward normal and downward normal
        }
    }
}
