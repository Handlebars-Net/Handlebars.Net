namespace HandlebarsDotNet.Polyfills
{
    internal static class StringExtensions
    {
        public static string Intern(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
#if netstandard1_3
            return str;
#else
            return string.Intern(str);
#endif
        }
    }
}