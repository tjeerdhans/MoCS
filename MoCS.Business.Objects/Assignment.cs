using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business.Objects
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string Tagline { get; set; }
        public DateTime CreateDate { get; set; }
        public int Version { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public int Difficulty { get; set; }
        public string ClassNameToImplement { get; set; }
        public string InterfaceNameToImplement { get; set; }
        public string Path { get; set; }
        public List<AssignmentFile> AssignmentFiles { get; set; }
        //public string ZipFileName { get; set; }
        //public string CaseFileName { get; set; }
        //public string InterfaceFileName { get; set; }
        //public string ClassFileName { get; set; }
        //public string UnitTestClientFileName { get; set; }
        //public string UnitTestServerFileName { get; set; }
        //public string ExtraFileNames { get; set; }
        public List<TournamentAssignment> TournamentAssignmentList { get; set; }
    }
}
