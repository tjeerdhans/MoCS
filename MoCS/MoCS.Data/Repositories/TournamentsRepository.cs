using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoCS.Data.Entity;

namespace MoCS.Data.Repositories
{
    public class TournamentsRepository : GenericRepository<Tournament>
    {
        public TournamentsRepository(DbContext context) : base(context) { }
    }
}
