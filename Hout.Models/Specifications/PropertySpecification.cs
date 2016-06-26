using System;
using Hout.Models.ParamValidation;
using Newtonsoft.Json;

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
        public IParamValidator Validator { get; set; }
        [JsonIgnore]
        public CustomElement CustomElement { get; set; }

        public virtual ValidationResult ValidateValue(object value)
        {
            if (Validator == null)
                return new ValidationResult(Name);
            try
            {
                Validator.ValidateValue(value);
                return new ValidationResult(Name);
            }
            catch (Exception e)
            {
                return new ValidationResult(Name, false, e.Message);
            }
        }
    }
}
