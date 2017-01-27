namespace TwitterWebApi.Extensions
{
    public static class StringExtensions
    {
        public static string HtmlDecode(this string input)
        {
            return System.Net.WebUtility.HtmlDecode(input);
        }
    }
}