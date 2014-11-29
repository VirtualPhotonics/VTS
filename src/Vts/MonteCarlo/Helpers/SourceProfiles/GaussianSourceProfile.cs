using System.Runtime.Serialization;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources.SourceProfiles
{
    /// <summary>
    /// Defines Gaussian Source Profile
    /// </summary>
    public class GaussianSourceProfile : ISourceProfile
    {
        /// <summary>
        /// Returns Gaussian profile type
        /// </summary>
        //[IgnoreDataMember]
        public SourceProfileType SourceProfileType { get { return SourceProfileType.Gaussian; } }
        
        /// <summary>
        /// Full width half maximum beam diameter
        /// </summary>
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
        /// Initializes the default constructor of GaussianSourceProfile class (BeamDiaFWHM = 1.0)
        /// </summary>
        public GaussianSourceProfile()
            : this(1.0)
        {
        }
    }
}
