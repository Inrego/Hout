using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.ParamValidation
{
    public class NumberValidator : IParamValidator
    {
        public NumberValidator(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
        public int MinValue;
        public int MaxValue;
        public void ValidateValue(object val)
        {
            if (!(val is int))
            {
                throw new InvalidParameterException("The value is not a whole number.");
            }
            var number = (int) val;
            if (number < MinValue)
                throw new InvalidParameterException($"The value cannot be lower than {MinValue}");
            if (number > MaxValue)
                throw new InvalidParameterException($"The value cannot be larger than {MaxValue}");
        }
    }
}
