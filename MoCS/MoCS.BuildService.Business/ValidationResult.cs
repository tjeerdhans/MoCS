using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business
{
    public class ValidationResult
    {
        public SubmitStatusCode Status { get; set; }
        public List<string> Messages { get; private set; }

        public ValidationResult()
        {
            Status = SubmitStatusCode.Unknown;
            Messages = new List<string>();
        }
    }
}
