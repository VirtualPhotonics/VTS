using System;
using Vts.Common;


namespace Vts.FemModeling.MGRTE._2D.DataStructures
{
    public abstract class ExtSourceBase : IExtSource
    {
        /// <summary>
        /// Starting cooordinates (x,z)  
        /// </summary>
        protected DoubleRange _start;        
        
        /// <summary>
        /// Ending cooordinates (x,z) 
        /// </summary>
        protected DoubleRange _end;

        /// <summary>
        /// Theta angle range
        /// </summary>
        protected DoubleRange _thetaRange;


        protected ExtSourceBase(
            DoubleRange start,
            DoubleRange end, 
            DoubleRange thetaRange)
        {
            _start = start;
            _end = end;
            _thetaRange = thetaRange;
        }

        public void AssignExtSource()
        {
            int test;
        }

    }
    
}
