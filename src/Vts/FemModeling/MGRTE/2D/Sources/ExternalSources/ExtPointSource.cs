using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class ExtPointSource : ExtSourceBase
    {
        public ExtPointSource(
            DoubleRange launchPoint,
            DoubleRange thetaRange)
            : base(
                launchPoint,
                launchPoint,
                thetaRange)
        { }        
    }
}
