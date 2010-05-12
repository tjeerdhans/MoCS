using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business.Objects
{
    /// <summary>
    /// Tournament which ca contain several assignment
    /// </summary>
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public List<TournamentAssignment> TournamentAssignmentList { get; set; }
    }
}
