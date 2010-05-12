using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using MoCS.Client.Business;
using MoCS.Client.Business.Entities;
using System.Windows;
using System.ServiceModel;
using System.ServiceModel.Web;
using MoCS.Service.DataContracts;
using System.Configuration;

namespace MoCS.Client.Model
{

    /// </summary>
    public class TournamentViewModel : ViewModel
    {
        private ObservableCollection<TeamViewModel> _teams;

        private System.Timers.Timer _t;

        private System.Timers.Timer _teamSubmitsTimer;

        private static TournamentViewModel _instance;
        private AssignmentViewModel _currentAssignment;
        private ObservableCollection<AssignmentViewModel> _tournamentAssignments;
       
        private ObservableCollection<TeamSubmitViewModel> _teamSubmits;

        public static TournamentViewModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TournamentViewModel();
            }
            return _instance;
        }

        private TournamentViewModel()
        {
            Teams = new ObservableCollection<TeamViewModel>();
            _tournamentAssignments = new ObservableCollection<AssignmentViewModel>();

            _teamSubmits = new ObservableCollection<TeamSubmitViewModel>();
           
            this.State = ModelState.Fectching;

            // Queue a work item to fetch the teams
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(FetchTeamsCallback)))
            {
                this.State = ModelState.Invalid;
            }

            // Queue a work item to fetch the assignments
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(FetchTournamentAssignmentsCallback)))
            {
                this.State = ModelState.Invalid;
            }

            _t = new System.Timers.Timer();
            _t.Interval = GetTeamAndAssignmentTimeOut();
            _t.Elapsed += new System.Timers.ElapsedEventHandler(_t_Elapsed);
            _t.Start();
        
        }


        private int GetTeamAndAssignmentTimeOut()
        {
            string s = ConfigurationSettings.AppSettings["AssignmentTimeOut"];
            int i = Convert.ToInt32(s);
            return i;
        }

        private int GetSubmitsTimeOut()
        {
            string s = ConfigurationSettings.AppSettings["SubmitsTimeOut"];
            int i = Convert.ToInt32(s);
            return i;
        }

        public void StartPollingTeamSubmits()
        {

            //do an intial load and then start the timer
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(FetchTeamSubmitsCallback)))
            {
                this.State = ModelState.Invalid;
            }
            
            if (_teamSubmitsTimer == null)
            {
                _teamSubmitsTimer = new System.Timers.Timer();
                _teamSubmitsTimer.Interval = GetSubmitsTimeOut();
                _teamSubmitsTimer.Elapsed += new System.Timers.ElapsedEventHandler(_teamSubmitsTimer_Elapsed);
                _teamSubmitsTimer.Start();
            }
        }

        void _teamSubmitsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Queue a work item to fetch the teams
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(FetchTeamSubmitsCallback)))
            {
                this.State = ModelState.Invalid;
            }
        }

        public void StopPollingTeamSubmits()
        {
            if (_teamSubmitsTimer != null)
            {
                _teamSubmitsTimer.Stop();
                _teamSubmitsTimer = null;
            }
        }

        public AssignmentViewModel CurrentAssignment
        {
            get { return _currentAssignment; }
            set { _currentAssignment = value;
                    OnPropertyChanged("CurrentAssignment");
            }
        }

        public ObservableCollection<TeamSubmitViewModel> TeamSubmits
        {
            get { return _teamSubmits; }
            set { _teamSubmits = value;
            OnPropertyChanged("TeamSubmits");
            }
        }

        
        public ObservableCollection<AssignmentViewModel> TournamentAssignments
        {
            get { return _tournamentAssignments; }
            set { _tournamentAssignments = value;
            OnPropertyChanged("TournamentAssignments");
            }
        }


        void _t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Queue a work item to fetch the teams
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(FetchTeamsCallback)))
            {
                this.State = ModelState.Invalid;
            }

            // Queue a work item to fetch the teams
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(FetchTournamentAssignmentsCallback)))
            {
                this.State = ModelState.Invalid;
            }

        }


        private void StopPolling()
        {
            State = ModelState.Invalid;
            _t.Stop();
            if (_teamSubmitsTimer != null)
            {
                _teamSubmitsTimer.Stop();
            }
        }

        public ObservableCollection<TeamViewModel> Teams
        {
            get
            {
                VerifyCalledOnUIThread();
                return _teams;
            }

            private set
            {
                VerifyCalledOnUIThread();
                _teams = value;
                OnPropertyChanged("Teams");
            }

        }



        private void FetchTeamSubmitsCallback(object state)
        {
            MoCSServiceProxy proxy = new MoCSServiceProxy();
            ObservableCollection<TeamSubmitViewModel> submits = proxy.GetMonitorSubmitsForTeamAndAssignment();

            //todo: sort these in descending order
            
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,

                             new ThreadStart(delegate
                             {
                                 this.TeamSubmits = submits;
                                 this.State = ModelState.Active;
                             }));
        }







        private void FetchTournamentAssignmentsCallback(object state)
        {
            MoCSServiceProxy proxy = new MoCSServiceProxy();
            ObservableCollection<AssignmentViewModel> asgmts = proxy.GetTeamTournamentAssignments();

            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,

                  new ThreadStart(delegate
                  {
                      this.TournamentAssignments = asgmts;
                      this.State = ModelState.Active;
                  }));
        }


       


        private void FetchTeamsCallback(object state)
        {
            ObservableCollection<TeamViewModel> teams;
            try
            {
                MoCSServiceProxy proxy = new MoCSServiceProxy();
                teams = proxy.GetTeamViewModels(); 

                this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,

                    new ThreadStart(delegate
                    {
                        this.Teams = teams;
                        this.State = ModelState.Active;
                    }));

            }
            catch (Exception)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                    new ThreadStart(delegate
                                    { this.State = ModelState.Invalid; }));
            }
        }
    }
}
