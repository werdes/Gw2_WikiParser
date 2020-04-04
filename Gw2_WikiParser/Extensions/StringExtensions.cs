using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Gw2_WikiParser.Extensions
{
    public static class StringExtensions
    {
        public static string ToBase64(this string s, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(s);
            return Convert.ToBase64String(bytes);
        }

        public static string EnvelopWith(this string s, string env)
        {
            return env + s + env;
        }

        public static bool ContainsAny(this string s, IEnumerable<string> phrases, StringComparison stringComparison)
        {
            foreach(string phrase in phrases)
            {
                if (s.Contains(phrase, stringComparison))
                    return true;
            }
            return false;
        }

        public static string Format(this string s, params object[] parameters)
        {
            return string.Format(s, parameters);
        }

        public static string RegexReplace(this string s, string oldValue, string newValue, RegexOptions options)
        {
            return Regex.Replace(s, Regex.Escape(oldValue), newValue.Replace("$", "$$"), options);
        }

        public static string StripHtml(this string s)
        {
            return Regex.Replace(s, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", string.Empty);
        }

        public static bool IsNumeric(this string s)
        {
            long dummy;
            return long.TryParse(s, out dummy);
        }
    }
}
