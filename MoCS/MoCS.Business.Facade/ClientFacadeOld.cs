//using System.Collections.Generic;
//using System.IO;
//using System;
//using System.Xml;
//using MoCS.Data;
//using MoCS.Business;
//using System.Configuration;

//namespace MoCS.Business.Facade
//{
//    public class ClientFacadeOld
//    {
//        private IDataAccess _dataAccess;
//        private IFileSystem _fileSystem;
//        private string _assignmentPath;


//        /// <summary>
//        /// Initializes a new instance of the <see cref="ClientFacade"/> class.
//        /// DataAccess and assignmentsBasePath are taken from App.config.
//        /// </summary>
//        public ClientFacadeOld()
//        {
//            string dataAccess = ConfigurationManager.AppSettings["DataAccess"];
//            string connectionString = null;
//            if (ConfigurationManager.ConnectionStrings[dataAccess] != null)
//            {
//                connectionString = ConfigurationManager.ConnectionStrings[dataAccess].ConnectionString;
//            }

//            Type type = Type.GetType(dataAccess + ", MoCS.Data");
//            if (connectionString!=null)
//            {
//                _dataAccess = (IDataAccess)Activator.CreateInstance(type, new object[] { connectionString });
//            }
//            else
//            {
//                _dataAccess = (IDataAccess)Activator.CreateInstance(type);
//            }
            
//            _assignmentPath = ConfigurationManager.AppSettings["assignmentsBasePath"];

//            _fileSystem = new FileSystemWrapper();
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ClientFacade"/> class.
//        /// DataAccess is injected.
//        /// </summary>
//        /// <param name="dataAccess">The data access.</param>
//        /// <param name="assignmentsBasePath">The assignments base path.</param>
//        public ClientFacadeOld(IDataAccess dataAccess, string assignmentsBasePath)
//        {
//            _dataAccess = dataAccess;
//            _fileSystem = new FileSystemWrapper();
//            _assignmentPath = assignmentsBasePath;
//        }

//        /// <summary>
//        /// Constructor for unit testing purposes
//        /// </summary>
//        /// <param name="dataAccess"></param>
//        /// <param name="fileSystem"></param>
//        public ClientFacadeOld(IDataAccess dataAccess, IFileSystem fileSystem)
//        {
//            _dataAccess = dataAccess;
//            _fileSystem = fileSystem;
//        }

//        #region Used by the BuildService

//        public List<SubmitToProcess> GetUnprocessedSubmits()
//        {
//            return _dataAccess.GetUnprocessedSubmits();
//        }

//        public void SetTeamSubmitToFinished(int submitId)
//        {
//            _dataAccess.SetTeamSubmitToFinished(submitId);
//        }
//        public void InsertSubmitStatus(int teamId, int submitId, int statusCode, string details)
//        {
//            _dataAccess.InsertSubmitStatus(teamId, submitId, statusCode, details);
//        }

//        #endregion


//        public Team GetTeam(int teamId)
//        {
//            return _dataAccess.GetTeamById(teamId);
//        }

//        public Team GetTeam(string teamName)
//        {
//            return _dataAccess.GetTeamByName(teamName);
//        }

//        public List<Tournament> GetTournaments()
//        {
//            return _dataAccess.GetTournaments();
//        }

//        public Tournament GetTournament(int tournamentId)
//        {
//            return _dataAccess.GetTournamentById(tournamentId);
//        }

//        public List<Tournament> GetTeamTournaments(int teamId)
//        {
//            return _dataAccess.GetTournaments();
//        }

//        public Tournament GetTeamTournament(int tournamentId, int teamId)
//        {
//            return _dataAccess.GetTournamentById(tournamentId);
//        }

//        public void DeleteTeamSubmit(int teamSubmitId)
//        {
//            _dataAccess.DeleteTeamSubmit(teamSubmitId);
//        }

//        private void AddAssignmentDetailsFromXml(List<Assignment> assignments)
//        {
//            foreach (Assignment a in assignments)
//            {
//                string path = Path.Combine(_assignmentPath, a.AssignmentName + @"\" + "assignment.xml");

