using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using MoCS.Service.DataContracts;
using System.Xml;

namespace MoCS.Service
{
    [ServiceContract]
    public interface IMoCSServiceOld
    {

        #region teams

        [WebGet(UriTemplate = "/teams")]
        [OperationContract]
        TeamsContract GetAllTeams();

        [WebGet(UriTemplate = "/teams/{teamId}")]
        [OperationContract]
        TeamContract GetTeam(string teamId);

        [WebInvoke(Method = "POST", UriTemplate = "/teams")]
        [OperationContract]
        TeamContract AddTeam(TeamContract team);

        [WebInvoke(Method = "PUT", UriTemplate = "/teams/{teamId}")]
        [OperationContract]
        TeamContract UpdateTeam(string teamId, TeamContract team);

        #endregion

        #region teams/{teamId}/tournaments

        [WebGet(UriTemplate = "teams/{teamId}/tournaments")]
        [OperationContract]
        TournamentsContract GetTeamTournaments(string teamId);

        [WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}")]
        [OperationContract]
        TournamentContract GetTeamTournament(string teamId, string tournamentId);

        [WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments")]
        [OperationContract]
        AssignmentsContract GetTeamTournamentAssignments(string teamId, string tournamentId);

        [WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}/submits")]
        [OperationContract]
        SubmitsContract GetTeamTournamentAssignmentSubmits(string teamId, string tournamentId, string assignmentId);

        [WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}/submits/{submitId}")]
        [OperationContract]
        SubmitContract GetTeamTournamentAssignmentSubmit(string teamId, string tournamentId, string assignmentId, string submitId);

        [WebInvoke(Method="PUT", UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}")]
        [OperationContract]
        AssignmentContract UpdateTeamTournamentAssignment(string teamId, string tournamentId, string assignmentId, AssignmentContract assignment);

        [WebInvoke(Method="POST", UriTemplate="teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}/submits")]
        [OperationContract]
        SubmitContract AddTeamTournamentAssignmentSubmit(string teamId, string tournamentId, string assignmentId, SubmitContract submit);

        [WebGet(UriTemplate = "tournamentreports/{tournamentId}")]
        [OperationContract]
        string GetTournamentReport(string tournamentId);

        #endregion

        #region teams/{teamId}/assignments

        [WebGet(UriTemplate = "teams/{teamId}/assignments")]
        [OperationContract]
        AssignmentsContract GetAllAssignmentsForTeam(string teamId);

        [WebGet(UriTemplate = "teams/{teamId}/assignments/{teamTournamentAssignmentId}")]
        [OperationContract]
        AssignmentContract GetTeamTournamentAssignment(string teamId, string teamTournamentAssignmentId);
        
        [WebInvoke(Method = "POST", UriTemplate = "/teams/{teamId}/assignments")]
        [OperationContract]
        AssignmentContract AddAssignmentForTeam(string teamId, AssignmentContract assignment);

        // No PUT operation

        #endregion

        #region  teams/{teamId}/assignments/{assignmentId}/submits
        [WebGet(UriTemplate = "tournamentasignments/{tournamentAssignmentId}/submits")]
        [OperationContract]
        SubmitsContract GetAllSubmitsForAssignmentForTeam(string tournamentAssignmentId);

        [WebGet(UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits/{submitId}")]
        [OperationContract]
        SubmitContract GetSubmitForAssignmentForTeam(string teamId, string assignmentId, string submitId);

        [WebInvoke(Method = "POST", UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits")]
        [OperationContract]
        SubmitContract AddSubmitForAssignmentForTeam(string teamId, string assignmentId, SubmitContract submit);

        [OperationContract]
        [WebGet(UriTemplate = "teamsubmits/{teamSubmitId}")]
        SubmitContract GetTeamSubmit(string teamSubmitId);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "teamsubmits/{teamSubmitId}")]
        void DeleteSubmit(string teamSubmitId);

        #endregion

        #region assignments resource

        [WebInvoke(Method = "POST", UriTemplate = "/teamtournamentassignments")]
        [OperationContract]
        AssignmentContract AddTeamTournamentAssignment(AssignmentContract assignment);

        [WebGet(UriTemplate = "/assignments/{assignmentId}")]
        [OperationContract]
        AssignmentContract GetAssignment(string assignmentId);

        [WebInvoke(Method = "POST", UriTemplate = "/assignments")]
        [OperationContract]
        AssignmentContract AddAssignment(AssignmentContract assignment);

        [WebInvoke(Method = "PUT", UriTemplate = "/assignments/{assignmentId}")]
        [OperationContract]
        AssignmentContract UpdateAssignment(string assignmentId, AssignmentContract assignment);

        #endregion

        #region tournaments resource

        [WebGet(UriTemplate = "/tournaments/{tournamentId}/submits")]
        [OperationContract]
        SubmitsContract GetTournamentSubmits(string tournamentId);

        [WebGet(UriTemplate = "/tournaments")]
        [OperationContract]
        TournamentsContract GetTournaments();

        [WebGet(UriTemplate = "/tournaments/{tournamentId}")]
        [OperationContract]
        TournamentContract GetTournament(string tournamentId);

        [WebGet(UriTemplate = "/tournaments/{tournamentId}/assignments")]
        [OperationContract]
        AssignmentsContract GetTournamentAssignments(string tournamentId);

        [WebInvoke(Method = "PUT", UriTemplate = "/tournaments/{tournamentId}/assignments/{assignmentId}")]
        [OperationContract]
        AssignmentContract UpdateTournamentAssignment(string tournamentId, string assignmentId, AssignmentContract assignment);

        #endregion

    }
}
