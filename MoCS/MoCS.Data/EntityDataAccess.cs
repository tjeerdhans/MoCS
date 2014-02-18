//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MoCS.Business.Objects;
//using MoCS.Business.Objects.Interfaces;
//using e = MoCS.Data.Entity;

//namespace MoCS.Data
//{
//    class EntityDataAccess : IDataAccess
//    {
//        private e.MoCSModelContainer _m;

//        public EntityDataAccess()
//        {
//            _m = new e.MoCSModelContainer();
//            _m.Submit.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
//            _m.AssignmentEnrollment.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
//            _m.Assignment.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
//            _m.Tournament.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
//            _m.TournamentAssignment.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
//            _m.Team.MergeOption = System.Data.Objects.MergeOption.OverwriteChanges;
//        }

//        public List<Submit> GetUnprocessedSubmits()
//        {
//            List<Submit> result = new List<Submit>();
//            string submitted = SubmitStatus.Submitted.ToString();
//            var query = from s in _m.Submit.Include("Team").Include("TournamentAssignment.Assignment")
//                        where s.Status == submitted
//                        select s;

//            foreach (var s in query.ToList())
//            {
//                result.Add(CreateSubmitFromEntity(s));
//            }
//            return result;
//        }

//        public void UpdateSubmitStatusDetails(int submitId, SubmitStatus newStatus, string details, DateTime statusDate)
//        {
//            e.Submit toUpdate = (from s in _m.Submit
//                                 where s.Id == submitId
//                                 select s).First();

//            toUpdate.Status = newStatus.ToString();
//            toUpdate.ProcessingDetails = details;
//            toUpdate.StatusDate = statusDate;

//            _m.SaveChanges();
//            _m.Refresh(System.Data.Objects.RefreshMode.StoreWins, toUpdate);
//        }


//        public List<Team> GetTeams()
//        {
//            List<Team> result = new List<Team>();

//            var query = from t in _m.Team
//                        select t; // CreateTeamFromEntity(t);

//            foreach (var t in query.ToList())
//            {
//                result.Add(CreateTeamFromEntity(t));
//            }

//            return result;
//            //return query.ToList();
//        }

//        public Team GetTeamByName(string teamName)
//        {
//            var query = from t in _m.Team
//                        where t.Name == teamName
//                        select t; // CreateTeamFromEntity(t);
//            //select new Team()
//            //{
//            //    Id = t.Id,
//            //    CreateDate = t.CreateDate,
//            //    IsAdmin = t.IsAdmin,
//            //    Name = t.Name,
//            //    Members = t.Members,
//            //    Password = t.Password,
//            //    Score = t.Score
//            //};

//            if (query.Count() > 0)
//            {
//                return CreateTeamFromEntity(query.First());
//                //return query.First();
//            }
//            return null;
//        }

//        public Team GetTeamById(int teamId)
//        {
//            var query = from t in _m.Team
//                        where t.Id == teamId
//                        select t; // CreateTeamFromEntity(t);

//            if (query.Count() > 0)
//            {
//                return CreateTeamFromEntity(query.First());
//                //return query.First();
//            }
//            return null;
//        }

//        public Team SaveTeam(Team team)
//        {
//            e.Team toSave = new e.Team()
//            {
//                IsAdmin = team.IsAdmin,
//                Members = team.Members,
//                Name = team.Name,
//                Password = team.Password,
//                Score = team.Score,
//                CreateDate = DateTime.Now
//            };

//            _m.AddToTeam(toSave);
//            _m.SaveChanges();
//            // _m.Refresh(System.Data.Objects.RefreshMode.StoreWins, toSave);

//            team.Id = toSave.Id;

//            return team;
//        }

//        public Team UpdateTeam(Team team)
//        {
//            e.Team toUpdate = (from t in _m.Team
//                               where t.Id == team.Id
//                               select t).First();

//            toUpdate.Members = team.Members;
//            toUpdate.Name = team.Name;
//            toUpdate.Password = team.Password;
//            toUpdate.IsAdmin = team.IsAdmin;
//            toUpdate.Score = team.Score;


