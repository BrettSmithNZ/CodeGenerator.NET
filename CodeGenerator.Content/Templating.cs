using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CodeGenerator.Core;

namespace CodeGenerator.Content
{
    public static class Templating
    {
        public static string Globals(this string content, string nameSpace, string typeName)
        {
            return content
                    .Replace("%nameSpace%", nameSpace, StringComparison.InvariantCultureIgnoreCase)
                    .Replace("%typeName%", typeName, StringComparison.InvariantCultureIgnoreCase);
        }
        public static string Model(this string content, ClassModel classModel)
        {
            return content
                    .Replace("%Models%", classModel.Model.PascalCase().Plural())
                    .Replace("%Model%", classModel.Model.PascalCase())
                    .Replace("%models%", classModel.Model.CamelCase().Plural())
                    .Replace("%model%", classModel.Model.CamelCase());
        }
        public static string Content(this string content, ClassModel classModel)
        {
            return content
                    .Model(classModel)
                    .Properties("allProperties", classModel.Properties)
                    .Properties("keyProperties", classModel.Properties.Where(o => o.PrimaryKey))
                    .Properties("filterableProperties", classModel.Properties.Where(o => o.Filterable))
                    .Properties("editableProperties", classModel.Properties.Where(o => !o.ReadOnly));
        }
        public static string Properties(this string content, string identifier, IEnumerable<PropertyModel> properties)
        {
            string pattern = $"%[\\s]*{identifier}[\\s]*\\|[\\s]*\"(.+)\"[\\s]*\\|[\\s]*\"(.+)\"[\\s]*%";
            return Regex.Replace(content,
                    pattern,
                    delegate (Match match)
              {
                  string separator = match.Groups[1].Value;
                  return string.Join(separator.Replace("\\n", "\n").Replace("\\t", "\t"),
                      properties.Select(property =>
                  {
                      string result = match.Groups[2].Value;
                      return result
                            .Replace("%type%", property.Nullable ? $"{property.Type}?" : property.Type)
                            .Replace("%Names%", property.Name.PascalCase().Plural())
                            .Replace("%Name%", property.Name.PascalCase())
                            .Replace("%names%", property.Name.CamelCase().Plural())
                            .Replace("%name%", property.Name.CamelCase());
                  }));
              }, RegexOptions.Compiled);
        }

    }
}
