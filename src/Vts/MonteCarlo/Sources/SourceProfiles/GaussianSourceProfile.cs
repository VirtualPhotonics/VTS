using System.Runtime.Serialization;
using Vts.MonteCarlo.Interfaces;
namespace Vts.MonteCarlo.Sources.SourceProfiles
{
    /// <summary>
    /// Defines Gaussian Source Profile
    /// </summary>
    public class GaussianSourceProfile : ISourceProfile
    {
        [IgnoreDataMember]
        public SourceProfileType ProfileType { get { return SourceProfileType.Gaussian; } }
        
        public double BeamDiaFWHM { get; set; }

        /// <summary>
        /// Initializes a new instance of the GaussianSourceProfile class
        /// </summary>
        /// <param name="beamDiaFWHM">Full width half maximum beam diameter</param>
        public GaussianSourceProfile(double beamDiaFWHM)
        {
            BeamDiaFWHM = beamDiaFWHM;
        }

        /// <summary>
        /// Initializes a new instance of the GaussianSourceProfile class
        /// </summary>
        public GaussianSourceProfile()
            : this(0.0)
        {
        }
    }
}
