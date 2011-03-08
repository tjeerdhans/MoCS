using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
using System.Linq;
using MoCS.Data;
using MoCS.Business.Objects;
using System.Configuration;
using MoCS.Business.Objects.Interfaces;
using System.Text;
using MoCS.Business.Facade.MoCSServiceReference;
using System.Threading;

namespace MoCS.Business.Facade
{
    public class ClientFacade
    {
        private IDataAccess _dataAccess;

        private IFileSystem _fileSystem;
        private string _assignmentsBasePath;
        private bool _useNotification;

        #region Instance plumbing

        private static readonly ClientFacade _instance = new ClientFacade();

        /// <summary>
        /// Static initialization of the singleton as described in:
        /// http://msdn.microsoft.com/en-us/library/ms998558.aspx
        /// </summary>
        /// <value>The instance.</value>
        public static ClientFacade Instance
        {
            get { return _instance; }
        }

        private IDataAccess CreateDataAccess()
        {
            string dataAccess = ConfigurationManager.AppSettings["DataAccess"];
            string connectionString = null;
            if (ConfigurationManager.ConnectionStrings[dataAccess] != null)
            {
                connectionString = ConfigurationManager.ConnectionStrings[dataAccess].ConnectionString;
            }

            Type type = Type.GetType(dataAccess + ", MoCS.Data", true);

            IDataAccess da = null;

            if (connectionString != null)
            {
                da = (IDataAccess)Activator.CreateInstance(type, new object[] { connectionString });
            }
            else
            {
                da = (IDataAccess)Activator.CreateInstance(type);
            }

            return da;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientFacade"/> class.
        /// DataAccess and assignmentsBasePath are taken from App.config.
        /// </summary>
        public ClientFacade()
        {
            _dataAccess = CreateDataAccess();

            _assignmentsBasePath = ConfigurationManager.AppSettings["assignmentsBasePath"];

            _fileSystem = new FileSystemWrapper();

            if (ConfigurationManager.AppSettings["UseNotification"] != null)
            {
                _useNotification = bool.Parse(ConfigurationManager.AppSettings["UseNotification"]);
            }
            else
            {
                _useNotification = false;
            }
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="ClientFacade"/> class.
        ///// DataAccess is injected.
        ///// </summary>
        ///// <param name="dataAccess">The data access.</param>
        ///// <param name="assignmentsBasePath">The assignments base path.</param>
        //public ClientFacade(IDataAccess dataAccess, string assignmentsBasePath)
        //{
        //    _dataAccess = dataAccess;
        //    _fileSystem = new FileSystemWrapper();
        //    _assignmentsBasePath = assignmentsBasePath;
        //}

        ///// <summary>
        ///// Constructor for unit testing purposes
        ///// </summary>
        ///// <param name="dataAccess"></param>
        ///// <param name="fileSystem"></param>
        //public ClientFacade(IDataAccess dataAccess, IFileSystem fileSystem)
        //{
        //    _dataAccess = dataAccess;
        //    _fileSystem = fileSystem;
        //}

        #endregion

        #region Used by the BuildService

        public List<Submit> GetUnprocessedSubmits()
        {
            return _dataAccess.GetUnprocessedSubmits();
        }

        public void UpdateSubmitStatusDetails(int submitId, SubmitStatus newStatus, string details, DateTime statusDate)
        {
            if (string.IsNullOrEmpty(details))
            {
                details = "Empty";
            }
            _dataAccess.UpdateSubmitStatusDetails(submitId, newStatus, details, statusDate);

            NotifySubmitStatusChange(submitId, newStatus, details, statusDate);

            Submit firstPlaceSubmit = null;
            //TODO If newstatus is success, check if this submit is in first place for the assignment
            if (newStatus == SubmitStatus.Success)
            {
                // Get the tournamentassignment of the submit
                Submit submit = _dataAccess.GetSubmitById(submitId);
                int taId = submit.TournamentAssignment.Id;

                // Get the submit in first place for the tournamentassignment
                firstPlaceSubmit = _dataAccess.GetFastestSubmitForTournamentAssignment(taId);
            }

            if (firstPlaceSubmit != null && firstPlaceSubmit.Id == submitId)
            {
                NotifyFirstPlace(firstPlaceSubmit);
            }
        }

        #endregion

        #region Scoreboard

        public List<TournamentAssignment> GetTournamentScoreboard(int tournamentId)
        {
            List<TournamentAssignment> result;
            //Get tournamentAssignments
            result = GetTournamentAssignmentsForTournament(tournamentId);

            //Get submits per tournamentassigment
            foreach (TournamentAssignment ta in result)
            {
                ////Get the submits per tournamentassignment

                //Get the enrollments per tournamentassignment
                ta.AssignmentEnrollmentList = _dataAccess.GetAssignmentEnrollmentsForTournamentAssignment(ta.Id);

                foreach (AssignmentEnrollment ae in ta.AssignmentEnrollmentList)
                {
                    ae.SubmitList = new List<Submit>();
                    Submit lastSubmit = GetLastSubmitForAssignmentEnrollment(ae.Id);
                    if (lastSubmit != null)
                    {
                        ae.SubmitList.Add(lastSubmit);
                    }
                }
            }


            return result;
        }

        public Submit GetLastSubmitForAssignmentEnrollment(int assignmentEnrollmentId)
        {
            Submit result = null;

            List<Submit> submits = GetSubmitsForAssignmentEnrollment(assignmentEnrollmentId);

            if (submits.Count > 0)
            {
                result = (from c in submits
                          orderby c.SubmitDate descending
                          select c).First();
            }

            // D'OH!
            //// First, see if there's a successful submit
            //result = candidates.Find(c => c.Status == SubmitStatus.Success.ToString());

            //// Second, any is there a submit in progress?
            //if (result == null)
            //{
            //    result = candidates.Find(c => (c.Status == SubmitStatus.Submitted.ToString()) || c.Status == SubmitStatus.Processing.ToString());
            //}

            //// Third, settle for the 
            //if (result == null)
            //{
            //    result = (from c in candidates
            //              orderby c.SubmitDate descending
            //              select c).First();
            //}

            return result;
        }

        #endregion

        public Team AuthenticateTeam(Team team)
        {
            Team result = null;

            Team authTeam = GetTeamByName(team.Name);

            if (authTeam != null && authTeam.Password == team.Password)
            {
                result = authTeam;
            }

            return result;
        }

        public List<Team> GetTeams()
        {
            return _dataAccess.GetTeams();
        }

        public Team GetTeamById(int teamId)
        {
            return _dataAccess.GetTeamById(teamId);
        }

        public Team GetTeamByName(string teamName)
        {
            return _dataAccess.GetTeamByName(teamName);
        }

        public Team SaveTeam(Team team)
        {
            return _dataAccess.SaveTeam(team);
        }

        public Team UpdateTeam(Team team)
        {
            return _dataAccess.UpdateTeam(team);
        }

        public List<Tournament> GetTournaments()
        {
            return _dataAccess.GetTournaments();
        }

        public Tournament GetTournamentById(int tournamentId)
        {
            return _dataAccess.GetTournamentById(tournamentId);
        }

        public List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeam(int teamId)
        {
            return _dataAccess.GetAssignmentEnrollmentsForTeam(teamId);
        }

        public List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeamForTournamentAssignment(int tournamentAssignmentId, int teamId)
        {
            return _dataAccess.GetAssignmentEnrollmentsForTeamForTournamentAssignment(tournamentAssignmentId, teamId);
        }

        public Assignment GetAssignmentById(int assignmentId, bool includeServerFiles)
        {
            Assignment result = _dataAccess.GetAssignmentById(assignmentId);

            if (result != null)
            {
                // TODO Fill the object with additional data from the XML-file
                if (!string.IsNullOrEmpty(result.Path))
                {
                    result = FacadeHelpers.FillAssignmentDetailsFromXml(result, _fileSystem, includeServerFiles);
                }
            }

            return result;
        }

        public TournamentAssignment GetTournamentAssignmentById(int id, bool includeServerFiles)
        {
            TournamentAssignment result = _dataAccess.GetTournamentAssignmentById(id);

            if (result != null)
            {
                // TODO Fill the object with additional data from the XML-file
                if (!string.IsNullOrEmpty(result.Assignment.Path))
                {
                    result.Assignment = FacadeHelpers.FillAssignmentDetailsFromXml(result.Assignment, _fileSystem, includeServerFiles);
                }

                //result.Assignment.AssignmentFiles
                //result.Assignment.Author
                //result.Assignment.Category
                //result.Assignment.Difficulty = 
                //result.Assignment.ClassNameToImplement = 
                //result.Assignment.InterfaceNameToImplement = 
                //result.Assignment.Version = 
            }

            return result;

        }

        public AssignmentEnrollment GetAssignmentEnrollmentById(int id)
        {
            return _dataAccess.GetAssignmentEnrollmentById(id);
        }

        public List<TournamentAssignment> GetTournamentAssignmentsForTournament(int tournamentId)
        {
            return _dataAccess.GetTournamentAssignmentsForTournament(tournamentId);
        }

        public AssignmentEnrollment SaveAssignmentEnrollment(AssignmentEnrollment ae)
        {
            return _dataAccess.SaveAssignmentEnrollment(ae);
        }

        public List<Submit> GetSubmitsForAssignmentEnrollment(int assignmentEnrollmentId)
        {
            return _dataAccess.GetSubmitsForAssignmentEnrollment(assignmentEnrollmentId);
        }

        public Submit SaveSubmit(Submit submit)
        {
            Submit result = null;
            //Check for unprocessed and already successful submits
            foreach (Submit s in GetSubmitsForAssignmentEnrollment(submit.AssignmentEnrollment.Id))
            {
                switch (s.ConvertStatus(s.Status))
                {
                    case SubmitStatus.Submitted:
                    case SubmitStatus.Processing:
                        throw new MoCSException("There are unprocessed submits. Please wait until all your submits have been processed.");
                    case SubmitStatus.Success:
                        throw new MoCSException("A successful solution has already been submitted.");
                    default:
                        break;
                }
            }

            submit.Status = SubmitStatus.Submitted.ToString();
            submit.SubmitDate = DateTime.Now;
            submit.StatusDate = DateTime.Now;
            submit.IsProcessed = false;

            // Get the AssignmentEnrollment in order to determine the time since enrollment.
            AssignmentEnrollment ae = GetAssignmentEnrollmentById(submit.AssignmentEnrollment.Id);
            TimeSpan timeSinceEnrollment = submit.SubmitDate - ae.StartDate;
            submit.SecondsSinceEnrollment = (long)timeSinceEnrollment.TotalSeconds;

            submit.ProcessingDetails = "Empty";
            submit.FileContents = UTF8Encoding.UTF8.GetString(submit.Data);

            result = _dataAccess.SaveSubmit(submit);

            NotifySubmitStatusChange(result.Id, SubmitStatus.Submitted, "", result.StatusDate);

            return result;
        }

        public byte[] GetAssignmentZip(Assignment assignment)
        {
            //Get the assignment
            assignment = _dataAccess.GetAssignmentById(assignment.Id);

            string path = Path.Combine(assignment.Path, assignment.Name + ".zip");

            if (!_fileSystem.FileExists(path))
            {
                throw new ApplicationException("Zip file does not exist");
            }

            byte[] zipBytes = null;
            using (FileStream fs = File.OpenRead(path))
            {
                zipBytes = FacadeHelpers.ConvertStreamToByteArray(fs);
            }

            return zipBytes;
        }

        #region Helpers

        private Team CreateIdTeam(Team t)
        {
            Team result = new Team
            {
                Id = t.Id,
                CreateDate = DateTime.Now,
                IsAdmin = false,
                Members = string.Empty,
                Name = string.Empty,
                Password = string.Empty,
                Score = 0
            };

            return result;
        }

        private Tournament CreateIdTournament(Tournament t)
        {
            Tournament result = new Tournament
            {
                Id = t.Id,
                CreateDate = DateTime.Now,
                Name = string.Empty
            };
            return result;
        }

        private TournamentAssignment CreateIdTournamentAssignment(TournamentAssignment ta)
        {
            TournamentAssignment result = new TournamentAssignment
            {
                Id = ta.Id,
                AssignmentOrder = 0,
                CreateDate = DateTime.Now,
                IsActive = true,
                Tournament = CreateIdTournament(ta.Tournament),
                Assignment = CreateIdAssignment(ta.Assignment),
                Points1 = 0,
                Points2 = 0,
                Points3 = 0
            };
            return result;
        }

        private Submit CreateIdSubmit(Submit s)
        {
            Submit result = new Submit
            {
                Id = s.Id,
                Team = CreateIdTeam(s.Team),
                AssignmentEnrollment = CreateIdAssignmentEnrollment(s.AssignmentEnrollment),
                TournamentAssignment = CreateIdTournamentAssignment(s.TournamentAssignment),
                Data = new byte[0],
                FileContents = string.Empty,
                FileName = string.Empty,
                IsProcessed = false,
                ProcessingDetails = string.Empty,
                Status = string.Empty,
                SubmitDate = DateTime.Now
            };
            return result;
        }

        private Assignment CreateIdAssignment(Assignment a)
        {
            Assignment result = new Assignment
            {
                Id = a.Id,
                AssignmentFiles = new List<AssignmentFile>(),
                Author = string.Empty,
                Category = string.Empty,
                CreateDate = DateTime.Now,
                Difficulty = 0,
                FriendlyName = string.Empty,
                ClassNameToImplement = string.Empty,
                InterfaceNameToImplement = string.Empty,
                Name = string.Empty,
                Path = string.Empty,
                Tagline = string.Empty,
                Version = 0
            };
            return result;
        }

        private AssignmentEnrollment CreateIdAssignmentEnrollment(AssignmentEnrollment ae)
        {
            AssignmentEnrollment result = new AssignmentEnrollment
            {
                Id = ae.Id,
                Team = CreateIdTeam(ae.Team),
                TournamentAssignment = CreateIdTournamentAssignment(ae.TournamentAssignment),
                IsActive = true,
                StartDate = DateTime.Now
            };
            return result;
        }

        //public string GetTournamentReport(int tournamentId)
        //{
        //    List<Team> teams = _dataAccess.GetTeams();
        //    Tournament tournament = _dataAccess.GetTournamentById(tournamentId);
        //    List<TournamentAssignment> assignments = _dataAccess.GetTournamentAssignmentsForTournament(tournamentId);

        //    List<Submit> submits = _dataAccess.GetSubmitsForReport(tournamentId);

        //    XmlDocument doc = TournamentReportGenerator.CreateTournamentReport(tournament, teams, assignments, submits);

        //    return doc.OuterXml;
        //}

        #endregion

        #region NotifyService

        public void NotifyFirstPlace(Submit submit)
        {

            if (_useNotification)
            {
                string notifyText;

                notifyText = string.Format("Team {0} currently holds first place for assignment {1}. They took {2} seconds to program a correct solution.",
                    new object[] { submit.Team.Name, submit.TournamentAssignment.Assignment.Name, submit.SecondsSinceEnrollment });
                NotifyAll(MessageType.Info, DateTime.Now, "", "FirstPlace", notifyText);
            }
        }

        public void NotifySubmitStatusChange(int submitId, SubmitStatus newStatus, string details, DateTime statusDate)
        {
            if (_useNotification)
            {
                string notifyText;
                //Get the submit

                //don't use global setting. this will break the multithreaded buildprocess
                IDataAccess dataAccess = CreateDataAccess();

                Submit s = dataAccess.GetSubmitById(submitId);

                switch (newStatus)
                {
                    case SubmitStatus.Success:
                        notifyText = string.Format(
                            "Team '{0}': Processing of submit for assignment {1} submitted at {2} resulted in the following: SUCCESS! You pass, now get coffee.",
                            new object[] { s.Team.Name, s.TournamentAssignment.Assignment.Name, s.SubmitDate.ToString("dd-MM-yyyy HH:mm:ss") });
                        NotifyAll(MessageType.Info, statusDate, "", newStatus.ToString(), notifyText);
                        break;
                    case SubmitStatus.Processing:
                        notifyText = string.Format(
                            "Team '{0}': Processing of submit for assignment {1} submitted at {2} has started. Now we wait.",
                            new object[] { s.Team.Name, s.TournamentAssignment.Assignment.Name, s.SubmitDate.ToString("dd-MM-yyyy HH:mm:ss") });
                        NotifyAll(MessageType.Info, statusDate, s.Team.Name, newStatus.ToString(), notifyText);
                        break;
                    case SubmitStatus.ErrorCompilation:
                    case SubmitStatus.ErrorValidation:
                    case SubmitStatus.ErrorTesting:
                    case SubmitStatus.ErrorServer:
                    case SubmitStatus.ErrorUnknown:
                        notifyText = string.Format(
                            "Team '{0}': Processing of submit {1} for assignment {2} submitted at {3} resulted in the following: {4}",
                            new object[] { s.Team.Name, s.Id, s.TournamentAssignment.Assignment.Name, s.SubmitDate.ToString("dd-MM-yyyy HH:mm:ss"), details });
                        NotifyAll(MessageType.Error, statusDate, s.Team.Name, newStatus.ToString(), notifyText);
                        break;
                    case SubmitStatus.Submitted:
                        notifyText = string.Format(
                            "Team '{0}': Submit {1} for assignment {2} has been received at {3}. It's in the queue.",
                            new object[] { s.Team.Name, s.Id, s.TournamentAssignment.Assignment.Name, s.SubmitDate.ToString("dd-MM-yyyy HH:mm:ss") });
                        NotifyAll(MessageType.Info, statusDate, "", newStatus.ToString(), notifyText);

                        break;
                    default:
                        break;
                }
            }

        }

        public void NotifyAll(MessageType messageType, DateTime dateTime, string teamId, string category, string text)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    using (NotifyClient notifyClient = new NotifyClient())
                    {
                        notifyClient.NotifyAll(messageType, dateTime, teamId, category, text);
                    }
                }
                catch { }
            }, null);
        }

        #endregion
    }
}
