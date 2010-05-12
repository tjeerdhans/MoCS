using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.ServiceModel.Web;
using MoCS.Business.Objects;
using System.ServiceModel;
using MoCS.Service.DataContracts;

namespace MoCS.WebClient.ServiceProxy
{
    public sealed class MoCSServiceProxy
    {
        #region Instance plumbing

        private static readonly MoCSServiceProxy _instance = new MoCSServiceProxy();

        private MoCSServiceProxy() { }

        /// <summary>
        /// Static initialization of the singleton as described in:
        /// http://msdn.microsoft.com/en-us/library/ms998558.aspx
        /// </summary>
        /// <value>The instance.</value>
        public static MoCSServiceProxy Instance
        {
            get { return _instance; }
        }

        private IMoCSService _moCSService = null;

        public IMoCSService MoCSService
        {
            get
            {
                if (_moCSService == null)
                {
                    _moCSService = CreateMoCSService();
                }
                return _moCSService;
            }
        }

        private IMoCSService CreateMoCSService()
        {
            //System.ServiceModel.WebHttpBinding binding = new System.ServiceModel.WebHttpBinding();
            //binding.ReaderQuotas = new XmlDictionaryReaderQuotas();
            //binding.ReaderQuotas.MaxArrayLength = XmlDictionaryReaderQuotas.Max.MaxArrayLength;
            //binding.ReaderQuotas.MaxBytesPerRead = XmlDictionaryReaderQuotas.Max.MaxBytesPerRead;
            //binding.ReaderQuotas.MaxDepth = XmlDictionaryReaderQuotas.Max.MaxDepth;
            //binding.ReaderQuotas.MaxNameTableCharCount = XmlDictionaryReaderQuotas.Max.MaxNameTableCharCount;
            //binding.ReaderQuotas.MaxStringContentLength = XmlDictionaryReaderQuotas.Max.MaxStringContentLength;
            ////binding.MaxBufferSize = int.MaxValue;
            ////binding.MaxBufferPoolSize = binding.MaxBufferSize;
            //binding.MaxReceivedMessageSize = 50000000;

            string serviceUri = ConfigurationManager.AppSettings["MoCSServiceURI"];
            Uri moCSServiceUri = new Uri(serviceUri);

            //WebChannelFactory<IMoCSService> channel = new WebChannelFactory<IMoCSService>(binding, moCSServiceUri);
            WebChannelFactory<IMoCSService> channel = new WebChannelFactory<IMoCSService>(moCSServiceUri);
            IMoCSService moCSService = channel.CreateChannel();

            return moCSService;
        }

        #endregion

        internal bool CheckCredentials(string username, string password)
        {
            bool result = false;

            using (new OperationContextScope((IClientChannel)MoCSService))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", username);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", password);

                TeamContract team = MoCSService.GetTeamByName(username);
                
                if (team != null && !string.IsNullOrEmpty(team.Password))
                {
                    result = true;
                    
                    //HACK!
                    HttpContext.Current.Session["teamId"] = team.Id;
                }
            }

