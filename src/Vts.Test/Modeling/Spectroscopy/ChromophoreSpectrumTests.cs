using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.SpectralMapping;
using Vts.IO;

namespace Vts.Test.Modeling.Spectroscopy
{
    [TestFixture]
    public class ChromophoreSpectrumTests
    {
        [Test]
        public void validate_Serializing_Chromophore_Spectrum()
        {
            string name = "Melanin";
            AbsorptionCoefficientUnits muaUnits = AbsorptionCoefficientUnits.PerMillimeter;
            ChromophoreCoefficientType coeffType = ChromophoreCoefficientType.FractionalAbsorptionCoefficient;

            // populate list of wavelengths
            List<double> wavelengths = new List<double>();
            wavelengths.Add(0.0);
            wavelengths.Add(1.0);
            wavelengths.Add(2.0);

            // populate list of values
            List<double> values = new List<double>();
            values.Add(0.1);
            values.Add(1.1);
            values.Add(2.1);

            ChromophoreSpectrum chromophoreSpectrum = new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnits);
            chromophoreSpectrum.WriteToXML("ChromophoreSpectrum.xml");
        }

        [Test]
        public void validate_Deserializing_Chromophore_Spectrum()
        {
            string name = "Melanin";
            AbsorptionCoefficientUnits muaUnits = AbsorptionCoefficientUnits.PerMillimeter;
            ChromophoreCoefficientType coeffType = ChromophoreCoefficientType.FractionalAbsorptionCoefficient;

            // populate list of wavelengths
            List<double> wavelengths = new List<double>();
            wavelengths.Add(0.0);
            wavelengths.Add(1.0);
            wavelengths.Add(2.0);

            // populate list of values
            List<double> values = new List<double>();
            values.Add(0.1);
            values.Add(1.1);
            values.Add(2.1);

            ChromophoreSpectrum chromophoreSpectrum = new ChromophoreSpectrum(wavelengths, values, name, coeffType, muaUnits);
            chromophoreSpectrum.WriteToXML("ChromophoreSpectrum.xml");

            var chromophoreSpectrumRead = FileIO.ReadFromXML<ChromophoreSpectrum>("ChromophoreSpectrum.xml");

            Console.WriteLine(chromophoreSpectrumRead);
        }
    }
}
