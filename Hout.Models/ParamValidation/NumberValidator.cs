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
            _minValue = minValue;
            _maxValue = maxValue;
        }
        private readonly int _minValue;
        private readonly int _maxValue;
        public void ValidateValue(object val)
        {
            if (!(val is int))
            {
                throw new InvalidParameterException("The value is not a whole number.");
            }
            var number = (int) val;
            if (number < _minValue)
                throw new InvalidParameterException($"The value cannot be lower than {_minValue}");
            if (number > _maxValue)
                throw new InvalidParameterException($"The value cannot be larger than {_maxValue}");
        }
    }
}
