using MoCS.Data.Entity;
using System;

namespace MoCS.Data.Repositories
{
    public class UnitOfWork
    {
        private readonly MoCSContainer _context = new MoCSContainer();

        private TeamsRepository _teamsRepository;
        private TournamentsRepository _tournamentsRepository;
        private AssignmentsRepository _assignmentsRepository;
        private TournamentAssignmentsRepository _tournamentAssignmentsRepository;
        private AssignmentEnrollmentsRepository _assignmentEnrollmentRepository;
        private SubmitsRepository _submitsRepository;

        public TeamsRepository TeamsRepository
        {
            get { return _teamsRepository ?? (_teamsRepository = new TeamsRepository(_context)); }
        }

        public TournamentsRepository TournamentsRepository
        {
            get { return _tournamentsRepository ?? (_tournamentsRepository = new TournamentsRepository(_context)); }
        }
        public AssignmentsRepository AssignmentsRepository
        {
            get { return _assignmentsRepository ?? (_assignmentsRepository = new AssignmentsRepository(_context)); }
        }
        public TournamentAssignmentsRepository TournamentAssignmentsRepository
        {
            get { return _tournamentAssignmentsRepository ?? (_tournamentAssignmentsRepository = new TournamentAssignmentsRepository(_context)); }
        }
        public AssignmentEnrollmentsRepository AssignmentEnrollmentRepository
        {
            get { return _assignmentEnrollmentRepository ?? (_assignmentEnrollmentRepository = new AssignmentEnrollmentsRepository(_context)); }
        }
        public SubmitsRepository SubmitsRepository
        {
            get { return _submitsRepository ?? (_submitsRepository = new SubmitsRepository(_context)); }
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        private bool _disposed;

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
