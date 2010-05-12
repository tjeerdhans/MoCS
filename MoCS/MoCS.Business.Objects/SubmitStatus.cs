using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business.Objects
{
    public enum SubmitStatus
    {
        Success = 0,
        Submitted = 1,
        Processing = 2,
        NullSubmit = 3,
        ErrorCompilation = 20,
        ErrorValidation = 21,
        ErrorTesting = 22,
        ErrorServer = 23,
        ErrorUnknown = 90,//90,99,else
    }
}
