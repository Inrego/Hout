using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.Specifications.ViewModels
{
    public class PropertySpecViewModel
    {
        public PropertySpecViewModel(PropertySpecification propertySpec)
        {
            PropertySpecification = propertySpec;
            if (propertySpec.Type == typeof (string))
            {
                ElementPath = "/elements/hout-property/hout-property-string.html";
            }
        }
        public PropertySpecification PropertySpecification { get; set; }
        public string ElementPath { get; set; }
    }
}
