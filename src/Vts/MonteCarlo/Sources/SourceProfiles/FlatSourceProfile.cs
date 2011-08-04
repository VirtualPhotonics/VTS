using System.Runtime.Serialization;
using Vts.MonteCarlo.Interfaces;
namespace Vts.MonteCarlo.Sources.SourceProfiles
{
    /// <summary>
    /// Defines Flat Source Profile
    /// </summary>
    public class FlatSourceProfile : ISourceProfile
    {
        /// <summary>
        /// Returns flat profile type
        /// </summary>
        [IgnoreDataMember]
        public SourceProfileType ProfileType { get { return SourceProfileType.Flat; } }
    }
}
