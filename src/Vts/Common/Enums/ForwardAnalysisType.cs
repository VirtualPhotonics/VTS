namespace Vts
{
    /// <summary>
    /// forward analysis types
    /// </summary>
    public enum ForwardAnalysisType
    {
        /// <summary>
        /// reflectance forward analysis type
        /// </summary>
        R,
        /// <summary>
        /// the derivative of reflectance (R) with respect to absorption coefficient (mua)
        /// </summary>
        dRdMua,
        /// <summary>
        /// the derivative of reflectance (R) with respect to reduced scattering coefficient (musp)
        /// </summary>
        dRdMusp,
        /// <summary>
        /// the derivative of reflectance (R) with respect to anisotropy coefficient (g)
        /// </summary>
        dRdG,
        /// <summary>
        /// the derivative of reflectance (R) with respect to refractive index (n)
        /// </summary>
        dRdN,
        //dRdIV,
    }
}