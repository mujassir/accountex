using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace AccountEx.Common
{
    /// <summary>
    /// Converts a data type to another data type.
    /// </summary>
    public static class Cast
    {
        /// <summary>
        /// Converts input to Type of default value or given as typeparam T
        /// </summary>
        /// <typeparam name="T">typeparam is the type in which value will be returned, it could be any type eg. int, string, bool, decimal etc.</typeparam>
        /// <param name="input">Input that need to be converted to specified type</param>
        /// <param name="defaultValue">defaultValue will be returned in case of value is null or any exception occures</param>
        /// <returns>Input is converted in Type of default value or given as typeparam T and returned</returns>
        public static T To<T>(object input, T defaultValue)
        {
            var result = defaultValue;
            try
            {
                if (input == null || input == DBNull.Value) return result;
                if (typeof(T).IsEnum)
                {
                    result = (T)Enum.ToObject(typeof(T), To(input, Convert.ToInt32(defaultValue)));
                }
                else
                {
                    result = (T)Convert.ChangeType(input, typeof(T));
                }
            }
            catch
            {
                // ignored
            }

            return result;
        }

        /// <summary>
        /// Converts input to Type of typeparam T
        /// </summary>
        /// <typeparam name="T">typeparam is the type in which value will be returned, it could be any type eg. int, string, bool, decimal etc.</typeparam>
        /// <param name="input">Input that need to be converted to specified type</param>
        /// <returns>Input is converted in Type of default value or given as typeparam T and returned</returns>
        public static T To<T>(object input)
        {
            return To(input, default(T));
        }



    }
}
