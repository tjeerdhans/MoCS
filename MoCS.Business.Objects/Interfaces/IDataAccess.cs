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
        //List<SubmitToProcess> GetUnprocessedSubmits();
        //void SetTeamSubmitToFinished(int submitId);
        //void InsertSubmitStatus(int teamId, int submitId, int statusCode, string details);

        //FOR CLIENT
        List<Team> GetTeams();
        Team GetTeamById(int teamId);
        Team GetTeamByName(string teamName);
        Team SaveTeam(Team team);
        Team UpdateTeam(Team team);

        List<Tournament> GetTournaments();
        List<TournamentAssignment> GetTournamentAssignmentsForTournament(int tournamentId);
        Tournament GetTournamentById(int tournamentId);

        TournamentAssignment GetTournamentAssignmentById(int tournamentAssignmentId);

        List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeamForTournament(int tournamentId, int teamId);
        AssignmentEnrollment GetAssignmentEnrollmentById(int assignmentEnrollmentId);

        AssignmentEnrollment SaveAssignmentEnrollment(AssignmentEnrollment assignmentEnrollment);
    }
}