using System;

namespace CodeGenerator.Core
{
    public class ForeignKey
    {
        public string Model { get; set; }
        public string Property { get; set; }
        public string OnDelete { get; set; }
    }
}
