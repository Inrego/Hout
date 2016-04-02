using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.Specifications
{
    public class CommandSpecification : INameDesc
    {
        public CommandSpecification()
        {
            ParameterSpecifications = new NameDescCollection<ParameterSpecification>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public NameDescCollection<ParameterSpecification> ParameterSpecifications { get; set; }
    }
}
