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
        /// Initializes the default constructor of FlatSourceProfile class
        /// for serialization purposes
        /// </summary>
        public FlatSourceProfile()            
            { }
		
        /// <summary>
        /// Returns flat profile type
        /// </summary>
        //[IgnoreDataMember]
        public SourceProfileType SourceProfileType { get { return SourceProfileType.Flat; } }
    }
}
