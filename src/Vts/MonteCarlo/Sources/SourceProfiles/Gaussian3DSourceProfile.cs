using System.Runtime.Serialization;
using Vts.MonteCarlo.Interfaces;
namespace Vts.MonteCarlo.Sources.SourceProfiles
{
    public class Gaussian3DSourceProfile : ISourceProfile
    {
        [IgnoreDataMember]
        public SourceProfileType ProfileType { get { return SourceProfileType.Gaussian3D; } }

        public double StdDevX { get; set; }
        public double StdDevY { get; set; }
        public double StdDevZ { get; set; }
    }
}
