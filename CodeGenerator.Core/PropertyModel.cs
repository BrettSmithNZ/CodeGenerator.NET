using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core
{
    public class PropertyModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool PrimaryKey { get; set; }
        public bool ReadOnly { get; set; }
        public bool Filterable { get; set; }
        public bool Nullable { get; set; }
        public IEnumerable<ForeignKey> ForeignKeys { get; set; }
    }
}
