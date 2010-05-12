using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
using MoCS.Data;
using MoCS.Business.Objects;
using System.Configuration;
using MoCS.Business.Objects.Interfaces;

namespace MoCS.Business.Facade
{
    public class ClientFacade
    {
        private IDataAccess _dataAccess;
        private IFileSystem _fileSystem;
        private string _assignmentPath;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientFacade"/> class.
        /// DataAccess and assignmentsBasePath are taken from App.config.
        /// </summary>
        public ClientFacade()
        {
            string dataAccess = ConfigurationManager.AppSettings["DataAccess"];
            string connectionString = null;
            if (ConfigurationManager.ConnectionStrings[dataAccess] != null)
            {
                connectionString = ConfigurationManager.ConnectionStrings[dataAccess].ConnectionString;
            }

            Type type = Type.GetType(dataAccess + ", MoCS.Data", true);
            if (connectionString != null)
            {
                _dataAccess = (IDataAccess)Activator.CreateInstance(type, new object[] { connectionString });
            }
            else
            {
                _dataAccess = (IDataAccess)Activator.CreateInstance(type);
            }

            _assignmentPath = ConfigurationManager.AppSettings["assignmentsBasePath"];

            _fileSystem = new FileSystemWrapper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientFacade"/> class.
        /// DataAccess is injected.
        /// </summary>
        /// <param name="dataAccess">The data access.</param>
        /// <param name="assignmentsBasePath">The assignments base path.</param>
        public ClientFacade(IDataAccess dataAccess, string assignmentsBasePath)
        {
            _dataAccess = dataAccess;
            _fileSystem = new FileSystemWrapper();
            _assignmentPath = assignmentsBasePath;
        }

        /// <summary>
        /// Constructor for unit testing purposes
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="fileSystem"></param>
        public ClientFacade(IDataAccess dataAccess, IFileSystem fileSystem)
        {
            _dataAccess = dataAccess;
            _fileSystem = fileSystem;
        }

        #endregion
        //#region Used by the BuildService

        //public List<SubmitToProcess> GetUnprocessedSubmits()
        //{
        //    return _dataAccess.GetUnprocessedSubmits();
        //}

        //public void SetTeamSubmitToFinished(int submitId)
        //{
        //    _dataAccess.SetTeamSubmitToFinished(submitId);
        //}
        //public void InsertSubmitStatus(int teamId, int submitId, int statusCode, string details)
        //{
        //    _dataAccess.InsertSubmitStatus(teamId, submitId, statusCode, details);
        //}

        //#endregion

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

        public List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeamForTournament(int tournamentId, int teamId)
        {
            return _dataAccess.GetAssignmentEnrollmentsForTeamForTournament(tournamentId, teamId);
        }

        public TournamentAssignment GetTournamentAssignmentById(int id)
        {
            return _dataAccess.GetTournamentAssignmentById(id);
        }

        public AssignmentEnrollment GetAssignmentEnrollment(int id)
        {
            return _dataAccess.GetAssignmentEnrollmentById(id);
        }

        private string GetNodeValue(XmlNode node, string xpath)
        {
            XmlNode n = node.SelectSingleNode(xpath);
            if (n != null)
            {
                return n.InnerText;
            }
            return "";
        }

        public List<TournamentAssignment> GetTournamentAssignmentsForTournament(int tournamentId)
        {
            return _dataAccess.GetTournamentAssignmentsForTournament(tournamentId);
        }

        public AssignmentEnrollment SaveAssignmentEnrollment(AssignmentEnrollment assignment)
        {
            return _dataAccess.SaveAssignmentEnrollment(assignment);
        }

        private byte[] GetAssignmentZip(Assignment assignment)
        {
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

        private Assignment FillAssignmentDetailsFromXml(Assignment a)
        {
            if (a == null)
            {
                return null;    //no active assignment
            }

            string path = Path.Combine(a.Path, "assignment.xml");

            if (_fileSystem.FileExists(path))
            {
                XmlDocument doc = _fileSystem.LoadXml(path);

                a.FriendlyName = GetNodeValue(doc, "Assignment/DisplayName");
                //a.Tagline = GetNodeValue(doc, "Assignment/Hint");

                a.Difficulty = int.Parse(GetNodeValue(doc, "Assignment/Difficulty"));
                a.Author = GetNodeValue(doc, "Assignment/Author");
                a.Category = GetNodeValue(doc, "Assignment/Category");

                a.InterfaceNameToImplement = GetNodeValue(doc, "Assignment/Rules/InterfaceNameToImplement");
                a.ClassNameToImplement = GetNodeValue(doc, "Assignment/Rules/ClassNameToImplement");

                //a.ClassFileName = GetNodeValue(doc, "Assignment/Files/ClassFile");
                //a.InterfaceFileName = GetNodeValue(doc, "Assignment/Files/InterfaceFile");

                //a.UnitTestClientFileName = GetNodeValue(doc, "Assignment/Files/NunitTestFileClient");
                //a.UnitTestServerFileName = GetNodeValue(doc, "Assignment/Files/NunitTestFileServer");

                //a.CaseFileName = GetNodeValue(doc, "Assignment/Files/Case");

                XmlNode fileNode = doc.SelectSingleNode("Assignment/Files");
                foreach (XmlNode fileChildNode in fileNode.ChildNodes)
                {
                    string nodeName = fileChildNode.Name;
                    string fileName = fileChildNode.InnerText;

                    string filepath = Path.Combine(a.Path, fileName);
                    if (File.Exists(filepath))
                    {
                        if (nodeName != "NunitTestFileServer" && nodeName != "ServerFileToCopy")
                        {
                            AssignmentFile assignmentFile = new AssignmentFile();
                            assignmentFile.Name = nodeName;
                            assignmentFile.Data = FacadeHelpers.ReadByteArrayFromFile(filepath);
                            a.AssignmentFiles.Add(assignmentFile);
                        }
                    }
                }
            }
            else
            {
                throw new ApplicationException("Details for the assignment could not be found");
            }
            return a;

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

    }
}
