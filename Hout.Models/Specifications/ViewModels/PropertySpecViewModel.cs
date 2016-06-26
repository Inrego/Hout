using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hout.Models.ParamValidation;

namespace Hout.Models.Specifications.ViewModels
{
    public class PropertySpecViewModel
    {
        public PropertySpecViewModel(PropertySpecification propertySpec)
        {
            PropertySpecification = propertySpec;
            CustomElement = propertySpec.CustomElement ?? GetDefaultElementForType();
        }
        public PropertySpecification PropertySpecification { get; set; }
        public CustomElement CustomElement { get; set; }

        private CustomElement GetDefaultElementForType()
        {
            if (PropertySpecification.Validator != null && PropertySpecification.Validator.GetType() == typeof(NumberValidator))
                return new CustomElement {Name = "hout-property-slider" };
            if (PropertySpecification.Type == typeof (string))
                return new CustomElement {Name = "hout-property-string" };
            if (PropertySpecification.Type == typeof(sbyte))
                return new CustomElement { Name = "hout-property-string" };
            return null;
        }
    }
}
