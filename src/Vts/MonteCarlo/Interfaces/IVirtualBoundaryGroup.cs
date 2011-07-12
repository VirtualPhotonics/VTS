using System.Collections.Generic;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo
{
    public interface IVirtualBoundaryGroup
    {
        IList<IDetectorInput> DetectorInputs { get; }
        bool WriteToDatabase { get; }
        VirtualBoundaryType VirtualBoundaryType { get; }
        string Name { get; }
    }
}