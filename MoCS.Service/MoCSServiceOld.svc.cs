//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.ServiceModel;
//using System.Text;
//using dc = MoCS.Service.DataContracts;
//using Microsoft.ServiceModel.Web;
//using System.Net;
//using System.ServiceModel.Web;
//using be = MoCS.Business;
//using System.Configuration;
//using MoCS.Business.Facade;
//using MoCS.Data;
//using MoCS.Business;


//namespace MoCS.Service
//{

//    [ServiceBehavior]
//    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
//    public class MoCSServiceOld : IMoCSServiceOld
//    {

//        private ClientFacade _facade = null;

//        string baseFilePath = AppDomain.CurrentDomain.BaseDirectory; // @"C:\Projects\Atos-Origin\XPG\MoCS\MoCS.Service\State";

//        IncomingWebRequestContext inRequest = WebOperationContext.Current.IncomingRequest;
//        OutgoingWebResponseContext outResponse = WebOperationContext.Current.OutgoingResponse;

//        //string basePath = "/MoCS.Service/MoCSService.svc";
//        string basePath = "/MoCSService.svc";

//        public MoCSServiceOld()
//        {
//            string path = ConfigurationManager.AppSettings["AssignmentBasePath"];
//            string connectionString = ConfigurationManager.ConnectionStrings["MoCS"].ConnectionString;
//            SQLDataAccess da = new SQLDataAccess(connectionString);
//            _facade = new ClientFacade(da, path);
//        }

//        #region Teams resource

//        [OperationBehavior]
//        //[WebGet(UriTemplate = "/teams")]
//        public dc.TeamsContract GetAllTeams()
//        {
//            dc.TeamContract authTeam = Authenticate();

//            List<be.Team> teams = _facade.GetTournamentTeams(1);

//            dc.TeamsContract resultTeams = new dc.TeamsContract();
//            foreach (be.Team t in teams)
//            {
//                resultTeams.Add(CreateDCTeam(t));
//            }

//            return resultTeams;
//        }

//        [OperationBehavior]
//        //[WebGet(UriTemplate = "/teams/{teamId}")]
//        public dc.TeamContract GetTeam(string teamId)
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//            }

//            if (authTeam.TeamId == teamId)
//            {
//                return authTeam;
//            }

//            //not me...
//            be.Team t = _facade.GetTeam(teamId);

//            dc.TeamContract foundTeam = CreateDCTeam(t);

//            if (foundTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.NotFound, "Resource not found.");
//                return null;
//            }
//            else
//            {
//                if (authTeam.TeamType == dc.TeamType.Administrator)
//                {
//                    return foundTeam;
//                }
//                else
//                {
//                    foundTeam.Password = "";
//                    return foundTeam;

//                }
//            }
//        }

//        [OperationBehavior]
//        //[WebInvoke(Method = "POST", UriTemplate = "/teams")]
//        public dc.TeamContract AddTeam(dc.TeamContract team)
//        {

//            dc.TeamContract authTeam = Authenticate();
//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//            }

//            if (authTeam.TeamType != dc.TeamType.Administrator)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not authorized to add a resource.");
//            }

//            be.Team beTeam = new be.Team();
//            if (team.TeamType == dc.TeamType.Administrator)
//            {
//                beTeam.IsAdmin = true;
//            }
//            else
//            {
//                beTeam.IsAdmin = false;
//            }
//            beTeam.Id = Convert.ToInt32(team.TeamId);
//            beTeam.Password = team.Password;
//            beTeam.Members = team.TeamMembers;
//            beTeam.Name = team.TeamName;

//            beTeam = _facade.SaveTeam(beTeam);

//            team.TeamId = beTeam.Id.ToString();

//            return team;
//        }

