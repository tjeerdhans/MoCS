using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using MoCS.Client.Business.Entities;
using System.IO;

namespace MoCS.Client.Data
{
    public class DataAccess : IDataAccess
    {
        private string _connectionString;

        public DataAccess(string connectionString)
        {
            this._connectionString = connectionString;
        }


        #region BuildServer

        public List<SubmitToProcess> GetUnprocessedSubmits()
        {
            SqlCommand cmd = new SqlCommand("TeamSubmit_SelectUnprocessed");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            DataSet ds = ExecuteCommand(cmd);

            List<SubmitToProcess> submits = new List<SubmitToProcess>();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SubmitToProcess submit = new SubmitToProcess();
                    submits.Add(submit);
                    submit.TeamID = Convert.ToInt32(dr["TeamID"]);
                    submit.TeamName = dr["TeamName"].ToString();
                    submit.FileName = dr["FileName"].ToString();
                    submit.AssignmentID = Convert.ToInt32(dr["AssignmentID"]);
                    submit.AssignmentName = dr["AssignmentName"].ToString();
                    byte[] buffer = (byte[])dr["FileStream"];
                    submit.SubmitDate = Convert.ToDateTime(dr["SubmitDate"]);
                    submit.FileStream = ConvertByteArrayToStream(buffer);
                    submit.SubmitID = Convert.ToInt32(dr["ID"]);
                }

            }

