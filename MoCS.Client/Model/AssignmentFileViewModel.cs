using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Model
{
    public class AssignmentFileViewModel : ViewModel
    {
        public string Name {get;set;}
        public byte[] Contents{get;set;}
    }
}
