using System.Runtime.CompilerServices;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Sources.SourceProfiles
{
    /// <summary>
    /// The <see cref="SourceProfiles"/> namespace contains the Monte Carlo input source profiles
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Defines Arbitrary Source Profile
    /// </summary>
    public class ArbitrarySourceProfile : ISourceProfile
    {
        /// <summary>
        /// Returns Arbitrary profile type
        /// </summary>
        //[IgnoreDataMember]
        public SourceProfileType SourceProfileType => SourceProfileType.Arbitrary;

    }

}
