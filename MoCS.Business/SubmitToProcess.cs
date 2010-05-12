using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MoCS.Business
{
    public class SubmitToProcess
    {
        public int SubmitID { get; set; }
        public int AssignmentID{get;set;}
        public string AssignmentName{get;set;}
        public string FileName { get; set; }
        public Stream FileStream { get; set; }
        public int TeamID { get; set; }
        public string TeamName {get;set;}
        public DateTime SubmitDate { get; set; }
    }
}