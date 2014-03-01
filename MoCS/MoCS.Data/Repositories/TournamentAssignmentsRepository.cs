using MoCS.Data.Entity;
using System.Data.Entity;

namespace MoCS.Data.Repositories
{
    public class TournamentAssignmentsRepository : GenericRepository<TournamentAssignment>
    {
        public TournamentAssignmentsRepository(DbContext context) : base(context) { }
    }
}
