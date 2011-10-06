using System.Linq;

namespace Vts.SpectralMapping
{
    ///// <summary>
    ///// Class that allows conversion from one spectral unit to another. Utilizes an internal xtension method  
    ///// that converts unit enums ToInverseMeters and ToInverseMolar, which makes the conversion logic simpler
    ///// </summary>
    //public static class ChromophoreSpectrumUnitConverter
    //{
    //    public static void ConvertUnits(this ChromophoreSpectrum myChromophoreSpectrum,
    //        ChromDataConcentrationUnits fromConcentrationUnits, ChromDataDistanceUnits fromDistanceUnits,
    //        ChromDataConcentrationUnits toConcentrationUnits, ChromDataDistanceUnits toDistanceUnits)
    //    {
    //        // convert (someone needs to check my math here...)
    //        double multiplicationFactor =
    //            fromConcentrationUnits.InInverseMolar() / toConcentrationUnits.InInverseMolar() *
    //            fromDistanceUnits.InInverseMeters() / toDistanceUnits.InInverseMeters();

    //        // use LINQ to iterate over all the values in the IList
    //        myChromophoreSpectrum.Spectrum = myChromophoreSpectrum.Spectrum.Select(s => s * multiplicationFactor).ToArray();
    //    }
    //}

    ///// <summary>
    ///// Helper class to convert from one set of units to the other
    ///// </summary>
    //internal static class ChromophoreUnitEnumExtensions
    //{
    //    public static double InInverseMeters(this ChromDataDistanceUnits units)
    //    {
    //        switch (units)
    //        {
    //            case ChromDataDistanceUnits.PerMillimeter:
    //                return 1000.0;
    //            case ChromDataDistanceUnits.PerCentimeter:
    //                return 100.0;
    //            case ChromDataDistanceUnits.PerMeter:
    //            default:
    //                return 1.0;
    //        }
    //    }
    //    public static double InInverseMolar(this ChromDataConcentrationUnits units)
    //    {
    //        switch (units)
    //        {
    //            case ChromDataConcentrationUnits.PerMicroMolar:
    //                return 1000000.0;
    //            case ChromDataConcentrationUnits.PerMilliMolar:
    //                return 1000.0;
    //            case ChromDataConcentrationUnits.PerMolar:
    //            default:
    //                return 1.0;
    //        }
    //    }
    //}
}
