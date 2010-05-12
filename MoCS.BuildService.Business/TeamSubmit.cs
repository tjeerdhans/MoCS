using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MoCS.BuildService.Business
{
    public class TeamSubmit
    {
        public string TeamId {get;set;}
        public string TeamName{get;set;}
        public string AssignmentId { get; set; }
        public FileStream FileStream {get;set;}
        public string FileName{get;set;}
    }
}
