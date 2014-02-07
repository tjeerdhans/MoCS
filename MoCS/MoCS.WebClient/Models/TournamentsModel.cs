using System.Collections.Generic;

namespace MoCS.WebClient.Models
{
    public class TournamentsModel : List<TournamentModel> { }

    public class TournamentModel
    {
        public int Id { get; set; }
        
        //[DisplayName("Name")]
        public string Name { get; set; }
    } 
}