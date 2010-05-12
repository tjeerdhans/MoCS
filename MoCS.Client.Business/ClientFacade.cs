using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;
using MoCS.Client.Data;
using MoCS.Client.Business.Entities;
using System.Configuration;

namespace MoCS.Client.Business
{
    public class ClientFacade
    {
        private IDataAccess _dataAccess;
        private IFileSystem _fileSystem;
        private string _assignmentPath;
        
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

        public Team GetTeam(int teamId)
        {
            return _dataAccess.GetTeamById(teamId);
        }

        public Team GetTeam(string teamName)
        {
            return _dataAccess.GetTeamByName(teamName);
        }
        
        public List<Tournament> GetTournaments()
        {
            return _dataAccess.GetTournaments();
        }

        public Tournament GetTournament(int tournamentId)
        {
            return _dataAccess.GetTournamentById(tournamentId);
        }

        public List<Tournament> GetTeamTournaments(int teamId)
        {
            return _dataAccess.GetTournaments();
        }

        public Tournament GetTeamTournament(int tournamentId, int teamId)
        {
            return _dataAccess.GetTournamentById(tournamentId);
        }

        public void DeleteTeamSubmit(int teamSubmitId)
        {
            _dataAccess.DeleteTeamSubmit(teamSubmitId);
        }
        
