using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business
{
    public class SubmitSettings
    {
        public string TeamId{get;set;}
        public string BasePath{get;set;}
        public DateTime TimeStamp { get; set; }
        public string AssignmentId { get; set; }


        public string GetDllName()
        {
            string name = TeamId + "_" + AssignmentId + "_";

            string date = Utils.DateToString(TimeStamp);

            string time = Utils.TimeToString(TimeStamp);

            name += date + "_" + time;

            return name + ".dll"; 
        }
    }
}
