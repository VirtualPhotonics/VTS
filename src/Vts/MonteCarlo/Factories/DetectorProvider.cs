using System;
using System.IO;
using System.Runtime.Serialization;
using Vts.Common;
using AutoMapper;

namespace Vts.MonteCarlo
{
    public class TallyDetails
    {
        public bool IsReflectanceTally { get; set; }
        public bool IsTransmittanceTally { get; set; }
        public bool IsSpecularReflectanceTally { get; set; }
        public bool IsInternalSurfaceTally { get; set; }
        public bool IspMCReflectanceTally { get; set; }
        public bool IsDosimetryTally { get; set; }
        public bool IsVolumeTally { get; set; }
        public bool IsCylindricalTally { get; set; }
        public bool IsNotImplementedForDAW { get; set; }
        public bool IsNotImplementedForCAW { get; set; }
        public bool IsNotImplementedYet { get; set; }
    }

    public class BinaryArrayInfo
    {
        public Array DataArray { get; set; }
        public string Name { get; set; }
        public string FileTag { get; set; }
        public int[] Dimensions { get; set; }

        public Action<BinaryWriter> WriteData { get; set; }
        public Action<BinaryReader> ReadData { get; set; }
    }

    /// <summary>
    /// Properties and methods that all IDetectors must implement
    /// </summary>
    public interface IDetector
    {
        /// <summary>
        /// TallyType enum specification
        /// </summary>
        string TallyType { get; }

        /// <summary>
        /// Name string of IDetector.  Default = TallyType.ToString().
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicates if 2nd moment is tallied
        /// </summary>
        bool TallySecondMoment { get; }

        /// <summary>
        /// Details of the tally - booleans that specify when they should be tallied
        /// </summary>
        TallyDetails TallyDetails { get; set; }

        /// <summary>
        /// Initialize the detector, using tissue information if necessary
        /// </summary>
        /// <param name="tissue"></param>
        void Initialize(ITissue tissue = null);

        /// <summary>
        /// Method to tally to detector using information in Photon
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        void Tally(Photon photon);

        /// <summary>
        /// Method to normalize the tally to get Mean and Second Moment estimates
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        void Normalize(long numPhotons);

        /// <summary>
        /// Method that returns info for each large binary data array
        /// </summary>
        /// <returns></returns>
        BinaryArrayInfo[] GetBinaryArrays();

        /// <summary>
        /// Method that returns info for each large binary data array
        /// </summary>
        /// <returns></returns>
        void SetBinaryArrays(params BinaryArrayInfo[] arrays);
    }

    public abstract class DetectorInput
    {
        public DetectorInput()
        {
            TallyDetails = new TallyDetails();
        }
        // mandatory user inputs (required for IDetetorInput contract)
        public string TallyType { get; set; }
        public string Name { get; set; }
        public bool TallySecondMoment { get; set; }
        public TallyDetails TallyDetails { get; set; }
    }

    public abstract class Detector
    {
        public Detector()
        {
            TallyType = "";
            Name = "";
            TallySecondMoment = false;
            TallyDetails = new TallyDetails();
        }

        /* ==== These public properties are mandatory (required for the IDetector contract) ==== */
        public string TallyType { get; set; }
        public string Name { get; set; }
        public bool TallySecondMoment { get; set; }
        public TallyDetails TallyDetails { get; set; }
    }

    // user code
    public class FancyDetectorInput : DetectorInput, IDetectorInput
    {
        public FancyDetectorInput()
        {
            // assign defaults for mandatory values
            TallyType = "Fancy";
            Name = "test";
            TallySecondMoment = true;           

            // modfy base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
            TallyDetails.IsReflectanceTally = true; // (ours is a simple x-y reflectance tally)
            
            // assign defaults for optional values
            XRange = new DoubleRange(0, 1, 10);
            YRange = new DoubleRange(0, 1, 10);
        }

        // optional/custom detector-specific user inputs necessary to specify detector
        public DoubleRange XRange { get; set; }
        public DoubleRange YRange { get; set; }

