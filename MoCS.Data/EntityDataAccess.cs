using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoCS.Business.Objects;
using MoCS.Business.Objects.Interfaces;
using e = MoCS.Data.Entity;

namespace MoCS.Data
{
    class EntityDataAccess : IDataAccess
    {
        private e.MoCSModelContainer _m;

        public EntityDataAccess()
        {
            _m = new e.MoCSModelContainer();
        }

        public List<Team> GetTeams()
        {
            List<Team> result = new List<Team>();

            var query = from t in _m.Team
                        select t; // CreateTeamFromEntity(t);

            foreach (var t in query.ToList())
            {
                result.Add(CreateTeamFromEntity(t));
            }

            return result;
            //return query.ToList();
        }

        public Team GetTeamByName(string teamName)
        {            
            var query = from t in _m.Team
                        where t.Name == teamName
                        select t; // CreateTeamFromEntity(t);
                        //select new Team()
                        //{
                        //    Id = t.Id,
                        //    CreateDate = t.CreateDate,
                        //    IsAdmin = t.IsAdmin,
                        //    Name = t.Name,
                        //    Members = t.Members,
                        //    Password = t.Password,
                        //    Score = t.Score
                        //};

            if (query.Count() > 0)
            {
                return CreateTeamFromEntity(query.First());
                //return query.First();
            }
            return null;
        }

        public Team GetTeamById(int teamId)
        {
            var query = from t in _m.Team
                        where t.Id == teamId
                        select t; // CreateTeamFromEntity(t);

            if (query.Count() > 0)
            {
                return CreateTeamFromEntity(query.First());
                //return query.First();
            }
            return null;
        }

        public Team SaveTeam(Team team)
        {
            e.Team toSave = new e.Team()
            {
                IsAdmin = team.IsAdmin,
                Members = team.Members,
                Name = team.Name,
                Password = team.Password,
                Score = team.Score,
                CreateDate = DateTime.Now
            };

            _m.AddToTeam(toSave);
            _m.SaveChanges();

            team.Id = toSave.Id;

            return team;
        }

        public Team UpdateTeam(Team team)
        {
            e.Team toUpdate = (from t in _m.Team
                               where t.Id == team.Id
                               select t).First();

            toUpdate.Members = team.Members;
            toUpdate.Name = team.Name;
            toUpdate.Password = team.Password;
            toUpdate.IsAdmin = team.IsAdmin;
            toUpdate.Score = team.Score;

            _m.SaveChanges();

            return team;
        }

        public List<TournamentAssignment> GetTournamentAssignmentsForTournament(int tournamentId)
        {
            List<TournamentAssignment> result = new List<TournamentAssignment>();

            var query = from t in _m.TournamentAssignment.Include("Tournament").Include("Assignment")
                        where t.Tournament.Id == tournamentId
                        select t; // CreateTournamentAssignmentFromEntity(t);

            foreach (var t in query.ToList())
            {
                result.Add(CreateTournamentAssignmentFromEntity(t));
            }
            return result;
            //return query.ToList();
        }

        public Tournament GetTournamentById(int tournamentId)
        {
            var query = from t in _m.Tournament
                        where t.Id == tournamentId
                        select t; // CreateTournamentFromEntity(t);

            if (query.Count() > 0)
            {
                return CreateTournamentFromEntity(query.First());
            }
            return null;
        }

        public List<Tournament> GetTournaments()
        {
            List<Tournament> result = new List<Tournament>();

            var query = from t in _m.Tournament
                        select t; // CreateTournamentFromEntity(t);

            foreach (var t in query.ToList())
            {
                result.Add(CreateTournamentFromEntity(t));
            }

            return result;
            //return query.ToList();
        }

        public List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeamForTournament(int tournamentId, int teamId)
        {
            List<AssignmentEnrollment> result = new List<AssignmentEnrollment>();

            var query = from a in _m.AssignmentEnrollment.Include("Team")
                        where a.TournamentAssignment.Tournament.Id == tournamentId &&
                        a.Team.Id == teamId
                        select a; // CreateAssignmentEnrollmentFromEntity(a);

            foreach (var a in query.ToList())
            {
                result.Add(CreateAssignmentEnrollmentFromEntity(a));
            }

            return result;
            //return query.ToList();
        }

