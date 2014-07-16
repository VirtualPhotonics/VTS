namespace Vts.MonteCarlo.Detectors
{
    public abstract class DetectorInput
    {
        public DetectorInput()
        {
            TallyType = "";
            Name = "";
            TallySecondMoment = false;
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
}