        public IDetector CreateDetector()
        {
            // create a detector with all the mandatory and optional input properties above
            return new FancyDetector
            {
                // required properties (part of DetectorInput/Detector base classes)
                TallyType = this.TallyType,
                Name = this.Name,
                TallyDetails = this.TallyDetails,

                // optional/custom detector-specific properties
                XRange = this.XRange,
                YRange = this.YRange,
            };
        }
    }

    public class FancyDetector : Detector, IDetector
    {
        // Place any private class variables here, and initialize them in the "Initialize()" method
        private Random _rng;

        /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
        /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
        public DoubleRange XRange { get; set; }
        public DoubleRange YRange { get; set; }

        /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
        /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
        [IgnoreDataMember] public double[,] Mean { get; set; }
        [IgnoreDataMember] public double[,] SecondMoment { get; set; }

        /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
        public long TallyCount { get; set; }

        /* ==== Public methods Initialize/Tally/Normalize/GetBinaryArays/SetBinaryArrays are required (IDetector contract) ==== */
        public void Initialize(ITissue tissue)
        {
            // assign any user-defined outputs (except arrays...we'll make those on-demand)
            TallyCount = 0;

            // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
            Mean = Mean ?? new double[XRange.Count, YRange.Count];
            SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[XRange.Count, YRange.Count] : null);

            // intialize any other necessary variables here
            _rng = new Random();
        }

        public void Tally(Photon photon)
        {
            // for demonstration, we use a random number to figure out which index to bin the photon
            var xIndex = _rng.Next(XRange.Count);
            var yIndex = _rng.Next(YRange.Count);

            // tally the first moment (mean) of the objective function in the appropriate bin
            Mean[xIndex, yIndex] += photon.DP.Weight;

            // tallying second moment will help later in determining standard deviation
            if (TallySecondMoment) {
                SecondMoment[xIndex, yIndex] += photon.DP.Weight * photon.DP.Weight;
            }

            TallyCount++; // for fun, save a running tally of how many photons were detected by this detector (for reporting purposes)
        }

        public void Normalize(long numPhotons)
        {
            for (int i = 0; i < XRange.Count; i++){
                for (int j = 0; j < YRange.Count; j++){
                    Mean[i, j] /= numPhotons;
                }
            }

            if (TallySecondMoment){
                for (int i = 0; i < XRange.Count; i++){
                    for (int j = 0; j < YRange.Count; j++){
                        SecondMoment[i, j] /= numPhotons;
                    }
                }
            }
        }

        // this is to allow saving of large arrays separately as a binary file
        public BinaryArrayInfo[] GetBinaryArrays()
        {
            return new[] {
                new BinaryArrayInfo {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = (BinaryWriter bw) => {
                        for (int i = 0; i < XRange.Count; i++) {
                            for (int j = 0; j < YRange.Count; j++) {
                                bw.Write(Mean[i, j]);
			                }
                        }
                    },
                    ReadData = (BinaryReader br) => {
                        for (int i = 0; i < XRange.Count; i++) {
                            for (int j = 0; j < YRange.Count; j++) {
                                Mean[i, j] = br.ReadDouble();
			                }
                        }
                    }
                },
                new BinaryArrayInfo {
                    DataArray = SecondMoment,
                    Name = "SecondMoment",
                    FileTag = "_2",
                    ReadData = (BinaryReader br) => {
                        for (int i = 0; i < XRange.Count; i++) {
                            for (int j = 0; j < YRange.Count; j++) {
                                SecondMoment[i, j] = br.ReadDouble();
			                }
                        }
                    },
                    WriteData = (BinaryWriter bw) => {
                        for (int i = 0; i < XRange.Count; i++) {
                            for (int j = 0; j < YRange.Count; j++) {
                                bw.Write(SecondMoment[i, j]);
			                }
                        }
                    }
                },
            };
        }

