namespace Vts
{
    /// <summary>
    /// solver type. Added to determine which panel is in context LMM 
    /// </summary>
    public enum SolverType
    {
        /// <summary>
        /// forward solver
        /// </summary>
        Forward,
        /// <summary>
        /// fluence solver
        /// </summary>
        Fluence,
        /// <summary>
        /// inverse solver
        /// </summary>
        Inverse,
    }
}