        public TournamentAssignment GetTournamentAssignmentById(int tournamentAssignmentId)
        {
            var query = from t in _m.TournamentAssignment.Include("Tournament").Include("Assignment")
                        where t.Id == tournamentAssignmentId
                        select t; // CreateTournamentAssignmentFromEntity(t);

            if (query.Count() > 0)
            {
                return CreateTournamentAssignmentFromEntity(query.First());
            }
            return null;
        }

        public AssignmentEnrollment GetAssignmentEnrollmentById(int assignmentEnrollmentId)
        {
            var query = from a in _m.AssignmentEnrollment.Include("Team")
                        where a.Id == assignmentEnrollmentId
                        select a; // CreateAssignmentEnrollmentFromEntity(a);

            if (query.Count() > 0)
            {
                return CreateAssignmentEnrollmentFromEntity(query.First());
            }
            return null;
        }

        public AssignmentEnrollment SaveAssignmentEnrollment(AssignmentEnrollment assignmentEnrollment)
        {
            throw new NotImplementedException();
        }

        #region Translation helpers

        private static Assignment CreateAssignmentFromEntity(e.Assignment eAssignment)
        {
            Assignment result = new Assignment()
            {
                Id = eAssignment.Id,
                Name = eAssignment.Name,
                FriendlyName = eAssignment.FriendlyName,
                Tagline = eAssignment.Tagline,
                CreateDate = eAssignment.CreateDate
            };

            return result;
        }

        private static Team CreateTeamFromEntity(e.Team eTeam)
        {
            Team result = new Team()
            {
                Id = eTeam.Id,
                CreateDate = eTeam.CreateDate,
                IsAdmin = eTeam.IsAdmin,
                Name = eTeam.Name,
                Members = eTeam.Members,
                Password = eTeam.Password,
                Score = eTeam.Score
            };

            return result;
        }

        private static TournamentAssignment CreateTournamentAssignmentFromEntity(e.TournamentAssignment eTournamentAssignment)
        {
            TournamentAssignment result = new TournamentAssignment()
            {
                Id = eTournamentAssignment.Id,
                AssignmentOrder = eTournamentAssignment.AssignmentOrder,
                IsActive = eTournamentAssignment.IsActive,
                CreateDate = eTournamentAssignment.CreateDate,
                Points1 = eTournamentAssignment.Points1,
                Points2 = eTournamentAssignment.Points2,
                Points3 = eTournamentAssignment.Points3,
                Assignment = CreateAssignmentFromEntity(eTournamentAssignment.Assignment),
                Tournament = CreateTournamentFromEntity(eTournamentAssignment.Tournament)
            };

            return result;
        }

        private static Tournament CreateTournamentFromEntity(e.Tournament eTournament)
        {
            Tournament result = new Tournament()
            {
                CreateDate = eTournament.CreateDate,
                Id = eTournament.Id,
                Name = eTournament.Name
            };

            return result;
        }

        private static AssignmentEnrollment CreateAssignmentEnrollmentFromEntity(e.AssignmentEnrollment eAE)
        {
            AssignmentEnrollment result = new AssignmentEnrollment()
            {
                Id = eAE.Id,
                IsActive = eAE.IsActive,
                StartDate = eAE.StartDate,
                Team = CreateTeamFromEntity(eAE.Team),
                TournamentAssignment = CreateTournamentAssignmentFromEntity(eAE.TournamentAssignment)
            };

            return result;
        }

        private static Submit CreateSubmitFromEntity(e.Submit eSubmit)
        {
            Submit result = new Submit()
            {
                Id = eSubmit.Id,
                FileName = eSubmit.FileName,
                FileContents = eSubmit.FileContents,
                Data = eSubmit.Data,
                IsProcessed = eSubmit.IsProcessed,
                Status = eSubmit.Status,
                SubmitDate = eSubmit.SubmitDate,
                Team = CreateTeamFromEntity(eSubmit.Team),
                ProcessingDetails = eSubmit.ProcessingDetails,
                SecondsSinceEnrollment = eSubmit.SecondsSinceEnrollment,
                AssignmentEnrollment = CreateAssignmentEnrollmentFromEntity(eSubmit.AssignmentEnrollment),
                TournamentAssignment = CreateTournamentAssignmentFromEntity(eSubmit.TournamentAssignment)
            };

            return result;
        }

        #endregion
    }
}
