using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoCS.Data.Entity;

namespace MoCS.Data.Repositories
{
    public class AssignmentsRepository : GenericRepository<Assignment>
    {
        public AssignmentsRepository(DbContext context) : base(context) { }
    }
}
