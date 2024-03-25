using System;
using System.Globalization;

namespace AccountEx.Common
{
    /// <summary>
    /// Provide methods related to numeric types
    /// </summary>
    public static class Numerics
    {
        public static int GetInt(object input)
        {
            if (input == null || input + "" == "") return 0;
            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static byte GetByte(object input)
        {
            if (input == null || input + "" == "") return 0;
            try
            {
                return Convert.ToByte(input);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static string IntToString(object number)
        {
            return IntToString(Numerics.GetInt(number));
        }
        public static string IntToString(int number)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            return number.ToString("#,##0");
        }
        public static decimal AddDecimal(params decimal?[] values)
        {
            decimal sum = 0;
            foreach (var item in values)
            {
                if (item.HasValue) sum += item.Value;
            }
            return sum;
        }
        public static string DecimalToString(decimal? value, int decimalPoints)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalDigits = decimalPoints;
            if (value.HasValue)
            {
                if (value.Value < 0)
                    return "(" + (value.Value * -1).ToString("N", nfi) + ")";
                return value.Value.ToString("N", nfi);
            }
            return "";
        }
        public static string DecimalToString(decimal? value, int decimalPoints, string zeroChar)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = ",";
            nfi.NumberDecimalDigits = decimalPoints;
            if (value.HasValue)
            {
                if (value.Value == 0) return zeroChar;
                if (value.Value < 0)
                    return "(" + (value.Value * -1).ToString("N", nfi) + ")";
                return value.Value.ToString("N", nfi);
            }
            return "";
        }
        public static string DecimalToString(decimal? value)
        {
            return DecimalToString(value, 2);
        }
        public static string DecimalToString(decimal? value, string zeroChar)
        {
            return DecimalToString(value, 2, zeroChar);
        }
        public static string DecimalToString(decimal? debit, decimal? credit)
        {
            if (debit.HasValue && debit.Value > 0)
                return DecimalToString(debit);
            return DecimalToString(credit);
        }
        public static decimal GetDecimal(object input)
        {
            if (input == null || input + "" == "") return 0;
            try
            {
                return Convert.ToDecimal(input);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static double GetDouble(object input)
        {
            if (input == null || input + "" == "") return 0;
            try
            {
                return Convert.ToDouble(input);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static bool GetBool(object input)
        {
            if (input == null || input + "" == "") return false;
            try
            {
                return Convert.ToBoolean(input);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string BoolToLowerString(Boolean input)
        {
            if (input == null || input + "" == "") return "false";
            try
            {
                return input.ToString().ToLower();

            }
            catch (Exception)
            {
                return "false";
            }
        }
        public static string DateToString(DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToString(AppSetting.DateFormat);
            return "";
        }
        public static T To<T>(object input, T defaultValue)
        {
            T result = defaultValue;
            try
            {
                if (typeof(T).IsEnum)
                    result = (T)Enum.Parse(typeof(T), input.ToString());
                else
                    result = (T)Convert.ChangeType(input, typeof(T));
            }
            catch { }
            return result;
        }
        public static T To<T>(object input)
        {
            return To(input, default(T));
        }

        //public static decimal Sum(params decimal?[] input)
        //{
        //    decimal sum = 0;

        //    foreach (var item in input)
        //    {
        //        if (item.HasValue)
        //            sum += item.Value;
        //    }
        //    return sum;
        //}

        public static decimal Sum(decimal? openingBalance, decimal? debit, decimal? credit)
        {
            decimal sum = 0;

            sum += openingBalance.HasValue ? openingBalance.Value : 0;
            sum += debit.HasValue ? debit.Value : 0;
            sum -= credit.HasValue ? credit.Value : 0;

            return sum;
        }
    }
}