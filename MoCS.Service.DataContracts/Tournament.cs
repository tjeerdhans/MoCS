using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    [DataContract(Namespace = "")]
    public class Tournament
    {
        [DataMember]
        public string TournamentId { get; set; }
        [DataMember]
        public string TournamentName { get; set; }
        [DataMember]
        public bool Current { get; set; }
    }

    [CollectionDataContract(Name = "Tournaments", Namespace = "")]
    public class Tournaments : List<Tournament> { }
}
