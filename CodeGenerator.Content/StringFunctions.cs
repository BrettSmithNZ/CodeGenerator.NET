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
    }
}
