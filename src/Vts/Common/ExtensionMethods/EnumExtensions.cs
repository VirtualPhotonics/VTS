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
        // The following set of extension methods aid in accessing enums set up to
        // be bit maps
        /// <summary>
        /// Has checks whether enum has this bit turned on
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Is checks whether enum is exclusively a particular type(s)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Add turns on this bit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                //return (T)(object)(((int)(object)type | (int)(object)value));
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }
        /// <summary>
        /// Remove turns off this bit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Remove<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type ^ (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

    }
}
