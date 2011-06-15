﻿
namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for SourceInput classes.
    /// </summary>
    public interface ISourceInput
    {
        SourceType SourceType { get; set; }
        int InitialTissueRegionIndex { get; set; }
    }
}
