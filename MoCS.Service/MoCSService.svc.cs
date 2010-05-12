using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using dc = MoCS.Service.DataContracts;
using Microsoft.ServiceModel.Web;
using System.Net;
using System.ServiceModel.Web;
using System.IO;
using System.Xml;
using System.Reflection;
using MoCS.Client.Business;
using System.Configuration;
using be = MoCS.Client.Business.Entities;
using MoCS.Client.Business.Entities;
using MoCS.Client.Data;


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
            string path = ConfigurationManager.AppSettings["AssignmentBasePath"];
            string connectionString = ConfigurationManager.ConnectionStrings["MoCS"].ConnectionString;
            DataAccess da = new DataAccess(connectionString);
            _facade = new ClientFacade(da, path);
        }

        #region IMoCSService Members - Teams resource

        [OperationBehavior]
        //[WebGet(UriTemplate = "/teams")]
        public dc.Teams GetAllTeams()
        {
            dc.Team authTeam = Authenticate();

            List<be.Team> teams = _facade.GetTournamentTeams(1);

            dc.Teams resultTeams = new dc.Teams();
            foreach (be.Team t in teams)
            {
                resultTeams.Add(CreateDCTeam(t));
            }

            return resultTeams;
        }






        [OperationBehavior]
        //[WebGet(UriTemplate = "/teams/{teamId}")]
        public dc.Team GetTeam(string teamId)
        {
            dc.Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
            }

            if (authTeam.TeamId == teamId)
            {
                return authTeam;
            }

            //not me...
            be.Team t = _facade.GetTeam(teamId);

            dc.Team foundTeam = CreateDCTeam(t);

            if (foundTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.NotFound, "Resource not found.");
                return null;
            }
            else
            {
                if (authTeam.TeamType == dc.TeamType.Administrator)
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
        public dc.Team AddTeam(dc.Team team)
        {

            dc.Team authTeam = Authenticate();
            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
            }

            if (authTeam.TeamType != dc.TeamType.Administrator)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not authorized to add a resource.");
            }

            be.Team beTeam = new be.Team();
            if (team.TeamType == dc.TeamType.Administrator)
            {
                beTeam.IsAdmin = true;
            }
            else
            {
                beTeam.IsAdmin = false;
            }
            beTeam.ID = Convert.ToInt32(team.TeamId);
            beTeam.Password = team.Password;
            beTeam.TeamMembers = team.TeamMembers;
            beTeam.TeamName = team.TeamName;

            beTeam = _facade.SaveTeam(beTeam);

            team.TeamId = beTeam.ID.ToString();

            return team;
        }

        [OperationBehavior]
        //[WebInvoke(Method = "PUT", UriTemplate = "/teams/{teamId}")]
        public dc.Team UpdateTeam(string teamId, dc.Team team)
        {
            dc.Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            if (team.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Cannot change TeamId (teamId of the resource you're trying to update (" + teamId + ") is different from the teamId you supplied (" + team.TeamId + ").");
                return null;
            }

            if (team.TeamStatus == MoCS.Service.DataContracts.TeamStatus.Closed)
            {
                //delete
                _facade.DeleteTeam(Convert.ToInt32(teamId));
                return null;
            }


            //update
            be.Team facadeTeam = _facade.GetTeam(int.Parse(team.TeamId));

            //update the team
            facadeTeam.TeamName = team.TeamName;
            //facadeTeam.Username = team.Username;
            facadeTeam.Password = team.Password;
            facadeTeam.IsAdmin = (team.TeamType == MoCS.Service.DataContracts.TeamType.Administrator);

            //update through the facade
            _facade.SaveTeam(facadeTeam);

            outResponse.StatusCode = HttpStatusCode.Created;
            outResponse.StatusDescription = basePath + "/teams/" + team.TeamId;
            return team;
        }

        #endregion

        #region teams/{teamId}/tournaments

        //[WebGet(UriTemplate = "teams/{teamId}/tournaments")]
        [OperationBehavior]
        public dc.Tournaments GetTeamTournaments(string teamId)
        {
            throw new NotImplementedException();
        }

        //[WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}")]
        [OperationBehavior]
        public dc.Tournament GetTeamTournament(string teamId, string tournamentId)
        {
            throw new NotImplementedException();
        }


        #region IMoCSService Members

        [OperationBehavior]
        MoCS.Service.DataContracts.Assignment IMoCSService.AddTeamTournamentAssignment(MoCS.Service.DataContracts.Assignment assignment)
        {
            be.TeamTournamentAssignment a = new be.TeamTournamentAssignment();
            a.TeamTournamentAssignmentId = -1;
            a.TeamId = int.Parse(assignment.TeamId);
            a.TournamentAssignmentId = int.Parse(assignment.TournamentAssignmentId);
            a = _facade.SaveTeamTournamentAssignment(a);
            assignment.TeamTournamentAssignmentId = a.TeamTournamentAssignmentId.ToString();
            return assignment;
        }

        #endregion


        ////[WebInvoke(Method = "POST", UriTemplate = "/teamtournamentassignments")]
        //[OperationBehavior]
        //dc.Assignment AddTeamTournamentAssignment(dc.Assignment assignment)
        //{
        //    be.TeamTournamentAssignment a = new be.TeamTournamentAssignment();
        //    a.AssignmentId = int.Parse(assignment.AssignmentId);
        //    a.TournamentAssignmentId = int.Parse(assignment.TournamentAssignmentId);
            
        //    a = _facade.SaveTeamTournamentAssignment(a);

        //    assignment.TeamTournamentAssignmentId = a.TeamTournamentAssignmentId.ToString();

        //    return assignment;
        //}


        //[WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments")]
        [OperationBehavior]
        public dc.Assignments GetTeamTournamentAssignments(string teamId, string tournamentId)
        {
           List<TeamTournamentAssignment> assignments = _facade.GetTeamTournamentAssignments(int.Parse(tournamentId), int.Parse(teamId));

           dc.Assignments dcAssignments = new MoCS.Service.DataContracts.Assignments();

           foreach (be.TeamTournamentAssignment beAssignment in assignments)
           {
               dc.Assignment dcAssignment = CreateDCAssignment(beAssignment);
               dcAssignments.Add(dcAssignment);
           }

           return dcAssignments;
        }

        //[WebGet(UriTemplate = "tournamentreports/{tournamentId}")]
        [OperationBehavior]
        public string GetTournamentReport(string tournamentId)
        {
            return _facade.GetTournamentReport(int.Parse(tournamentId));
        }


        //[WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}")]
        [OperationBehavior]
        public dc.Assignment GetTeamTournamentAssignment(string teamId, string tournamentId, string assignmentId)
        {
            throw new NotImplementedException();
        }

        ////[WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}/submits/{submitId}")]
        //[OperationBehavior]
        //public dc.Submit GetTeamTournamentAssignmentSubmit(string teamId, string tournamentId, string assignmentId, string submitId)
        //{
        //    throw new NotImplementedException();
        //}

        //[WebInvoke(Method = "PUT", UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}")]
        [OperationBehavior]
        public dc.Assignment UpdateTeamTournamentAssignment(string teamId, string tournamentId, string assignmentId, dc.Assignment assignment)
        {
            throw new NotImplementedException();
        }

        //[WebInvoke(Method="POST", UriTemplate="teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}/submits")]
        [OperationBehavior]
        public dc.Submit AddTeamTournamentAssignmentSubmit(string teamId, string tournamentId, string assignmentId, dc.Submit submit)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMoCSService Members - teams/{teamId}/assignments resource

        [OperationBehavior]
        //[WebGet(UriTemplate = "/teams/{teamId}/assignments")]
        public dc.Assignments GetAllAssignmentsForTeam(string teamId)
        {
            dc.Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
                return null;
            }

            List<be.TeamTournamentAssignment> beAssignments = _facade.GetTeamTournamentAssignments(1, int.Parse(teamId));
            

            dc.Assignments dcAssignments = new dc.Assignments();

            foreach (be.TeamTournamentAssignment beAssignment in beAssignments)
            {
                dc.Assignment dcAssignment = CreateDCAssignment(beAssignment);
                dcAssignments.Add(dcAssignment);
            }

            return dcAssignments;

        }


        [OperationBehavior]
        //[WebGet(UriTemplate = "teams/{teamId}/assignments/{teamTournamentAssignmentId}")]
        public dc.Assignment GetTeamTournamentAssignment(string teamId, string teamTournamentAssignmentId)
        {
            dc.Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
                return null;
            }


            be.TeamTournamentAssignment beAssignment = _facade.GetTeamTournamentAssignment(Convert.ToInt32(teamTournamentAssignmentId));

            if (beAssignment == null)
                return null;

            dc.Assignment dcAssignment = CreateDCAssignment(beAssignment);

            return dcAssignment;
        }

        [OperationBehavior]
        //[WebInvoke(Method = "POST", UriTemplate = "/teams/{teamId}/assignments")]
        public dc.Assignment AddAssignmentForTeam(string teamId, dc.Assignment assignment)
        {
            dc.Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
                return null;
            }

            TeamTournamentAssignment tta = new TeamTournamentAssignment();
            be.TeamTournamentAssignment beAssignment = _facade.SaveTeamTournamentAssignment(tta);

            dc.Assignment dcAssignment = CreateDCAssignment(beAssignment);

            return dcAssignment;
        }

        #endregion

        #region IMoCSService Members - teams/{teamId}/assignments/{assignmentId}/submits resource

        [OperationBehavior]
        //[WebGet(UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits")]
        public dc.Submits GetAllSubmitsForAssignmentForTeam(string tournamentAssignmentId)
        {
            dc.Team authTeam = Authenticate();
            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            //if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
            //{
            //    ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
            //    return null;
            //}

            List<be.Submit> mySubmits = _facade.GetTeamSubmitsForAssignment(Convert.ToInt32(tournamentAssignmentId));

            dc.Submits result = new dc.Submits();

            foreach (be.Submit submit in mySubmits)
            {
                dc.Submit s = new dc.Submit();
                result.Add(s);

                s.SubmitId = submit.ID.ToString();
                s.CurrentStatusCode = submit.CurrentStatus;
                s.Details = submit.Details;
                s.FileName = submit.FileName;
                s.IsFinished = submit.IsFinished;
                s.StartDate = submit.StartDate;
                s.StatusDate = submit.StatusDate;
                s.SubmitDate = submit.SubmitDate;
                s.TeamMembers = submit.TeamMembers;
                s.FileContents = submit.FileContents;

                s.TeamName = submit.TeamName;
            }

            return result;
        }

        [OperationBehavior]
        public dc.Submit GetTeamSubmit(string teamSubmitId)
        {
            dc.Team authTeam = Authenticate();
            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            Submit submit = _facade.GetTeamSubmit(int.Parse(teamSubmitId));

            dc.Submit result = CreateDCSubmit(submit);

            result.Details = submit.Details;

            System.Text.ASCIIEncoding sx = new ASCIIEncoding();
            result.Payload = sx.GetBytes(submit.FileContents);
            result.FileContents = submit.FileContents;

            return result;
        }



        [OperationBehavior]
        //[WebGet(UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits/{submitId}")]
        public dc.Submit GetSubmitForAssignmentForTeam(string teamId, string assignmentId, string submitId)
        {
            //dc.Team authTeam = Authenticate();
            //if (authTeam == null)
            //{
            //    ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
            //    return null;
            //}

            //if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
            //{
            //    ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
            //    return null;
            //}




            //List<be.Submit> submits = _facade.GetTeamSubmitsForAssignment(

            

            //var x = from s in submits
            //        where s.SubmitID.ToString() == submitId
            //        select s;

            //if (x == null)
            //    return null;

            //foreach (var y in x)
            //{
            //    be.Submit singleSubmit = (be.Submit)y;

            //    dc.Submit result = CreateDCSubmit(singleSubmit);
            //    result.Details = singleSubmit.Details;

            //    System.Text.ASCIIEncoding sx = new ASCIIEncoding();
            //    //result.Payload = sx.GetBytes(singleSubmit.FileContents);
            //    result.FileContents = singleSubmit.FileContents;


            //    return result;
            //}

            return null;
        }

        [OperationBehavior]
        //[WebInvoke(Method = "POST", UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits")]
        public dc.Submit AddSubmitForAssignmentForTeam(string teamId, string assignmentId, dc.Submit submit)
        {
            dc.Team authTeam = Authenticate();
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

            string username = inRequest.Headers["Username"];
            string password = inRequest.Headers["Password"];

          //  _facade.Upload(username, password, Convert.ToInt32(assignmentId), submit.FileName, submit.Payload);

            //get all the previous submits for this assignment
            dc.Submits currentSubmits = GetAllSubmitsForAssignmentForTeam(submit.TeamTournamentAssignmentId);


            //check if there is a successful submit already
            int numberOfSuccesfulSubmits = (from x in currentSubmits
                                     where x.IsFinished == true
                                     where x.CurrentStatusCode == 1
                                     select x).Count();

            if (numberOfSuccesfulSubmits > 0)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.PreconditionFailed, "You already submitted a succesful solution");
                return null;
            }

            //check if there are unprocessed submits
            int nonProceccesSubmits = (from x in currentSubmits
                         where x.IsFinished == false
                         where x.CurrentStatusCode == 0
                         select x).Count();


            if (nonProceccesSubmits > 0)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.PreconditionFailed, "There are still unprocessed submits. Wait until this is processed");
                return null;
            }

            Submit subm = new Submit();

            subm.ID = -1;
            subm.FileName = submit.FileName;
            subm.UploadStream = submit.Payload;
            subm.TeamId = int.Parse(authTeam.TeamId);
            subm.TeamTournamentAssignmentId = int.Parse(submit.TeamTournamentAssignmentId);
            _facade.InsertSubmit(subm);

            return submit; //TODO: return the submit containing the id
        }

        #endregion

        #region assignments resource

        [OperationBehavior]
        //[WebGet(UriTemplate = "/assignments")]
        public dc.Assignments GetTournamentAssignments(string tournamentId)
        {
            dc.Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            List<Assignment> assignments = new List<Assignment>();

            dc.Assignments result = new dc.Assignments();

            List<TournamentAssignment> tournAssignments = new List<TournamentAssignment>();

            if (authTeam.TeamType == dc.TeamType.Administrator)
            {
                tournAssignments = _facade.GetTournamentAssignments(int.Parse(tournamentId));

                foreach (be.TournamentAssignment assignment in tournAssignments)
                {
                    dc.Assignment a = new dc.Assignment();
                    result.Add(a);
                    a.Active = assignment.Active;
                    a.AssignmentId = assignment.AssignmentId.ToString();
                    a.TournamentAssignmentId = assignment.TournamentAssignmentId.ToString();
                    a.AssignmentName = assignment.AssignmentName;
                    a.DisplayName = assignment.DisplayName;
                    a.Hint = assignment.Hint;
                    a.Points = assignment.Points;
                    a.Difficulty = assignment.Difficulty;
                    a.Category = assignment.Category;
                    a.AssignmentOrder = assignment.AssignmentOrder;
                    a.Points1 = assignment.Points1;
                    a.Points2 = assignment.Points2;
                    a.Points3 = assignment.Points3;

                    a.TournamentAssignmentId = assignment.TournamentAssignmentId.ToString();
                    a.TournamentId = assignment.TournamentId.ToString();
                }

            }
            else
            {
                List<TeamTournamentAssignment> assignments2 = _facade.GetTeamTournamentAssignments(Convert.ToInt32(tournamentId)
                                                                        , Convert.ToInt32(authTeam.TeamId));

                foreach (be.TeamTournamentAssignment assignment in assignments2)
                {
                    dc.Assignment a = new dc.Assignment();
                    result.Add(a);
                    a.Active = assignment.Active;
                    a.AssignmentId = assignment.AssignmentId.ToString();
                    a.AssignmentName = assignment.AssignmentName;
                    a.DisplayName = assignment.DisplayName;
                    a.Hint = assignment.Hint;
                    a.Points = assignment.Points;
                    a.Difficulty = assignment.Difficulty;
                    a.Category = assignment.Category;
                    a.AssignmentOrder = assignment.AssignmentOrder;
                    a.Points1 = assignment.Points1;
                    a.Points2 = assignment.Points2;
                    a.Points3 = assignment.Points3;
                    a.TournamentAssignmentId = assignment.TournamentAssignmentId.ToString();
                    a.TournamentId = assignment.TournamentId.ToString();
                }
            }



            return result;
        }


        //[WebGet(UriTemplate = "/tournaments")]
        [OperationBehavior]
        public List<dc.Tournament> GetTournaments()
        {
            dc.Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            List<be.Tournament> tournaments = _facade.GetTournaments();
            List<dc.Tournament> result = new List<dc.Tournament>();
            foreach (be.Tournament t in tournaments)
            {
                dc.Tournament tr = new dc.Tournament();
                result.Add(tr);
                tr.TournamentId = t.Id.ToString();
                tr.TournamentName = t.Name;
            }
            return result;
        }


        [OperationBehavior]
        //[WebGet(UriTemplate = "/assignments/{assignmentId}")]
        public dc.Assignment GetAssignment(string assignmentId)
        {
            dc.Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            be.Assignment assignment = null;
            dc.Assignment result = new dc.Assignment();

            if (authTeam.TeamType == dc.TeamType.Administrator)
            {
                assignment = _facade.GetAssignment(int.Parse(assignmentId));
            }
            else
            {
                // I've decided to get the full list of tournament assignments and filter on it. 
                // I'm not touching sp's or the facade at the moment!
                List<be.TeamTournamentAssignment> assignments = _facade.GetTeamTournamentAssignments(1, Convert.ToInt32(authTeam.TeamId));

                var query = from a in assignments
                            where a.AssignmentId == int.Parse(assignmentId) && a.Active == true
                            select a;
                if (query.Count() > 0)
                {
                    //TODO: fix this
                  //  assignment = query.First();
                }
                else
                {
                    ReturnWithStatusAndDescription(HttpStatusCode.Forbidden, "You are not authorized to view assignment with id: " + assignmentId);
                    return null;
                }
            }

            if (assignment != null)
            {
                result.Active = assignment.Active;
                result.AssignmentId = assignment.AssignmentId.ToString();
                result.AssignmentName = assignment.AssignmentName;
                result.DisplayName = assignment.DisplayName;
                result.Hint = assignment.Hint;
                result.Points = assignment.Points;
                result.Difficulty = assignment.Difficulty;
                result.Category = assignment.Category;
            }

            return result;
        }

        [OperationBehavior]
        //[WebInvoke(Method = "POST", UriTemplate = "/assignments")]
        public dc.Assignment AddAssignment(dc.Assignment assignment)
        {
            throw new NotImplementedException();
        }

        [OperationBehavior]
        //[WebInvoke(Method = "PUT", UriTemplate = "/assignments/{assignmentId}")]
        public dc.Assignment UpdateAssignment(string assignmentId, dc.Assignment assignment)
        {
            dc.Team authTeam = Authenticate();

            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            be.Assignment beAssignment;
            dc.Assignment result = new dc.Assignment();

            if (authTeam.TeamType == dc.TeamType.Administrator)
            {
                try
                {
                    beAssignment = CreateBEAssignment(assignment);
                    be.Assignment beResult = _facade.SaveAssignment(beAssignment);
                    result = CreateDCAssignment(beResult);
                }
                catch (Exception ex)
                {
                    ReturnWithStatusAndDescription(HttpStatusCode.InternalServerError, "An internal server error has occurred. Details: " + ex.Message);
                    return null;
                }

            }
            else
            {
                ReturnWithStatusAndDescription(HttpStatusCode.Forbidden, "You are not authorized to update assignment with id: " + assignmentId);
                return null;
            }

            return result;
        }

        #endregion

        #region tournaments resource

        [OperationBehavior]
        //[WebGet(UriTemplate = "/tournament/{tournamentId}/submits")]
        public dc.Submits GetTournamentSubmits(string tournamentId)
        {
            List<be.Submit> submits = _facade.GetTournamentSubmits(int.Parse(tournamentId));

            dc.Submits result = new dc.Submits();

            foreach (be.Submit sub in submits)
            {
                result.Add(CreateDCSubmit(sub));
            }

            return result;
        }

        [OperationBehavior]
        //[WebInvoke(Method = "PUT", UriTemplate = "/tournaments/{tournamentId}/assignments/{assignmentId}")]
        public dc.Assignment UpdateTournamentAssignment(string tournamentId, string assignmentId, dc.Assignment assignment)
        {
            TournamentAssignment a = new TournamentAssignment();
            a.TournamentAssignmentId = int.Parse(assignment.TournamentAssignmentId);
            a.TournamentId = int.Parse(assignment.TournamentId);
            a.AssignmentId = int.Parse(assignment.AssignmentId);
            a.AssignmentOrder = assignment.AssignmentOrder;
            a.Points1 = assignment.Points1;
            a.Points2 = assignment.Points2;
            a.Points3 = assignment.Points3;
            a.Active = assignment.Active;

            a = _facade.SaveTournamentAssignment(a);
            return assignment;


        }

        #endregion

        #region helpers

        private dc.Team Authenticate()
        {
            string username = inRequest.Headers["Username"];
            string password = inRequest.Headers["Password"];

            if (username == null || password == null)
            {
                return null;
            }

            be.Team businessTeam = _facade.GetTeam(username);

            if (businessTeam.Password == password && businessTeam.TeamName.ToLower() == username.ToLower())
            {
                MoCS.Service.DataContracts.Team t = new MoCS.Service.DataContracts.Team();

                t.TeamName = businessTeam.TeamName;
                t.TeamId = businessTeam.ID.ToString();
                t.Points = businessTeam.Points;
                t.TeamMembers = businessTeam.TeamMembers;
                t.TeamStatus = dc.TeamStatus.Active;    //zijn allemaal active
                //t.Username = businessTeam.TeamName; //teamname = username
                if (businessTeam.IsAdmin)
                {
                    t.TeamType = dc.TeamType.Administrator;
                    t.Password = businessTeam.Password;
                }
                else if (businessTeam.TeamName.ToUpper() == username.ToUpper())
                {
                    t.Password = businessTeam.Password;
                }
                else
                {
                    t.TeamType = dc.TeamType.Normal;
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

        private dc.Team CreateDCTeam(be.Team team)
        {
            dc.Team newTeam = new dc.Team();

            newTeam.TeamName = team.TeamName;
            newTeam.TeamId = team.ID.ToString();
            newTeam.Points = team.Points;
            newTeam.TeamStatus = dc.TeamStatus.Active;    //zijn allemaal active
            //newTeam.Username = team.TeamName; //teamname = username
            newTeam.Password = team.Password;
            newTeam.TeamMembers = team.TeamMembers;
            if (team.IsAdmin)
            {
                newTeam.TeamType = dc.TeamType.Administrator;
            }
            else
            {
                newTeam.TeamType = dc.TeamType.Normal;
            }

            return newTeam;
        }

        public void DeleteSubmit(string teamSubmitId)
        {
            _facade.DeleteTeamSubmit(int.Parse(teamSubmitId));
        }


        private dc.Submit CreateDCSubmit(be.Submit sub)
        {
            dc.Submit s = new dc.Submit();
            s.SubmitId = sub.SubmitID.ToString();
            s.AssignmentId = sub.AssignmentId.ToString();
            s.TeamTournamentAssignmentId = sub.TeamTournamentAssignmentId.ToString();
            s.TeamId = sub.TeamId.ToString();
            s.IsFinished = sub.IsFinished;
            s.FileName = sub.FileName;
            s.StartDate = sub.StartDate;
            s.SubmitDate = sub.SubmitDate;
            s.StatusDate = sub.StatusDate;
            s.TeamMembers = sub.TeamMembers;
            s.TeamName = sub.TeamName;
            s.CurrentStatusCode = sub.CurrentStatus;
            s.TournamentAssignmentId = sub.TournamentAssignmentId;
            s.TeamName = sub.TeamName;
            s.TeamMembers = sub.TeamMembers;

            return s;
        }

        private be.Assignment CreateBEAssignment(dc.Assignment dcAssignment)
        {
            be.Assignment result = new be.Assignment();
            result.Active = dcAssignment.Active;
            //result.Author
            result.Category = dcAssignment.Category;
            result.Difficulty = dcAssignment.Difficulty;
            result.DisplayName = dcAssignment.DisplayName;
            result.Hint = dcAssignment.Hint;
            result.AssignmentId = int.Parse(dcAssignment.AssignmentId);
            //result.IsValid
            result.AssignmentName = dcAssignment.AssignmentName;
            result.Points = dcAssignment.Points;
            if (dcAssignment.StartDate.HasValue)
            {
        //        result.StartDate = dcAssignment.StartDate.Value;
            }

            result.ZipFile = dcAssignment.Zipfile;

            return result;
        }

        private dc.Assignment CreateDCAssignment(be.Assignment beAssignment)
        {
            dc.Assignment result = new dc.Assignment();

            result.Active = beAssignment.Active;
            //result.Author
            result.Category = beAssignment.Category;
            result.Difficulty = beAssignment.Difficulty;
            result.DisplayName = beAssignment.DisplayName;
            result.Hint = beAssignment.Hint;
            result.AssignmentId = beAssignment.AssignmentId.ToString();
            //result.IsValid
            result.AssignmentName = beAssignment.AssignmentName;
            result.Points = beAssignment.Points;
         //   result.StartDate = beAssignment.StartDate;
            result.Zipfile = beAssignment.ZipFile;

            result.Files = new Dictionary<string, dc.AssignmentFile>();
            foreach (string key in beAssignment.Files.Keys)
            {
                dc.AssignmentFile f = new dc.AssignmentFile();
                f.Name = beAssignment.Files[key].Name;
                f.Contents = beAssignment.Files[key].Contents;
                result.Files.Add(key, f);
            }

            return result;
        }


        private dc.Assignment CreateDCAssignment(be.TeamTournamentAssignment beAssignment)
        {
            dc.Assignment result = new dc.Assignment();

            //ID's
            result.AssignmentId = beAssignment.AssignmentId.ToString();
            result.TournamentId = beAssignment.TournamentId.ToString();
            result.TournamentAssignmentId = beAssignment.TournamentAssignmentId.ToString();
            result.TeamTournamentAssignmentId = beAssignment.TeamTournamentAssignmentId.ToString();
            

            result.Active = beAssignment.Active;
           // result.Author = beAssignment.Author;
            result.Category = beAssignment.Category;
            result.Difficulty = beAssignment.Difficulty;
            result.DisplayName = beAssignment.DisplayName;
            result.Hint = beAssignment.Hint;
            result.AssignmentId = beAssignment.AssignmentId.ToString();
            //result.IsValid
            result.AssignmentName = beAssignment.AssignmentName;
            result.Points = beAssignment.Points;
            result.StartDate = beAssignment.StartDate;
            result.Zipfile = beAssignment.ZipFile;

            result.Files = new Dictionary<string, dc.AssignmentFile>();
            foreach (string key in beAssignment.Files.Keys)
            {
                dc.AssignmentFile f = new dc.AssignmentFile();
                f.Name = beAssignment.Files[key].Name;
                f.Contents = beAssignment.Files[key].Contents;
                result.Files.Add(key, f);
            }

            return result;
        }



        #endregion






        #region IMoCSService Members


        public MoCS.Service.DataContracts.Submits GetTeamTournamentAssignmentSubmits(string teamId, string tournamentId, string assignmentId)
        {


            dc.Team authTeam = Authenticate();
            if (authTeam == null)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
                return null;
            }

            if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
            {
                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
                return null;
            }

            List<be.Submit> mySubmits = _facade.GetTeamSubmitsForAssignment(int.Parse(assignmentId));

            dc.Submits result = new dc.Submits();

            foreach (be.Submit submit in mySubmits)
            {
                dc.Submit s = new dc.Submit();
                result.Add(s);

                s.SubmitId = submit.ID.ToString();
                s.CurrentStatusCode = submit.CurrentStatus;
                s.Details = submit.Details;
                s.FileName = submit.FileName;
                s.IsFinished = submit.IsFinished;
                s.StatusDate = submit.StatusDate;
                s.SubmitDate = submit.SubmitDate;
                s.TeamMembers = submit.TeamMembers;
                s.FileContents = submit.FileContents;

                s.TeamName = submit.TeamName;
            }

            return result;






        }

        public MoCS.Service.DataContracts.Submit GetTeamTournamentAssignmentSubmit(string teamId, string tournamentId, string assignmentId, string submitId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