            return submits;
        }

        public void InsertSubmitStatus(int teamId, int submitId, int statusCode, string details)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand("TeamSubmitStatus_Insert", connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TeamID", teamId));
            cmd.Parameters.Add(new SqlParameter("@TeamSubmitID", submitId));
            cmd.Parameters.Add(new SqlParameter("@StatusCode", statusCode));
            cmd.Parameters.Add(new SqlParameter("@Details", details));

            cmd.ExecuteNonQuery();

            cmd.Clone();
            connection.Close();
        }

        public void SetTeamSubmitToFinished(int submitId)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand("TeamSubmit_Finished", connection);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@SubmitId", submitId));
            cmd.ExecuteNonQuery();

            cmd.Clone();
            connection.Close();
        }


        private Stream ConvertByteArrayToStream(byte[] buffer)
        {
            MemoryStream memStream = new MemoryStream(buffer);

            memStream.Position = 0;

            return memStream;
        }


        #endregion





        #region Tournament
        public Tournament SaveTournament(Tournament tournament)
        {
            SqlCommand cmd = new SqlCommand("Tournament_Save");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", tournament.Id));
            cmd.Parameters.Add(new SqlParameter("@Name", tournament.Name));
            DataSet ds = ExecuteCommand(cmd);

            if (tournament.Id == -1)
            {
                if (ds.Tables.Count > 0)
                {
                    tournament.Id = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
            }
            return tournament;
        }

        public Tournament GetTournamentById(int tournamentId)
        {
            SqlCommand cmd = new SqlCommand("Tournament_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", tournamentId));

            DataSet ds = ExecuteCommand(cmd);

            Tournament tournament = new Tournament();
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    tournament.Id = Convert.ToInt32(dr["ID"]);
                    tournament.Name = dr["Name"].ToString();
                }
            }
            return tournament;
        }

        public List<Tournament> GetTournaments()
        {
            SqlCommand cmd = new SqlCommand("Tournament_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            DataSet ds = ExecuteCommand(cmd);

            List<Tournament> tournaments = new List<Tournament>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Tournament tournament = new Tournament();
                    tournaments.Add(tournament);
                    tournament.Id = Convert.ToInt32(dr["ID"]);
                    tournament.Name = dr["Name"].ToString();
                }
            }
            return tournaments;
        }

        public void DeleteTournament(int tournamentId)
        {
            SqlCommand cmd = new SqlCommand("Tournament_Delete");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", tournamentId));
            ExecuteNonQuery(cmd);
        }
        #endregion

        #region Team

        private Team CreateTeamFromRow(DataRow dr)
        {
            Team team = new Team();
            team.CreationDate = Convert.ToDateTime(dr["CreationDate"]);
            team.ID = Convert.ToInt32(dr["ID"]);
            team.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
            team.Password = dr["Password"].ToString();
            team.Points = 0;        //not yet
            team.TeamMembers = dr["TeamMembers"].ToString();
            team.TeamName = dr["TeamName"].ToString();
            return team;
        }

        public List<Team> GetTeams()
        {
            SqlCommand cmd = new SqlCommand("Team_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            
            DataSet ds = ExecuteCommand(cmd);
            List<Team> teams = new List<Team>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Team team = new Team();
                    teams.Add(CreateTeamFromRow(dr));
                }
            }
            return teams;
        }

        public Team GetTeamById(int teamId)
        {
            SqlCommand cmd = new SqlCommand("Team_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", teamId));

            DataSet ds = ExecuteCommand(cmd);
            Team team = new Team();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    team = CreateTeamFromRow(dr);
                }
            }
            return team;
        }

        public Team GetTeamByName(string teamName)
        {
            SqlCommand cmd = new SqlCommand("Team_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TeamName", teamName));

            DataSet ds = ExecuteCommand(cmd);
            Team team = new Team();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    team = CreateTeamFromRow(dr);
                }
            }
            return team;
        }

        public Team SaveTeam(Team team)
        {
            SqlCommand cmd = new SqlCommand("Team_Save");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", team.ID));
            cmd.Parameters.Add(new SqlParameter("@TeamName", team.TeamName));
            cmd.Parameters.Add(new SqlParameter("@Password", team.Password));
            cmd.Parameters.Add(new SqlParameter("@TeamMembers", team.TeamMembers));
            cmd.Parameters.Add(new SqlParameter("@IsAdmin", team.IsAdmin));
            DataSet ds = ExecuteCommand(cmd);

            if (team.ID == -1)
            {
                if (ds.Tables.Count > 0)
                {
                    team.ID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
            }
            return team;
        }

        public void DeleteTeam(int teamId)
        {
            SqlCommand cmd = new SqlCommand("Team_Delete");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", teamId));
            ExecuteNonQuery(cmd);
        }

        #endregion

        #region Assignment

        private Assignment CreateAssignmentFromRow(DataRow dr)
        {
            Assignment assignment = new Assignment();
            assignment.AssignmentId = Convert.ToInt32(dr["ID"]);
            assignment.AssignmentName = dr["Name"].ToString();
            return assignment;
        }

        public List<Assignment> GetAssignments()
        {
            SqlCommand cmd = new SqlCommand("Assignment_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            DataSet ds = ExecuteCommand(cmd);
            List<Assignment> assignments = new List<Assignment>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Team Assignment = new Team();
                    assignments.Add(CreateAssignmentFromRow(dr));
                }
            }
            return assignments;
        }

        public Assignment GetAssignmentByID(int assignmentId)
        {
            SqlCommand cmd = new SqlCommand("Assignment_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", assignmentId));

            DataSet ds = ExecuteCommand(cmd);
            Assignment assignment = new Assignment();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    assignment = CreateAssignmentFromRow(dr);
                }
            }
            return assignment;
        }

        public Assignment SaveAssignment(Assignment assignment)
        {
            SqlCommand cmd = new SqlCommand("Assignment_Save");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", assignment.AssignmentId));
            cmd.Parameters.Add(new SqlParameter("@Name", assignment.AssignmentName));
            DataSet ds = ExecuteCommand(cmd);

            if (assignment.AssignmentId == -1)
            {
                if (ds.Tables.Count > 0)
                {
                    assignment.AssignmentId = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
            }
            return assignment;
        }

        public void DeleteAssignment(int assignmentId)
        {
            SqlCommand cmd = new SqlCommand("Assignment_Delete");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", assignmentId));
            ExecuteNonQuery(cmd);
        }


        #endregion

        #region TournamentAssignment

        public TournamentAssignment CreateTournamentAssignmentFromDataRow(DataRow dr)
        {
            TournamentAssignment a = new TournamentAssignment();
            a.TournamentAssignmentId = Convert.ToInt32(dr["Id"]);
            a.TournamentId = Convert.ToInt32(dr["TournamentId"]);
            a.AssignmentId = Convert.ToInt32(dr["AssignmentId"]);
            a.AssignmentOrder = Convert.ToInt32(dr["AssignmentOrder"]);
            a.Points1 = Convert.ToInt32(dr["Points1"]);
            a.Points2 = Convert.ToInt32(dr["Points2"]);
            a.Points3 = Convert.ToInt32(dr["Points3"]);
            a.Active = Convert.ToBoolean(dr["Active"]);
            a.AssignmentName = dr["AssignmentName"].ToString();

            return a;
        }

        public List<TournamentAssignment> GetTournamentAssignmentsForTournament(int tournamentId)
        {
            SqlCommand cmd = new SqlCommand("TournamentAssignment_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TournamentID", tournamentId));
            DataSet ds = ExecuteCommand(cmd);

            List<TournamentAssignment> result = new List<TournamentAssignment>();
            if (ds.Tables.Count > 0)
            {
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    TournamentAssignment ta = CreateTournamentAssignmentFromDataRow(dr);
                    result.Add(ta);
               }

            }
            return result;
        }

        public TournamentAssignment SaveTournamentAssignment(TournamentAssignment ta)
        {
            
            SqlCommand cmd = new SqlCommand("TournamentAssignment_Save");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", ta.TournamentAssignmentId));
            cmd.Parameters.Add(new SqlParameter("@TournamentID", ta.TournamentId));
            cmd.Parameters.Add(new SqlParameter("@AssignmentID", ta.AssignmentId));
            cmd.Parameters.Add(new SqlParameter("@AssignmentOrder", ta.AssignmentOrder));
            cmd.Parameters.Add(new SqlParameter("@Points1", ta.Points1));
            cmd.Parameters.Add(new SqlParameter("@Points2",ta.Points2));
            cmd.Parameters.Add(new SqlParameter("@Points3", ta.Points3));
            cmd.Parameters.Add(new SqlParameter("@Active", ta.Active));

            DataSet ds = ExecuteCommand(cmd);

            if (ta.TournamentAssignmentId == -1)
            {
                if (ds.Tables.Count > 0)
                {
                    ta.TournamentAssignmentId = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
            }
            return ta;
        }

        public TournamentAssignment GetTournamentAssignmentById(int id)
        {
            SqlCommand cmd = new SqlCommand("TournamentAssignment_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TournamentAssignmentID", id));
            DataSet ds = ExecuteCommand(cmd);

            TournamentAssignment result = new TournamentAssignment();
            if (ds.Tables.Count > 0)
            {
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    result = CreateTournamentAssignmentFromDataRow(dr);
                }

            }
            return result;

        }

        public void DeleteTournamentAssignment(int id)
        {
            SqlCommand cmd = new SqlCommand("TournamentAssignment_Delete");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", id));
            ExecuteNonQuery(cmd);
        }


        #endregion


        #region TeamTournamentAssignment

        private TeamTournamentAssignment CreateTeamTournamentAssignmentFromDataRow(DataRow dr)
        {
            TeamTournamentAssignment tta = new TeamTournamentAssignment();

            tta.TeamTournamentAssignmentId = null;
            if(dr["ID"]!=DBNull.Value)
            {
                tta.TeamTournamentAssignmentId = Convert.ToInt32(dr["ID"]);
            }
            tta.TournamentAssignmentId = Convert.ToInt32(dr["TournamentAssignmentId"]);
            tta.TeamId = Convert.ToInt32(dr["TeamId"]);
            tta.TournamentId = Convert.ToInt32(dr["TournamentId"]);
            tta.AssignmentId = Convert.ToInt32(dr["AssignmentId"]);
            tta.StartDate = null;
            if(dr["StartDate"]!=DBNull.Value)
            {
                tta.StartDate = Convert.ToDateTime(dr["StartDate"]);
            }
            tta.Points1 = Convert.ToInt32(dr["Points1"]);
            tta.Points2 = Convert.ToInt32(dr["Points2"]);
            tta.Points3 = Convert.ToInt32(dr["Points3"]);
            tta.Active = Convert.ToBoolean(dr["Active"]);
            tta.AssignmentName = dr["AssignmentName"].ToString();







            return tta;
        }


        public TeamTournamentAssignment SaveTeamTournamentAssignment(TeamTournamentAssignment tta)
        {
            SqlCommand cmd = new SqlCommand("TeamTournamentAssignment_Save");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", tta.TeamTournamentAssignmentId));
            cmd.Parameters.Add(new SqlParameter("@TeamID", tta.TeamId));
            cmd.Parameters.Add(new SqlParameter("@TournamentAssignmentID", tta.TournamentAssignmentId));

            DataSet ds = ExecuteCommand(cmd);

            if (tta.TeamTournamentAssignmentId == -1)
            {
                if (ds.Tables.Count > 0)
                {
                    tta.TeamTournamentAssignmentId = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
            }
            return tta;
        }

        public List<TeamTournamentAssignment> GetTeamTournamentAssignmentsForTeam(int tournamentId, int teamId)
        {

            SqlCommand cmd = new SqlCommand("TeamTournamentAssignment_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TournamentID", tournamentId));
            cmd.Parameters.Add(new SqlParameter("@TeamID", teamId));
            DataSet ds = ExecuteCommand(cmd);

            List<TeamTournamentAssignment> result = new List<TeamTournamentAssignment>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result.Add(CreateTeamTournamentAssignmentFromDataRow(dr));
                }
            }
            return result;
        }

        public TeamTournamentAssignment GetTeamTournamentAssignmentById(int id)
        {
            SqlCommand cmd = new SqlCommand("TeamTournamentAssignment_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", id));
            DataSet ds = ExecuteCommand(cmd);

            TeamTournamentAssignment result = new TeamTournamentAssignment();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    result = CreateTeamTournamentAssignmentFromDataRow(dr);
                    
                }
            }
            return result;
        }

        public void DeleteTeamTournamentAssignment(int id)
        {
            SqlCommand cmd = new SqlCommand("TeamTournamentAssignment_Delete");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", id));
            ExecuteNonQuery(cmd);

        }



        #endregion


        #region reporting

    
        public List<Submit> GetSubmitsForReport(int tournamentId)
        {
            SqlCommand cmd = new SqlCommand("tournament_submits_selectforreport");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TournamentId", tournamentId));
            DataSet ds = ExecuteCommand(cmd);
            List<Submit> submits = new List<Submit>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    submits.Add(CreateSubmitFromDataRow(dr));
                }
            }
            return submits;
        }

        #endregion



        private void ExecuteNonQuery(SqlCommand cmd)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            cmd.Connection = connection;
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private DataSet ExecuteCommand(SqlCommand cmd)
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            cmd.Connection = connection;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            connection.Close();
            return ds;
        }

        public Submit InsertTeamSubmit(Submit submit)
        {
            SqlCommand cmd = new SqlCommand("TeamSubmit_Insert");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TeamID", submit.TeamId));
            cmd.Parameters.Add(new SqlParameter("@TeamTournamentAssignmentID", submit.TeamTournamentAssignmentId));
            cmd.Parameters.Add(new SqlParameter("@FileName", submit.FileName));
            cmd.Parameters.Add(new SqlParameter("@FileStream", submit.UploadStream));
            DataSet ds = ExecuteCommand(cmd);
            if (ds.Tables.Count > 0)
            {
                submit.ID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            return submit;

        }

        public List<Submit> GetTeamSubmitsForAssignment(int tournamentAssignmentId)
        {
            SqlCommand cmd = new SqlCommand("TeamSubmit_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@TournamentAssignmentId", tournamentAssignmentId));
            DataSet ds = ExecuteCommand(cmd);
            List<Submit> submits = new List<Submit>();
            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    submits.Add(CreateSubmitFromDataRow(dr));
                }
            }
            return submits;
        }

        public void DeleteTeamSubmit(int id)
        {
            SqlCommand cmd = new SqlCommand("TeamSubmit_Delete");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Id", id));
            ExecuteNonQuery(cmd);
        }


        public List<Submit> GetTournamentSubmits(int tournamentId)
        {
            //right now, don't do anything with tournamentId

            SqlCommand cmd = new SqlCommand("Tournament_Submits_SelectAll");
            cmd.Parameters.Add(new SqlParameter("@TournamentId", tournamentId));
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            DataSet ds = ExecuteCommand(cmd);

            List<Submit> submits = new List<Submit>();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Submit s = new Submit();
                    submits.Add(s);

                    if (dr["ID"] != DBNull.Value)
                    {
                        s.ID = Convert.ToInt32(dr["ID"]);
                        s.SubmitID = Convert.ToInt32(dr["ID"]);
                    }

                    if (dr["StartDate"] != DBNull.Value)
                    {
                        s.StartDate = Convert.ToDateTime(dr["StartDate"]);
                    }

                    if (dr["SubmitDate"] != DBNull.Value)
                    {
                        s.SubmitDate = Convert.ToDateTime(dr["SubmitDate"]);
                    }
                    s.IsFinished = Convert.ToBoolean(dr["IsFinished"]);
                    s.CurrentStatus = Convert.ToInt32(dr["StatusCode"]);
                    s.TeamId = Convert.ToInt32(dr["TeamId"]);
                    s.AssignmentId = Convert.ToInt32(dr["AssignmentId"]);
                    s.TournamentAssignmentId = Convert.ToInt32(dr["TournamentAssignmentId"]);
                    s.TeamName = dr["TeamName"].ToString();
                    s.TeamMembers = dr["TeamMembers"].ToString();
                    s.TournamentId = tournamentId;
                }
            }
            return submits;
        }



        private Submit CreateSubmitFromDataRow(DataRow dr)
        {
            Submit s = new Submit();
            s.ID = Convert.ToInt32(dr["ID"]);
            s.SubmitID = Convert.ToInt32(dr["ID"]);
            s.Details = dr["Details"].ToString();
            s.FileName = dr["FileName"].ToString();
            byte[] buffer = (byte[])dr["FileStream"];

            Encoding enc = new UTF8Encoding();
            s.FileContents = enc.GetString(buffer);
            s.UploadStream = buffer;

            s.StartDate = Convert.ToDateTime(dr["StartDate"]);
            s.StatusDate = Convert.ToDateTime(dr["StatusDate"]);
            s.SubmitDate = Convert.ToDateTime(dr["SubmitDate"]);
            s.IsFinished = Convert.ToBoolean(dr["IsFinished"]);
            s.CurrentStatus = Convert.ToInt32(dr["StatusCode"]);

            s.TeamId = Convert.ToInt32(dr["TeamId"]);
            s.AssignmentId = Convert.ToInt32(dr["AssignmentId"]);
            s.TournamentId = Convert.ToInt32(dr["TournamentId"]);
            s.TournamentAssignmentId = Convert.ToInt32(dr["TournamentAssignmentId"]);
            s.TeamTournamentAssignmentId = Convert.ToInt32(dr["TeamTournamentAssignmentId"]);

            return s;
        }

        public Submit GetTeamSubmitById(int id)
        {
            SqlCommand cmd = new SqlCommand("TeamSubmit_Select");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ID", id));
            DataSet ds = ExecuteCommand(cmd);
            Submit submit = new Submit();
            if (ds.Tables.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                submit = CreateSubmitFromDataRow(dr);
            }
            return submit;
   
        }

       
    }
}
