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
        TeamsContract GetAllTeams();

        [WebGet(UriTemplate = "/teams/{teamId}")]
        [OperationContract]
        TeamContract GetTeam(string teamId);

        [OperationContract]
        [WebGet(UriTemplate = "/teams?name={teamName}")]
        public TeamContract GetTeamByName(string teamName);

        [WebInvoke(Method = "POST", UriTemplate = "/teams")]
        [OperationContract]
        TeamContract AddTeam(TeamContract team);

        [WebInvoke(Method = "PUT", UriTemplate = "/teams/{teamId}")]
        [OperationContract]
        TeamContract UpdateTeam(string teamId, TeamContract team);

        [WebGet(UriTemplate = "/teams/{teamId}/assignmentenrollments?tournamentId={tournamentId}")]
        [OperationContract]
        AssignmentEnrollmentsContract GetAssignmentEnrollmentsForTeamForTournament(string teamId, string tournamentId);

        [WebInvoke(UriTemplate = "/teams/{teamId}/assignmentenrollments")]
        [OperationContract]
        AssignmentEnrollmentContract AddAssignmentEnrollmentsForTeam(string teamId, AssignmentEnrollmentContract assignmentEnrollment);

        #endregion

        #region tournaments

        [WebGet(UriTemplate = "/tournaments")]
        [OperationContract]
        TournamentsContract GetTournaments();

        [WebGet(UriTemplate = "/tournaments/{tournamentId}")]
        [OperationContract]
        TournamentContract GetTournament(string tournamentId);

        [WebGet(UriTemplate = "/tournaments/{tournamentId}/tournamentassignments")]
        [OperationContract]
        TournamentAssignmentsContract GetTournamentAssignmentsForTournament(string tournamentId);

        #endregion

        #region tournamentassignments

        [WebGet(UriTemplate = "/tournamentassignments/{tournamentassignmentId}")]
        [OperationContract]
        TournamentAssignmentContract GetTournamentAssignment(string tournamentassignmentId);

        //Only accessible for a team if it has an active enrollment for the assignment
        [WebGet(UriTemplate = "/tournamentassignments/{tournamentassignmentId}/assignmentzipfile")]
        [OperationContract]
        AssignmentZipFileContract GetAssignmentZipFileForTournamentAssignment(string tournamentassignmentId);

        //Only accessible for a team if it has an active enrollment for the assignment
        [WebGet(UriTemplate = "/tournamentassignments/{tournamentassignmentId}/assignmentfiles")]
        [OperationContract]
        AssignmentFilesContract GetAssignmentFilesForTournamentAssignment(string tournamentassignmentId);

        #endregion

        #region assignmentenrollments/submits

        //Only accessible for a team if it has an active enrollment
        [WebGet(UriTemplate = "/assignmentenrollments/{assignmentenrollmentId}/submits")]
        [OperationContract]
        SubmitsContract GetSubmitsForAssignmentEnrollment(string assignmentenrollmentId);

        //Only accessible for a team if it has an active enrollment
        [WebGet(UriTemplate = "/assignmentenrollments/{assignmentenrollmentId}/submits/{submitId}/assignmentfile")]
        [OperationContract]
        AssignmentFileContract GetAssignmentFileForSubmit(string assignmentenrollmentId, string submitId);

        //Only accessible for a team if it has an active enrollment
        [WebInvoke(UriTemplate = "/assignmentenrollments/{assignmentenrollmentId}/submits")]
        [OperationContract]
        SubmitContract AddSubmitForAssignmentEnrollment(string assignmentenrollmentId, SubmitContract submit);

        #endregion
    }
}
