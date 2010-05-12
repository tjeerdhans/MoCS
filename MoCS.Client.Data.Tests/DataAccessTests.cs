using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MoCS.Client.Business.Entities;
using System.Configuration;
using System.Xml;

namespace MoCS.Client.Data.Tests
{
    [TestFixture]
    public class DataAccessTests
    {

        private string GetConnectionstring()
        {
            return ConfigurationManager.ConnectionStrings["MoCS"].ConnectionString;
        }

        [Test]
        public void Tournament_SelectAddSelectDeleteSelect_OK()
        {
            using (System.Transactions.TransactionScope updateTransaction =
                     new System.Transactions.TransactionScope())
            {
                string connectionString = GetConnectionstring();
                DataAccess d2 = new DataAccess(connectionString);

                List<Tournament> tournamentsBefore = d2.GetTournaments();

                Tournament t = new Tournament();
                t.Id = -1;
                t.Name = "asldfkjsldfjsdlf";

                d2.SaveTournament(t);

                Assert.AreNotEqual(t.Id, -1);

                List<Tournament> tournamentsAfterInsert = d2.GetTournaments();

                Assert.AreEqual(tournamentsAfterInsert.Count, tournamentsBefore.Count + 1);

                Tournament gettournament = d2.GetTournamentById(t.Id);

                Assert.AreEqual(gettournament.Name, t.Name);

                d2.DeleteTournament(t.Id);

                List<Tournament> tournamentsAfterDelete = d2.GetTournaments();

                Assert.AreEqual(tournamentsBefore.Count, tournamentsAfterDelete.Count);

            }


        }

        [Test]
        public void Team_SelectAddSelectDeleteSelect_OK()
        {
            using (System.Transactions.TransactionScope updateTransaction =
         new System.Transactions.TransactionScope())
            {
                string connectionString = GetConnectionstring();
                DataAccess d2 = new DataAccess(connectionString);

                List<Team> teamsBefore = d2.GetTeams();

                Team team = new Team();
                team.ID = -1;
                team.TeamName = "NAME00001";
                team.Password = "topsecret";
                team.IsAdmin = false;
                team.Points = 0;
                team.TeamMembers = "Joe, Jack";

                Team teamafterSave = d2.SaveTeam(team);
  
                Team teamGetByID = d2.GetTeamById(team.ID);
                Team teamGetByName = d2.GetTeamByName(team.TeamName);

                Assert.AreNotEqual(team.ID, -1);
                Assert.AreEqual(team.TeamName, teamGetByName.TeamName);
                

                List<Team> teamsAfterSave = d2.GetTeams();

                Assert.AreEqual(team.ID, teamafterSave.ID);
                Assert.AreEqual(teamsAfterSave.Count, teamsBefore.Count + 1);


                d2.DeleteTeam(team.ID);

                List<Team> teamsAfterDelete = d2.GetTeams();

                Assert.AreEqual(teamsBefore.Count, teamsAfterDelete.Count);
            }

        }

        [Test]
        public void Assignment_SelectAddSelectDeleteSelect_OK()
        {
            using (System.Transactions.TransactionScope updateTransaction =
         new System.Transactions.TransactionScope())
            {
                string connectionString = GetConnectionstring();
                DataAccess d2 = new DataAccess(connectionString);

                List<Assignment> assignmentsBefore = d2.GetAssignments();

                Assignment assignment = new Assignment();
                assignment.AssignmentId = -1;
                assignment.AssignmentName = "asfdasfasdfsaf";

                Assignment assignmentAfterSave = d2.SaveAssignment(assignment);

                Assignment assignmentGetById = d2.GetAssignmentByID(assignment.AssignmentId);

                Assert.AreNotEqual(assignment.AssignmentId, -1);
                Assert.AreEqual(assignment.AssignmentName, assignmentGetById.AssignmentName);

                List<Assignment> assignmentsAfterSave = d2.GetAssignments();

                Assert.AreEqual(assignmentsAfterSave.Count, assignmentsBefore.Count + 1);

                d2.DeleteAssignment(assignment.AssignmentId);

                List<Assignment> assignmentsAfterDelete = d2.GetAssignments();

                Assert.AreEqual(assignmentsBefore.Count, assignmentsAfterDelete.Count);

            }
        }

