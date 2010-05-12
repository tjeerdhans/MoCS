using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    /// <summary>
    /// Indicates the participation of a team in a tournament-assignment.
    /// </summary>
    [DataContract(Namespace = "")]
    public class AssignmentEnrollmentContract
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int TeamId { get; set; }
        [DataMember]
        public int TournamentAssignmentId { get; set; }
        [DataMember]
        public int TournamentId { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }

    [CollectionDataContract(Name = "AssignmentEnrollmentsContract", Namespace = "")]
    public class AssignmentEnrollmentsContract : List<AssignmentEnrollmentContract> { }
}