        public void SetBinaryArrays(params BinaryArrayInfo[] arrays)
        {
            if (arrays == null) return;
            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i].Name == "Mean")
                    Mean = (double[,])arrays[i].DataArray;
                if (arrays[i].Name == "SecondMoment")
                    SecondMoment = (double[,])arrays[i].DataArray;
            }
        }
    }


    // base class implementations

    ///// <summary>
    ///// Base class for all detectors.
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public abstract class DetectorBase<T> : IDetector<T>
    //{
    //    private T _mean;
    //    private T _secondMoment;
    //    private int[] _dimensions;

    //    protected DetectorBase()
    //    {
    //        TallySecondMoment = false;
    //        Name = "";

    //        TallyDetails = new TallyDetails
    //        {
    //            IsReflectanceTally = false,
    //            IsTransmittanceTally = false,
    //            IsSpecularReflectanceTally = false,
    //            IsInternalSurfaceTally = false,
    //            IspMCReflectanceTally = false,
    //            IsDosimetryTally = false,
    //            IsVolumeTally = false,
    //            IsCylindricalTally = false,
    //            IsNotImplementedForCAW = false,
    //            IsNotImplementedYet = false
    //        };
    //    }

    //    public string Name { get; set; } // shouldn't have public set_Name (need constructor injection for AutoMapper)
    //    public string TallyType { get; set; } // shouldn't have public set_TallyType (need constructor injection for AutoMapper)

    //    public T Mean
    //    {
    //        get
    //        {
    //            if (_mean == null != null)
    //            {
    //                _dimensions = GetDimensions();
    //                _mean = (T)((dynamic)Array.CreateInstance(typeof(T).GetElementType(), _dimensions));
    //            }
    //            return _mean;
    //        }
    //    }

    //    public T SecondMoment
    //    {
    //        get
    //        {
    //            if (_secondMoment == null && TallySecondMoment)
    //            {
    //                _dimensions = GetDimensions();
    //                _secondMoment = (T)((dynamic)Array.CreateInstance(typeof(T).GetElementType(), _dimensions));
    //            }
    //            return _secondMoment;
    //        }
    //    }

    //    public int[] Dimensions
    //    {
    //        get { return _dimensions; }
    //    }

    //    public bool TallySecondMoment { get; set; }
    //    public long TallyCount { get; set; } // shouldn't have public set_TallyCount

    //    protected TallyDetails TallyDetails { get; set; }

    //    protected abstract int[] GetDimensions();
    //    public abstract void Tally(Photon photon);
    //    public abstract void Normalize(long numPhotons);

    //    public bool IsSurfaceTally()
    //    {
    //        return TallyDetails.IsTransmittanceTally || TallyDetails.IsReflectanceTally ||
    //               TallyDetails.IsSpecularReflectanceTally || TallyDetails.IsInternalSurfaceTally;
    //    }

    //    public bool IsReflectanceTally() { return TallyDetails.IsReflectanceTally; }
    //    public bool IsTransmittanceTally() { return TallyDetails.IsTransmittanceTally; }
    //    public bool IsSpecularReflectanceTally() { return TallyDetails.IsSpecularReflectanceTally; }
    //    public bool IsInternalSurfaceTally() { return TallyDetails.IsInternalSurfaceTally; }
    //    public bool IspMCReflectanceTally() { return TallyDetails.IspMCReflectanceTally; }
    //    public bool IsVolumeTally() { return TallyDetails.IsVolumeTally; }
    //    public bool IsCylindricalTally() { return TallyDetails.IsCylindricalTally; }
    //    public bool IsNotImplementedForCAW() { return TallyDetails.IsNotImplementedForCAW; }
    //    public bool IsNotImplementedYet() { return TallyDetails.IsNotImplementedYet; }

    //}

    //public class DetectorProvider<TDetectorInput, TDetector, TDetectorOutput>
    //    where TDetector : IDetector
    //    where TDetectorOutput : IDetectorOutput
    //{
    //    static DetectorProvider()
    //    {
    //        Mapper.CreateMap<TDetectorInput, TDetector>();
    //        Mapper.CreateMap<TDetector, TDetectorOutput>();

    //        KnownTypes.Add(typeof(TDetectorInput));
    //        KnownTypes.Add(typeof(TDetectorOutput));
    //    }

    //    //public static DetectorProvider<TIn, TDet, TOut> Create<TIn, TDet, TOut>(Type inputType, Type detectorType, Type outputType)
    //    //{
    //    //    MethodInfo genericMethod = typeof(DetectorProvider).GetMethod("ContainSameValues");
    //    //    return new DetectorProvider<TIn, TDet, TOut>();
    //    //}

    //    public DetectorProvider()
    //    {
    //        CreateDetector = input => Mapper.Map<TDetectorInput, TDetector>(input);
    //        CreateOutput = detector => Mapper.Map<TDetector, TDetectorOutput>(detector);

    //        ReadInputFromFile = filename => Vts.IO.FileIO.ReadFromXML<TDetectorInput>(filename);
    //        WriteInputToFile = (input, filename) => Vts.IO.FileIO.WriteToXML(input, filename);
    //        ReadInputFromResources = (filename, projectName) => Vts.IO.FileIO.ReadFromXMLInResources<TDetectorInput>(filename, projectName);

    //        WriteOutputToFile = (output, filename) => DetectorIO.WriteDetectorOutputToFile(output, filename);
    //        ReadOutputFromFile = (filename, folderPath) => DetectorIO.ReadDetectorOutputFromFile<TDetectorOutput>(filename, folderPath);

    //        TargetType = typeof(TDetector);
    //    }

    //    public Type TargetType { get; set; }
    //    public Func<TDetectorInput, TDetector> CreateDetector { get; set; }
    //    public Func<TDetector, TDetectorOutput> CreateOutput { get; set; }
    //    public Func<string, TDetectorInput> ReadInputFromFile { get; set; }
    //    public Func<string, string, TDetectorInput> ReadInputFromResources { get; set; }
    //    public Action<TDetectorInput, string> WriteInputToFile { get; set; }
    //    public Func<string, string, TDetectorOutput> ReadOutputFromFile { get; set; }
    //    public Action<TDetectorOutput, string> WriteOutputToFile { get; set; }

    //}


    //public class DetectorProvider2<TDetectorInput, TDetector, TDetectorOutput>
    //    where TDetector : IDetector2
    //    where TDetectorOutput : IDetectorOutput2
    //{
    //    static DetectorProvider2()
    //    {
    //        KnownTypes.Add(typeof(TDetectorInput));
    //        KnownTypes.Add(typeof(TDetectorOutput));
    //    }

    //    //public static DetectorProvider<TIn, TDet, TOut> Create<TIn, TDet, TOut>(Type inputType, Type detectorType, Type outputType)
    //    //{
    //    //    MethodInfo genericMethod = typeof(DetectorProvider).GetMethod("ContainSameValues");
    //    //    return new DetectorProvider<TIn, TDet, TOut>();
    //    //}

    //    public DetectorProvider2()
    //    {
    //        CreateDetector = input => Mapper.Map<TDetectorInput, TDetector>(input);
    //        CreateOutput = detector => Mapper.Map<TDetector, TDetectorOutput>(detector);

    //        ReadInputFromFile = filename => Vts.IO.FileIO.ReadFromXML<TDetectorInput>(filename);
    //        WriteInputToFile = (input, filename) => Vts.IO.FileIO.WriteToXML(input, filename);
    //        ReadInputFromResources = (filename, projectName) => Vts.IO.FileIO.ReadFromXMLInResources<TDetectorInput>(filename, projectName);

    //        WriteOutputToFile = (output, filename) => DetectorIO.WriteDetectorOutputToFile(output, filename);
    //        ReadOutputFromFile = (filename, folderPath) => DetectorIO.ReadDetectorOutputFromFile<TDetectorOutput>(filename, folderPath);

    //        TargetType = typeof(TDetector);
    //    }

    //    public Type TargetType { get; set; }
    //    public Func<TDetectorInput, TDetector> CreateDetector { get; set; }
    //    public Func<TDetector, TDetectorOutput> CreateOutput { get; set; }
    //    public Func<string, TDetectorInput> ReadInputFromFile { get; set; }
    //    public Func<string, string, TDetectorInput> ReadInputFromResources { get; set; }
    //    public Action<TDetectorInput, string> WriteInputToFile { get; set; }
    //    public Func<string, string, TDetectorOutput> ReadOutputFromFile { get; set; }
    //    public Action<TDetectorOutput, string> WriteOutputToFile { get; set; }

    //}

    //////////////////////////////////////////////////////////////////////////////////////////////////////////
}
