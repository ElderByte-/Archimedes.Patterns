using System;
using System.Globalization;

namespace Archimedes.Patterns.Utils
{
    /// <summary>
    /// Provides simple parse methods for primitives.
    /// 
    /// The difference to the build in .NET parse methods is, that in case of an erorr more detailed exceptions are thrown which 
    /// help to identify the problem.
    /// </summary>
    public static class ParseUtil
    {

        private static readonly CultureInfo DefaultCulture = CultureInfo.CurrentUICulture;

        /// <summary>
        /// Try to parese the given object to the given type. Returns an optional result, depending on success.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static Optional<T> ParseSave<T>(object value, CultureInfo culture = null)
        {
            if (value == null) return Optional.Empty<T>();

            try
            {
                return Optional.Of(Parse<T>(value, culture));
            }
            catch (FormatException)
            {
                return Optional.Empty<T>();
            }
        }

        /// <summary>
        /// Try to parese the given string to the given type. Returns an optional result, depending on success.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static Optional<T> ParseSave<T>(string value, CultureInfo culture = null)
        {
            if (value == null) return Optional.Empty<T>();

            try
            {
                return Optional.Of(Parse<T>(value, culture));
            }
            catch (FormatException)
            {
                return Optional.Empty<T>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="culture">Optionally specify the culture.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public static T Parse<T>(object value, CultureInfo culture = null)
        {
            if (value == null) throw new ArgumentNullException("value");
            culture = culture ?? DefaultCulture;

            if (typeof (T) == value.GetType())
            {
                return (T) value;
            }
            return Parse<T>(Convert.ToString(value, culture));
        }

        /// <summary>
        /// Parses the given string to the desired type. If this is not possible, an FormatException is thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="culture">Optionally specify the culture.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public static T Parse<T>(string value, CultureInfo culture = null)
        {
            if (value == null) throw new ArgumentNullException("value");
            culture = culture ?? DefaultCulture;


            // First try to handle specail cases

            if (typeof (T) == typeof(string)) return (T)(object)value;

            if (typeof(T).IsEnum) return EnumTryParse<T>(value);

            if (typeof(T) == typeof(Guid)) return (T)(object)ParseGuid(value);

            // Hande the remainig cases with a generic converter
            try
            {
                return (T)Convert.ChangeType(value, typeof(T), culture);
            }
            catch (Exception e)
            {
                throw new FormatException(string.Format("Failed to parse string '{0}' to an {1} with culture {2}!", value, typeof(T), culture), e);
            }
        }

        #region Private parse helpers

        /// <summary>
        /// Parses the given string to an double
        /// </summary>
        /// <exception cref="FormatException">Thrown when the string is not a valid double number</exception>
        /// <param name="str"></param>
        /// <returns></returns>
        private static Guid ParseGuid(string str)
        {
            Guid value;
            if (Guid.TryParse(str, out value))
            {
                return value;
            }
            throw new FormatException(string.Format("Failed to parse string '{0}' to an Guid!", str));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        private static T EnumTryParse<T>(string str)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException(string.Format("You must only use Enum Types for parameter T! '{0}' is not an enum type!", typeof(T)));
            }

            var enumIndexO = ParseSave<int>(str);

            if (enumIndexO.IsPresent)
            {
                int enumIndex = enumIndexO.Value;
                T enumValue = (T)(object)enumIndex; // This allows any number to be converted in an Enum

                if (!Enum.IsDefined(typeof(T), enumValue))
                {
                    throw new FormatException(string.Format("The enum Index {0} does not exist in Enum {1}", enumIndex, typeof(T)));
                }

                return enumValue;
            }
            else
            {
                foreach (string en in Enum.GetNames(typeof(T)))
                {
                    if (en.Equals(str, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return (T)Enum.Parse(typeof(T), str, true);
                    }
                }

                throw new FormatException(string.Format("The value '{0}' is not a valid member of enum {1}", str, typeof(T)));
            }
        }

        #endregion

    }
}
