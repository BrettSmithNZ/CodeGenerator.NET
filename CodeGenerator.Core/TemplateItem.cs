using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core
{
    public class TemplateItem
    {
        public string Name { get; set; }
        public bool? Consolidated { get; set; }
        public bool? UseProjectDirectory { get; set; }
        public IEnumerable<TemplatePart> Parts { get; set; }
        public string FileName { get; set; } = null;
    }
}
