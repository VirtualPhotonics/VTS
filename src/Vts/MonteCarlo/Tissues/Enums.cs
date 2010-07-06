using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vts.MonteCarlo.Tissues
{
    public enum TissueGeometryType
    {
        Layer,  // includes homogenous
        EmbeddedEllipsoid,
        Voxelized,
    }
    public enum TissueRegionDirectionType
    {
        PositiveUx,
        NegativeUx,
        PositiveUy, 
        NegativeUy,
        PositiveUz,
        NegativeUz,
    }
}
