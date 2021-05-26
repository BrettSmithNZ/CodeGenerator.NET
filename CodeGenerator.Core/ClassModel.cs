using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.Core
{
    public class ClassModel
    {
        public string Model { get; set; }
        public IEnumerable<PropertyModel> Properties { get; set; }
    }
}