        private void AddAssignmentDetailsFromXml(List<Assignment> assignments)
        {
            foreach (Assignment a in assignments)
            {
                string path = Path.Combine(_assignmentPath, a.AssignmentName + @"\" + "assignment.xml");

                if (_fileSystem.FileExists(path))
                {
                    XmlDocument doc = _fileSystem.LoadXml(path);
                    a.DisplayName = GetNodeValue(doc, "Assignment/DisplayName");
                    a.Hint = GetNodeValue(doc, "Assignment/Hint");
                    a.Difficulty = GetNodeValue(doc, "Assignment/Difficulty");
                    a.Author = GetNodeValue(doc, "Assignment/Author");
                    a.Category = GetNodeValue(doc, "Assignment/Category");
                    a.IsValid = true;
                }
                else
                {
                    a.Points = 0;
                    a.Hint = "ERROR: DETAILS NOT FOUND";
                    a.Difficulty = "0";
                    a.IsValid = false;
                }
            }
        }


        public List<TeamTournamentAssignment> GetTeamTournamentAssignments(int tournamentId, int teamId)
        {
            List<TeamTournamentAssignment> assignments = _dataAccess.GetTeamTournamentAssignmentsForTeam(tournamentId, teamId);
            List<Assignment> casted = new List<Assignment>();
            foreach (TeamTournamentAssignment tta in assignments)
            {
                casted.Add(tta);
            }
            AddAssignmentDetailsFromXml(casted);
            return assignments;
        }

        public TournamentAssignment GetTournamentAssignment(int id)
        {
            return _dataAccess.GetTournamentAssignmentById(id);
        }

        public TeamTournamentAssignment GetTeamTournamentAssignment(int id)
        {
            TeamTournamentAssignment a = _dataAccess.GetTeamTournamentAssignmentById(id);
            AppendAssignmentDetailsFromXml(a);
            a.ZipFile = GetAssignmentZip(a.AssignmentName);

            return a;
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


        public List<TournamentAssignment> GetTournamentAssignments(int tournamentId)
        {
            List<TournamentAssignment> assignments = _dataAccess.GetTournamentAssignmentsForTournament(tournamentId);

            List<Assignment> casted = new List<Assignment>();
            foreach (TournamentAssignment ta in assignments)
            {
                casted.Add(ta);
            }
            AddAssignmentDetailsFromXml(casted);

            return assignments;

        }

        public List<Submit> GetTeamSubmitsForAssignment(int tournamentAssignmentId)
        {
            return _dataAccess.GetTeamSubmitsForAssignment(tournamentAssignmentId);
        }

        public Submit GetTeamSubmit(int id)
        {
            return _dataAccess.GetTeamSubmitById(id);
        }

        public TeamTournamentAssignment SaveTeamTournamentAssignment(TeamTournamentAssignment assignment)
        {
            return _dataAccess.SaveTeamTournamentAssignment(assignment);
        }

        public List<Team> GetTournamentTeams(int tournamentId)
        {
            return _dataAccess.GetTeams();
        }

        public List<Submit> GetTournamentSubmits(int tournamentId)
        {
            return _dataAccess.GetTournamentSubmits(tournamentId);
        }

        public List<Assignment> GetAssignments()
        {
            return _dataAccess.GetAssignments();
        }

        public Assignment GetAssignment(int id)
        {
            return _dataAccess.GetAssignmentByID(id);
        }

        public Assignment SaveAssignment(Assignment assignment)
        {
            return _dataAccess.SaveAssignment(assignment);
        }

        public Team SaveTeam(Team team)
        {
            return _dataAccess.SaveTeam(team);
        }

        public void DeleteTeam(int teamId)
        {
            _dataAccess.DeleteTeam(teamId);
        }

        public void DeleteTournament(int tournamentId)
        {
            _dataAccess.DeleteTournament(tournamentId);
        }

        public void DeleteAssignment(int assignmentId)
        {
            _dataAccess.DeleteAssignment(assignmentId);
        }

        public Tournament SaveTournament(Tournament tournament)
        {
            return _dataAccess.SaveTournament(tournament);
        }

        public TournamentAssignment SaveTournamentAssignment(TournamentAssignment assignment)
        {
            return _dataAccess.SaveTournamentAssignment(assignment);
        }

        public Submit InsertSubmit(Submit submit)
        {
            return _dataAccess.InsertTeamSubmit(submit);
        }








        //public List<Submit> GetMySubmits(string teamName, string password, int assignmentId)
        //{
        //    int id = _dataAccess.AuthenticateTeam(teamName, password).ID;
        //    return _dataAccess.GetMySubmits(id, assignmentId);
        //}


        //public Assignment GetTeamAssignmentById(int teamId, int assignmentId)
        //{
        //    Assignment a = _dataAccess.GetTeamAssignmentById(teamId, assignmentId);

        //    a = AppendAssignmentDetailsFromXml(a);

        //    if (a != null)
        //    {
        //        byte[] zipfile = GetAssignmentZip(a.Name);
        //        a.ZipFile = zipfile;
        //    }

        //    return a;
        //}

        //public List<Assignment> GetTeamAssignments(int teamId)
        //{
        //    return _dataAccess.GetTeamAssignments(teamId);
        //}

        //public Assignment InsertTeamAssignment(int teamId, int assignmentId)
        //{
        //    return _dataAccess.InsertTeamAssignment(teamId, assignmentId);
        //}

        //public Team GetTeamById(int id)
        //{
        //    return _dataAccess.GetTeamById(id);
        //}

        //public Team GetTeamByName(string teamName)
        //{
        //    return _dataAccess.GetTeamByName(teamName);
        //}

        //public Team InsertTeam(Team team)
        //{
        //    return _dataAccess.InsertTeam(team);
        //}


        private byte[] GetAssignmentZip(string assignmentName)
        {
            string path = Path.Combine(_assignmentPath, assignmentName + @"\" + assignmentName + ".zip");

            if (!_fileSystem.FileExists(path))
            {
                throw new ApplicationException("Zip file does not exist");
            }

            byte[] zipBytes = null;
            using (FileStream fs = File.OpenRead(path))
            {
                zipBytes = ConvertStreamToByteArray(fs);
            }

            return zipBytes;
        }
        
        //}

        //public void Upload(string teamName, string password, int assignmentId, string fileName, byte[] bytes)
        //{ 
        //    int id = _dataAccess.AuthenticateTeam(teamName, password).ID;

        //    List<Submit> submits = _dataAccess.GetMySubmits(id, assignmentId);

        //    bool allSubmitsAreFinished = true;
        //    bool succesfullSubmitFound = false;
        //    foreach (Submit submit in submits)
        //    {
        //        if (!submit.IsFinished)
        //        {
        //            allSubmitsAreFinished = false;
        //            break;
        //        }
        //        if (submit.CurrentStatus == 1)
        //        {
        //            succesfullSubmitFound = true;
        //            break;
        //        }
        //    }

        //    if (!allSubmitsAreFinished)
        //    {
        //        throw new ApplicationException("Cannot upload. There is still a submit that's still being processed.");
        //    }

        //    if (succesfullSubmitFound)
        //    {
        //        throw new ApplicationException("Cannot upload. There is a successful submit already.");
        //    }


        //    _dataAccess.Upload(id, assignmentId, fileName, bytes);
        //}

        //public Team Authenticate(string teamName, string password)
        //{
        //    return _dataAccess.AuthenticateTeam(teamName, password);
        //}

       

        //public List<Submit> GetAllSubmitsForMonitorDashboard(int assignmentId)
        //{
        //    return _dataAccess.GetTeamSubmitsForMonitoringDashboard(assignmentId);
        //}

        //public List<Submit> TeamSubmit_SelectForAdmin(int assignmentId)
        //{
        //    return _dataAccess.TeamSubmit_SelectForAdmin(assignmentId);
        //}



        //public List<Assignment> GetTournamentAssignmentsForTeam(int teamId)
        //{
        //    List<Assignment> assignments = _dataAccess.GetTournamentAssignments(teamId);
        //    foreach (Assignment a in assignments)
        //    {
        //        string path = Path.Combine(_assignmentPath, a.Name + @"\" + "assignment.xml");

        //        if (_fileSystem.FileExists(path))
        //        {
        //            XmlDocument doc = _fileSystem.LoadXml(path);
        //            a.DisplayName = GetNodeValue(doc, "Assignment/DisplayName");
        //            a.Hint = GetNodeValue(doc, "Assignment/Hint");
        //            a.Difficulty = GetNodeValue(doc, "Assignment/Difficulty");
        //            a.Author = GetNodeValue(doc, "Assignment/Author");
        //            a.Category = GetNodeValue(doc, "Assignment/Category");
        //            a.IsValid = true;
        //        }
        //        else
        //        {
        //            a.Points = 0;
        //            a.Hint = "ERROR: DETAILS NOT FOUND";
        //            a.Difficulty = "0";
        //            a.IsValid = false;
        //        }
        //    }

        //    return assignments;
        //}


        //public List<Assignment> GetAllAssignments_Admin()
        //{
        //    List<Assignment> assignments = _dataAccess.GetAllAssignments();

            
        //    foreach (Assignment assignment in assignments)
        //    {

        //        string path = Path.Combine(_assignmentPath, assignment.Name + @"\" + "assignment.xml");

        //        if (_fileSystem.FileExists(path))
        //        {
        //            XmlDocument doc = _fileSystem.LoadXml(path);
        //            assignment.DisplayName = GetNodeValue(doc, "Assignment/DisplayName");
        //            assignment.Hint = GetNodeValue(doc, "Assignment/Hint");
        //            assignment.Difficulty = GetNodeValue(doc, "Assignment/Difficulty");
        //            assignment.Author = GetNodeValue(doc, "Assignment/Author");
        //            assignment.Category = GetNodeValue(doc, "Assignment/Category");
        //        }
        //    }

        //    return assignments;
        //}


        //public List<Submit> GetTournamentSubmits(int tournamentId)
        //{
        //    return _dataAccess.GetTournamentSubmits(tournamentId);
        //}


        private Assignment AppendAssignmentDetailsFromXml(Assignment a)
        {
            if (a == null)
            {
                return null;    //no active assignment
            }

            string path = Path.Combine(_assignmentPath, a.AssignmentName + @"\" + "assignment.xml");

            if (_fileSystem.FileExists(path))
            {
                XmlDocument doc = _fileSystem.LoadXml(path);

                a.DisplayName = GetNodeValue(doc, "Assignment/DisplayName");
                a.Hint = GetNodeValue(doc, "Assignment/Hint");
                a.Difficulty = GetNodeValue(doc, "Assignment/Difficulty");
                a.Author = GetNodeValue(doc, "Assignment/Author");
                a.Category = GetNodeValue(doc, "Assignment/Category");

                XmlNode fileNode = doc.SelectSingleNode("Assignment/Files");
                foreach (XmlNode fileChildNode in fileNode.ChildNodes)
                {
                    string nodeName = fileChildNode.Name;
                    string text = fileChildNode.InnerText;

                    string filepath = Path.Combine(_assignmentPath, a.AssignmentName + @"\" + text);
                    if (File.Exists(filepath))
                    {
                        if (nodeName != "NunitTestFileServer" && nodeName != "ServerFileToCopy")
                        {
                            AssignmentFile file = new AssignmentFile();
                            file.Name = nodeName;
                            file.Contents = ReadByteArrayFromFile(filepath);
                            a.Files.Add(file.Name, file);
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


        public string GetTournamentReport(int tournamentId)
        {


            List<Team> teams = _dataAccess.GetTeams();
            Tournament tournament = _dataAccess.GetTournamentById(tournamentId);
            List<TournamentAssignment> assignments = _dataAccess.GetTournamentAssignmentsForTournament(tournamentId);

            List<Submit> submits = _dataAccess.GetSubmitsForReport(tournamentId);

            XmlDocument doc = TournamentReportGenerator.CreateTournamentReport(tournament, teams, assignments, submits);

            return doc.OuterXml;

        }


        //public Assignment GetCurrentAssignment()
        //{
        //    Assignment a = _dataAccess.GetCurrentAssignment();

        //    a = AppendAssignmentDetailsFromXml(a);

        //    return a;
        //}

        private byte[] ReadByteArrayFromFile(string path)
        {
            FileStream fs = File.OpenRead(path);
            byte[] result = ConvertStreamToByteArray(fs);
            fs.Close();
            return result;
        }


        private byte[] ConvertStreamToByteArray(Stream stream)
        {
            byte[] respBuffer = new byte[stream.Length];
            try
            {
                int bytesRead = stream.Read(respBuffer, 0, respBuffer.Length);
            }
            finally
            {
                stream.Close();
            }

            return respBuffer;
        }


        //public List<Team> GetTeams()
        //{
        //    return _dataAccess.GetTeams();
        //}


        //public List<Team> GetTeams_Admin()
        //{
        //    return _dataAccess.GetTeams_Admin();
        //}

        //public void UpdateTeam(Team team)
        //{
        //    _dataAccess.UpdateTeam(team);
        //}

        //public void DeleteTeam(int id)
        //{
        //    _dataAccess.DeleteTeam(id);
        //}


        //public void DeactivateAssignment_Admin(int id)
        //{
        //    _dataAccess.DeactivateAssignment(id);
        //}

        //public void ActivateAssignment_Admin(int id)
        //{
        //    _dataAccess.ActivateAssignment(id);
        //}

        //public Assignment GetAssignment_Admin(string assignmentId)
        //{
        //    return _dataAccess.GetAssignment_Admin(assignmentId);
        //}

        //public Assignment UpdateAssignment_Admin(Assignment beAssignment)
        //{
        //    return _dataAccess.UpdateAssignment_Admin(beAssignment);
        //}
    }
}