//                if (_fileSystem.FileExists(path))
//                {
//                    XmlDocument doc = _fileSystem.LoadXml(path);
//                    a.DisplayName = GetNodeValue(doc, "Assignment/DisplayName");
//                    a.Hint = GetNodeValue(doc, "Assignment/Hint");
//                    a.Difficulty = GetNodeValue(doc, "Assignment/Difficulty");
//                    a.Author = GetNodeValue(doc, "Assignment/Author");
//                    a.Category = GetNodeValue(doc, "Assignment/Category");
//                    a.IsValid = true;
//                }
//                else
//                {
//                    a.Points = 0;
//                    a.Hint = "ERROR: DETAILS NOT FOUND";
//                    a.Difficulty = "0";
//                    a.IsValid = false;
//                }
//            }
//        }

//        public List<AssignmentEnrollment> GetTeamTournamentAssignments(int tournamentId, int teamId)
//        {
//            List<AssignmentEnrollment> assignments = _dataAccess.GetTeamTournamentAssignmentsForTeam(tournamentId, teamId);
//            List<Assignment> casted = new List<Assignment>();
//            foreach (AssignmentEnrollment tta in assignments)
//            {
//                casted.Add(tta);
//            }
//            AddAssignmentDetailsFromXml(casted);
//            return assignments;
//        }



//        public TournamentAssignment GetTournamentAssignment(int id)
//        {
//            return _dataAccess.GetTournamentAssignmentById(id);
//        }

//        public AssignmentEnrollment GetTeamTournamentAssignment(int id)
//        {
//            AssignmentEnrollment a = _dataAccess.GetTeamTournamentAssignmentById(id);
//            AppendAssignmentDetailsFromXml(a);
//            a.ZipFile = GetAssignmentZip(a.AssignmentName);

//            return a;
//        }

//        private string GetNodeValue(XmlNode node, string xpath)
//        {
//            XmlNode n = node.SelectSingleNode(xpath);
//            if (n != null)
//            {
//                return n.InnerText;
//            }
//            return "";
//        }

//        public List<TournamentAssignment> GetTournamentAssignments(int tournamentId)
//        {
//            List<TournamentAssignment> assignments = _dataAccess.GetTournamentAssignmentsForTournament(tournamentId);

//            List<Assignment> casted = new List<Assignment>();
//            foreach (TournamentAssignment ta in assignments)
//            {
//                casted.Add(ta);
//            }
//            AddAssignmentDetailsFromXml(casted);

//            return assignments;

//        }

//        public List<Submit> GetTeamSubmitsForAssignment(int tournamentAssignmentId)
//        {
//            return _dataAccess.GetTeamSubmitsForAssignment(tournamentAssignmentId);
//        }

//        public Submit GetTeamSubmit(int id)
//        {
//            return _dataAccess.GetTeamSubmitById(id);
//        }

//        public AssignmentEnrollment SaveTeamTournamentAssignment(AssignmentEnrollment assignment)
//        {
//            return _dataAccess.SaveTeamTournamentAssignment(assignment);
//        }

//        public List<Team> GetTournamentTeams(int tournamentId)
//        {
//            return _dataAccess.GetTeams();
//        }

//        public List<Submit> GetTournamentSubmits(int tournamentId)
//        {
//            return _dataAccess.GetTournamentSubmits(tournamentId);
//        }

//        public List<Assignment> GetAssignments()
//        {
//            return _dataAccess.GetAssignments();
//        }

//        public Assignment GetAssignment(int id)
//        {
//            return _dataAccess.GetAssignmentByID(id);
//        }

//        public Assignment SaveAssignment(Assignment assignment)
//        {
//            return _dataAccess.SaveAssignment(assignment);
//        }

//        public Team SaveTeam(Team team)
//        {
//            return _dataAccess.SaveTeam(team);
//        }

//        public void DeleteTeam(int teamId)
//        {
//            _dataAccess.DeleteTeam(teamId);
//        }

