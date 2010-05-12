using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using MoCS.Service.DataContracts;
using Microsoft.ServiceModel.Web;
using System.Net;
using System.ServiceModel.Web;
using System.IO;
using System.Xml;
using System.Reflection;

namespace MoCS.Service
{
    [ServiceBehavior]
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MoCSService : IMoCSService
    {

        string baseFilePath = AppDomain.CurrentDomain.BaseDirectory; // @"C:\Projects\Atos-Origin\XPG\MoCS\MoCS.Service\State";

        private static Teams _teamList = new Teams();
        private static Assignments _assignmentList = new Assignments();

        //Dictionary<string teamId, EnrolledAssignments>
        private static Dictionary<string, EnrolledAssignments> _teamEnrolledAssignments = new Dictionary<string, EnrolledAssignments>();

        IncomingWebRequestContext inRequest = WebOperationContext.Current.IncomingRequest;
        OutgoingWebResponseContext outResponse = WebOperationContext.Current.OutgoingResponse;

        //string basePath = "/MoCS.Service/MoCSService.svc";
        string basePath = "/MoCSService.svc";

        #region IMoCSService Members - Teams resource

        [OperationBehavior]
        public Teams GetAllTeams()
        {
            Team authTeam = Authenticate();

            if (authTeam != null && authTeam.TeamType == TeamType.Administrator)
            {
                return _teamList;
            }
            else
            {
                // Construct new teamlist without credentials
                Teams result = new Teams();
                result = (Teams)_teamList.DeepClone();
                result.ForEach(delegate(Team t) { t.Username = ""; t.Password = ""; });
                return result;
                //ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not authorized to view this resource.");
                //return null;
            }
        }

        [OperationBehavior]
        public Team GetTeam(string teamId)
        {
            Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
            }

            if (authTeam.TeamId == teamId)
            {
                return authTeam;
            }

            Team foundTeam = _teamList.Find(t => t.TeamId == teamId);
            if (foundTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.NotFound, "Resource not found.");
                return null;
            }
            else
            {
                if (authTeam.TeamType == TeamType.Administrator)
                {
                    return foundTeam;
                }
                else
                {
                    Team result;
                    result = (Team)foundTeam.DeepClone();
                    result.Username = "";
                    result.Password = "";
                    return result;

                }
            }
        }

        [OperationBehavior]
        public Team AddTeam(Team team)
        {
            if (_teamList.Find(t => t.TeamName == team.TeamName) == null)
            {
                lock (_teamList)
                {
                    // Get new teamId
                    int newTeamId = 1;
                    if (_teamList.Count > 0)
                    {
                        newTeamId = _teamList.Max(t => int.Parse(t.TeamId));
                        newTeamId++;
                    }

                    team.TeamId = newTeamId.ToString();
                    team.TeamType = TeamType.Normal;
                    team.TeamStatus = TeamStatus.Active;
                    _teamList.Add(team);

                    // Add an entry for the new team in the enrolled assignments registry
                    lock (_teamEnrolledAssignments)
                    {
                        _teamEnrolledAssignments.Add(team.TeamId, new EnrolledAssignments());
                    }
                }

                outResponse.StatusCode = HttpStatusCode.Created;
                outResponse.StatusDescription = basePath + "/teams/" + team.TeamId;
                //outResponse.Location = basePath + "/teams/" + team.TeamId; ;
                return team;
            }
            else
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Team name " + team.TeamName + " already registered");
                return null;
            }
        }

        [OperationBehavior]
        public Team UpdateTeam(string teamId, Team team)
        {
            Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            if (team.TeamId != teamId)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Cannot change TeamId (teamId of the resource you're trying to update (" + teamId + ") is different from the teamId you supplied (" + team.TeamId + ").");
                return null;
            }

            if (authTeam.TeamType == TeamType.Administrator)
            {

                authTeam.TeamName = team.TeamName;
                authTeam.TeamStatus = team.TeamStatus;
                authTeam.TeamType = team.TeamType;
                authTeam.Username = team.Username;
                authTeam.Password = team.Password;

                return authTeam;
            }

            if (authTeam.TeamId != teamId)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId " + team.TeamId);
                return null;
            }
            else
            {
                authTeam.TeamName = team.TeamName;
                authTeam.Username = team.Username;
                authTeam.Password = team.Password;

                outResponse.StatusCode = HttpStatusCode.Created;
                outResponse.StatusDescription = basePath + "/teams/" + team.TeamId;
                return authTeam;
            }
        }

        #endregion

        #region IMoCSService Members - teams/{teamId}/enrolledassignments resource

        [OperationBehavior]
        public EnrolledAssignments GetAllEnrolledAssignmentsForTeam(string teamId)
        {
            return _teamEnrolledAssignments[teamId];
        }

        [OperationBehavior]
        public EnrolledAssignment GetEnrolledAssignmentForTeam(string teamId, string enrolledAssignmentId)
        {

            return _teamEnrolledAssignments[teamId].Find(e => e.AssignmentId == enrolledAssignmentId);
        }

        [OperationBehavior]
        public EnrolledAssignment AddEnrolledAssignmentForTeam(string teamId, EnrolledAssignment enrolledAssignment)
        {
            Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            if (authTeam.TeamId != teamId)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
                return null;
            }

            Assignment foundAssignment = _assignmentList.Find(a => a.AssignmentId == enrolledAssignment.AssignmentId);
            if (foundAssignment == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Assignment with Id: " + enrolledAssignment.AssignmentId + " does not exist.");
                return null;
            }

            EnrolledAssignment foundEnrolledAssignment = _teamEnrolledAssignments[teamId].Find(e => e.AssignmentId == enrolledAssignment.AssignmentId);
            if (foundEnrolledAssignment != null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You have already enrolled for assignment with Id: " + enrolledAssignment.AssignmentId + ". EnrollTime: " + foundEnrolledAssignment.EnrollTime.ToString());
                return null;
            }

            EnrolledAssignment newEnrolledAssignment = new EnrolledAssignment
                {
                    AssignmentId = enrolledAssignment.AssignmentId,
                    AssignmentName = foundAssignment.AssignmentName,
                    EnrollTime = DateTime.Now
                };

            _teamEnrolledAssignments[teamId].Add(newEnrolledAssignment);

            return newEnrolledAssignment;
        }

        #endregion

        #region IMoCSService Members - teams/{teamId}/enrolledassignments/{enrolledAssignmentId}/submits resource

        [OperationBehavior]
        public Submits GetAllSubmitsForEnrolledAssignmentForTeam(string teamId, string enrolledAssignmentId)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public Submit GetSubmitForEnrolledAssignmentForTeam(string teamId, string enrolledAssignmentId, string submitId)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public Submit AddSubmitForEnrolledAssignmentForTeam(string teamId, string enrolledAssignmentId, Submit submit)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region assignments resource

        [OperationBehavior]
        public Assignments GetAllAssignments()
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public Assignment GetAssignment(string assignmentId)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public Assignment AddAssignment(Assignment assignment)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public Assignment UpdateAssignment(string assignmentId, Assignment assignment)
        {
            throw new NotImplementedException();
        }

        #endregion

        private Team Authenticate()
        {
            string username = inRequest.Headers["Username"];
            string password = inRequest.Headers["Password"];

            if (username == null || password == null)
            {
                return null;
            }
            else
            {
                return _teamList.Find(t => t.Username == username && t.Password == password);
            }

        }

        private void ReturnWithStatusAndDescription(HttpStatusCode statusCode, string Description)
        {
            int httpStatusCode;

            switch (statusCode)
            {

                case HttpStatusCode.BadRequest:
                    httpStatusCode = 400;
                    break;
                case HttpStatusCode.NotFound:
                    httpStatusCode = 404;
                    break;
                case HttpStatusCode.NotImplemented:
                    httpStatusCode = 501;
                    break;
                default:
                    httpStatusCode = 0;
                    break;
            }
            HttpStatusDetails details = new HttpStatusDetails { HttpStatusCode = httpStatusCode, Details = Description };
            throw new WebProtocolException(statusCode, details.Details, details, null, null);
        }


        #region effe - laden en persisteren van de lijstjes
        [OperationBehavior]
        public bool PersistList()
        {
            try
            {
                persistLists();
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }

        [OperationBehavior]
        public bool LoadList()
        {
            try
            {
                LoadLists();
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }

        private void LoadLists()
        {
            //_teamList
            string teamsFilePath = Path.Combine(baseFilePath, "teams.xml");
            //_assignmentList
            string assignmentsFilePath = Path.Combine(baseFilePath, "assignments.xml");
            //_teamEnrolledAssignments
            string teamEnrolledAssignmentsFilePath = Path.Combine(baseFilePath, "teamenrolledassignments.xml");

            //_teamList
            using (XmlReader xmlReader = XmlTextReader.Create(teamsFilePath))
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Teams));
                _teamList = (Teams)dcSerializer.ReadObject(xmlReader);
            }

            //_assignmentList
            using (XmlReader xmlReader = XmlTextReader.Create(assignmentsFilePath))
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Assignments));
                _assignmentList = (Assignments)dcSerializer.ReadObject(xmlReader);
            }

            //_teamEnrolledAssignments
            using (XmlReader xmlReader = XmlTextReader.Create(teamEnrolledAssignmentsFilePath))
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Dictionary<string, EnrolledAssignments>));
                _teamEnrolledAssignments = (Dictionary<string, EnrolledAssignments>)dcSerializer.ReadObject(xmlReader);
            }
        }

        private void persistLists()
        {
            //_teamList
            string teamsFilePath = Path.Combine(baseFilePath, "teams.xml");
            //_assignmentList
            string assignmentsFilePath = Path.Combine(baseFilePath, "assignments.xml");
            //_teamEnrolledAssignments
            string teamEnrolledAssignmentsFilePath = Path.Combine(baseFilePath, "teamenrolledassignments.xml");

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.NewLineOnAttributes = true;
            writerSettings.Indent = true;

            //_teamList
            using (XmlWriter xmlWriter = XmlTextWriter.Create(teamsFilePath, writerSettings))
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Teams));
                dcSerializer.WriteObject(xmlWriter, _teamList);
            }

            //_assignmentList
            using (XmlWriter xmlWriter = XmlTextWriter.Create(assignmentsFilePath, writerSettings))
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Assignments));
                dcSerializer.WriteObject(xmlWriter, _assignmentList);
            }

            //_teamEnrolledAssignments
            using (XmlWriter xmlWriter = XmlTextWriter.Create(teamEnrolledAssignmentsFilePath, writerSettings))
            {
                DataContractSerializer dcSerializer = new DataContractSerializer(typeof(Dictionary<string, EnrolledAssignments>));
                dcSerializer.WriteObject(xmlWriter, _teamEnrolledAssignments);
            }
        }
        #endregion
    }


}
