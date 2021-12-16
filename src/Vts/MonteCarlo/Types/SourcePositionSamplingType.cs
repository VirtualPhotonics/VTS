namespace Vts.MonteCarlo
{
    /// <summary>
    /// fluorescent source sampling types: using a CDF generated from a PDF or Uniform
    /// </summary>
    public enum SourcePositionSamplingType
    {
        /// <summary>
        /// Sample location using PDF
        /// </summary>
        CDF,
        /// <summary>
        /// Sample location uniformly in space
        /// </summary>
        Uniform
    }
}