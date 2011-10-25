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
                case MolarUnit.NanoMolar:
                    return coeff * 1000;
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

        /// <summary>
        /// Passes a text string of wavelength unit and returns the relevant enum
        /// </summary>
        /// <param name="wavelengthUnit">string representing wavelength unit</param>
        /// <returns>enum of type WavelengthUnit</returns>
        public static WavelengthUnit getWavelengthUnit(string wavelengthUnit)
        {
            switch (wavelengthUnit.ToLower())
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
                    throw new Exception("Not a valid wavelength unit");
            }
        }

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
                    throw new Exception("Unknown wavelength unit");
            }
        }

        /// <summary>
        /// Passes a text string of absorption coefficient unit and returns the relevant enum
        /// </summary>
        /// <param name="absorptionCoefficientUnit">string representing absorption coefficient unit</param>
        /// <returns>enum of type AbsorptionCoefficientUnit</returns>
        public static AbsorptionCoefficientUnit getAbsorptionCoefficientUnit(string absorptionCoefficientUnit)
        {
            string units;
            string[] unit;

            //pull out the unit values
            Match match = Regex.Match(absorptionCoefficientUnit, @"1/\(?([a-zA-Z\*]+)\)?");
            if (match.Success)
            {
                //get the value(s) in parentheses
                units = match.Groups[1].Value;
                unit = units.Split('*');
                switch (unit[0])
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
                        throw new Exception("Not a valid absorption coefficient unit");
                }
            }
            else
            {
                throw new Exception("Not a valid absorption coefficient unit");
            }
        }

        /// <summary>
        /// Passes a text string of molar unit and returns the relevant enum
        /// </summary>
        /// <param name="molarUnit">string representing molar unit</param>
        /// <returns>enum of type MolarUnit</returns>
        public static MolarUnit getMolarUnit(string molarUnit)
        {
            string units;
            string[] unit;

            //pull out the unit values
            Match match = Regex.Match(molarUnit, @"1/\(*([a-zA-Z\*]+)\)*");
            if (match.Success)
            {
                //get the value(s) in parentheses
                units = match.Groups[1].Value;
                if (units.Contains("*"))
                {
                    unit = units.Split('*');
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
                else
                {
                    return MolarUnit.None;
                }
            }
            else
            {
                throw new Exception("Not a valid molar unit");
            }
        }

        /// <summary>
        /// Passes the molar unit enum and the absorption coefficient unit enum and returns it as a string
        /// </summary>
        /// <param name="molarUnit">The molar unit enum</param>
        /// <returns>A string representing the molar units</returns>
        public static string getSpectralUnit(MolarUnit molarUnit, AbsorptionCoefficientUnit absorptionCoefficientUnit)
        {
            string mU;
            string aCU;
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
                    throw new Exception("Unknown molar unit");
            }

            switch (absorptionCoefficientUnit)
            {
                case AbsorptionCoefficientUnit.InverseCentimeters:
                    aCU = "cm";
                    break;
                case AbsorptionCoefficientUnit.InverseMeters:
                    aCU = "m";
                    break;
                case AbsorptionCoefficientUnit.InverseMicrometers:
                    aCU = "um";
                    break;
                case AbsorptionCoefficientUnit.InverseMillimeters:
                    aCU = "mm";
                    break;
                default:
                    throw new Exception("Unknown absorption coefficient unit");
            }
            if (molarUnit == MolarUnit.None)
            {
                sU = "1/" + aCU;
            }
            else
            {
                sU = "1/(" + aCU + "*" + mU + ")";
            }
            return sU;
        }
    }
}
