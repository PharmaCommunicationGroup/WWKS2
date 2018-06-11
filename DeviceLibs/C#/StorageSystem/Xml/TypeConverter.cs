using System;

namespace CareFusion.Lib.StorageSystem.Xml
{
    /// <summary>
    /// Class which defines WWKS 2.0 protocol related type conversion methods.
    /// </summary>
    internal static class TypeConverter
    {
        /// <summary>
        /// Converts the specified string to the enum of the specified type in a type safe manner.
        /// </summary>
        /// <typeparam name="T">Type to convert the string to.</typeparam>
        /// <param name="enumString">The enum string to convert.</param>
        /// <param name="defaultValue">The default value to set, if the conversion fails.</param>
        /// <returns>Type safe converted value or the specified default value.</returns>
        public static T ConvertEnum<T>(string enumString, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(enumString))
            {
                return defaultValue;
            }

            T result = defaultValue;

            if (Enum.TryParse<T>(enumString, out result) == false)
            {
                return defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Converts the specified boolean string into a boolean value.
        /// </summary>
        /// <param name="booleanString">The boolean string to convert.</param>
        /// <returns>An appropriate conversion of the specified bool string.</returns>
        public static bool ConvertBool(string booleanString)
        {
            bool result = false;

            if (string.IsNullOrWhiteSpace(booleanString))
            {
                return result;
            }

            if (bool.TryParse(booleanString, out result) == false)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Converts the specified int string into an integer.
        /// </summary>
        /// <param name="intString">The int string to convert.</param>
        /// <returns>An appropriate conversion of the specified int string.</returns>
        public static int ConvertInt(string intString)
        {
            int result = 0;

            if (string.IsNullOrWhiteSpace(intString))
            {
                return result;
            }

            if (int.TryParse(intString, out result) == false)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Converts the specified uint string into an unsigned integer.
        /// </summary>
        /// <param name="intString">The uint string to convert.</param>
        /// <returns>An appropriate conversion of the specified uint string.</returns>
        public static uint ConvertUInt(string intString)
        {
            uint result = 0;

            if (string.IsNullOrWhiteSpace(intString))
            {
                return result;
            }

            if (uint.TryParse(intString, out result) == false)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Converts the specified long string into an unsigned integer.
        /// </summary>
        /// <param name="longString">The ulong string to convert.</param>
        /// <returns>An appropriate conversion of the specified ulong string.</returns>
        public static ulong ConvertULong(string longString)
        {
            ulong result = 0;

            if (string.IsNullOrWhiteSpace(longString))
            {
                return result;
            }

            if (ulong.TryParse(longString, out result) == false)
            {
                result = 0;
            }

            return result;
        }

        /// <summary>
        /// Converts the specified date string into a date value.
        /// </summary>
        /// <param name="dateString">The date string to convert.</param>
        /// <returns>An appropriate conversion of the specified date string.</returns>
        public static DateTime ConvertDate(string dateString)
        {
            DateTime result = DateTime.MinValue;

            if (string.IsNullOrWhiteSpace(dateString))
            {
                return result;
            }

            if (DateTime.TryParse(dateString, out result) == false)
            {
                result = DateTime.MinValue;
            }

            return result;
        }

        /// <summary>
        /// Converts the specified date value into a valid protocol date string.
        /// </summary>
        /// <param name="date">The date value to convert.</param>
        /// <returns>The converted protocol date string.</returns>
        public static string ConvertDate(DateTime date)
        {
            return string.Format("{0:yyyy-MM-dd}", date);
        }

        /// <summary>
        /// Converts the specified date value into a valid protocol date time string.
        /// </summary>
        /// <param name="date">The date value to convert.</param>
        /// <returns>The converted protocol date time value.</returns>
        public static string ConvertDateTime(DateTime date)
        {
            return string.Format("{0:yyyy-MM-ddTHH:mm:ssZ}", date);
        }

        /// <summary>
        /// Converts the specified decimal strign into a decimal
        /// </summary>
        /// <param name="decimalString">The decimal strign to convert.</param>
        /// <returns>The converted Decimal.</returns>
        public static decimal ConvertDecimal(string decimalString)
        {
            decimal result = 0;

            if (string.IsNullOrWhiteSpace(decimalString))
            {
                return result;
            }

            if (decimal.TryParse(decimalString, out result) == false)
            {
                result = 0;
            }

            return result;
        }

        
    }
}