//            _m.SaveChanges();
//            _m.Refresh(System.Data.Objects.RefreshMode.StoreWins, toUpdate);

//            return team;
//        }

//        public List<AssignmentEnrollment> GetAssignmentEnrollmentsForTournamentAssignment(int tournamentAssignmentId)
//        {
//            List<AssignmentEnrollment> result = new List<AssignmentEnrollment>();

//            var query = from ae in _m.AssignmentEnrollment.Include("TournamentAssignment").Include("Team")
//                        where ae.TournamentAssignment.Id == tournamentAssignmentId
//                        select ae;

//            foreach (var ae in query.ToList())
//            {
//                result.Add(CreateAssignmentEnrollmentFromEntity(ae));
//            }

//            return result;
//        }

//        public List<TournamentAssignment> GetTournamentAssignmentsForTournament(int tournamentId)
//        {
//            List<TournamentAssignment> result = new List<TournamentAssignment>();

//            var query = from t in _m.TournamentAssignment.Include("Tournament").Include("Assignment")
//                        where t.Tournament.Id == tournamentId
//                        orderby t.AssignmentOrder
//                        select t; // CreateTournamentAssignmentFromEntity(t);

//            foreach (var t in query.ToList())
//            {
//                result.Add(CreateTournamentAssignmentFromEntity(t));
//            }
//            return result;
//            //return query.ToList();
//        }

//        public Tournament GetTournamentById(int tournamentId)
//        {
//            var query = from t in _m.Tournament
//                        where t.Id == tournamentId
//                        select t; // CreateTournamentFromEntity(t);

//            if (query.Count() > 0)
//            {
//                return CreateTournamentFromEntity(query.First());
//            }
//            return null;
//        }

//        public List<Tournament> GetTournaments()
//        {
//            List<Tournament> result = new List<Tournament>();

//            var query = from t in _m.Tournament
//                        select t; // CreateTournamentFromEntity(t);

//            foreach (var t in query.ToList())
//            {
//                result.Add(CreateTournamentFromEntity(t));
//            }

//            return result;
//            //return query.ToList();
//        }

//        public List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeam(int teamId)
//        {
//            List<AssignmentEnrollment> result = new List<AssignmentEnrollment>();

//            var query = from a in _m.AssignmentEnrollment.Include("Team").Include("TournamentAssignment.Tournament").Include("TournamentAssignment.Assignment")
//                        where a.Team.Id == teamId
//                        select a;

//            foreach (var ae in query.ToList())
//            {
//                result.Add(CreateAssignmentEnrollmentFromEntity(ae));
//            }

//            return result;
//        }

//        public List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeamForTournamentAssignment(int tournamentAssignmentId, int teamId)
//        {
//            List<AssignmentEnrollment> result = new List<AssignmentEnrollment>();

//            var query = from a in _m.AssignmentEnrollment.Include("Team")
//                        where a.TournamentAssignment.Id == tournamentAssignmentId &&
//                        a.Team.Id == teamId
//                        select a; // CreateAssignmentEnrollmentFromEntity(a);

//            foreach (var a in query.ToList())
//            {
//                result.Add(CreateAssignmentEnrollmentFromEntity(a));
//            }

//            return result;
//            //return query.ToList();
//        }

//        public Assignment GetAssignmentById(int assignmentId)
//        {
//            var query = from a in _m.Assignment
//                        where a.Id == assignmentId
//                        select a;

//            if (query.Count() > 0)
//            {
//                return CreateAssignmentFromEntity(query.First());
//            }
//            return null;
//        }

//        public TournamentAssignment GetTournamentAssignmentById(int tournamentAssignmentId)
//        {
//            var query = from t in _m.TournamentAssignment.Include("Tournament").Include("Assignment")
//                        where t.Id == tournamentAssignmentId
//                        select t; // CreateTournamentAssignmentFromEntity(t);

