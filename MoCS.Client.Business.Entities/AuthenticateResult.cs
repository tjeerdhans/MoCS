using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Business.Entities
{
    public class AuthenticateResult
    {
        public int TeamId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
