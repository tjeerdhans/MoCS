using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using MoCS.Client.Business;
using MoCS.Client.Business.Entities;
using System.ServiceModel;
using MoCS.Service.DataContracts;
using MoCS.Client.Page;
using System.ServiceModel.Web;
using System.Configuration;

namespace MoCS.Client.Model
{
    public class MonitorSubmitsViewModel : ViewModel
    {
        MoCSServiceProxy _proxy = new MoCSServiceProxy();
        private ObservableCollection<MonitorSubmitViewModel> _submits;
        private System.Timers.Timer _timer;
        private SubmitsMatrix _matrix;
        public MonitorSubmitsViewModel()
        {
            _submits = new ObservableCollection<MonitorSubmitViewModel>();

            this.State = ModelState.Fectching;

            // Queue a work item to fetch the teams
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(FetchSubmitsCallback)))
            {
                this.State = ModelState.Invalid;
            }
            
            _timer = new System.Timers.Timer();
            _timer.Interval = GetSubmitsMatrixTimeOut();
            _timer.Start();
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);

        }


        private int GetSubmitsMatrixTimeOut()
        {
            string s = ConfigurationSettings.AppSettings["SubmitsMatrixTimeOut"];
            int i = Convert.ToInt32(s);
            return i;
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Queue a work item to fetch the teams
            if (!ThreadPool.QueueUserWorkItem(new WaitCallback(FetchSubmitsCallback)))
            {
                this.State = ModelState.Invalid;
            }
        }


        public ObservableCollection<MonitorSubmitViewModel> Submits
        {
            get{ return _submits;}
            set{_submits = value;
            OnPropertyChanged("Submits");}
        }


        public SubmitsMatrix Matrix
        {
            get { return _matrix; }
            set
            {
                _matrix = value;
                OnPropertyChanged("Matrix");
            }
        }




  

        private SubmitsMatrix CreateMatrix(Submits submits)
        {
            SubmitsMatrix matrix = new SubmitsMatrix();

            //set the assignments

            Assignments assignments = _proxy.GetTournamentAssignmentsForMatrix();
            MatrixAssignment[] matrixAssignments = new MatrixAssignment[assignments.Count];
            int index = 0;
            foreach (MoCS.Service.DataContracts.Assignment a in assignments)
            {
                matrixAssignments[index] = new MatrixAssignment(a.AssignmentId,
                                                    a.AssignmentName, a.Active);
                index++;
            }
            matrix.Assignments = matrixAssignments;

            //set the teams
            Teams teams = _proxy.GetTeamsForMatrix();

            int numberOfNormalTeams = teams.Count(c=>c.TeamType==TeamType.Normal);

            var noAdminTeams = (from noadminteam in teams
                               where noadminteam.TeamType == TeamType.Normal
                               select noadminteam);

            MatrixTeam[] matrixTeams = new MatrixTeam[numberOfNormalTeams];
            Dictionary<string, MatrixTeam> dictionaryTeams = new Dictionary<string, MatrixTeam>();
            index = 0;
            foreach (MoCS.Service.DataContracts.Team team in noAdminTeams)
            {
                    matrixTeams[index] = new MatrixTeam(team.TeamId, team.TeamName, team.TeamMembers);
                    matrixTeams[index].Me = (TeamSession.Instance.TeamName.ToUpper() == team.TeamName.ToUpper());
                    dictionaryTeams.Add(team.TeamId, matrixTeams[index]);   
                index++;
            }
            
            matrix.Teams = matrixTeams;

            if (submits == null)
            {
                return matrix;
            }

            //select the submits that have no submitID
            //  these are the submits that have started, but no submits
            var startedonly = from submit in submits
                    where submit.SubmitId == "0"
                              select new { submit.TeamId, submit.AssignmentId };

            //select the submits that still have to be processed
            var waitingsubmits = from submit in submits
                    where submit.IsFinished = true
                    where submit.SubmitId != "0"
                    where submit.CurrentStatusCode == 0
                                 select new { submit.TeamId, submit.AssignmentId };

            //select the submits that have failed
            var failingsubmits = from submit in submits
                                 where submit.IsFinished = true
                                 where submit.CurrentStatusCode != 0
                                 where submit.CurrentStatusCode != 1
                                 select new { submit.TeamId, submit.AssignmentId };


            Dictionary<string, MatrixSubmit> matrixSubmits = new Dictionary<string, MatrixSubmit>();
            foreach (var x in startedonly)
            {
                matrixSubmits.Add(x.TeamId + "_" + x.AssignmentId, new MatrixSubmit(x.TeamId, x.AssignmentId, "1"));
                dictionaryTeams[x.TeamId].SortingPoints += 1;
            }
            foreach (var x in waitingsubmits)
            {
                matrixSubmits.Add(x.TeamId + "_" + x.AssignmentId, new MatrixSubmit(x.TeamId, x.AssignmentId, "2"));                
                dictionaryTeams[x.TeamId].SortingPoints += 10;
            }
            foreach (var x in failingsubmits)
            {
                matrixSubmits.Add(x.TeamId + "_" + x.AssignmentId, new MatrixSubmit(x.TeamId, x.AssignmentId, "3"));
            }

            //loop per assignment to hand out medals
            foreach (MoCS.Service.DataContracts.Assignment a in assignments)
            {
                var winningsubmits = from submit in submits
                                     where submit.IsFinished = true
                                     where submit.CurrentStatusCode == 1
                                     where submit.TournamentAssignmentId.ToString() == a.TournamentAssignmentId
                                     orderby (submit.DeltaTime) ascending
                                     select new { submit.TeamId, submit.AssignmentId, submit.DeltaTime};
                int medalsGiven = 0;

                foreach(var x in winningsubmits)
                {
                    string value = "";
                    int sortingValue = 0;
                    switch(medalsGiven)
                    {
                        case 0:
                            value = "G"; //gold
                            sortingValue = 10000;
                            medalsGiven++;
                            break;
                        case 1:
                            value = "S"; //silver
                            sortingValue = 2000;
                            medalsGiven++;
                            break;
                        case 2:
                            value = "B"; //bronze
                            sortingValue = 250;
                            medalsGiven++;
                            break;
                        default:
                            value = "F"; //finished, but no medal
                            sortingValue = 50;
                            break;
                    }
                    matrixSubmits.Add(x.TeamId + "_" + x.AssignmentId, new MatrixSubmit(x.TeamId, x.AssignmentId, value));
                    dictionaryTeams[x.TeamId].SortingPoints += sortingValue;
                }
            }

            matrix.Submits = matrixSubmits;
            Array.Sort(matrix.Teams);
            return matrix;

        }

        private void FetchSubmitsCallback(object state)
        {
          
            Submits submits = _proxy.GetTournamentSubmitsForMatrix();

            SubmitsMatrix matrix = CreateMatrix(submits);

            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,

                             new ThreadStart(delegate
                             {
                                 this.Matrix = matrix;
                                 this.State = ModelState.Active;
                             }));
        }



    }
}
