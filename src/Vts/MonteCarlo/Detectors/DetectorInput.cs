using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.IO;

namespace Vts.MonteCarlo.Detectors
{
    [KnownType(typeof(DetectorInput))]
    public class DetectorInput : IDetectorInput
    {
        public DetectorInput(List<TallyType> tallyTypeList, 
            DoubleRange rho,
            DoubleRange z,
            DoubleRange angle,
            DoubleRange time,
            DoubleRange omega,
            DoubleRange x,
            DoubleRange y)
        {
            TallyTypeList = tallyTypeList;
            Rho = rho;
            Z = z;
            Angle = angle;
            Time = time;
            Omega = omega;
            X = x;
            Y = y;
        }
        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public DetectorInput() : this(
            new List<TallyType>()
                {
                    TallyType.RDiffuse,
                    TallyType.ROfAngle,
                    TallyType.ROfRho,
                    TallyType.ROfRhoAndAngle,
                    TallyType.ROfRhoAndTime,
                    TallyType.ROfXAndY,
                    TallyType.ROfRhoAndOmega,
                    TallyType.TDiffuse,
                    TallyType.TOfAngle,
                    TallyType.TOfRho,
                    TallyType.TOfRhoAndAngle,
                },
            new DoubleRange(0.0, 10, 101), // rho
            new DoubleRange(0.0, 10, 101),  // z
            new DoubleRange(0.0, Math.PI / 2, 2), // angle
            new DoubleRange(0.0, 10000, 101), // time
            new DoubleRange(0.0, 1000, 21), // omega
            new DoubleRange(-10.0, 10.0, 201), // x
            new DoubleRange(-10.0, 10.0, 201) // y
        ) {}
        public List<TallyType> TallyTypeList { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Angle { get; set; }
        public DoubleRange Time { get; set; }
        public DoubleRange Omega { get; set; }
        public DoubleRange X { get; set; }
        public DoubleRange Y { get; set; }
        public DoubleRange Z { get; set; }

        public static DetectorInput FromFile(string filename)
        {
            return FileIO.ReadFromXML<DetectorInput>(filename);
        }

        public void ToFile(string filename)
        {
            FileIO.WriteToXML(this, filename);
        }

        public static DetectorInput FromFileInResources(string filename, string project)
        {
            return FileIO.ReadFromXMLInResources<DetectorInput>(filename, project);
        }
    }
}
