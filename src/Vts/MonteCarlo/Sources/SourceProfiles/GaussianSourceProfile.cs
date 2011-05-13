using System.Runtime.Serialization;
using Vts.MonteCarlo.Interfaces;
namespace Vts.MonteCarlo.Sources.SourceProfiles
{
    public class GaussianSourceProfile : ISourceProfile
    {
        [IgnoreDataMember]
        public SourceProfileType ProfileType { get { return SourceProfileType.Gaussian; } }

        public double BeamWaist { get; set; }
    }
}
