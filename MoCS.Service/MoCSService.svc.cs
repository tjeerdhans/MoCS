using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using MoCS.Service.DataContracts;
using Microsoft.ServiceModel.Web;
using System.Net;
using System.ServiceModel.Web;
using MoCS.Business.Objects;
using System.Configuration;
using MoCS.Business.Facade;
//using MoCS.Data;


namespace MoCS.Service
{

    [ServiceBehavior]
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MoCSService : IMoCSService
    {

        private ClientFacade _facade = null;

        string baseFilePath = AppDomain.CurrentDomain.BaseDirectory; // @"C:\Projects\Atos-Origin\XPG\MoCS\MoCS.Service\State";

        IncomingWebRequestContext inRequest = WebOperationContext.Current.IncomingRequest;
        OutgoingWebResponseContext outResponse = WebOperationContext.Current.OutgoingResponse;

        //string basePath = "/MoCS.Service/MoCSService.svc";
        string basePath = "/MoCSService.svc";

        public MoCSService()
        {
            //string path = ConfigurationManager.AppSettings["AssignmentBasePath"];
            //string connectionString = ConfigurationManager.ConnectionStrings["MoCS"].ConnectionString;
            //SQLDataAccess da = new SQLDataAccess(connectionString);
            //_facade = new ClientFacade(da, path);
            _facade = new ClientFacade();
        }

        #region Teams resource

        [OperationBehavior]
        //[WebGet(UriTemplate = "/teams")]
        public TeamsContract GetAllTeams()
        {
            TeamContract authTeam = Authenticate();

            List<Team> teams = _facade.GetTeams();

            TeamsContract resultTeams = new TeamsContract();
            foreach (Team t in teams)
            {
                if (t.Id != authTeam.Id || !authTeam.IsAdmin)
                {
                    t.Password = "";
                }
                resultTeams.Add(CreateDCTeam(t));
            }

            return resultTeams;
        }

        [OperationBehavior]
        //[WebGet(UriTemplate = "/teams?name={teamName}")]
        public TeamContract GetTeamByName(string teamName)
        {
            TeamContract authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
            }

            Team t = _facade.GetTeamByName(teamName);

            if (t != null)
            {
                if (t.Id == authTeam.Id || authTeam.IsAdmin)
                {
                    return CreateDCTeam(t);
                }
                else
                {
                    t.Password = "";
                    return CreateDCTeam(t);
                }
            }
            else
            {
                ReturnWithStatusAndDescription(HttpStatusCode.NotFound, "Resource not found.");
                return null;
            }
        }

        [OperationBehavior]
        //[WebGet(UriTemplate = "/teams/{teamId}")]
        public TeamContract GetTeam(string teamIdString)
        {
            TeamContract authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
            }

            int teamId;


