namespace Vts
{
    /// <summary>
    /// inverse solution parameter types
    /// </summary>
    public enum InverseFitType
    {
        /// <summary>
        /// fit inverse solution using two parameters: mua and musp (mus')
        /// </summary>
        MuaMusp,
        /// <summary>
        /// fit inverse solution using one parameter: mua
        /// </summary>
        Mua,
        /// <summary>
        /// fit inverse solution using one parameter: musp (mus')
        /// </summary>
        Musp,
        /// <summary>
        /// fit inverse solution using three parameters: mua, musp (mus') and g
        /// </summary>
        MuaMuspG,
    }
}