//            if (query.Count() > 0)
//            {
//                return CreateTournamentAssignmentFromEntity(query.First());
//            }
//            return null;
//        }

//        public AssignmentEnrollment GetAssignmentEnrollmentById(int assignmentEnrollmentId)
//        {
//            var query = from a in _m.AssignmentEnrollment.Include("Team").Include("TournamentAssignment.Tournament").Include("TournamentAssignment.Tournament")
//                        where a.Id == assignmentEnrollmentId
//                        select a; // CreateAssignmentEnrollmentFromEntity(a);

//            if (query.Count() > 0)
//            {
//                return CreateAssignmentEnrollmentFromEntity(query.First());
//            }
//            return null;
//        }

//        public AssignmentEnrollment SaveAssignmentEnrollment(AssignmentEnrollment ae)
//        {
//            e.AssignmentEnrollment toSave = CreateEntityFromAssignmentEnrollment(ae);

//            _m.AddToAssignmentEnrollment(toSave);
//            _m.SaveChanges();
//            _m.Refresh(System.Data.Objects.RefreshMode.StoreWins, toSave);

//            ae.Id = toSave.Id;

//            return ae;
//        }

//        public Submit GetSubmitById(int submitId)
//        {
//            Submit result = null;

//            var query = from s in _m.Submit.Include("Team").Include("TournamentAssignment.Assignment")
//                        where s.Id == submitId
//                        select s;

//            if (query.Count() > 0)
//            {
//                result = CreateSubmitFromEntity(query.First());
//            }

//            return result;
//        }

//        public List<Submit> GetSubmitsForAssignmentEnrollment(int assignmentEnrollmentId)
//        {
//            List<Submit> result = new List<Submit>();

//            var query = from s in _m.Submit
//                        where s.AssignmentEnrollment.Id == assignmentEnrollmentId
//                        orderby s.StatusDate descending
//                        select s;

//            foreach (var s in query.ToList())
//            {
//                result.Add(CreateSubmitFromEntity(s));
//            }
//            return result;
//        }

//        public Submit GetFastestSubmitForTournamentAssignment(int tournamentAssignmentId)
//        {
//            var query = from s in _m.Submit.Include("Team").Include("TournamentAssignment.Assignment")
//                        where s.Status == "Success" && s.TournamentAssignment.Id == tournamentAssignmentId
//                        orderby s.SecondsSinceEnrollment
//                        select s;

//            if (query.Count() > 0)
//            {
//                return CreateSubmitFromEntity(query.First());
//            }
//            return null;
//        }

//        public Submit SaveSubmit(Submit submit)
//        {
//            e.Submit toSave = CreateEntityFromSubmit(submit);

//            _m.AddToSubmit(toSave);
//            _m.SaveChanges();
//            _m.Refresh(System.Data.Objects.RefreshMode.StoreWins, toSave);

//            submit.Id = toSave.Id;

//            return submit;
//        }

//        #region Translation helpers

//        private Assignment CreateAssignmentFromEntity(e.Assignment eAssignment)
//        {
//            if (eAssignment == null)
//            {
//                return null;
//            }
//            Assignment result = new Assignment()
//            {
//                Id = eAssignment.Id,
//                Name = eAssignment.Name,
//                FriendlyName = eAssignment.FriendlyName,
//                Tagline = eAssignment.Tagline,
//                CreateDate = eAssignment.CreateDate,
//                Path = eAssignment.Path
//            };

//            return result;
//        }

//        private e.Assignment CreateEntityFromAssignment(Assignment a)
//        {
//            if (a == null)
//            {
//                return null;
//            }

//            e.Assignment result = e.Assignment.CreateAssignment(a.Id, a.Name, a.CreateDate, a.Path);

//            //e.Assignment result = new e.Assignment()
//            //{
//            //    Id = a.Id,
//            //    Name = a.Name,
//            //    FriendlyName = a.FriendlyName,
//            //    Tagline = a.Tagline,
//            //    CreateDate = a.CreateDate,
//            //    Path = a.Path
//            //};

