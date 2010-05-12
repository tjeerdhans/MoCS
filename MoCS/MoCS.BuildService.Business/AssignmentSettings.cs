using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business
{
    public class AssignmentSettings
    {
        public string AssignmentId {get;set;}
        public string ClassnameToImplement {get;set;}
        public string InterfaceNameToImplement { get; set; }
        public string NUnitClassClient { get; set; }
        public string NUnitClassServer { get; set; }

    }
}
