using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.ParamValidation
{
    public class ValidationResult
    {
        public ValidationResult(string paramName, bool success = true, string message = null)
        {
            ParameterName = paramName;
            Success = success;
            Message = message;
        }
        public string ParameterName { get; private set; }
        public bool Success { get; private set; }
        public string Message { get; private set; }
    }
}
