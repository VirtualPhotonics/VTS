using System;
using System.Text.RegularExpressions;

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
        /// <param name="wavelengthValue">The wavelength value to convert</param>
        /// <param name="unit">An enum representing the units of the value to convert</param>
        /// <returns>The converted value as a double</returns>
        public static double ConvertWavelength(this double wavelengthValue, WavelengthUnit unit)
        {
            switch (unit)
            {
                case WavelengthUnit.Nanometers:
                default:
                    return wavelengthValue;
                case WavelengthUnit.Micrometers:
                    return wavelengthValue * 1000; //10^3
                case WavelengthUnit.Meters:
                    return wavelengthValue * 1000000000; //10^9
                case WavelengthUnit.InverseMeters:
                    return 1000000000 / wavelengthValue; //10^9
                case WavelengthUnit.InverseCentimeters:
                    return 10000000 / wavelengthValue; //10^7
            }
        }

        /// <summary>
        /// Method to convert coefficient values
        /// </summary>
        /// <param name="coefficientValue">The coefficient value to convert</param>
        /// <param name="unit">An enum representing the units of the value to convert</param>
        /// <param name="molarUnit">An enum to represent a molar based coefficient</param>
        /// <returns>The converted value as a double</returns>
        public static double ConvertCoefficient(this double coefficientValue, AbsorptionCoefficientUnit unit, MolarUnit molarUnit)
        {
            double coeff;
            switch (unit)
            {
                case AbsorptionCoefficientUnit.InverseMillimeters:
                default:
                    coeff = coefficientValue;
                    break;
                case AbsorptionCoefficientUnit.InverseMeters:
                    coeff = coefficientValue / 1000; //10^3
                    break;
                case AbsorptionCoefficientUnit.InverseCentimeters:
                    coeff = coefficientValue / 10;
                    break;
                case AbsorptionCoefficientUnit.InverseMicrometers:
                    coeff = coefficientValue * 1000; //10^3
                    break;
            }

            //molar coefficient applied
            switch (molarUnit)
            {
                case MolarUnit.MicroMolar:
                default:
                    return coeff;
                case MolarUnit.MilliMolar:
                    return coeff / 1000; //10^3
                case MolarUnit.Molar:
                    return coeff / 1000000; //10^6
                case MolarUnit.NanoMolar:
                    return coeff * 1000;
            }
        }

        /// <summary>
        /// Method to convert coefficient values with a default MolarUnit of MicroMolar
        /// </summary>
        /// <param name="coefficientValue">The coefficient value to convert</param>
        /// <param name="unit">An enum representing the units of the value to convert</param>
        /// <returns>The converted value as a double</returns>
        public static double ConvertCoefficient(this double coefficientValue, AbsorptionCoefficientUnit unit)
        {
            return ConvertCoefficient(coefficientValue, unit, MolarUnit.MicroMolar);
        }

        /// <summary>
        /// Passes a text string of wavelength unit and returns the relevant enum
        /// </summary>
        /// <param name="wavelengthUnit">string representing wavelength unit</param>
        /// <returns>enum of type WavelengthUnit</returns>
        public static WavelengthUnit getWavelengthUnit(string wavelengthUnit)
        {
            //pull out the unit values
            var match = Regex.Match(wavelengthUnit, @"([a-zA-Z/1]+)", RegexOptions.None, TimeSpan.FromSeconds(2));
            if (match.Success)
            {
                switch (match.Groups[1].Value.ToLower())
                {
                    case "nm":
                        return WavelengthUnit.Nanometers;
                    case "um":
                        return WavelengthUnit.Micrometers;
                    case "m":
                        return WavelengthUnit.Meters;
                    case "1/m":
                        return WavelengthUnit.InverseMeters;
                    case "1/cm":
                        return WavelengthUnit.InverseCentimeters;
                    default:
                        throw new ArgumentException("Not a valid wavelength unit");
                }
            }
            throw new ArgumentException("Not a valid wavelength unit");
        }

        /// <summary>
        /// passes an enum of WavelengthUnit and returns a string of wavelength unit
        /// </summary>
        /// <param name="wavelengthUnit">enum of type WavelengthUnit</param>
        /// <returns>string representing wavelength unit</returns>
        public static string getWavelengthUnit(WavelengthUnit wavelengthUnit)
        {
            switch (wavelengthUnit)
            {
                case WavelengthUnit.InverseCentimeters:
                    return "1/cm";
                case WavelengthUnit.Nanometers:
                    return "nm";
                case WavelengthUnit.Micrometers:
                    return "um";
                case WavelengthUnit.Meters:
                    return "m";
                case WavelengthUnit.InverseMeters:
                    return "1/m";
                default:
                    throw new ArgumentException("Unknown wavelength unit");
            }
        }

        /// <summary>
        /// Passes a text string of absorption coefficient unit and returns the relevant enum
        /// </summary>
        /// <param name="absorptionCoefficientUnit">string representing absorption coefficient unit</param>
        /// <returns>enum of type AbsorptionCoefficientUnit</returns>
        public static AbsorptionCoefficientUnit getAbsorptionCoefficientUnit(string absorptionCoefficientUnit)
        {
            //pull out the unit values
            Match match = Regex.Match(absorptionCoefficientUnit, @"1/\(?([a-zA-Z\*]+)\)?", RegexOptions.None, TimeSpan.FromSeconds(2));
            if (match.Success)
            {
                //get the value(s) in parentheses
                var units = match.Groups[1].Value;
                var unit = units.Split('*');
                switch (unit[0].ToLower())
                {
                    case "mm":
                        return AbsorptionCoefficientUnit.InverseMillimeters;
                    case "cm":
                        return AbsorptionCoefficientUnit.InverseCentimeters;
                    case "m":
                        return AbsorptionCoefficientUnit.InverseMeters;
                    case "um":
                        return AbsorptionCoefficientUnit.InverseMicrometers;
                    default:
                        throw new ArgumentException("Not a valid absorption coefficient unit");
                }
            }
            throw new ArgumentException("Not a valid absorption coefficient unit");
        }

        /// <summary>
        /// Passes a text string of molar unit and returns the relevant enum
        /// </summary>
        /// <param name="molarUnit">string representing molar unit</param>
        /// <returns>enum of type MolarUnit</returns>
        public static MolarUnit getMolarUnit(string molarUnit)
        {
            //pull out the unit values
            var match = Regex.Match(molarUnit, @"1/\(*([a-zA-Z\*]+)\)*", RegexOptions.None, TimeSpan.FromSeconds(2));
            if (!match.Success) throw new ArgumentException("Not a valid molar unit");
            //get the value(s) in parentheses
            var units = match.Groups[1].Value;
            if (!units.Contains("*")) return MolarUnit.None;
            var unit = units.Split('*');
            switch (unit[1])
            {
                default:
                    return MolarUnit.None;
                case "M":
                    return MolarUnit.Molar;
                case "mM":
                    return MolarUnit.MilliMolar;
                case "uM":
                    return MolarUnit.MicroMolar;
                case "nM":
                    return MolarUnit.NanoMolar;
            }
        }

        /// <summary>
        /// Passes the molar unit enum and the absorption coefficient unit enum and returns it as a string
        /// </summary>
        /// <param name="molarUnit">The molar unit enum</param>
        /// <param name="absorptionCoefficientUnit">an absorption coefficient enum</param>
        /// <returns>A string representing the molar units</returns>
        public static string getSpectralUnit(MolarUnit molarUnit, AbsorptionCoefficientUnit absorptionCoefficientUnit)
        {
            string mU;
            string aCu;
            string sU;

            switch (molarUnit)
            {
                case MolarUnit.None:
                    mU = "";
                    break;
                case MolarUnit.Molar:
                    mU = "M";
                    break;
                case MolarUnit.MilliMolar:
                    mU = "mM";
                    break;
                case MolarUnit.MicroMolar:
                    mU = "uM";
                    break;
                case MolarUnit.NanoMolar:
                    mU = "nM";
                    break;
                default:
                    throw new ArgumentException("Unknown molar unit");
            }

            switch (absorptionCoefficientUnit)
            {
                case AbsorptionCoefficientUnit.InverseCentimeters:
                    aCu = "cm";
                    break;
                case AbsorptionCoefficientUnit.InverseMeters:
                    aCu = "m";
                    break;
                case AbsorptionCoefficientUnit.InverseMicrometers:
                    aCu = "um";
                    break;
                case AbsorptionCoefficientUnit.InverseMillimeters:
                    aCu = "mm";
                    break;
                default:
                    throw new ArgumentException("Unknown absorption coefficient unit");
            }
            if (molarUnit == MolarUnit.None)
            {
                sU = "1/" + aCu;
            }
            else
            {
                sU = "1/(" + aCu + "*" + mU + ")";
            }
            return sU;
        }
    }
}
