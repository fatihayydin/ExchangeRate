using System.Text.RegularExpressions;

namespace ExchangeRate.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex sWhitespace = new Regex(@"\s+");

        public static string RemoveJsonUnescapeCharacters(this string value)
        {
            value = Regex.Unescape(value); //almost there
            value = value.Remove(value.Length - 1, 1).Remove(0, 1); //remove first and last qoutes

            return value;
        }

        public static string ReplaceWhitespace(this string input)
        {
            return input.Replace(" ", String.Empty);
        }
    }
}
