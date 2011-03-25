using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(TDiffuseDetector))]
    /// <summary>
    /// Implements ITerminationTally<double>.  Tally for diffuse transmittance.
    /// This implemenation works for Analog, DAW and CAW processing.
    /// </summary>
    public class TDiffuseDetector : ITerminationDetector<double>
    {
        /// <summary>
        /// Returns an instance of TDiffuseDetector
        /// </summary>
        public TDiffuseDetector(String name)
        {
            Mean = 0.0;
            SecondMoment = 0.0;
            TallyType = TallyType.TDiffuse;
            Name = name;
        }
        /// <summary>
        /// Returns a default instaf TDiffuseDetector (for serialization purposes only)
        /// </summary>
        public TDiffuseDetector()
            : this(TallyType.TDiffuse.ToString())
        {
        }
        public double Mean { get; set; }

        public double SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public void Tally(PhotonDataPoint dp)
        {
            Mean += dp.Weight;
            SecondMoment += dp.Weight * dp.Weight;
            TallyCount++;
        }

        public void Normalize(long numPhotons)
        {
            Mean /= numPhotons;
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutBottom);
        }
    }
}
