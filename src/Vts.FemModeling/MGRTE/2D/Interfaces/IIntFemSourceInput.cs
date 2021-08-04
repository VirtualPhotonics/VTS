
namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines a contract for SourceInput classes.
    /// </summary>
    public interface IIntFemSourceInput
    {
        /// <summary>
        /// Type of source
        /// </summary>
        FemSourceType SourceType { get; set; }      
    }
}
