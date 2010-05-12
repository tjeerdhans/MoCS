using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business
{

        public enum SubmitStatusCode
        {
            Unknown,
            CompilationError,
            ValidationError,
            TestError,
            ServerError,
            Success
        }

}
