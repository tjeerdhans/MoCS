using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MoCS.Business.Objects.Interfaces
{
    public interface IDataAccess
    {
        //FOR BUILDSERVER

        List<Submit> GetUnprocessedSubmits();
        void UpdateSubmitStatusDetails(int submitId, SubmitStatus newStatus, string details, DateTime statusDate);
        //void SetTeamSubmitToFinished(int submitId);
        //void InsertSubmitStatus(int teamId, int submitId, int statusCode, string details);

        //FOR CLIENT

        // Teams
        List<Team> GetTeams();
        Team GetTeamById(int teamId);
        Team GetTeamByName(string teamName);
        Team SaveTeam(Team team);
        Team UpdateTeam(Team team);

        // Tournaments
        List<Tournament> GetTournaments();
        Tournament GetTournamentById(int tournamentId);

        // Assignments / TournamentAssignments
        Assignment GetAssignmentById(int assignmentId);
        TournamentAssignment GetTournamentAssignmentById(int tournamentAssignmentId);
        List<TournamentAssignment> GetTournamentAssignmentsForTournament(int tournamentId);

        // AssignmentEnrollments
        List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeam(int teamId);
        List<AssignmentEnrollment> GetAssignmentEnrollmentsForTournamentAssignment(int tournamentAssignmentId);
        List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeamForTournamentAssignment(int tournamentAssignmentId, int teamId);
        AssignmentEnrollment GetAssignmentEnrollmentById(int assignmentEnrollmentId);
        AssignmentEnrollment SaveAssignmentEnrollment(AssignmentEnrollment assignmentEnrollment);

        // Submits
        Submit GetSubmitById(int submitId);
        List<Submit> GetSubmitsForAssignmentEnrollment(int assignmentEnrollmentId);
        Submit GetFastestSubmitForTournamentAssignment(int tournamentAssignmentId);
        Submit SaveSubmit(Submit submit);
    }
}