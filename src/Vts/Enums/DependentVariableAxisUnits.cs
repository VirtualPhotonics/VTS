namespace Vts
{
    /// <summary>
    /// dependent variable axis unit types
    /// </summary>
    public enum DependentVariableAxisUnits
    {
        /// <summary>
        /// per millimeter squared [1/(mm * mm)], inverse area
        /// </summary>
        PerMMSquared,
        /// <summary>
        /// per millimeter squared per nanosecond [1/(mm * mm * ns)]
        /// </summary>
        PerMMSquaredPerNS,
        /// <summary>
        /// per millimeter squared per giga-Hertz [1/(mm * mm * GHz)]
        /// </summary>
        PerMMSquaredPerGHz,
        /// <summary>
        /// unitless dependent variance axis units
        /// </summary>
        Unitless,
        /// <summary>
        /// per nanosecond [1/ns]
        /// </summary>
        PerNS,
        /// <summary>
        /// per giga-Hertz [1/GHz]
        /// </summary>
        PerGHz,
        /// <summary>
        /// per millimeter cubed [1/(mm * mm * mm)]
        /// </summary>
        PerMMCubed,
        /// <summary>
        /// per millimeter cubed per nanosecond [1/(mm * mm * mm * ns)]
        /// </summary>
        PerMMCubedPerNS,
        /// <summary>
        /// per millimeter cubed per giga-Hertz [1/(mm * mm * mm * GHz)]
        /// </summary>
        PerMMCubedPerGHz,
        /// <summary>
        /// per millimeter [1/mm]
        /// </summary>
        PerMM,
        /// <summary>
        /// per millimeter per nanosecond [1/(mm * ns)]
        /// </summary>
        PerMMPerNS,
        /// <summary>
        /// per millimeter per giga-Hertz [1/(mm * GHz)]
        /// </summary>
        PerMMPerGHz
    }
}