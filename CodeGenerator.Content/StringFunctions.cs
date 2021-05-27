using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGenerator.Content
{
    public static class StringFunctions
    {
        public static string Singular(this string content)
        {
            content = Regex.Replace(content, "ies$", "y", RegexOptions.Compiled);
            return Regex.Replace(content, "s$", string.Empty, RegexOptions.Compiled);
        }
        public static string Plural(this string content)
        {
            content = Regex.Replace(content, "y$", "ies", RegexOptions.Compiled);
            return Regex.Replace(content, "([^y])$", "$1s", RegexOptions.Compiled);
        }
        public static string PascalCase(this string content)
        {
            return $"{content.Substring(0, 1).ToUpper()}{content.Substring(1)}";
        }
        public static string CamelCase(this string content)
        {
            return $"{content.Substring(0, 1).ToLower()}{content.Substring(1)}";
        }
    }
}
