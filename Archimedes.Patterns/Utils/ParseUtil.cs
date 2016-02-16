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
        /// Try to parese the given object to the given type. Returns an optional result, depending on success.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static Optional<object> ParseSave(object value, Type targetType, CultureInfo culture = null)
        {
            if (value == null) return Optional.Empty<object>();

            try
            {
                return Optional.Of(Parse(value, targetType, culture));
            }
            catch (FormatException)
            {
                return Optional.Empty<object>();
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
        /// Try to parese the given string to the given type. Returns an optional result, depending on success.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static Optional<object> ParseSave(string value, Type targetType, CultureInfo culture = null)
        {
            if (value == null) return Optional.Empty<object>();

            try
            {
                return Optional.Of(Parse(value, targetType, culture));
            }
            catch (FormatException)
            {
                return Optional.Empty<object>();
            }
        }

        /// <summary>
        ///  Parses the given object to the desired type. If this is not possible, an FormatException is thrown.
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
            return (T)Parse(value, typeof(T), culture);
        }

        /// <summary>
        ///  Parses the given object to the desired type. If this is not possible, an FormatException is thrown.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public static object Parse(object value, Type targetType, CultureInfo culture = null)
        {
            if (value == null) throw new ArgumentNullException("value");
            culture = culture ?? DefaultCulture;

            if (targetType == value.GetType())
            {
                return value;
            }
            return Parse(Convert.ToString(value, culture), targetType, culture);
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
            return (T)Parse(value, typeof(T), culture);
        }

        /// <summary>
        ///  Parses the given string to the desired type. If this is not possible, an FormatException is thrown.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="culture">Optionally specify the culture.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public static object Parse(string value, Type targetType, CultureInfo culture = null)
        {
            if (value == null) throw new ArgumentNullException("value");
            culture = culture ?? DefaultCulture;


            // First try to handle specail cases

            if (targetType == typeof(string)) return value;

            if (targetType.IsEnum) return EnumTryParse(targetType, value);

            if (targetType == typeof(Guid)) return ParseGuid(value);

            if (targetType == typeof(bool)) return ParseBoolean(value);


            // Hande the remainig cases with a generic converter
            try
            {
                return Convert.ChangeType(value, targetType, culture);
            }
            catch (Exception e)
            {
                throw new FormatException(string.Format("Failed to parse string '{0}' to an {1} with culture {2}!", value, targetType, culture), e);
            }
        }

        #region Private parse helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="FormatException">Thrown when the string is not a valid boolean</exception>
        private static bool ParseBoolean(string value)
        {
            value = value.Trim().ToLowerInvariant();

            switch (value)
            {
                    case "1":
                    case "true":
                        return true;

                    case "0":
                    case "false":
                        return false;

                default:
                    throw new FormatException(string.Format("Can not parse '{0}' into Boolean!"));                 
            }
        }

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
            return (T)EnumTryParse(typeof(T), str);
        }


        private static object EnumTryParse(Type enumType, string str)
        {
            if (!enumType.IsEnum) throw new ArgumentException(string.Format("You must only use Enum Types for parameter T! '{0}' is not an enum type!", enumType));

            if (string.IsNullOrEmpty(str)) throw new FormatException("An empty string can not be parsed to an Enum.");

            var enumIndexO = ParseSave<int>(str);

            if (enumIndexO.IsPresent)
            {
                int enumIndex = enumIndexO.Value;
                object enumValue = enumIndex; // This allows any number to be converted in an Enum

                if (!Enum.IsDefined(enumType, enumValue))
                {
                    throw new FormatException(string.Format("The enum Index {0} does not exist in Enum {1}", enumIndex, enumType));
                }

                return enumValue;
            }
            else
            {
                foreach (string en in Enum.GetNames(enumType))
                {
                    if (en.Equals(str, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return Enum.Parse(enumType, str, true);
                    }
                }

                throw new FormatException(string.Format("The value '{0}' is not a valid member of enum {1}", str, enumType));
            }
        }



        #endregion

    }
}
