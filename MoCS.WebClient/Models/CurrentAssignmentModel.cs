using System.ComponentModel;
using System.Collections.Generic;
using System;

namespace MoCS.WebClient.Models
{
    public class CurrentAssignmentModel
    {
        //Assignment label
        public string AssignmentName { get; set; }
        public string AssignmentTagline { get; set; }
        public string AssignmentCategory { get; set; }
        public int AssignmentDifficulty { get; set; }
        public DateTime AssignmentEnrollmentTime { get; set; }
        public string DownloadURL { get; set; }

        //Tab control
        public List<TabContentModel> TabContentModelList { get; set; }

        // Submits list
        public List<SubmitModel> SubmitModelList { get; set; }

    }
}
