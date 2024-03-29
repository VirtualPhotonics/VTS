using System;
using System.Resources;
using System.Threading;
using Vts.Common.Resources;

namespace Vts
{
    /// <summary>
    /// methods to ease access to enums
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns a string in the correct language representing the enum 
        /// </summary>
        /// <param name="enumType">The enum type</param>
        /// <returns>A string representing the enum</returns>
        public static string GetInternationalizedString(this Enum enumType)
        {
            var baseString = enumType.GetType().ToString();
            var type = baseString.Substring(baseString.IndexOf('.') + 1);
            var name = enumType.ToString();

            var rm = new ResourceManager("Vts.Common.Resources.Strings", typeof(Strings).Assembly);

            var s = rm.GetString(type + "_" + name, Thread.CurrentThread.CurrentCulture);
            return s != null ? s : "";
        }

        /// <summary>
        /// Factory method to return ChromophoreCoefficientType given a ChromophoreType
        /// </summary>
        /// <param name="chromophoreType">The chromophore type e.g. HbO2,Hb,Melanin,H2O,Fat</param>
        /// <returns>A ChromophoreCoefficientType associated with the input</returns>
        public static ChromophoreCoefficientType GetCoefficientType(this ChromophoreType chromophoreType)
        {
            switch (chromophoreType)
            {
                case ChromophoreType.HbO2:
                case ChromophoreType.Hb:
                case ChromophoreType.Nigrosin:
                default:
                    return ChromophoreCoefficientType.MolarAbsorptionCoefficient;
                case ChromophoreType.H2O:
                case ChromophoreType.Fat:
                case ChromophoreType.Melanin:
                case ChromophoreType.Baseline:
                    return ChromophoreCoefficientType.FractionalAbsorptionCoefficient;
            }
        }
        
        /// <summary>
        /// Method 'Add' turns on this bit
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="type">The Enum type</param>
        /// <param name="value">The value to add</param>
        /// <returns>An Enum type with "value" bit turned on</returns>
        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)((int)(object)type | (int)(object)value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not append value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }

        /// <summary>
        /// Method 'Remove' turns off this bit (does nothing if flag not present)
        /// </summary>
        /// <typeparam name="T">The generic type</typeparam>
        /// <param name="type">The Enum type</param>
        /// <param name="value">The value to remove</param>
        /// <returns>An Enum with "value" bit turned off (nothing if value not present)</returns>
        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                if (type.HasFlag((Enum)(object)value))
                {
                    return (T)(object)((int)(object)type ^ (int)(object)value);
                }
                return (T) (object) type;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not remove value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }
    }
}
