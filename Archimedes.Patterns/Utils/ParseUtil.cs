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

        #region Parse Save API

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

            culture = culture ?? DefaultCulture;

            if (typeof(T) == value.GetType())
            {
                return Optional.Of(value).CastTo<T>();
            }

            return ParseSave<T>(Convert.ToString(value, culture), culture);
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

            culture = culture ?? DefaultCulture;

            if (targetType == value.GetType())
            {
                return Optional.Of(value);
            }

            return ParseSave(Convert.ToString(value, culture), targetType, culture);
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
            return ParseSave(value, typeof(T), culture).CastTo<T>();
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
            culture = culture ?? DefaultCulture;

            if (value == null) return Optional.Empty<object>();

            // First try to handle specail cases

            if (targetType == typeof(string)) return Optional.Of(value).CastTo<object>();

            if (targetType.IsEnum) return TryParseEnum(targetType, value);

            if (targetType == typeof(Guid)) return TryParseGuid(value).CastTo<object>();

            if (targetType == typeof(bool)) return TryParseBoolean(value).CastTo<object>();


            // Hande the remainig cases with a generic converter
            try
            {
                var obj = Convert.ChangeType(value, targetType, culture);
                return Optional.Of(obj);
            }
            catch (Exception)
            {
                return Optional.Empty<object>();
            }
        }


        #endregion

        #region Parse API

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

            return ParseSave(value, targetType, culture).OrElseThrow(
                () => new FormatException(string.Format("Failed to parse '{0}' to '{1}' with culture {2}!", value, targetType, culture)));
        }

        #endregion

        #region Private parse helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="FormatException">Thrown when the string is not a valid boolean</exception>
        private static Optional<bool> TryParseBoolean(string value)
        {
            value = value.Trim().ToLowerInvariant();

            switch (value)
            {
                    case "1":
                    case "true":
                        return Optional.Of(true);

                    case "0":
                    case "false":
                    return Optional.Of(false);

                default:
                    return Optional.Empty<bool>();
            }
        }

        /// <summary>
        /// Parses the given string to an double
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static Optional<Guid> TryParseGuid(string str)
        {
            Guid value;
            if (Guid.TryParse(str, out value))
            {
                return Optional.Of(value);
            }
            return Optional.Empty<Guid>();
        }


        private static Optional<object> TryParseEnum(Type enumType, string str)
        {
            if (!enumType.IsEnum) throw new ArgumentException(string.Format("You must only use Enum Types for parameter T! '{0}' is not an enum type!", enumType));

            if (string.IsNullOrEmpty(str)) return Optional.Empty<object>();

            var enumIndexO = ParseSave<int>(str);

            if (enumIndexO.IsPresent)
            {
                int enumIndex = enumIndexO.Value;
                object enumValue = enumIndex; // This allows any number to be converted in an Enum

                if (!Enum.IsDefined(enumType, enumValue))
                {
                    return Optional.Empty<object>();
                }

                return Optional.Of(enumValue);
            }
            else
            {
                foreach (string en in Enum.GetNames(enumType))
                {
                    if (en.Equals(str, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return Optional.Of(Enum.Parse(enumType, str, true));
                    }
                }

                return Optional.Empty<object>();
            }
        }



        #endregion

    }
}
