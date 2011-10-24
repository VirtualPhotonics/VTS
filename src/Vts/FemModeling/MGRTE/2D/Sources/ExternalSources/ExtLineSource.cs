using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public class ExtLineSource : ExtSourceBase
    {
        public ExtLineSource(
            DoubleRange startLine,
            DoubleRange endLine,
            DoubleRange thetaRange)
            : base(
                startLine,
                endLine,
                thetaRange)
        { }        
    }
}
