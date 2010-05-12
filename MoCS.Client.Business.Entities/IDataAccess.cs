using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MoCS.Client.Business.Entities
{
    public interface IDataAccess
    {
        //FOR BUILDSERVER
        List<SubmitToProcess> GetUnprocessedSubmits();
        void SetTeamSubmitToFinished(int submitId);
        void InsertSubmitStatus(int teamId, int submitId, int statusCode, string details);

        //FOR CLIENT
        List<Submit> GetTournamentSubmits(int tournamentId);
        TournamentAssignment CreateTournamentAssignmentFromDataRow(System.Data.DataRow dr);
        void DeleteAssignment(int assignmentId);
        void DeleteTeam(int teamId);
        void DeleteTeamSubmit(int id);
        void DeleteTeamTournamentAssignment(int id);
        void DeleteTournament(int tournamentId);
        void DeleteTournamentAssignment(int id);
        Assignment GetAssignmentByID(int assignmentId);
        List<Assignment> GetAssignments();
        Team GetTeamById(int teamId);
        Team GetTeamByName(string teamName);
        List<Team> GetTeams();
        Submit GetTeamSubmitById(int id);
        List<Submit> GetTeamSubmitsForAssignment(int tournamentAssignmentId);
        TeamTournamentAssignment GetTeamTournamentAssignmentById(int id);
        List<TeamTournamentAssignment> GetTeamTournamentAssignmentsForTeam(int tournamentId, int teamId);
        TournamentAssignment GetTournamentAssignmentById(int id);
        List<TournamentAssignment> GetTournamentAssignmentsForTournament(int tournamentId);
        Tournament GetTournamentById(int tournamentId);
        List<Tournament> GetTournaments();
        Submit InsertTeamSubmit(Submit submit);
        Assignment SaveAssignment(Assignment assignment);
        Team SaveTeam(Team team);
        TeamTournamentAssignment SaveTeamTournamentAssignment(TeamTournamentAssignment tta);
        Tournament SaveTournament(Tournament tournament);
        TournamentAssignment SaveTournamentAssignment(TournamentAssignment ta);
        List<Submit> GetSubmitsForReport(int tournamentId);
    }
}
