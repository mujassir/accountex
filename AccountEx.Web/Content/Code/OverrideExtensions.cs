using AccountEx.Common;
using System;
using System.Globalization;

namespace AccountEx.Web
{
    public static class DecimalExtensions
    {
        public static string ToString(this decimal value)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalDigits = 0;
            if (value < 0)
                return "(" + value.ToString("N", nfi) + ")";
            return value.ToString("N", nfi);
        }
    }
    public static class DateTimeExtensions
    {
        public static string ToString(this DateTime value)
        {
            return value.ToString(AppSetting.DateFormat);
        }
    }
}