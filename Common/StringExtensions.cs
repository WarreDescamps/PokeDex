namespace Common
{
    public static class StringExtensions
    {
        public static string FirstToUpper(this string input)
            => input != null ? (input.Length > 1 ? char.ToUpper(input[0]) + input[1..].ToLower() : input.ToUpper()) : null;
    }
}