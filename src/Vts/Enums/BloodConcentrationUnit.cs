namespace Vts
{
    /// <summary>
    /// units allowed for blood concentration
    /// </summary>
    public enum BloodConcentrationUnit
    {
        /// <summary>
        /// oxy-hemoglobin + deoxy-hemoglobin
        /// </summary>
        OxyPlusDeoxy,
        /// <summary>
        /// total hemoglobin + oxygen saturation
        /// </summary>
        HbTPlusStO2,
        /// <summary>
        /// blood volume + oxygenation
        /// </summary>
        VbPlusOxygenation,
    }
}