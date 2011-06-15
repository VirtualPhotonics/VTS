using System.Runtime.Serialization;
using Vts.MonteCarlo.Interfaces;
namespace Vts.MonteCarlo.Sources.SourceProfiles
{
    public class GaussianSourceProfile : ISourceProfile
    {
        [IgnoreDataMember]
        public SourceProfileType ProfileType { get { return SourceProfileType.Gaussian; } }

        public double BeamDiaFWHM { get; set; }

        public GaussianSourceProfile(double beamDiaFWHM)
        {
            BeamDiaFWHM = beamDiaFWHM;
        }

        public GaussianSourceProfile()
            : this(0.0)
        {
        }
    }
}
