using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Windows;
using MoCS.Client.Model;
using System.ServiceModel.Web;
using MoCS.Service.DataContracts;
using MoCS.Client.RestService;
using System.Collections.ObjectModel;
using System.Xml;

namespace MoCS.Client
{

    public class MoCSServiceProxy
    {
        MoCS.Client.RestService.IMoCSService _service = TeamSession.GetInstance().RestService;


        public Team Login(string teamname, string password)
        {
            Team result = null;

            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", teamname);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", password);

                try
                {
                    Teams teams = _service.GetAllTeams();
                    result = teams.Find(ByName(teamname));

                    if (result!= null && result.Password != password)
                    {
                        result = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }

            return result;
        }

        static Predicate<Team> ByName(string teamName) { return delegate(Team team) { return team.TeamName.ToLower() == teamName.ToLower(); }; }


        public void DeleteSubmit(string submitId)
        {
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                _service.DeleteSubmit(submitId);
            }
        }


        public void UpdateTournamentAssignment(string tournamentId, string assignmentId, Assignment assignment)
        {
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                _service.UpdateTournamentAssignment(tournamentId, assignmentId, assignment);
            }
        }


        public List<Tournament> GetTournaments()
        {
            List<Tournament> tournaments = null;
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                tournaments = _service.GetTournaments();
                foreach (Tournament t in tournaments)
                {
                    if (t.TournamentId == TeamSession.GetInstance().CurrentTournamentId.ToString())
                    {
                        t.Current = true;
                    }
                    else
                    {
                        t.Current = false;
                    }
                }
            }
            return tournaments;
        }

        public Assignment StartTeamTournamentAssignment(int teamId, int tournamentAssignmentId)
        {
            Assignment a = null;
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                a = new Assignment();
                a.TeamId = teamId.ToString();
                a.TournamentAssignmentId = tournamentAssignmentId.ToString();

                a = _service.AddTeamTournamentAssignment(a);

                
            }
            return a;
        }



        public void Upload(byte[] bytes, string fileName)
        {
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                MoCS.Service.DataContracts.Submit submit = new MoCS.Service.DataContracts.Submit();
                submit.FileName = fileName;
                submit.Payload = bytes;
                submit.TeamTournamentAssignmentId = TeamSession.GetInstance().CurrentTeamTournamentAssignmentId.ToString();

                submit = _service.AddSubmitForAssignmentForTeam(TeamSession.GetInstance().CurrentTeam.TeamId,
                                    TeamSession.GetInstance().CurrentTeamTournamentAssignmentId.ToString(),
                                        submit);
            }
        }




        public Submits GetTournamentTeamSubmits()
        {
            Submits result = null;
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    result = _service.GetTournamentSubmits(TeamSession.Instance.CurrentTournamentId.ToString());
                }
                catch (Exception)
                {
                    result = null;
                }
            }
            return result;

        }


        public MoCS.Service.DataContracts.Submit GetSubmitDetails(string teamSubmitId)
        {
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    return _service.GetTeamSubmit(teamSubmitId);
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }


        public Teams GetTeams()
        {
            Teams teams = null;
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    teams = _service.GetAllTeams();
                }
                catch (Exception ex)
                {
                    teams = null;
                    MessageBox.Show(ex.Message);
                }
            }

            return teams;
        }

        public MoCS.Service.DataContracts.Team UpdateTeam(Team team)
        {
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    team = _service.UpdateTeam(team.TeamId, team);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return team;
        }

        public void DeleteTeam(Team team)
        {
            team.TeamStatus = TeamStatus.Closed;
            UpdateTeam(team);
        }

        public Team InsertTeam(Team team)
        {
            MoCS.Client.RestService.IMoCSService service = TeamSession.GetInstance().RestService;

            using (new OperationContextScope((IClientChannel)service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    team = service.AddTeam(team);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return team;
        }


        public string GetTournamentReport(int tournamentId)
        {
            string report = null;

            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    report = _service.GetTournamentReport(tournamentId.ToString());
                }
                catch (Exception)
                {

                    //   this.State = ModelState.Invalid;
                }
            }
            return report;

        }


        public ObservableCollection<TeamViewModel> GetTeamViewModels()
        {
            ObservableCollection<TeamViewModel> result = new ObservableCollection<TeamViewModel>();

            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    MoCS.Service.DataContracts.Teams teams = _service.GetAllTeams();
                    foreach (MoCS.Service.DataContracts.Team team in teams)
                    {
                        if (team.TeamType == TeamType.Normal)
                        {
                            result.Add(ViewModelFactory.CreateTeam(team));
                        }
                    }
                }
                catch (Exception)
                {

                 //   this.State = ModelState.Invalid;
                }
            }



            return result;

        }


        public ObservableCollection<AssignmentViewModel> GetTeamTournamentAssignments()
        {
            MoCS.Client.RestService.IMoCSService service = TeamSession.GetInstance().RestService;

            ObservableCollection<AssignmentViewModel> result = new ObservableCollection<AssignmentViewModel>();

            using (new OperationContextScope((IClientChannel)service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    Assignments assignments = service.GetTeamTournamentAssignments(TeamSession.GetInstance().CurrentTeam.TeamId,
                                                                                    TeamSession.GetInstance().CurrentTournamentId.ToString());

                    foreach (MoCS.Service.DataContracts.Assignment assignment in assignments)
                    {
                        result.Add(ViewModelFactory.CreateAssignment(assignment));
                    }
                }
                catch (Exception)
                {
                   // this.State = ModelState.Invalid;
                }
            }

            return result;
        }


        public ObservableCollection<TeamSubmitViewModel> GetMonitorSubmitsForTeamAndAssignment()
        {
            ObservableCollection<TeamSubmitViewModel> result = new ObservableCollection<TeamSubmitViewModel>();

            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {

                    Submits submits = _service.GetTeamTournamentAssignmentSubmits(
                        TeamSession.GetInstance().CurrentTeam.TeamId.ToString(),
                            TeamSession.GetInstance().CurrentTournamentId.ToString(),
                            TeamSession.GetInstance().CurrentTeamTournamentAssignmentId.ToString());

                    submits.Sort(new SubmitsComparer());


                    foreach (MoCS.Service.DataContracts.Submit submit in submits)
                    {
                        result.Add(ViewModelFactory.CreateTeamSubmit(submit));
                    }
                }
                catch (Exception)
                {

                  //  this.State = ModelState.Invalid;
                }
            }

            return result;
        }



        public ObservableCollection<AssignmentViewModel> GetTournamentAssignments()
        {

            MoCS.Client.RestService.IMoCSService service = TeamSession.GetInstance().RestService;

            ObservableCollection<AssignmentViewModel> result = new ObservableCollection<AssignmentViewModel>();

            using (new OperationContextScope((IClientChannel)service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    Assignments assignments = service.GetTournamentAssignments(TeamSession.GetInstance().CurrentTournamentId.ToString());

                    foreach (MoCS.Service.DataContracts.Assignment assignment in assignments)
                    {
                        result.Add(ViewModelFactory.CreateAssignment(assignment));
                    }
                }
                catch (Exception)
                {
                    result = null;
                }
            }

            return result;
        }



        public Teams GetTeamsForMatrix()
        {
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    return _service.GetAllTeams();
                }
                catch (Exception)
                {

                  //  this.State = ModelState.Invalid;
                }
            }

            return null;
        }


        public Submits GetTournamentSubmitsForMatrix()
        {
            using (new OperationContextScope((IClientChannel)_service))
            {
                try
                {
                    return _service.GetTournamentSubmits(TeamSession.GetInstance().CurrentTournamentId.ToString());
                }
                catch (Exception)
                {
                  //  this.State = ModelState.Invalid;
                }
            }

            return null;
        }

        public Assignments GetTournamentAssignmentsForMatrix()
        {
            ObservableCollection<AssignmentViewModel> result = new ObservableCollection<AssignmentViewModel>();

            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    return _service.GetTournamentAssignments(TeamSession.GetInstance().CurrentTournamentId.ToString());
                }
                catch (Exception)
                {
                   // this.State = ModelState.Invalid;
                }
            }
            return null;
        }


        public MoCS.Service.DataContracts.Assignment GetTeamAssignment()
        {
            MoCS.Service.DataContracts.Assignment teamAssignment = null;
            using (new OperationContextScope((IClientChannel)_service))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", TeamSession.GetInstance().TeamName);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", TeamSession.GetInstance().Password);

                try
                {
                    teamAssignment = _service.GetTeamTournamentAssignment(TeamSession.GetInstance().CurrentTeam.TeamId,
                                                                    TeamSession.GetInstance().CurrentTeamTournamentAssignmentId.ToString());
                    if (teamAssignment == null)
                    {
                        //if not, add it to their collection (and set the startdate and time)
                        MoCS.Service.DataContracts.Assignment a = new MoCS.Service.DataContracts.Assignment();
                        a.AssignmentId = TeamSession.GetInstance().CurrentTeamTournamentAssignmentId.ToString();

                        teamAssignment = _service.GetTeamTournamentAssignment(TeamSession.GetInstance().CurrentTeam.TeamId,
                                                                    TeamSession.GetInstance().CurrentTeamTournamentAssignmentId.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return null;
                }
            }
            return teamAssignment;

        }

    }

    public class SubmitsComparer : IComparer<Submit>
    {

        #region IComparer<Submit> Members

        public int Compare(Submit x, Submit y)
        {
            return y.SubmitDate.CompareTo(x.SubmitDate);
        }

        #endregion
    }
}
