using System;
using Hout.Models.ParamValidation;

namespace Hout.Models.Specifications
{
    public class ParameterSpecification : INameDesc
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public Type Type { get; set; }
        public IParamValidator Validator { get; set; }

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
