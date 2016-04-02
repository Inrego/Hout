using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models
{
    public class CommandResponse
    {
        public CommandResponse(bool success = true, string message = "")
        {
            Success = success;
            Message = message;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
