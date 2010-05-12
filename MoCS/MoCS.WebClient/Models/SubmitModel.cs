using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace MoCS.WebClient.Models
{
    public class SubmitModel
    {
        public int Id { get; set; }
        public DateTime SubmitDate { get; set; }
        public string Result { get; set; }
        public string TimeTaken { get; set; }
        public string ProcessingDetails { get; set; }
        public string FileContents { get; set; }
    }
}
