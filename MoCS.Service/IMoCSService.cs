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
    public interface IMoCSService
    {

        #region teams

        [WebGet(UriTemplate = "/teams")]
        [OperationContract]
        Teams GetAllTeams();

        [WebGet(UriTemplate = "/teams/{teamId}")]
        [OperationContract]
        Team GetTeam(string teamId);

        [WebInvoke(Method = "POST", UriTemplate = "/teams")]
        [OperationContract]
        Team AddTeam(Team team);

        [WebInvoke(Method = "PUT", UriTemplate = "/teams/{teamId}")]
        [OperationContract]
        Team UpdateTeam(string teamId, Team team);

        #endregion

        #region teams/{teamId}/tournaments

        [WebGet(UriTemplate = "teams/{teamId}/tournaments")]
        [OperationContract]
        Tournaments GetTeamTournaments(string teamId);

        [WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}")]
        [OperationContract]
        Tournament GetTeamTournament(string teamId, string tournamentId);

        [WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments")]
        [OperationContract]
        Assignments GetTeamTournamentAssignments(string teamId, string tournamentId);

        [WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}/submits")]
        [OperationContract]
        Submits GetTeamTournamentAssignmentSubmits(string teamId, string tournamentId, string assignmentId);

        [WebGet(UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}/submits/{submitId}")]
        [OperationContract]
        Submit GetTeamTournamentAssignmentSubmit(string teamId, string tournamentId, string assignmentId, string submitId);

        [WebInvoke(Method="PUT", UriTemplate = "teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}")]
        [OperationContract]
        Assignment UpdateTeamTournamentAssignment(string teamId, string tournamentId, string assignmentId, Assignment assignment);

        [WebInvoke(Method="POST", UriTemplate="teams/{teamId}/tournaments/{tournamentId}/assignments/{assignmentId}/submits")]
        [OperationContract]
        Submit AddTeamTournamentAssignmentSubmit(string teamId, string tournamentId, string assignmentId, Submit submit);

        [WebGet(UriTemplate = "tournamentreports/{tournamentId}")]
        [OperationContract]
        string GetTournamentReport(string tournamentId);

        #endregion

        #region teams/{teamId}/assignments

        [WebGet(UriTemplate = "teams/{teamId}/assignments")]
        [OperationContract]
        Assignments GetAllAssignmentsForTeam(string teamId);

        [WebGet(UriTemplate = "teams/{teamId}/assignments/{teamTournamentAssignmentId}")]
        [OperationContract]
        Assignment GetTeamTournamentAssignment(string teamId, string teamTournamentAssignmentId);
        
        [WebInvoke(Method = "POST", UriTemplate = "/teams/{teamId}/assignments")]
        [OperationContract]
        Assignment AddAssignmentForTeam(string teamId, Assignment assignment);

        // No PUT operation

        #endregion

        #region  teams/{teamId}/assignments/{assignmentId}/submits
        [WebGet(UriTemplate = "tournamentasignments/{tournamentAssignmentId}/submits")]
        [OperationContract]
        Submits GetAllSubmitsForAssignmentForTeam(string tournamentAssignmentId);

        [WebGet(UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits/{submitId}")]
        [OperationContract]
        Submit GetSubmitForAssignmentForTeam(string teamId, string assignmentId, string submitId);

        [WebInvoke(Method = "POST", UriTemplate = "teams/{teamId}/assignments/{assignmentId}/submits")]
        [OperationContract]
        Submit AddSubmitForAssignmentForTeam(string teamId, string assignmentId, Submit submit);

        [OperationContract]
        [WebGet(UriTemplate = "teamsubmits/{teamSubmitId}")]
        Submit GetTeamSubmit(string teamSubmitId);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "teamsubmits/{teamSubmitId}")]
        void DeleteSubmit(string teamSubmitId);

        #endregion

        #region assignments resource

        [WebInvoke(Method = "POST", UriTemplate = "/teamtournamentassignments")]
        [OperationContract]
        Assignment AddTeamTournamentAssignment(Assignment assignment);

        [WebGet(UriTemplate = "/tournaments/{tournamentId}assignments")]
        [OperationContract]
        Assignments GetTournamentAssignments(string tournamentId);

        [WebGet(UriTemplate = "/assignments/{assignmentId}")]
        [OperationContract]
        Assignment GetAssignment(string assignmentId);

        [WebInvoke(Method = "POST", UriTemplate = "/assignments")]
        [OperationContract]
        Assignment AddAssignment(Assignment assignment);

        [WebInvoke(Method = "PUT", UriTemplate = "/assignments/{assignmentId}")]
        [OperationContract]
        Assignment UpdateAssignment(string assignmentId, Assignment assignment);

        #endregion

        #region tournaments resource

        [WebGet(UriTemplate = "/tournaments/{tournamentId}/submits")]
        [OperationContract]
        Submits GetTournamentSubmits(string tournamentId);

        [WebGet(UriTemplate = "/tournaments")]
        [OperationContract]
        List<Tournament> GetTournaments();

        [WebInvoke(Method = "PUT", UriTemplate = "/tournaments/{tournamentId}/assignments/{assignmentId}")]
        [OperationContract]
        Assignment UpdateTournamentAssignment(string tournamentId, string assignmentId, Assignment assignment);

        #endregion

    }
}
