using System;

namespace Hout.Models.Specifications
{
    public class PropertySpecification : INameDesc
    {
        public bool ReadOnly { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public Type Type { get; set; }
        public bool Hidden { get; set; }
    }
}
