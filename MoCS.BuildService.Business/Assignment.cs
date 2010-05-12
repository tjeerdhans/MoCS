using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business
{
    public class Assignment
    {
        public string InterfaceFile {get;set;}
        public string NunitTestFileServer {get;set;}
        public string NunitTestFileClient { get; set; }
        public string ClassNameToImplement {get;set;}
        public string InterfaceNameToImplement { get; set; }
        public List<string> ServerFilesToCopy { get; set; }

        public Assignment()
        {
            ServerFilesToCopy = new List<string>();
        }
    }
}