        [Test]
        public void TournamentAssignment_SelectAddSelectDeleteSelect_OK()
        {
            using (System.Transactions.TransactionScope updateTransaction =
          new System.Transactions.TransactionScope())
            {
                string connectionString = GetConnectionstring();
                DataAccess d2 = new DataAccess(connectionString);

                Tournament t = new Tournament();
                t.Id = -1;
                t.Name = "TESTING";
                d2.SaveTournament(t);

                Assignment a = new Assignment();
                a.AssignmentId = -1;
                a.AssignmentName = "ASSIGNMENT";
                d2.SaveAssignment(a);

                List<TournamentAssignment> tournamentAssignmentsBefore = d2.GetTournamentAssignmentsForTournament(t.Id);
                //there should be no assignments for the new tournament
                Assert.AreEqual(tournamentAssignmentsBefore.Count, 0);

                TournamentAssignment ta = new TournamentAssignment();
                ta.TournamentAssignmentId = -1;
                ta.TournamentId = t.Id;
                ta.AssignmentId = a.AssignmentId;
                ta.AssignmentOrder = 1;
                ta.Points1 = 100;
                ta.Points2 = 50;
                ta.Points3 = 25;
                ta.Active = false;

                d2.SaveTournamentAssignment(ta);
                //id should be set
                Assert.AreNotEqual(ta.TournamentAssignmentId, -1);
                
                List<TournamentAssignment> tournamentAssignmentsAfterSave = d2.GetTournamentAssignmentsForTournament(t.Id);
                //there should be one more
                Assert.AreEqual(tournamentAssignmentsAfterSave.Count, tournamentAssignmentsBefore.Count + 1);
                
                TournamentAssignment byId = d2.GetTournamentAssignmentById(ta.TournamentAssignmentId);
                Assert.AreEqual(ta.Points1, byId.Points1);
                Assert.AreEqual(ta.Points2, byId.Points2);
                Assert.AreEqual(ta.Points3, byId.Points3);
                Assert.AreEqual(ta.Active, byId.Active);
                Assert.AreEqual(a.AssignmentName, byId.AssignmentName);

                d2.DeleteTournamentAssignment(ta.TournamentAssignmentId);

                List<TournamentAssignment> tournamentAssignmentsAfterDelete = d2.GetTournamentAssignmentsForTournament(t.Id);
                //after deleting, there should be 0 entries again
                Assert.AreEqual(tournamentAssignmentsBefore.Count, tournamentAssignmentsAfterDelete.Count);
            }
        }

