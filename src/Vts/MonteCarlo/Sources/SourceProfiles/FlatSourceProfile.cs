using System.Runtime.Serialization;
using Vts.MonteCarlo.Interfaces;
namespace Vts.MonteCarlo.Sources.SourceProfiles
{
    /// <summary>
    /// Defines Flat Source Profile
    /// </summary>
    public class FlatSourceProfile : ISourceProfile
    {
        [IgnoreDataMember]
        public SourceProfileType ProfileType { get { return SourceProfileType.Flat; } }
    }
}
