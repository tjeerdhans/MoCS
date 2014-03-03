using System.Linq;
using MoCS.Data.Entity;
using System.Data.Entity;

namespace MoCS.Data.Repositories
{
    public class SubmitsRepository : GenericRepository<Submit>
    {
        public SubmitsRepository(DbContext context) : base(context) { }

        /// <summary>
        /// Delete all of the submits in
        /// the given tournament
        /// </summary>
        /// <param name="tournament">a tournament</param>
        public void DeleteAllForTournament(Tournament tournament)
        {
            var submits = tournament
                .TournamentAssignments
                .SelectMany(ta => ta.AssignmentEnrollments)
                .SelectMany(ae => ae.Submits).ToList();
            DeleteRange(submits);
        }
    }
}