//        public void DeleteTournament(int tournamentId)
//        {
//            _dataAccess.DeleteTournament(tournamentId);
//        }

//        public void DeleteAssignment(int assignmentId)
//        {
//            _dataAccess.DeleteAssignment(assignmentId);
//        }

//        public Tournament SaveTournament(Tournament tournament)
//        {
//            return _dataAccess.SaveTournament(tournament);
//        }

//        public TournamentAssignment SaveTournamentAssignment(TournamentAssignment assignment)
//        {
//            return _dataAccess.SaveTournamentAssignment(assignment);
//        }

//        public Submit InsertSubmit(Submit submit)
//        {
//            return _dataAccess.InsertTeamSubmit(submit);
//        }

//        private byte[] GetAssignmentZip(string assignmentName)
//        {
//            string path = Path.Combine(_assignmentPath, assignmentName + @"\" + assignmentName + ".zip");

//            if (!_fileSystem.FileExists(path))
//            {
//                throw new ApplicationException("Zip file does not exist");
//            }

//            byte[] zipBytes = null;
//            using (FileStream fs = File.OpenRead(path))
//            {
//                zipBytes = ConvertStreamToByteArray(fs);
//            }

//            return zipBytes;
//        }

//        private Assignment AppendAssignmentDetailsFromXml(Assignment a)
//        {
//            if (a == null)
//            {
//                return null;    //no active assignment
//            }

//            string path = Path.Combine(_assignmentPath, a.AssignmentName + @"\" + "assignment.xml");

//            if (_fileSystem.FileExists(path))
//            {
//                XmlDocument doc = _fileSystem.LoadXml(path);

//                a.DisplayName = GetNodeValue(doc, "Assignment/DisplayName");
//                a.Hint = GetNodeValue(doc, "Assignment/Hint");
//                a.Difficulty = GetNodeValue(doc, "Assignment/Difficulty");
//                a.Author = GetNodeValue(doc, "Assignment/Author");
//                a.Category = GetNodeValue(doc, "Assignment/Category");

//                XmlNode fileNode = doc.SelectSingleNode("Assignment/Files");
//                foreach (XmlNode fileChildNode in fileNode.ChildNodes)
//                {
//                    string nodeName = fileChildNode.Name;
//                    string text = fileChildNode.InnerText;

//                    string filepath = Path.Combine(_assignmentPath, a.AssignmentName + @"\" + text);
//                    if (File.Exists(filepath))
//                    {
//                        if (nodeName != "NunitTestFileServer" && nodeName != "ServerFileToCopy")
//                        {
//                            AssignmentFile file = new AssignmentFile();
//                            file.Name = nodeName;
//                            file.Contents = ReadByteArrayFromFile(filepath);
//                            a.Files.Add(file.Name, file);
//                        }
//                    }
//                }
//            }
//            else
//            {
//                throw new ApplicationException("Details for the assignment could not be found");
//            }
//            return a;

//        }

//        public string GetTournamentReport(int tournamentId)
//        {
//            List<Team> teams = _dataAccess.GetTeams();
//            Tournament tournament = _dataAccess.GetTournamentById(tournamentId);
//            List<TournamentAssignment> assignments = _dataAccess.GetTournamentAssignmentsForTournament(tournamentId);

//            List<Submit> submits = _dataAccess.GetSubmitsForReport(tournamentId);

//            XmlDocument doc = TournamentReportGenerator.CreateTournamentReport(tournament, teams, assignments, submits);

//            return doc.OuterXml;
//        }

//        private byte[] ReadByteArrayFromFile(string path)
//        {
//            FileStream fs = File.OpenRead(path);
//            byte[] result = ConvertStreamToByteArray(fs);
//            fs.Close();
//            return result;
//        }

//        private byte[] ConvertStreamToByteArray(Stream stream)
//        {
//            byte[] respBuffer = new byte[stream.Length];
//            try
//            {
//                int bytesRead = stream.Read(respBuffer, 0, respBuffer.Length);
//            }
//            finally
//            {
//                stream.Close();
//            }

//            return respBuffer;
//        }
//    }
//}
