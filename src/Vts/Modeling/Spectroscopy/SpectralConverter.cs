namespace Vts.SpectralMapping
{
    /// <summary>
    /// Enum to represent the possible wavelength and wavenumber inputs
    /// </summary>
    public enum Wavelength_Unit
    {
        Nanometers,
        Micrometers,
        Meters,
        InverseMeters,
        InverseCentimeters,
    }

    /// <summary>
    /// Enum to represent the possible coefficient inputs
    /// </summary>
    public enum Coefficient_Unit
    {
        InverseMillimeters,
        InverseMeters,
        InverseCentimeters,
        InverseMicrometers,
    }

    public enum Molar_Unit
    {
        Molar,
        MilliMolar,
        MicroMolar,
    }

    /// <summary>
    /// Class to convert imported spectral data to uniform values
    /// </summary>
    public static class SpectralConverter
    {
        /// <summary>
        /// Method to convert wavelength values
        /// </summary>
        /// <param name="WavelengthValue">The wavelength value to convert</param>
        /// <param name="Unit">An enum representing the units of the value to convert</param>
        /// <returns>The converted value as a double</returns>
        public static double ConvertWavelength(this double WavelengthValue, Wavelength_Unit Unit)
        {
            switch (Unit)
            {
                case Wavelength_Unit.Nanometers:
                default:
                    return WavelengthValue;
                case Wavelength_Unit.Micrometers:
                    return WavelengthValue * 1000; 
                case Wavelength_Unit.Meters:
                    return WavelengthValue * 1000000000;
                case Wavelength_Unit.InverseMeters:
                    return 1000000000 / WavelengthValue;
                case Wavelength_Unit.InverseCentimeters:
                    return 10000000 / WavelengthValue;
            }
        }

        /// <summary>
        /// Method to convert coefficient values
        /// </summary>
        /// <param name="CoefficientValue">The coefficient value to convert</param>
        /// <param name="Unit">An enum representing the units of the value to convert</param>
        /// <param name="MolarUnit">An enum to represent a molar based coefficient</param>
        /// <returns>The converted value as a double</returns>
        public static double ConvertCoefficient(this double CoefficientValue, Coefficient_Unit Unit, Molar_Unit MolarUnit)
        {
            double coeff;
            switch (Unit)
            {
                case Coefficient_Unit.InverseMillimeters:
                default:
                    coeff = CoefficientValue;
                    break;
                case Coefficient_Unit.InverseMeters:
                    coeff = CoefficientValue / 1000;
                    break;
                case Coefficient_Unit.InverseCentimeters:
                    coeff = CoefficientValue / 10;
                    break;
                case Coefficient_Unit.InverseMicrometers:
                    coeff = CoefficientValue * 1000;
                    break;
            }

            //molar coefficient applied
            switch (MolarUnit)
            {
                case Molar_Unit.MicroMolar:
                default:
                    return coeff;
                case Molar_Unit.MilliMolar:
                    return coeff / 1000;
                case Molar_Unit.Molar:
                    return coeff / 1000000;
            }
        }
    }
}
