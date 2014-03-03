using System.Linq;
using MoCS.Data.Entity;
using System.Data.Entity;

namespace MoCS.Data.Repositories
{
    public class AssignmentEnrollmentsRepository : GenericRepository<AssignmentEnrollment>
    {
        public AssignmentEnrollmentsRepository(DbContext context) : base(context) { }

        /// <summary>
        /// Delete all of the assignmentenrollments in
        /// the given tournament
        /// </summary>
        /// <param name="tournament">a tournament</param>
        public void DeleteAllForTournament(Tournament tournament)
        {
            var assignmentEnrollments = tournament
                .TournamentAssignments
                .SelectMany(ta => ta.AssignmentEnrollments).ToList();
            DeleteRange(assignmentEnrollments);
        }
    }
}