            if (!int.TryParse(teamIdString, out teamId))
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "teamId must be an integer.");
                return null;
            }

            if (authTeam.Id == teamId)
            {
                return authTeam;
            }

            //A team other than the authenticated team is requested
            Team t = _facade.GetTeamById(teamId);

            TeamContract foundTeam = CreateDCTeam(t);

            if (foundTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.NotFound, "Resource not found.");
                return null;
            }
            else
            {
                if (authTeam.IsAdmin)
                {
                    return foundTeam;
                }
                else
                {
                    foundTeam.Password = "";
                    return foundTeam;

                }
            }
        }

        [OperationBehavior]
        //[WebInvoke(Method = "POST", UriTemplate = "/teams")]
        public TeamContract AddTeam(TeamContract team)
        {

            TeamContract authTeam = Authenticate();
            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
            }

            if (authTeam.IsAdmin)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not authorized to add a resource.");
            }

            Team beTeam = new Team();

            beTeam.IsAdmin = team.IsAdmin;
            //beTeam.Id = Convert.ToInt32(team.Id);
            beTeam.Password = team.Password;
            beTeam.Members = team.Members;
            beTeam.Name = team.Name;
            beTeam.Score = 0;

            beTeam = _facade.SaveTeam(beTeam);

            team.Id = beTeam.Id;

            return team;
        }

        [OperationBehavior]
        //[WebInvoke(Method = "PUT", UriTemplate = "/teams/{teamId}")]
        public TeamContract UpdateTeam(string teamIdString, TeamContract team)
        {
            TeamContract authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            int teamId;

            if (!int.TryParse(teamIdString, out teamId))
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "teamId must be an integer.");
                return null;
            }

            if (team.Id != teamId)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Cannot change TeamId (teamId of the resource you're trying to update (" + teamId + ") is different from the teamId you supplied (" + team.Id + ").");
                return null;
            }

            if (authTeam.Id != team.Id && !authTeam.IsAdmin)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.Forbidden, "You are not authorized to change this resource.");
                return null;
            }

            //update
            Team facadeTeam = _facade.GetTeamById(team.Id);

            //update the team           
            if (authTeam.IsAdmin || (authTeam.Id == teamId))
            {
                facadeTeam.Name = team.Name;
                facadeTeam.Password = team.Password;
            }

            if (authTeam.IsAdmin)
            {
                facadeTeam.IsAdmin = team.IsAdmin;
            }

            //update through the facade
            _facade.SaveTeam(facadeTeam);

            outResponse.StatusCode = HttpStatusCode.Created;
            outResponse.StatusDescription = basePath + "/teams/" + team.Id;
            return team;
        }

        [OperationBehavior]
        //[WebGet(UriTemplate = "/teams/{teamId}/assignmentenrollments?tournamentId={tournamentId}")]
        public AssignmentEnrollmentsContract GetAssignmentEnrollmentsForTeamForTournament(string teamIdString, string tournamentIdString)
        {
            AssignmentEnrollmentsContract result = new AssignmentEnrollmentsContract();

            List<AssignmentEnrollment> beAEList = _facade.GetAssignmentEnrollmentsForTeamForTournament(

            return result;
            throw new NotImplementedException();
        }

        [OperationBehavior]
        //[WebInvoke(UriTemplate = "/teams/{teamId}/assignmentenrollments")]
        public AssignmentEnrollmentContract AddAssignmentEnrollmentsForTeam(string teamId, AssignmentEnrollmentContract assignmentEnrollment)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region tournaments resource

        //[WebGet(UriTemplate = "/tournaments")]
        [OperationBehavior]
        public TournamentsContract GetTournaments()
        {
            TeamContract authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            List<Tournament> tournaments = _facade.GetTournaments();
            TournamentsContract result = new TournamentsContract();
            foreach (Tournament t in tournaments)
            {
                TournamentContract tr = new TournamentContract();
                result.Add(tr);
                tr.Id = t.Id;
                tr.Name = t.Name;
            }
            return result;
        }

        //[WebGet(UriTemplate = "/tournaments/{tournamentId}")]
        [OperationBehavior]
        public TournamentContract GetTournament(string tournamentIdString)
        {
            TeamContract authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            int tournamentId;

            if (!int.TryParse(tournamentIdString, out tournamentId))
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "tournamentId must be an integer.");
                return null;
            }

            TournamentContract result = new TournamentContract();

            Tournament beTournament = _facade.GetTournamentById(tournamentId);

            if (beTournament != null)
            {
                result.Id = beTournament.Id;
                result.Name = beTournament.Name;

                return result;
            }
            else
            {
                ReturnWithStatusAndDescription(HttpStatusCode.NotFound, "Tournament not found.");
                return null;

            }


        }

        [OperationBehavior]
        //[WebGet(UriTemplate = "tournaments/{tournamentId}/tournamentassignments")]
        public TournamentAssignmentsContract GetTournamentAssignmentsForTournament(string tournamentIdString)
        {
            TeamContract authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            int tournamentId;

            if (!int.TryParse(tournamentIdString, out tournamentId))
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "tournamentId must be an integer.");
                return null;
            }

            TournamentAssignmentsContract result = new TournamentAssignmentsContract();
            List<TournamentAssignment> beTAList = new List<TournamentAssignment>();

            beTAList = _facade.GetTournamentAssignmentsForTournament(tournamentId);

            foreach (TournamentAssignment ta in beTAList)
            {
                result.Add(CreateDCTournamentAssignment(ta));
            }

            return result;
        }

        #endregion

        #region tournamentassignments

        [OperationBehavior]
        //[WebGet(UriTemplate = "/tournamentassignments/{tournamentassignmentId}")]
        public TournamentAssignmentContract GetTournamentAssignment(string tournamentAssignmentIdString)
        {
            TeamContract authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            TournamentAssignmentContract result;

            int tournamentAssignmentId;

            if (!int.TryParse(tournamentAssignmentIdString, out tournamentAssignmentId))
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "tournamentAssignmentId must be an integer.");
                return null;
            }

            TournamentAssignment beTA = _facade.GetTournamentAssignmentById(tournamentAssignmentId);

            if (beTA != null)
            {
                result = CreateDCTournamentAssignment(beTA);
                return result;
            }
            else
            {
                ReturnWithStatusAndDescription(HttpStatusCode.NotFound, "TournamentAssignment not found.");
                return null;
            }
        }

        [OperationBehavior]
        //[WebGet(UriTemplate = "/tournamentassignments/{tournamentassignmentId}/assignmentzipfile")]
        public AssignmentZipFileContract GetAssignmentZipFileForTournamentAssignment(string tournamentassignmentId)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        //[WebGet(UriTemplate = "/tournamentassignments/{tournamentassignmentId}/assignmentfiles")]
        public AssignmentFilesContract GetAssignmentFilesForTournamentAssignment(string tournamentassignmentId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region assignmentenrollment/submits

        [OperationBehavior]
        public SubmitsContract GetSubmitsForAssignmentEnrollment(string assignmentenrollmentId)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public AssignmentFileContract GetAssignmentFileForSubmit(string assignmentenrollmentId, string submitId)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        public SubmitContract AddSubmitForAssignmentEnrollment(string assignmentenrollmentId, SubmitContract submit)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region helpers

        private TeamContract Authenticate()
        {
            string username = inRequest.Headers["Username"];
            string password = inRequest.Headers["Password"];

            #region Kan worden gebruikt voor HMAC authorization
            //string authorization = inRequest.Headers["Authorization"];

            //string username = authorization.Substring(0, authorization.IndexOf(':') - 1);
            //string providedHash = authorization.Substring(authorization.IndexOf(':') + 1);

            //Team businessTeam = _facade.GetTeam(username);

            //byte[] key = UTF8Encoding.UTF8.GetBytes(businessTeam.Password);

            //HMACMD5 hmacMD5 = new HMACMD5(key);
            //string computedHash = UTF8Encoding.UTF8.GetString(hmacMD5.ComputeHash(inRequest.serializedStream()));

            //if (computedHash==providedHash)
            //{

            //} 
            #endregion

            if (username == null || password == null)
            {
                return null;
            }

            Team businessTeam = _facade.GetTeamByName(username);

            if (businessTeam.Password == password && businessTeam.Name.ToLower() == username.ToLower())
            {
                TeamContract t = new TeamContract();

                t.Name = businessTeam.Name;
                t.Id = businessTeam.Id;
                t.Score = businessTeam.Score;
                t.Members = businessTeam.Members;
                t.IsAdmin = businessTeam.IsAdmin;

                if (businessTeam.IsAdmin || (businessTeam.Name.ToLower() == username.ToLower()))
                {
                    t.Password = businessTeam.Password;
                }

                return t;
            }

            return null;
        }

        private void ReturnWithStatusAndDescription(HttpStatusCode statusCode, string Description)
        {
            int httpStatusCode;

            switch (statusCode)
            {

                case HttpStatusCode.BadRequest:
                    httpStatusCode = 400;
                    break;
                case HttpStatusCode.Forbidden:
                    httpStatusCode = 403;
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
        #endregion

        #region Helpers voor translatie tussen DataContracts <-> Business Entities

        private TeamContract CreateDCTeam(Team team)
        {
            TeamContract newTeam = new TeamContract();

            newTeam.Name = team.Name;
            newTeam.Id = team.Id;
            newTeam.Score = team.Score;
            newTeam.Password = team.Password;
            newTeam.Members = team.Members;
            newTeam.IsAdmin = team.IsAdmin;

            return newTeam;
        }

        private SubmitContract CreateDCSubmit(Submit submit)
        {
            SubmitContract s = new SubmitContract();
            s.Id = submit.Id;
            s.TournamentAssignmentId = submit.TournamentAssignment.Id;
            s.AssignmentId = submit.TournamentAssignment.Assignment.Id;
            s.AssignmentEnrollmentId = submit.AssignmentEnrollment.Id;
            s.TeamId = submit.Team.Id;

            s.IsProcessed = submit.IsProcessed;
            s.FileName = submit.FileName;
            s.SubmitDate = submit.SubmitDate;
            s.Status = submit.Status;
            s.ProcessingDetails = submit.ProcessingDetails;
            s.SecondsSinceEnrollment = submit.SecondsSinceEnrollment;
            s.Data = submit.Data;

            //Team properties
            s.TeamMembers = submit.Team.Members;
            s.TeamName = submit.Team.Name;

            //Assignment properties
            s.AssignmentName = submit.TournamentAssignment.Assignment.Name;

            return s;
        }

        private Assignment CreateBEAssignment(AssignmentContract dcAssignment)
        {
            Assignment result = new Assignment();
            result.Id = dcAssignment.Id;
            result.Name = dcAssignment.Name;
            result.CreateDate = dcAssignment.CreateDate;
            result.Author = dcAssignment.Author;
            result.Version = dcAssignment.Version;
            result.Category = dcAssignment.Category;
            result.Difficulty = dcAssignment.Difficulty;
            result.FriendlyName = dcAssignment.FriendlyName;
            result.Tagline = dcAssignment.Tagline;

            return result;
        }

        private AssignmentContract CreateDCAssignment(Assignment beAssignment)
        {
            AssignmentContract result = new AssignmentContract();

            result.Id = beAssignment.Id;
            result.Name = beAssignment.Name;
            result.CreateDate = beAssignment.CreateDate;
            result.Author = beAssignment.Author;
            result.Version = beAssignment.Version;
            result.Category = beAssignment.Category;
            result.Difficulty = beAssignment.Difficulty;
            result.FriendlyName = beAssignment.FriendlyName;
            result.Tagline = beAssignment.Tagline;

            return result;
        }

        private TournamentAssignmentContract CreateDCTournamentAssignment(TournamentAssignment beTA)
        {
            TournamentAssignmentContract result = new TournamentAssignmentContract();

            result.AssignmentId = beTA.Assignment.Id;
            result.TournamentId = beTA.Tournament.Id;
            result.Id = beTA.Id;

            result.AssignmentOrder = beTA.AssignmentOrder;
            result.Points1 = beTA.Points1;
            result.Points2 = beTA.Points2;
            result.Points3 = beTA.Points3;
            result.CreateDate = beTA.CreateDate;
            result.IsActive = beTA.IsActive;

            result.AssignmentName = beTA.Assignment.Name;
            result.Author = beTA.Assignment.Author;
            result.Version = beTA.Assignment.Version;
            result.Category = beTA.Assignment.Category;
            result.Difficulty = beTA.Assignment.Difficulty;
            result.FriendlyName = beTA.Assignment.FriendlyName;
            result.Tagline = beTA.Assignment.Tagline;

            return result;
        }

        #endregion
    }
}
