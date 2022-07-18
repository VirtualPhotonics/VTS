namespace Vts
{
    /// <summary>
    /// input parameters types used in the Monte Carlo CommandLine application for parameter sweeps
    /// </summary>
    public enum InputParameterType
    {
        /// <summary>
        /// x position of the source definition
        /// </summary>
        XSourcePosition,
        /// <summary>
        /// y position of the source definition
        /// </summary>
        YSourcePosition,
        /// <summary>
        /// x center position of the embedded ellipse
        /// </summary>
        XInclusionPosition,
        /// <summary>
        /// y center position of the embedded ellipse
        /// </summary>
        YInclusionPosition,
        /// <summary>
        /// z center position of the embedded ellipse
        /// </summary>
        ZInclusionPosition,
        /// <summary>
        /// x-axis radius of embedded ellipse
        /// </summary>
        XInclusionRadius,
        /// <summary>
        /// y-axis radius of embedded ellipse
        /// </summary>
        YInclusionRadius,
        /// <summary>
        /// z-axis radius of embedded ellipse
        /// </summary>
        ZInclusionRadius,
        /// <summary>
        /// absorption coefficient of top layer of tissue
        /// </summary>
        Mua1,
        /// <summary>
        /// absorption coefficient of second layer of tissue
        /// </summary>
        Mua2,
        /// <summary>
        /// scattering coefficient of top layer of tissue
        /// </summary>
        Mus1,
        /// <summary>
        /// scattering coefficient of second layer of tissue
        /// </summary>
        Mus2,
        /// <summary>
        /// anisotropy coefficient of top layer of tissue
        /// </summary>
        G1,
        /// <summary>
        /// anisotropy coefficient of second layer of tissue
        /// </summary>
        G2,
        /// <summary>
        /// refractive index of top layer of tissue
        /// </summary>
        N1,
        /// <summary>
        /// refractive index of second layer of tissue
        /// </summary>
        N2,
        /// <summary>
        /// thickness of top layer of tissue
        /// </summary>
        D1,
        /// <summary>
        /// thickness of second layer of tissue
        /// </summary>
        D2,
    }
}