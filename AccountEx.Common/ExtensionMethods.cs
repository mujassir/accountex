using System;

namespace AccountEx.Common
{
    public static class DateTimeExtensions
    {
        public static string ToStringDmy(this DateTime input)
        {
            return input.ToString("dd/MM/yyyy");
        }
    }

    public static class StringExtensions
    {
        public static DateTime ToDateDmy(this string input)
        {
            try
            {
                var provider = System.Globalization.CultureInfo.InvariantCulture;
                return DateTime.ParseExact(input, "dd/MM/yyyy", provider);
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }
    }
    /// <summary>Provides functionalities related to <c>Enum</c> extension methods</summary>
    public static class EnumExtensions
    {
        /// <summary> Extension method to return an enum value of type T for the given string. </summary>
        /// <typeparam name="T">The type of objects to enumerate.This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived.</typeparam>
        /// <param name="value">The value of object to convert to specified enum</param>
        /// <returns>An enum that can be used in social media authentication</returns>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary> Extension method to return an enum value of type T for the given int. </summary>
        /// <typeparam name="T">The type of objects to enumerate.This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived.</typeparam>
        /// <param name="value">The value of object to convert to specified enum</param>
        /// <returns>An enum that can be used in social media authentication</returns>
        public static T ToEnum<T>(this int value)
        {
            var name = Enum.GetName(typeof(T), value);
            return name.ToEnum<T>();
        }
    }
}
