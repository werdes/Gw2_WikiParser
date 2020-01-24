using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
