using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business.Facade
{
    public class MoCSException : ApplicationException
    {
        public MoCSException(string message) : base(message) { }
    }
}
