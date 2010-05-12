using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Business.Entities
{
    public class Team
    {
        public int ID { get; set; }
        public string TeamName { get; set; }
        public string TeamMembers { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsAdmin { get; set; }
        public int Points { get; set; }
        public string Password { get; set; }
    }
}
