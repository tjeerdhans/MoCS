using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MoCS.WebClient.Models
{
    public class HomeModel
    {
        public string TeamName { get; set; }
        public List<EnrollmentModel> EnrollmentList { get; set; }
        public MembersModel MembersModel { get; set; }
    }

    public class EnrollmentModel
    {
        public string TournamentName { get; set; }
        public string AssignmentName { get; set; }
        public int AssignmentEnrollmentId { get; set; }
    }

    public class MembersModel
    {        
        [DataType(DataType.MultilineText)]
        [DisplayName("Team members:")]
        public string Members { get; set; }
    }
}