namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// detector input abstract class
    /// </summary>
    public abstract class DetectorInput
    {
        /// <summary>
        /// default constructor for detector input
        /// </summary>
        protected DetectorInput()
        {
            TallyType = "";
            Name = "";
            TallySecondMoment = false;
            TallyDetails = new TallyDetails();
        }
        // mandatory user inputs (required for IDetectorInput contract)
        /// <summary>
        /// tally type enum identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// tally name string
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Boolean indicating whether to tally 2nd moment
        /// </summary>
        public bool TallySecondMoment { get; set; }
        /// <summary>
        /// tally details describing aspects of detector, e.g. IsReflectanceTally
        /// </summary>
        public TallyDetails TallyDetails { get; set; }
    }

    /// <summary>
    /// detector abstract class
    /// </summary>
    public abstract class Detector
    {
        /// <summary>
        /// default constructor for detector
        /// </summary>
        protected Detector()
        {
            TallyType = "";
            Name = "";
            TallySecondMoment = false;
            TallyDetails = new TallyDetails();
        }

        /* ==== These public properties are mandatory (required for the IDetector contract) ==== */
        /// <summary>
        /// tally type enum
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// tally name string
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Boolean indicating whether to tally 2nd moment
        /// </summary>
        public bool TallySecondMoment { get; set; }
        /// <summary>
        /// tally details 
        /// </summary>
        public TallyDetails TallyDetails { get; set; }
    }
}