//        [OperationBehavior]
//        //[WebInvoke(Method = "PUT", UriTemplate = "/teams/{teamId}")]
//        public dc.TeamContract UpdateTeam(string teamId, dc.TeamContract team)
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            if (team.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Cannot change TeamId (teamId of the resource you're trying to update (" + teamId + ") is different from the teamId you supplied (" + team.TeamId + ").");
//                return null;
//            }

//            if (team.TeamStatus == MoCS.Service.DataContracts.TeamStatus.Closed)
//            {
//                //delete
//                _facade.DeleteTeam(Convert.ToInt32(teamId));
//                return null;
//            }


//            //update
//            be.Team facadeTeam = _facade.GetTeam(int.Parse(team.TeamId));

//            //update the team
//            facadeTeam.Name = team.TeamName;
//            //facadeTeam.Username = team.Username;
//            facadeTeam.Password = team.Password;
//            facadeTeam.IsAdmin = (team.TeamType == MoCS.Service.DataContracts.TeamType.Administrator);

//            //update through the facade
//            _facade.SaveTeam(facadeTeam);

//            outResponse.StatusCode = HttpStatusCode.Created;
//            outResponse.StatusDescription = basePath + "/teams/" + team.TeamId;
//            return team;
//        }

//        #endregion

//        #region teams/{teamId}/tournaments

//        //[WebGet(UriTemplate = "teams/{teamId}/tournaments")]
//        [OperationBehavior]
//        public dc.TournamentsContract GetTeamTournaments(string teamId)
//        {
//            throw new NotImplementedException();
//        }

//        //[WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}")]
//        [OperationBehavior]
//        public dc.TournamentContract GetTeamTournament(string teamId, string tournamentId)
//        {
//            throw new NotImplementedException();
//        }

//        //[WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId]/assignments/{assignmentId}/submits")]
//        [OperationBehavior]
//        public dc.SubmitsContract GetTeamTournamentAssignmentSubmits(string teamId, string tournamentId, string assignmentId)
//        {
//            dc.TeamContract authTeam = Authenticate();
//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
//                return null;
//            }

//            List<be.Submit> mySubmits = _facade.GetTeamSubmitsForAssignment(int.Parse(assignmentId));

//            dc.SubmitsContract result = new dc.SubmitsContract();

//            foreach (be.Submit submit in mySubmits)
//            {
//                dc.SubmitContract s = new dc.SubmitContract();
//                result.Add(s);

//                s.SubmitId = submit.ID.ToString();
//                s.CurrentStatusCode = submit.CurrentStatus;
//                s.Details = submit.Details;
//                s.FileName = submit.FileName;
//                s.IsFinished = submit.IsFinished;
//                s.StatusDate = submit.StatusDate;
//                s.SubmitDate = submit.SubmitDate;
//                s.TeamMembers = submit.TeamMembers;
//                s.FileContents = submit.FileContents;

//                s.TeamName = submit.TeamName;
//            }

//            return result;
//        }

//        //[WebGet(UriTemplate="/teams/{teamId]/tournaments/{tournamentId}/assignments/{assignmentId}/submits/{submitId}")]
//        [OperationBehavior]
//        public dc.SubmitContract GetTeamTournamentAssignmentSubmit(string teamId, string tournamentId, string assignmentId, string submitId)
//        {
//            throw new NotImplementedException();
//        }

//        //[WebInvoke(UriTemplate="/teams/{teamId}/tournaments/{tournamentId}/assignment")]
//        [OperationBehavior]
//        public dc.AssignmentContract AddTeamTournamentAssignment(dc.AssignmentContract assignment)
//        {
//            be.AssignmentEnrollment a = new be.AssignmentEnrollment();
//            a.TeamTournamentAssignmentId = -1;
//            a.TeamId = int.Parse(assignment.TeamId);
//            a.TournamentAssignmentId = int.Parse(assignment.TournamentAssignmentId);
//            a = _facade.SaveTeamTournamentAssignment(a);
//            assignment.TeamTournamentAssignmentId = a.TeamTournamentAssignmentId.ToString();
//            return assignment;
//        }
        
//        ////[WebInvoke(Method = "POST", UriTemplate = "/teamtournamentassignments")]
//        //[OperationBehavior]
//        //dc.Assignment AddTeamTournamentAssignment(dc.Assignment assignment)
//        //{
//        //    be.TeamTournamentAssignment a = new be.TeamTournamentAssignment();
//        //    a.AssignmentId = int.Parse(assignment.AssignmentId);
//        //    a.TournamentAssignmentId = int.Parse(assignment.TournamentAssignmentId);

//        //    a = _facade.SaveTeamTournamentAssignment(a);

//        //    assignment.TeamTournamentAssignmentId = a.TeamTournamentAssignmentId.ToString();

//        //    return assignment;
//        //}

//        //[WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments")]
//        [OperationBehavior]
//        public dc.AssignmentsContract GetTeamTournamentAssignments(string teamId, string tournamentId)
//        {
//            List<be.AssignmentEnrollment> assignments = _facade.GetTeamTournamentAssignments(int.Parse(tournamentId), int.Parse(teamId));

//            dc.AssignmentsContract dcAssignments = new MoCS.Service.DataContracts.AssignmentsContract();

//            foreach (be.AssignmentEnrollment beAssignment in assignments)
//            {
//                dc.AssignmentContract dcAssignment = CreateDCAssignment(beAssignment);
//                dcAssignments.Add(dcAssignment);
//            }

//            return dcAssignments;
//        }

//                //[WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}")]
//        [OperationBehavior]
//        public dc.AssignmentContract GetTeamTournamentAssignment(string teamId, string tournamentId, string assignmentId)
//        {
//            throw new NotImplementedException();
//        }


//        //[WebInvoke(Method = "PUT", UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}")]
//        [OperationBehavior]
//        public dc.AssignmentContract UpdateTeamTournamentAssignment(string teamId, string tournamentId, string assignmentId, dc.AssignmentContract assignment)
//        {
//            throw new NotImplementedException();
//        }

//        //[WebInvoke(Method="POST", UriTemplate="teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}/submits")]
//        [OperationBehavior]
//        public dc.SubmitContract AddTeamTournamentAssignmentSubmit(string teamId, string tournamentId, string assignmentId, dc.SubmitContract submit)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion

//        #region teams/{teamId}/assignments resource

//        [OperationBehavior]
//        //[WebGet(UriTemplate = "/teams/{teamId}/assignments")]
//        public dc.AssignmentsContract GetAllAssignmentsForTeam(string teamId)
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
//                return null;
//            }

//            List<be.AssignmentEnrollment> beAssignments = _facade.GetTeamTournamentAssignments(1, int.Parse(teamId));


//            dc.AssignmentsContract dcAssignments = new dc.AssignmentsContract();

//            foreach (be.AssignmentEnrollment beAssignment in beAssignments)
//            {
//                dc.AssignmentContract dcAssignment = CreateDCAssignment(beAssignment);
//                dcAssignments.Add(dcAssignment);
//            }

//            return dcAssignments;

//        }


//        [OperationBehavior]
//        //[WebGet(UriTemplate = "teams/{teamId}/assignments/{teamTournamentAssignmentId}")]
//        public dc.AssignmentContract GetTeamTournamentAssignment(string teamId, string teamTournamentAssignmentId)
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
//                return null;
//            }


//            be.AssignmentEnrollment beAssignment = _facade.GetTeamTournamentAssignment(Convert.ToInt32(teamTournamentAssignmentId));

//            if (beAssignment == null)
//                return null;

//            dc.AssignmentContract dcAssignment = CreateDCAssignment(beAssignment);

//            return dcAssignment;
//        }

//        [OperationBehavior]
//        //[WebInvoke(Method = "POST", UriTemplate = "/teams/{teamId}/assignments")]
//        public dc.AssignmentContract AddAssignmentForTeam(string teamId, dc.AssignmentContract assignment)
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
//                return null;
//            }

//            be.AssignmentEnrollment tta = new be.AssignmentEnrollment();
//            be.AssignmentEnrollment beAssignment = _facade.SaveTeamTournamentAssignment(tta);

//            dc.AssignmentContract dcAssignment = CreateDCAssignment(beAssignment);

//            return dcAssignment;
//        }

//        #endregion

//        #region teams/{teamId}/assignments/{assignmentId}/submits resource

//        [OperationBehavior]
//        //[WebGet(UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits")]
//        public dc.SubmitsContract GetAllSubmitsForAssignmentForTeam(string tournamentAssignmentId)
//        {
//            dc.TeamContract authTeam = Authenticate();
//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            //if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
//            //{
//            //    ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
//            //    return null;
//            //}

//            List<be.Submit> mySubmits = _facade.GetTeamSubmitsForAssignment(Convert.ToInt32(tournamentAssignmentId));

//            dc.SubmitsContract result = new dc.SubmitsContract();

//            foreach (be.Submit submit in mySubmits)
//            {
//                dc.SubmitContract s = new dc.SubmitContract();
//                result.Add(s);

//                s.SubmitId = submit.ID.ToString();
//                s.CurrentStatusCode = submit.CurrentStatus;
//                s.Details = submit.Details;
//                s.FileName = submit.FileName;
//                s.IsFinished = submit.IsFinished;
//                s.StartDate = submit.StartDate;
//                s.StatusDate = submit.StatusDate;
//                s.SubmitDate = submit.SubmitDate;
//                s.TeamMembers = submit.TeamMembers;
//                s.FileContents = submit.FileContents;

//                s.TeamName = submit.TeamName;
//            }

//            return result;
//        }

//        [OperationBehavior]
//        public dc.SubmitContract GetTeamSubmit(string teamSubmitId)
//        {
//            dc.TeamContract authTeam = Authenticate();
//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            be.Submit submit = _facade.GetTeamSubmit(int.Parse(teamSubmitId));

//            dc.SubmitContract result = CreateDCSubmit(submit);

//            result.Details = submit.Details;

//            System.Text.ASCIIEncoding sx = new ASCIIEncoding();
//            result.Payload = sx.GetBytes(submit.FileContents);
//            result.FileContents = submit.FileContents;

//            return result;
//        }



//        [OperationBehavior]
//        //[WebGet(UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits/{submitId}")]
//        public dc.SubmitContract GetSubmitForAssignmentForTeam(string teamId, string assignmentId, string submitId)
//        {
//            //dc.Team authTeam = Authenticate();
//            //if (authTeam == null)
//            //{
//            //    ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//            //    return null;
//            //}

//            //if (authTeam.TeamId != teamId && authTeam.TeamType != dc.TeamType.Administrator)
//            //{
//            //    ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
//            //    return null;
//            //}




//            //List<be.Submit> submits = _facade.GetTeamSubmitsForAssignment(



//            //var x = from s in submits
//            //        where s.SubmitID.ToString() == submitId
//            //        select s;

//            //if (x == null)
//            //    return null;

//            //foreach (var y in x)
//            //{
//            //    be.Submit singleSubmit = (be.Submit)y;

//            //    dc.Submit result = CreateDCSubmit(singleSubmit);
//            //    result.Details = singleSubmit.Details;

//            //    System.Text.ASCIIEncoding sx = new ASCIIEncoding();
//            //    //result.Payload = sx.GetBytes(singleSubmit.FileContents);
//            //    result.FileContents = singleSubmit.FileContents;


//            //    return result;
//            //}

//            return null;
//        }

//        [OperationBehavior]
//        //[WebInvoke(Method = "POST", UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits")]
//        public dc.SubmitContract AddSubmitForAssignmentForTeam(string teamId, string assignmentId, dc.SubmitContract submit)
//        {
//            dc.TeamContract authTeam = Authenticate();
//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            if (authTeam.TeamId != teamId)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "You are not registered for team with teamId: " + teamId);
//                return null;
//            }

//            string username = inRequest.Headers["Username"];
//            string password = inRequest.Headers["Password"];

//            //  _facade.Upload(username, password, Convert.ToInt32(assignmentId), submit.FileName, submit.Payload);

//            //get all the previous submits for this assignment
//            dc.SubmitsContract currentSubmits = GetAllSubmitsForAssignmentForTeam(submit.TeamTournamentAssignmentId);


//            //check if there is a successful submit already
//            int numberOfSuccesfulSubmits = (from x in currentSubmits
//                                            where x.IsFinished == true
//                                            where x.CurrentStatusCode == 1
//                                            select x).Count();

//            if (numberOfSuccesfulSubmits > 0)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.PreconditionFailed, "You already submitted a succesful solution");
//                return null;
//            }

//            //check if there are unprocessed submits
//            int nonProceccesSubmits = (from x in currentSubmits
//                                       where x.IsFinished == false
//                                       where x.CurrentStatusCode == 0
//                                       select x).Count();


//            if (nonProceccesSubmits > 0)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.PreconditionFailed, "There are still unprocessed submits. Wait until this is processed");
//                return null;
//            }

//            be.Submit subm = new be.Submit();

//            subm.ID = -1;
//            subm.FileName = submit.FileName;
//            subm.UploadStream = submit.Payload;
//            subm.TeamId = int.Parse(authTeam.TeamId);
//            subm.TeamTournamentAssignmentId = int.Parse(submit.TeamTournamentAssignmentId);
//            _facade.InsertSubmit(subm);

//            return submit; //TODO: return the submit containing the id
//        }

//        #endregion

//        #region assignments resource

//        [OperationBehavior]
//        //[WebGet(UriTemplate = "tournaments/{tournamentId}/assignments")]
//        public dc.AssignmentsContract GetTournamentAssignments(string tournamentId)
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.Forbidden, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            List<be.Assignment> assignments = new List<be.Assignment>();

//            dc.AssignmentsContract result = new dc.AssignmentsContract();

//            List<be.TournamentAssignment> tournAssignments = new List<be.TournamentAssignment>();

//            if (authTeam.TeamType == dc.TeamType.Administrator)
//            {
//                tournAssignments = _facade.GetTournamentAssignments(int.Parse(tournamentId));

//                foreach (be.TournamentAssignment assignment in tournAssignments)
//                {
//                    dc.AssignmentContract a = new dc.AssignmentContract();
//                    result.Add(a);
//                    a.Active = assignment.Active;
//                    a.AssignmentId = assignment.AssignmentId.ToString();
//                    a.TournamentAssignmentId = assignment.TournamentAssignmentId.ToString();
//                    a.AssignmentName = assignment.AssignmentName;
//                    a.DisplayName = assignment.DisplayName;
//                    a.Hint = assignment.Hint;
//                    a.Points = assignment.Points;
//                    a.Difficulty = assignment.Difficulty;
//                    a.Category = assignment.Category;
//                    a.AssignmentOrder = assignment.AssignmentOrder;
//                    a.Points1 = assignment.Points1;
//                    a.Points2 = assignment.Points2;
//                    a.Points3 = assignment.Points3;

//                    a.TournamentAssignmentId = assignment.TournamentAssignmentId.ToString();
//                    a.TournamentId = assignment.TournamentId.ToString();
//                }

//            }
//            else
//            {
//                List<be.AssignmentEnrollment> assignments2 = _facade.GetTeamTournamentAssignments(Convert.ToInt32(tournamentId)
//                                                                        , Convert.ToInt32(authTeam.TeamId));

//                foreach (be.AssignmentEnrollment assignment in assignments2)
//                {
//                    dc.AssignmentContract a = new dc.AssignmentContract();
//                    result.Add(a);
//                    a.Active = assignment.Active;
//                    a.AssignmentId = assignment.AssignmentId.ToString();
//                    a.AssignmentName = assignment.AssignmentName;
//                    a.DisplayName = assignment.DisplayName;
//                    a.Hint = assignment.Hint;
//                    a.Points = assignment.Points;
//                    a.Difficulty = assignment.Difficulty;
//                    a.Category = assignment.Category;
//                    a.AssignmentOrder = assignment.AssignmentOrder;
//                    a.Points1 = assignment.Points1;
//                    a.Points2 = assignment.Points2;
//                    a.Points3 = assignment.Points3;
//                    a.TournamentAssignmentId = assignment.TournamentAssignmentId.ToString();
//                    a.TournamentId = assignment.TournamentId.ToString();
//                }
//            }



//            return result;
//        }

//        [OperationBehavior]
//        //[WebGet(UriTemplate = "/assignments/{assignmentId}")]
//        public dc.AssignmentContract GetAssignment(string assignmentId)
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            be.Assignment assignment = null;
//            dc.AssignmentContract result = new dc.AssignmentContract();

//            if (authTeam.TeamType == dc.TeamType.Administrator)
//            {
//                assignment = _facade.GetAssignment(int.Parse(assignmentId));
//            }
//            else
//            {
//                // I've decided to get the full list of tournament assignments and filter on it. 
//                // I'm not touching sp's or the facade at the moment!
//                List<be.AssignmentEnrollment> assignments = _facade.GetTeamTournamentAssignments(1, Convert.ToInt32(authTeam.TeamId));

//                var query = from a in assignments
//                            where a.AssignmentId == int.Parse(assignmentId) && a.Active == true
//                            select a;
//                if (query.Count() > 0)
//                {
//                    //TODO: fix this
//                    //  assignment = query.First();
//                }
//                else
//                {
//                    ReturnWithStatusAndDescription(HttpStatusCode.Forbidden, "You are not authorized to view assignment with id: " + assignmentId);
//                    return null;
//                }
//            }

//            if (assignment != null)
//            {
//                result.Active = assignment.Active;
//                result.AssignmentId = assignment.AssignmentId.ToString();
//                result.AssignmentName = assignment.AssignmentName;
//                result.DisplayName = assignment.DisplayName;
//                result.Hint = assignment.Hint;
//                result.Points = assignment.Points;
//                result.Difficulty = assignment.Difficulty;
//                result.Category = assignment.Category;
//            }

//            return result;
//        }

//        [OperationBehavior]
//        //[WebInvoke(Method = "POST", UriTemplate = "/assignments")]
//        public dc.AssignmentContract AddAssignment(dc.AssignmentContract assignment)
//        {
//            throw new NotImplementedException();
//        }

//        [OperationBehavior]
//        //[WebInvoke(Method = "PUT", UriTemplate = "/assignments/{assignmentId}")]
//        public dc.AssignmentContract UpdateAssignment(string assignmentId, dc.AssignmentContract assignment)
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.BadRequest, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            be.Assignment beAssignment;
//            dc.AssignmentContract result = new dc.AssignmentContract();

//            if (authTeam.TeamType == dc.TeamType.Administrator)
//            {
//                try
//                {
//                    beAssignment = CreateBEAssignment(assignment);
//                    be.Assignment beResult = _facade.SaveAssignment(beAssignment);
//                    result = CreateDCAssignment(beResult);
//                }
//                catch (Exception ex)
//                {
//                    ReturnWithStatusAndDescription(HttpStatusCode.InternalServerError, "An internal server error has occurred. Details: " + ex.Message);
//                    return null;
//                }

//            }
//            else
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.Forbidden, "You are not authorized to update assignment with id: " + assignmentId);
//                return null;
//            }

//            return result;
//        }

//        #endregion

//        #region tournaments resource


//        //[WebGet(UriTemplate = "/tournaments")]
//        [OperationBehavior]
//        public dc.TournamentsContract GetTournaments()
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.Forbidden, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            List<be.Tournament> tournaments = _facade.GetTournaments();
//            dc.TournamentsContract result = new dc.TournamentsContract();
//            foreach (be.Tournament t in tournaments)
//            {
//                dc.TournamentContract tr = new dc.TournamentContract();
//                result.Add(tr);
//                tr.TournamentId = t.Id.ToString();
//                tr.TournamentName = t.Name;
//            }
//            return result;
//        }

//        //[WebGet(UriTemplate = "/tournaments/{tournamentId}")]
//        [OperationBehavior]
//        public dc.TournamentContract GetTournament(string tournamentId)
//        {
//            dc.TeamContract authTeam = Authenticate();

//            if (authTeam == null)
//            {
//                ReturnWithStatusAndDescription(HttpStatusCode.Forbidden, "Credentials not found in headers or invalid.");
//                return null;
//            }

//            dc.TournamentContract result = new dc.TournamentContract();

//            be.Tournament beTournament = _facade.GetTournament(int.Parse(tournamentId));

//            result.TournamentId = beTournament.Id.ToString();
//            result.TournamentName = beTournament.Name;
            
//            return result;
//        }

//        [OperationBehavior]
//        //[WebGet(UriTemplate = "/tournament/{tournamentId}/submits")]
//        public dc.SubmitsContract GetTournamentSubmits(string tournamentId)
//        {
//            List<be.Submit> submits = _facade.GetTournamentSubmits(int.Parse(tournamentId));

//            dc.SubmitsContract result = new dc.SubmitsContract();

//            foreach (be.Submit sub in submits)
//            {
//                result.Add(CreateDCSubmit(sub));
//            }

//            return result;
//        }

//        [OperationBehavior]
//        //[WebInvoke(Method = "PUT", UriTemplate = "/tournaments/{tournamentId}/assignments/{assignmentId}")]
//        public dc.AssignmentContract UpdateTournamentAssignment(string tournamentId, string assignmentId, dc.AssignmentContract assignment)
//        {
//            be.TournamentAssignment a = new be.TournamentAssignment();
//            a.TournamentAssignmentId = int.Parse(assignment.TournamentAssignmentId);
//            a.TournamentId = int.Parse(assignment.TournamentId);
//            a.AssignmentId = int.Parse(assignment.AssignmentId);
//            a.AssignmentOrder = assignment.AssignmentOrder;
//            a.Points1 = assignment.Points1;
//            a.Points2 = assignment.Points2;
//            a.Points3 = assignment.Points3;
//            a.Active = assignment.Active;

//            a = _facade.SaveTournamentAssignment(a);
//            return assignment;


//        }

//        #endregion


//        #region Tournamentreport
//        //[WebGet(UriTemplate = "tournamentreports/{tournamentId}")]
//        [OperationBehavior]
//        public string GetTournamentReport(string tournamentId)
//        {
//            return _facade.GetTournamentReport(int.Parse(tournamentId));
//        }

//        #endregion

//        #region helpers

//        private dc.TeamContract Authenticate()
//        {
//            string username = inRequest.Headers["Username"];
//            string password = inRequest.Headers["Password"];

//            //string authorization = inRequest.Headers["Authorization"];

//            //string username = authorization.Substring(0, authorization.IndexOf(':') - 1);
//            //string providedHash = authorization.Substring(authorization.IndexOf(':') + 1);

//            //be.Team businessTeam = _facade.GetTeam(username);

//            //byte[] key = UTF8Encoding.UTF8.GetBytes(businessTeam.Password);

//            //HMACMD5 hmacMD5 = new HMACMD5(key);
//            //string computedHash = UTF8Encoding.UTF8.GetString(hmacMD5.ComputeHash(inRequest.serializedStream()));

//            //if (computedHash==providedHash)
//            //{
                
//            //}

//            if (username == null || password == null)
//            {
//                return null;
//            }

//            be.Team businessTeam = _facade.GetTeam(username);

//            if (businessTeam.Password == password && businessTeam.Name.ToLower() == username.ToLower())
//            {
//                MoCS.Service.DataContracts.TeamContract t = new MoCS.Service.DataContracts.TeamContract();

//                t.TeamName = businessTeam.Name;
//                t.TeamId = businessTeam.Id.ToString();
//                t.Points = businessTeam.Score;
//                t.TeamMembers = businessTeam.Members;
//                t.TeamStatus = dc.TeamStatus.Active;    //zijn allemaal active
//                //t.Username = businessTeam.TeamName; //teamname = username
//                if (businessTeam.IsAdmin)
//                {
//                    t.TeamType = dc.TeamType.Administrator;
//                    t.Password = businessTeam.Password;
//                }
//                else if (businessTeam.Name.ToUpper() == username.ToUpper())
//                {
//                    t.Password = businessTeam.Password;
//                }
//                else
//                {
//                    t.TeamType = dc.TeamType.Normal;
//                }
//                return t;
//            }

//            return null;
//        }

//        private void ReturnWithStatusAndDescription(HttpStatusCode statusCode, string Description)
//        {
//            int httpStatusCode;

//            switch (statusCode)
//            {

//                case HttpStatusCode.BadRequest:
//                    httpStatusCode = 400;
//                    break;
//                case HttpStatusCode.Forbidden:
//                    httpStatusCode = 403;
//                    break;
//                case HttpStatusCode.NotFound:
//                    httpStatusCode = 404;
//                    break;
//                case HttpStatusCode.NotImplemented:
//                    httpStatusCode = 501;
//                    break;
//                default:
//                    httpStatusCode = 0;
//                    break;
//            }
//            HttpStatusDetails details = new HttpStatusDetails { HttpStatusCode = httpStatusCode, Details = Description };
//            throw new WebProtocolException(statusCode, details.Details, details, null, null);
//        }
//        #endregion

//        #region Helpers voor translatie tussen DataContracts <-> Business Entities

//        private dc.TeamContract CreateDCTeam(be.Team team)
//        {
//            dc.TeamContract newTeam = new dc.TeamContract();

//            newTeam.TeamName = team.Name;
//            newTeam.TeamId = team.Id.ToString();
//            newTeam.Points = team.Score;
//            newTeam.TeamStatus = dc.TeamStatus.Active;    //zijn allemaal active
//            //newTeam.Username = team.TeamName; //teamname = username
//            newTeam.Password = team.Password;
//            newTeam.TeamMembers = team.Members;
//            if (team.IsAdmin)
//            {
//                newTeam.TeamType = dc.TeamType.Administrator;
//            }
//            else
//            {
//                newTeam.TeamType = dc.TeamType.Normal;
//            }

//            return newTeam;
//        }

//        public void DeleteSubmit(string teamSubmitId)
//        {
//            _facade.DeleteTeamSubmit(int.Parse(teamSubmitId));
//        }


//        private dc.SubmitContract CreateDCSubmit(be.Submit sub)
//        {
//            dc.SubmitContract s = new dc.SubmitContract();
//            s.SubmitId = sub.SubmitID.ToString();
//            s.AssignmentId = sub.AssignmentId.ToString();
//            s.TeamTournamentAssignmentId = sub.TeamTournamentAssignmentId.ToString();
//            s.TeamId = sub.TeamId.ToString();
//            s.IsFinished = sub.IsFinished;
//            s.FileName = sub.FileName;
//            s.StartDate = sub.StartDate;
//            s.SubmitDate = sub.SubmitDate;
//            s.StatusDate = sub.StatusDate;
//            s.TeamMembers = sub.TeamMembers;
//            s.TeamName = sub.TeamName;
//            s.CurrentStatusCode = sub.CurrentStatus;
//            s.TournamentAssignmentId = sub.TournamentAssignmentId;
//            s.TeamName = sub.TeamName;
//            s.TeamMembers = sub.TeamMembers;

//            return s;
//        }

//        private be.Assignment CreateBEAssignment(dc.AssignmentContract dcAssignment)
//        {
//            be.Assignment result = new be.Assignment();
//            result.Active = dcAssignment.Active;
//            //result.Author
//            result.Category = dcAssignment.Category;
//            result.Difficulty = dcAssignment.Difficulty;
//            result.DisplayName = dcAssignment.DisplayName;
//            result.Hint = dcAssignment.Hint;
//            result.AssignmentId = int.Parse(dcAssignment.AssignmentId);
//            //result.IsValid
//            result.AssignmentName = dcAssignment.AssignmentName;
//            result.Points = dcAssignment.Points;
//            if (dcAssignment.StartDate.HasValue)
//            {
//                //        result.StartDate = dcAssignment.StartDate.Value;
//            }

//            result.ZipFile = dcAssignment.Zipfile;

//            return result;
//        }

//        private dc.AssignmentContract CreateDCAssignment(be.Assignment beAssignment)
//        {
//            dc.AssignmentContract result = new dc.AssignmentContract();

//            result.Active = beAssignment.Active;
//            //result.Author
//            result.Category = beAssignment.Category;
//            result.Difficulty = beAssignment.Difficulty;
//            result.DisplayName = beAssignment.DisplayName;
//            result.Hint = beAssignment.Hint;
//            result.AssignmentId = beAssignment.AssignmentId.ToString();
//            //result.IsValid
//            result.AssignmentName = beAssignment.AssignmentName;
//            result.Points = beAssignment.Points;
//            //   result.StartDate = beAssignment.StartDate;
//            result.Zipfile = beAssignment.ZipFile;

//            result.Files = new Dictionary<string, dc.AssignmentFileContract>();
//            foreach (string key in beAssignment.Files.Keys)
//            {
//                dc.AssignmentFileContract f = new dc.AssignmentFileContract();
//                f.Name = beAssignment.Files[key].Name;
//                f.Data = beAssignment.Files[key].Contents;
//                result.Files.Add(key, f);
//            }

//            return result;
//        }


//        private dc.AssignmentContract CreateDCAssignment(be.AssignmentEnrollment beAssignment)
//        {
//            dc.AssignmentContract result = new dc.AssignmentContract();

//            //ID's
//            result.AssignmentId = beAssignment.AssignmentId.ToString();
//            result.TournamentId = beAssignment.TournamentId.ToString();
//            result.TournamentAssignmentId = beAssignment.TournamentAssignmentId.ToString();
//            result.TeamTournamentAssignmentId = beAssignment.TeamTournamentAssignmentId.ToString();


//            result.Active = beAssignment.Active;
//            // result.Author = beAssignment.Author;
//            result.Category = beAssignment.Category;
//            result.Difficulty = beAssignment.Difficulty;
//            result.DisplayName = beAssignment.DisplayName;
//            result.Hint = beAssignment.Hint;
//            result.AssignmentId = beAssignment.AssignmentId.ToString();
//            //result.IsValid
//            result.AssignmentName = beAssignment.AssignmentName;
//            result.Points = beAssignment.Points;
//            result.StartDate = beAssignment.StartDate;
//            result.Zipfile = beAssignment.ZipFile;

//            result.Files = new Dictionary<string, dc.AssignmentFileContract>();
//            foreach (string key in beAssignment.Files.Keys)
//            {
//                dc.AssignmentFileContract f = new dc.AssignmentFileContract();
//                f.Name = beAssignment.Files[key].Name;
//                f.Data = beAssignment.Files[key].Contents;
//                result.Files.Add(key, f);
//            }

//            return result;
//        }



//        #endregion

//    }
//}
