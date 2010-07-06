using System;
using System.Collections.Generic;

namespace Vts.SiteVisit.Extensions
{
    public static class OptionExtensions
    {
        public static bool HasSubOptions<TValue>(this TValue enumType)
        {
            return false;
        }

        public static IEnumerable<TValue> GetSubOptions<TValue>(this TValue enumType)
        {
            throw new NotImplementedException();
        }
    }
}
