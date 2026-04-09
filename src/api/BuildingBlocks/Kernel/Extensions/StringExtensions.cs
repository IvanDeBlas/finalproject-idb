using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WePlayRises.BuildingBlocks.Kernel.Extensions
{
    /// <summary>
    /// Extension methods for string manipulation and utilities
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class StringExtensions
    {
        /// <summary>
        /// Removes non-alphanumeric characters, replacing them with underscores
        /// </summary>
        /// <param name="str">The string to process</param>
        /// <returns>String with only letters, digits, and underscores</returns>
        public static string RemoveNonLetterOrNumberCharacters(this string str)
        {
            return !string.IsNullOrEmpty(str)
                ? new string(str.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray())
                : string.Empty;
        }

        /// <summary>
        /// Parses a key-value string format to a list of values
        /// </summary>
        /// <param name="keyValues">The key-value string to parse</param>
        /// <returns>List of parsed values</returns>
        public static List<string> KeyValueToList(this string keyValues)
        {
            if (string.IsNullOrWhiteSpace(keyValues))
                return new List<string>();

            var list = new List<string>();
            var pairs = keyValues.Replace("[[", "").Replace("]]", "").Split("],[");

            foreach (var pair in pairs)
            {
                var elements = pair.Split(',');

                if (elements.Length == 2)
                {
                    var value = elements[1].Replace("\"", "");
                    list.Add(value);
                }
            }

            return list;
        }

        /// <summary>
        /// Removes HTML tags from a string
        /// </summary>
        /// <param name="input">The string with HTML tags</param>
        /// <returns>String without HTML tags</returns>
        public static string RemoveHtmlTags(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Gets the MIME content type based on file extension
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The content type</returns>
        public static string GetContentType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "text/plain";

            var fileExtension = Path.GetExtension(fileName);

            return fileExtension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".xls" or ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "text/plain"
            };
        }

        internal static string? GetJsonString(this string? value)
        {
            if (value == null)
                return null;

            var result = value.Replace("\"", "").Replace("{", "[").Replace("}", "]")
                .Replace("\",", "\":").Replace("[[", "[{").Replace("]]", "}]").Replace("],[", "},{");

            return result;
        }

        internal static string? CleanTiny(this string? value)
        {
            return value?.Replace("\n", "");
        }

        internal static string? CleanCommas(this string? value)
        {
            return value?.Replace("'", "\"");
        }

        /// <summary>
        /// Capitalizes first letter of each word after removing diacritics
        /// </summary>
        /// <param name="str">The string to process</param>
        /// <returns>String with each word capitalized and no diacritics</returns>
        public static string SplitUpperFirst(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            var sinAcentos = RemoveDiacritics(str);
            var subs = sinAcentos.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return string.Concat(subs.Select(s => s.UpperFirstLetter()));
        }

        /// <summary>
        /// Truncates a string to a maximum length
        /// </summary>
        /// <param name="str">The string to truncate</param>
        /// <param name="length">Maximum length</param>
        /// <returns>Truncated string</returns>
        public static string Left(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative");

            if (str.Length <= length)
                return str;

            return str[..length];
        }

        /// <summary>
        /// Capitalizes the first letter of a string
        /// </summary>
        /// <param name="input">The string to capitalize</param>
        /// <returns>String with first letter capitalized</returns>
        /// <exception cref="ArgumentNullException">If input is null</exception>
        /// <exception cref="ArgumentException">If input is empty</exception>
        public static string UpperFirstLetter(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        /// <summary>
        /// Removes diacritics (accents) from a string
        /// </summary>
        /// <param name="text">The text with diacritics</param>
        /// <returns>Text without diacritics</returns>
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Generates a cryptographically secure random alphanumeric string
        /// </summary>
        /// <param name="length">The length of the random string</param>
        /// <returns>A random alphanumeric string</returns>
        /// <exception cref="ArgumentOutOfRangeException">If length is less than 1</exception>
        public static string RandomString(int length)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be at least 1");

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }

            return new string(result);
        }

        /// <summary>
        /// Formats a decimal value as currency with 2 decimal places
        /// </summary>
        /// <param name="amount">The amount to format</param>
        /// <param name="culture">The culture to use (defaults to invariant)</param>
        /// <returns>Formatted currency string</returns>
        public static string ToCurrencyFormat(this decimal amount, CultureInfo? culture = null)
        {
            return amount.ToString("N2", culture ?? CultureInfo.InvariantCulture);
        }
    }
}
