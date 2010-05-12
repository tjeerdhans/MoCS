using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Business.Entities
{
    public enum SubmitStatus
    {
        Submitted,//0
        Success,//1
        ErrorCompilation,//20
        ErrorValidation,//21
        ErrorTesting,//22
        ErrorServer,//23
        ErrorUnknown,//90,99,else
    }
}
