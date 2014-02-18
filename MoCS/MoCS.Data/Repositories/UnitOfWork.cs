using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoCS.Data.Entity;

namespace MoCS.Data.Repositories
{
    public class UnitOfWork
    {
        private readonly MoCSContainer _context = new MoCSContainer();



        private TeamsRepository _teamsRepository;

        public TeamsRepository TeamsRepository
        {
            get { return _teamsRepository ?? (_teamsRepository = new TeamsRepository(_context)); }
        }


        public void Save()
        {
            _context.SaveChanges();
        }
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
