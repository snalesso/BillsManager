using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Billy
{
    public static class StringExtensions
    {
        public static string NullIfEmpty(this string s)
        {
            if (s == null || s.Length <= 0)
            {
                return null;
            }

            return s;
        }

        public static string TrimToNull(this string s)
        {
            if (s == null)
            {
                return null;
            }

            s = s.Trim();

            if (s.Length <= 0)
            {
                return null;
            }

            return s;
        }

        public static string SurroundedBy(this string s, string prefix, string suffix)
        {
            if (s == null)
                return null;

            return prefix + s + suffix;
        }
        public static string SurroundedBy(this string s, char prefix, char suffix)
        {
            if (s == null)
                return null;

            return prefix + s + suffix;
        }
        public static string SurroundedBy(this string s, char prefix, string suffix)
        {
            if (s == null)
                return null;

            return prefix + s + suffix;
        }
        public static string SurroundedBy(this string s, string prefix, char suffix)
        {
            if (s == null)
                return null;

            return prefix + s + suffix;
        }

        public static string Join(string separator, params string[] strings)
        {
            return string.Join(separator, strings.Where(x => x != null));
        }
    }
}
