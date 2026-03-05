using WePlayRises.BuildingBlocks.Kernel.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    /// <summary>
    /// Extension methods and utilities for working with enumerations
    /// </summary>
    public static class EnumExtension
    {
        public static List<SelectItem> ToSelectList<T>() where T : struct, IComparable
        {
            var selectItems = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(x => new SelectItem(Convert.ToInt16(x).ToString(), x.ToString())).ToList();
            return selectItems;
        }

        /// <summary>
        /// Gets the Description attribute value of an enum value
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>The description if exists, otherwise the enum ToString()</returns>
        public static string GetDescription(this Enum value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
                return value.ToString();

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Gets the Display Name attribute value of an enum value
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>The display name if exists, otherwise the enum ToString()</returns>
        public static string GetDisplayName(this Enum value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
                return value.ToString();

            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Name ?? value.ToString();
        }

        /// <summary>
        /// Gets a custom attribute from an enum value
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>The attribute if exists, otherwise null</returns>
        public static TAttribute? GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            ArgumentNullException.ThrowIfNull(value);

            var fieldInfo = value.GetType().GetField(value.ToString());
            return fieldInfo?.GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// Gets the default value of an enum (first looks for "None", then for 0, otherwise throws)
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <returns>The default value</returns>
        /// <exception cref="ArgumentException">If T is not an enum or no default found</exception>
        public static T GetDefault<T>() where T : struct, Enum
        {
            if (IsDefined<T>("None"))
                return Enum.Parse<T>("None", true);

            if (IsDefined<T>(0))
                return (T)Enum.ToObject(typeof(T), 0);

            throw new ArgumentException($"No default value found for enum type {typeof(T).FullName}");
        }

        /// <summary>
        /// Gets all enum values as a list
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="excludeNone">If true, excludes the "None" value if exists</param>
        /// <returns>List of enum values</returns>
        public static List<T> GetItems<T>(bool excludeNone = false) where T : struct, Enum
        {
            var list = Enum.GetValues<T>().ToList();

            if (excludeNone && Enum.TryParse<T>("None", true, out var noneValue))
            {
                list.Remove(noneValue);
            }

            return list;
        }

        /// <summary>
        /// Gets the integer value of an enum
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>The integer value</returns>
        public static int GetValue<T>(this T value) where T : struct, Enum
        {
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets a sorted dictionary of enum integer values and their descriptions
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <returns>Dictionary with integer keys and description values</returns>
        public static SortedDictionary<int, string> GetValuesWithDescriptions<T>() where T : struct, Enum
        {
            var sortedList = new SortedDictionary<int, string>();

            foreach (T value in Enum.GetValues<T>())
            {
                var enumValue = value as Enum;
                var intValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                var description = enumValue?.GetDescription() ?? value.ToString();
                sortedList.Add(intValue, description);
            }

            return sortedList;
        }

        /// <summary>
        /// Checks if a string value is defined in the enum
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="strEnumText">The string to check</param>
        /// <returns>True if defined, false otherwise</returns>
        public static bool IsDefined<T>(string strEnumText) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(strEnumText))
                return false;

            return Enum.TryParse<T>(strEnumText, true, out _);
        }

        /// <summary>
        /// Checks if an integer value is defined in the enum
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="intValue">The integer to check</param>
        /// <returns>True if defined, false otherwise</returns>
        public static bool IsDefined<T>(int intValue) where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), intValue);
        }

        /// <summary>
        /// Parses an integer to enum, returns default if not valid
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="intValue">The integer value to parse</param>
        /// <returns>The parsed enum value or default if invalid</returns>
        public static T Parse<T>(int intValue) where T : struct, Enum
        {
            if (IsDefined<T>(intValue))
                return (T)Enum.ToObject(typeof(T), intValue);

            return GetDefault<T>();
        }

        /// <summary>
        /// Parses a string to enum, returns default if not valid
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="strEnumText">The string value to parse</param>
        /// <returns>The parsed enum value or default if invalid</returns>
        public static T Parse<T>(string strEnumText) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(strEnumText))
                return GetDefault<T>();

            if (Enum.TryParse<T>(strEnumText, true, out var result))
                return result;

            return GetDefault<T>();
        }

        /// <summary>
        /// Tries to parse a string to enum
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="text">The string to parse</param>
        /// <param name="enumValue">The output enum value</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryParse<T>(string text, out T enumValue) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                enumValue = GetDefault<T>();
                return false;
            }

            return Enum.TryParse(text, true, out enumValue);
        }

        /// <summary>
        /// Finds an enum value by its Display Name attribute
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <param name="displayName">The display name to search</param>
        /// <returns>The enum value if found, otherwise default</returns>
        public static T FromDisplayName<T>(string displayName) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(displayName))
                return GetDefault<T>();

            // Check for display name match
            foreach (T item in Enum.GetValues<T>())
            {
                var enumValue = item as Enum;
                if (enumValue?.GetDisplayName() == displayName)
                    return item;
            }

            // Check for ordinary name match
            if (Enum.TryParse<T>(displayName, true, out var result))
                return result;

            return GetDefault<T>();
        }
    }
}