//            return result;
//        }

//        private Team CreateTeamFromEntity(e.Team eTeam)
//        {
//            if (eTeam == null)
//            {
//                return null;
//            }
//            Team result = new Team()
//            {
//                Id = eTeam.Id,
//                CreateDate = eTeam.CreateDate,
//                IsAdmin = eTeam.IsAdmin,
//                Name = eTeam.Name,
//                Members = eTeam.Members,
//                Password = eTeam.Password,
//                Score = eTeam.Score
//            };

//            return result;
//        }

//        private e.Team CreateEntityFromTeam(Team t)
//        {
//            if (t == null)
//            {
//                return null;
//            }

//            e.Team result = e.Team.CreateTeam(t.Id, t.Name, t.Password, t.CreateDate, t.Score, t.IsAdmin);

//            //e.Team result = new e.Team()
//            //{
//            //    Id = t.Id,
//            //    CreateDate = t.CreateDate,
//            //    IsAdmin = t.IsAdmin,
//            //    Members = t.Members,
//            //    Name = t.Name,
//            //    Password = t.Password,
//            //    Score = t.Score
//            //};

//            return result;
//        }

//        private TournamentAssignment CreateTournamentAssignmentFromEntity(e.TournamentAssignment eTournamentAssignment)
//        {
//            if (eTournamentAssignment == null)
//            {
//                return null;
//            }
//            TournamentAssignment result = new TournamentAssignment()
//            {
//                Id = eTournamentAssignment.Id,
//                AssignmentOrder = eTournamentAssignment.AssignmentOrder,
//                IsActive = eTournamentAssignment.IsActive,
//                CreateDate = eTournamentAssignment.CreateDate,
//                Points1 = eTournamentAssignment.Points1,
//                Points2 = eTournamentAssignment.Points2,
//                Points3 = eTournamentAssignment.Points3,
//                Assignment = CreateAssignmentFromEntity(eTournamentAssignment.Assignment),
//                Tournament = CreateTournamentFromEntity(eTournamentAssignment.Tournament)
//            };

//            return result;
//        }

//        private e.TournamentAssignment CreateEntityFromTournamentAssignment(TournamentAssignment ta)
//        {
//            if (ta == null)
//            {
//                return null;
//            }
//            e.TournamentAssignment result = e.TournamentAssignment.CreateTournamentAssignment(ta.Id, ta.AssignmentOrder, ta.Points1, ta.Points2, ta.Points3, ta.IsActive, ta.CreateDate);

//            result.AssignmentReference.Value = (from a in _m.Assignment
//                                                where a.Id == ta.Assignment.Id
//                                                select a).First();
//            result.TournamentReference.Value = (from t in _m.Tournament
//                                                where t.Id == ta.Tournament.Id
//                                                select t).First();

//            //e.TournamentAssignment result = new e.TournamentAssignment()
//            //{
//            //    Id = TA.Id,
//            //    AssignmentOrder = TA.AssignmentOrder,
//            //    IsActive = TA.IsActive,
//            //    CreateDate = TA.CreateDate,
//            //    Points1 = TA.Points1,
//            //    Points2 = TA.Points2,
//            //    Points3 = TA.Points3,
//            //    Assignment = CreateEntityFromAssignment(TA.Assignment),
//            //    Tournament = CreateEntityFromTournament(TA.Tournament)
//            //};

//            return result;
//        }

//        private Tournament CreateTournamentFromEntity(e.Tournament eTournament)
//        {
//            if (eTournament == null)
//            {
//                return null;
//            }
//            Tournament result = new Tournament()
//            {
//                CreateDate = eTournament.CreateDate,
//                Id = eTournament.Id,
//                Name = eTournament.Name
//            };

//            return result;
//        }

//        private e.Tournament CreateEntityFromTournament(Tournament t)
//        {
//            if (t == null)
//            {
//                return null;
//            }
//            e.Tournament result = e.Tournament.CreateTournament(t.Id, t.Name, t.CreateDate);

