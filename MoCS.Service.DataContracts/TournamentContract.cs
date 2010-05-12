using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MoCS.Service.DataContracts
{
    [DataContract(Namespace = "")]
    public class TournamentContract
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime CreateDate { get; set; }
   }

    [CollectionDataContract(Name = "TournamentsContract", Namespace = "")]
    public class TournamentsContract : List<TournamentContract> { }
}
