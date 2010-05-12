using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace MoCS.WebClient.Models
{
    public class SubmitModel
    {
        public DateTime SubmitDate { get; set; }
        public string Result { get; set; }
        public string FileURL { get; set; }
        public string ResultDetailsURL { get; set; }
    }
}
