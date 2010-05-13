using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business
{
    public class SystemSettings
    {
        public string CscPath { get; set; }
        public string NunitAssemblyPath { get; set; }
        public string NunitConsolePath { get; set; }
        public string AssignmentsBasePath { get; set; }
        public int NunitTimeOut { get; set; }
        public string BaseResultPath { get; set; }
    }
}
