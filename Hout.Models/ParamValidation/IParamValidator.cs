using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.ParamValidation
{
    public interface IParamValidator
    {
        void ValidateValue(object val);
    }
}
