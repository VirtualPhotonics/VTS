using System;
using System.Resources;
using System.Threading;
using Vts.Common.Resources;

namespace Vts
{
    public static class EnumExtensions
    {
        public static string GetInternationalizedString(this Enum enumType)
        {
            string baseString = enumType.GetType().ToString();
            string type = baseString.Substring(baseString.IndexOf('.') + 1);
            string name = enumType.ToString();

            ResourceManager rm = new ResourceManager("Vts.Common.Resources.Strings", typeof(Strings).Assembly);

            string s = rm.GetString(type + "_" + name, Thread.CurrentThread.CurrentCulture);
            if (s != null)
                return s;
            else
                return "";
        }

        
        public static ChromophoreCoefficientType GetCoefficientType(this ChromophoreType chromophoreType)
        {
            switch (chromophoreType)
            {
                case ChromophoreType.HbO2:
                case ChromophoreType.Hb:
                case ChromophoreType.Melanin:
                case ChromophoreType.CPTA:
                case ChromophoreType.Nigrosin:
                default:
                    return ChromophoreCoefficientType.MolarAbsorptionCoefficient;
                case ChromophoreType.H2O:
                case ChromophoreType.Fat:
                case ChromophoreType.Baseline:
                    return ChromophoreCoefficientType.FractionalAbsorptionCoefficient;
            }
        }
    }
}
