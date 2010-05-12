using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Members { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsAdmin { get; set; }
        public int Score { get; set; }
        
    }
}
;