        [Test]
        public void TeamTournamentAssignment_SelectAddSelectDeleteSelect_OK()
        {
            using (System.Transactions.TransactionScope updateTransaction =
                new System.Transactions.TransactionScope())
            {
                string connectionString = GetConnectionstring();
                DataAccess d2 = new DataAccess(connectionString);

                Team team = new Team();
                team.ID = -1;
                team.TeamName = "blabla";
                team.TeamMembers = "asdf";
                team.Password = "xxx";
                team.IsAdmin=false;
                d2.SaveTeam(team);

                Tournament t = new Tournament();
                t.Id = -1;
                t.Name = "TESTING";
                d2.SaveTournament(t);

                Assignment a = new Assignment();
                a.AssignmentId = -1;
                a.AssignmentName = "ASSIGNMENT";
                d2.SaveAssignment(a);

                TournamentAssignment ta = new TournamentAssignment();
                ta.TournamentAssignmentId = -1;
                ta.TournamentId = t.Id;
                ta.AssignmentId = a.AssignmentId;
                ta.AssignmentOrder = 1;
                ta.Points1 = 100;
                ta.Points2 = 50;
                ta.Points3 = 25;
                ta.Active = false;

                d2.SaveTournamentAssignment(ta);

                List<TeamTournamentAssignment> ttaBefore = d2.GetTeamTournamentAssignmentsForTeam(ta.TournamentId, team.ID);
                Assert.AreNotEqual(ttaBefore.Count, 0);

                TeamTournamentAssignment tta = new TeamTournamentAssignment();
                tta.TeamTournamentAssignmentId = -1;
                tta.TeamId = team.ID;
                tta.TournamentAssignmentId = ta.TournamentAssignmentId;


                d2.SaveTeamTournamentAssignment(tta);
                //id should be set
                Assert.AreNotEqual(tta.TeamTournamentAssignmentId, -1);

                List<TeamTournamentAssignment> ttaAfterSave = d2.GetTeamTournamentAssignmentsForTeam(ta.TournamentId, team.ID);
                Assert.AreEqual(ttaAfterSave.Count, 1);

                TeamTournamentAssignment ttaById = d2.GetTeamTournamentAssignmentById((int)tta.TeamTournamentAssignmentId);
                Assert.AreEqual(tta.TeamId, ttaById.TeamId);

                d2.DeleteTeamTournamentAssignment((int)tta.TeamTournamentAssignmentId);

                List<TeamTournamentAssignment> ttaAfterDelete = d2.GetTeamTournamentAssignmentsForTeam(ta.TournamentId, team.ID);
            //    Assert.AreEqual(ttaAfterDelete.Count, 0);


            }


        }

        [Test]
        public void TeamSubmit_SelectAddSelectDeleteSelect_OK()
        {
            using (System.Transactions.TransactionScope updateTransaction =
                    new System.Transactions.TransactionScope())
            {
                string connectionString = GetConnectionstring();
                DataAccess d2 = new DataAccess(connectionString);

                Team team = new Team();
                team.ID = -1;
                team.TeamName = "blabla";
                team.TeamMembers = "asdf";
                team.Password = "xxx";
                team.IsAdmin = false;
                d2.SaveTeam(team);

                Tournament t = new Tournament();
                t.Id = -1;
                t.Name = "TESTING";
                d2.SaveTournament(t);

                Assignment a = new Assignment();
                a.AssignmentId = -1;
                a.AssignmentName = "ASSIGNMENT";
                d2.SaveAssignment(a);

                TournamentAssignment ta = new TournamentAssignment();
                ta.TournamentAssignmentId = -1;
                ta.TournamentId = t.Id;
                ta.AssignmentId = a.AssignmentId;
                ta.AssignmentOrder = 1;
                ta.Points1 = 100;
                ta.Points2 = 50;
                ta.Points3 = 25;
                ta.Active = false;
                d2.SaveTournamentAssignment(ta);
                
                TeamTournamentAssignment tta = new TeamTournamentAssignment();
                tta.TeamTournamentAssignmentId = -1;
                tta.TeamId = team.ID;
                tta.TournamentAssignmentId = ta.TournamentAssignmentId;
                d2.SaveTeamTournamentAssignment(tta);
                
                Submit submit = new Submit();
                submit.ID = -1;
                submit.TeamTournamentAssignmentId = (int)tta.TeamTournamentAssignmentId;
                submit.TeamId = team.ID;
                byte[] uploadstream = new byte[2] {1,2};
                submit.UploadStream = uploadstream;
                submit.FileName = "somename.cs";
                d2.InsertTeamSubmit(submit);

                Assert.AreNotEqual(submit.ID, -1);

                List<Submit> submits = d2.GetTeamSubmitsForAssignment(submit.TeamTournamentAssignmentId);

                Assert.AreEqual(submits.Count, 1);

                //getbyid
                Submit byId = d2.GetTeamSubmitById(submit.ID);

                Assert.AreEqual(byId.ID,submit.ID);

                d2.DeleteTeamSubmit(submit.ID);

                List<Submit> submitsAfterDelete = d2.GetTeamSubmitsForAssignment(submit.TeamTournamentAssignmentId);

                Assert.AreEqual(0, submitsAfterDelete.Count);



                


                
            }


        }

    }

}