//            //e.Tournament result = new e.Tournament()
//            //{
//            //    CreateDate = t.CreateDate,
//            //    Id = t.Id,
//            //    Name = t.Name
//            //};

//            return result;
//        }

//        private AssignmentEnrollment CreateAssignmentEnrollmentFromEntity(e.AssignmentEnrollment eAE)
//        {
//            if (eAE == null)
//            {
//                return null;
//            }
//            AssignmentEnrollment result = new AssignmentEnrollment()
//            {
//                Id = eAE.Id,
//                IsActive = eAE.IsActive,
//                StartDate = eAE.StartDate,
//                Team = CreateTeamFromEntity(eAE.Team),
//                TournamentAssignment = CreateTournamentAssignmentFromEntity(eAE.TournamentAssignment)
//            };

//            return result;
//        }

//        private e.AssignmentEnrollment CreateEntityFromAssignmentEnrollment(AssignmentEnrollment ae)
//        {
//            if (ae == null)
//            {
//                return null;
//            }
//            e.AssignmentEnrollment result = e.AssignmentEnrollment.CreateAssignmentEnrollment(ae.Id, DateTime.Now, true);

//            result.TeamReference.Value = (from t in _m.Team
//                                          where t.Id == ae.Team.Id
//                                          select t).First();
//            result.TournamentAssignmentReference.Value = (from ta in _m.TournamentAssignment
//                                                          where ta.Id == ae.TournamentAssignment.Id
//                                                          select ta).First();


//            //e.AssignmentEnrollment result = new MoCS.Data.Entity.AssignmentEnrollment()
//            //{
//            //    IsActive = ae.IsActive,
//            //    StartDate = ae.StartDate,
//            //    TournamentAssignment = CreateEntityFromTournamentAssignment(ae.TournamentAssignment),
//            //    Team = CreateEntityFromTeam(ae.Team)
//            //};

//            return result;
//        }

//        private Submit CreateSubmitFromEntity(e.Submit eSubmit)
//        {
//            if (eSubmit == null)
//            {
//                return null;
//            }

//            Submit result = new Submit()
//            {
//                Id = eSubmit.Id,
//                FileName = eSubmit.FileName,
//                FileContents = eSubmit.FileContents,
//                Data = eSubmit.Data,
//                IsProcessed = eSubmit.IsProcessed,
//                Status = eSubmit.Status,
//                SubmitDate = eSubmit.SubmitDate,
//                StatusDate = eSubmit.StatusDate,
//                Team = CreateTeamFromEntity(eSubmit.Team),
//                ProcessingDetails = eSubmit.ProcessingDetails,
//                SecondsSinceEnrollment = eSubmit.SecondsSinceEnrollment,
//                AssignmentEnrollment = CreateAssignmentEnrollmentFromEntity(eSubmit.AssignmentEnrollment),
//                TournamentAssignment = CreateTournamentAssignmentFromEntity(eSubmit.TournamentAssignment)
//            };

//            return result;
//        }

//        private e.Submit CreateEntityFromSubmit(Submit s)
//        {
//            if (s == null)
//            {
//                return null;

//            }
//            e.Submit result = e.Submit.CreateSubmit(s.Id, s.SubmitDate, s.Status, s.SecondsSinceEnrollment, s.IsProcessed, s.ProcessingDetails, s.FileName, s.FileContents, s.Data, s.StatusDate);

//            result.AssignmentEnrollmentReference.Value = (from ae in _m.AssignmentEnrollment
//                                                          where ae.Id == s.AssignmentEnrollment.Id
//                                                          select ae).First();
//            result.TournamentAssignmentReference.Value = (from ta in _m.TournamentAssignment
//                                                          where ta.Id == s.TournamentAssignment.Id
//                                                          select ta).First();
//            result.TeamReference.Value = (from t in _m.Team
//                                          where t.Id == s.Team.Id
//                                          select t).First();
//            return result;
//        }
//        #endregion

//    }
//}
