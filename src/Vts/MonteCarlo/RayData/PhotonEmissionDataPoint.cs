
namespace Vts.MonteCarlo.RayData
{
    /// <summary>
    /// Captures data describing current state of outgoing photons to become rays.
    /// Inherits base class RayDataPoint because same except adds TotalTime.
    /// </summary>
    public class PhotonEmissionDataPoint: RayDataPoint
    {
        /// <summary>
        /// Ray information for outgoing photons to become rays
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="positionZ"></param>
        /// <param name="directionUx"></param>
        /// <param name="directionUy"></param>
        /// <param name="directionUz"></param>
        /// <param name="weight"></param>
        /// <param name="totalTime"></param>
        public PhotonEmissionDataPoint(
            double positionX,
            double positionY,
            double positionZ,
            double directionUx,
            double directionUy,
            double directionUz,
            double weight,
            double totalTime) 
            : base(
                positionX, 
                positionY, 
                positionZ, 
                directionUx, 
                directionUy, 
                directionUz, 
                weight)
        {
            TotalTime = totalTime;
        }

        /// <summary>
        /// total time of ray in tissue
        /// </summary>
        public new double TotalTime { get; set; }

    }
}