            return result;
        }

        internal List<Tournament> GetTournaments(Credentials credentials)
        {
            List<Tournament> result = new List<Tournament>();

            using (new OperationContextScope((IClientChannel)MoCSService))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", credentials.Username);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", credentials.Password);

                TournamentsContract dcTournaments = MoCSService.GetTournaments();

                foreach (TournamentContract tournament in dcTournaments)
                {
                    result.Add(new Tournament()
                        {
                            Id = tournament.Id,
                            Name = tournament.Name
                        });
                }
            }

            return result;
        }

        internal Tournament GetTournament(int tournamentId, Credentials credentials)
        {
            Tournament result = new Tournament();

            using (new OperationContextScope((IClientChannel)MoCSService))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", credentials.Username);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", credentials.Password);

                TournamentContract dcTournament = MoCSService.GetTournament(tournamentId.ToString());

                result.Id = dcTournament.Id;
                result.Name = dcTournament.Name;
            }

            return result;
        }

        internal List<TournamentAssignment> GetTournamentAssignmentsForTournament(int tournamentId, Credentials credentials)
        {
            List<TournamentAssignment> result = new List<TournamentAssignment>();

            using (new OperationContextScope((IClientChannel)MoCSService))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", credentials.Username);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", credentials.Password);

                TournamentAssignmentsContract dcAssignments = MoCSService.GetTournamentAssignmentsForTournament(tournamentId.ToString());

                foreach (TournamentAssignmentContract dcTA in dcAssignments)
                {
                    result.Add(CreateBETournamentAssignment(dcTA));
                }
            }

            //result.Sort((n, m) => n.AssignmentOrder.CompareTo(m.AssignmentOrder));

            return result;
        }

        internal TournamentAssignment GetTournamentAssignment(int tournamentAssignmentId, Credentials credentials)
        {
            TournamentAssignment result;

            using (new OperationContextScope((IClientChannel)MoCSService))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", credentials.Username);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", credentials.Password);

                TournamentAssignmentContract dcTA = MoCSService.GetTournamentAssignment(tournamentAssignmentId.ToString());

                result = CreateBETournamentAssignment(dcTA);
            }

            return result;
        }

        internal List<AssignmentEnrollment> GetAssignmentEnrollmentsForTeamForTournament(int teamId, int tournamentId, Credentials credentials)
        {
            List<AssignmentEnrollment> result = new List<AssignmentEnrollment>();


            using (new OperationContextScope((IClientChannel)MoCSService))
            {
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Username", credentials.Username);
                WebOperationContext.Current.OutgoingRequest.Headers.Add("Password", credentials.Password);

                AssignmentEnrollmentsContract dcAEs = MoCSService.GetAssignmentEnrollmentsForTeamForTournament(teamId.ToString(), tournamentId.ToString());

                foreach (var dcAE in dcAEs)
                {
                    result.Add(CreateBEAssignmentEnrollment(dcAE));
                }
            }

            return result;
        }

        #region Translation helpers

        private Team CreateBETeam(TeamContract dcTeam)
        {
            Team result = new Team()
            {
                Id = dcTeam.Id,
                IsAdmin = dcTeam.IsAdmin,
                Members = dcTeam.Members,
                Name = dcTeam.Name,
                Score = dcTeam.Score,
                CreateDate = dcTeam.CreateDate
            };

            return result;
        }

        private TournamentAssignment CreateBETournamentAssignment(TournamentAssignmentContract dcTA)
        {
            TournamentAssignment result = new TournamentAssignment();

            result.Id = dcTA.Id;

            result.Assignment = new Assignment()
            {
                Id = dcTA.AssignmentId,
                Name = dcTA.AssignmentName,
                Difficulty = dcTA.Difficulty,
                FriendlyName = dcTA.FriendlyName,
                Tagline = dcTA.Tagline,
                Version = dcTA.Version,
                Category = dcTA.Category,
                Author = dcTA.Author
            };

            result.Tournament = new Tournament()
            {
                Id = dcTA.TournamentId
            };

            result.AssignmentOrder = dcTA.AssignmentOrder;
            result.Points1 = dcTA.Points1;
            result.Points2 = dcTA.Points2;
            result.Points3 = dcTA.Points3;
            result.CreateDate = dcTA.CreateDate;
            result.IsActive = dcTA.IsActive;

            return result;
        }

        private AssignmentEnrollment CreateBEAssignmentEnrollment(AssignmentEnrollmentContract dcAE)
        {
            AssignmentEnrollment result = new AssignmentEnrollment()
            {
                Id = dcAE.Id,
                IsActive = dcAE.IsActive,
                StartDate = dcAE.StartDate.Value,
                Team = new Team() { Id = dcAE.TeamId },
                TournamentAssignment = new TournamentAssignment()
                {
                    Id = dcAE.TournamentAssignmentId,
                    Tournament = new Tournament() { Id = dcAE.TournamentId }
                }
            };

            return result;
        }
        #endregion
    }
}
