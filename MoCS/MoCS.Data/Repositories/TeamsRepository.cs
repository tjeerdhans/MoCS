using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoCS.Data.Entity;

namespace MoCS.Data.Repositories
{
    public class TeamsRepository : GenericRepository<Team>
    {
        public TeamsRepository(DbContext context) : base(context) { }
    }
}
