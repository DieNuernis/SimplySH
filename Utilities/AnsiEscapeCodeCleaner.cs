using System.Text.RegularExpressions;

namespace SimplySH.Utilities
{
    public static class AnsiEscapeCodeCleaner
    {
        private static readonly Regex AnsiRegex = new(@"\x1B(?:[@-Z\\-_]|\[[0-?]*[ -/]*[@-~])", RegexOptions.Compiled);

        public static string Clean(string input)
        {
            return AnsiRegex.Replace(input, string.Empty);
        }
    }
}
