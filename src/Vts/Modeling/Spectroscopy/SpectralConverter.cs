namespace Vts.SpectralMapping
{
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
        public static double ConvertWavelength(this double WavelengthValue, WavelengthUnit Unit)
        {
            switch (Unit)
            {
                case WavelengthUnit.Nanometers:
                default:
                    return WavelengthValue;
                case WavelengthUnit.Micrometers:
                    return WavelengthValue * 1000; //10^3
                case WavelengthUnit.Meters:
                    return WavelengthValue * 1000000000; //10^9
                case WavelengthUnit.InverseMeters:
                    return 1000000000 / WavelengthValue; //10^9
                case WavelengthUnit.InverseCentimeters:
                    return 10000000 / WavelengthValue; //10^7
            }
        }

        /// <summary>
        /// Method to convert coefficient values
        /// </summary>
        /// <param name="CoefficientValue">The coefficient value to convert</param>
        /// <param name="Unit">An enum representing the units of the value to convert</param>
        /// <param name="MolarUnit">An enum to represent a molar based coefficient</param>
        /// <returns>The converted value as a double</returns>
        public static double ConvertCoefficient(this double CoefficientValue, AbsorptionCoefficientUnit Unit, MolarUnit MolarUnit)
        {
            double coeff;
            switch (Unit)
            {
                case AbsorptionCoefficientUnit.InverseMillimeters:
                default:
                    coeff = CoefficientValue;
                    break;
                case AbsorptionCoefficientUnit.InverseMeters:
                    coeff = CoefficientValue / 1000; //10^3
                    break;
                case AbsorptionCoefficientUnit.InverseCentimeters:
                    coeff = CoefficientValue / 10;
                    break;
                case AbsorptionCoefficientUnit.InverseMicrometers:
                    coeff = CoefficientValue * 1000; //10^3
                    break;
            }

            //molar coefficient applied
            switch (MolarUnit)
            {
                case MolarUnit.MicroMolar:
                default:
                    return coeff;
                case MolarUnit.MilliMolar:
                    return coeff / 1000; //10^3
                case MolarUnit.Molar:
                    return coeff / 1000000; //10^6
            }
        }

        /// <summary>
        /// Method to convert coefficient values with a default MolarUnit of MicroMolar
        /// </summary>
        /// <param name="CoefficientValue">The coefficient value to convert</param>
        /// <param name="Unit">An enum representing the units of the value to convert</param>
        /// <returns>The converted value as a double</returns>
        public static double ConvertCoefficient(this double CoefficientValue, AbsorptionCoefficientUnit Unit)
        {
            return ConvertCoefficient(CoefficientValue, Unit, MolarUnit.MicroMolar);
        }

    }
}
