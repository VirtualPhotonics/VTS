namespace Vts
{
    /// <summary>
    /// Monte Carlo command line parameter sweep types
    /// </summary>
    public enum ParameterSweepType
    {
        /// <summary>
        /// input of type start,stop,count
        /// </summary>
        Count,
        /// <summary>
        /// input of type start,stop,delta
        /// </summary>
        Delta,
        /// <summary>
        /// input of type number,value1,value2,...
        /// </summary>
        List
    }
}