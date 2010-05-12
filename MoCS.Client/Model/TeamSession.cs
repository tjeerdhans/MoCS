using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoCS.Client.Business;
using System.ServiceModel;
using System.ServiceModel.Web;
using MoCS.Service.DataContracts;
using System.Configuration;
using System.Xml;

namespace MoCS.Client.Model
{
    public class TeamSession 
    {
        public static TeamSession _instance;

        public string TeamName { get; set; }
        public string Password { get; set; }
        public bool IsLoggedIn {get;set;}
        public bool IsAdmin { get; set; }
        public int CurrentTeamTournamentAssignmentId { get; set; }
        public Team CurrentTeam { get; set; }
        public AssignmentViewModel CurrentAssignment { get; set; }
        public int CurrentTournamentId { get; set; }
        public Tournament CurrentTournament { get; set; }

        private MoCS.Client.RestService.IMoCSService _mocsService = null;

        public string CurrentAssignmentName
        {
            get{
            if(CurrentAssignment!=null)
            {
                return CurrentAssignment.DisplayName;
            }
            return "";
            }
        }



        public MoCS.Client.RestService.IMoCSService RestService 
        {
            get
            {
                //lazy loading
                if (_mocsService == null)
                {
                    _mocsService = CreateRestService();
                }
                return _mocsService;
            }
            private set { } 
        
        }

        private TeamSession()
        {
            IsLoggedIn = false;
            TeamName = string.Empty;
            Password = string.Empty;
        }

        public static TeamSession Instance
        {
            get { return GetInstance(); }
        }

        public static TeamSession GetInstance()
        {
            if (_instance == null)
            {
                _instance = new TeamSession();
            }

            return _instance;
        }


         private static MoCS.Client.RestService.IMoCSService CreateRestService()
        {
            string serviceUri = ConfigurationSettings.AppSettings["ServiceUri"];

            System.ServiceModel.WebHttpBinding binding = new System.ServiceModel.WebHttpBinding();
            

            binding.ReaderQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.MaxArrayLength = XmlDictionaryReaderQuotas.Max.MaxArrayLength;
            binding.ReaderQuotas.MaxBytesPerRead = XmlDictionaryReaderQuotas.Max.MaxBytesPerRead;
            binding.ReaderQuotas.MaxDepth = XmlDictionaryReaderQuotas.Max.MaxDepth;
            binding.ReaderQuotas.MaxNameTableCharCount = XmlDictionaryReaderQuotas.Max.MaxNameTableCharCount;
            binding.ReaderQuotas.MaxStringContentLength = XmlDictionaryReaderQuotas.Max.MaxStringContentLength;

//            binding.MaxBufferSize = int.MaxValue;
//            binding.MaxBufferPoolSize = binding.MaxBufferSize;
            
            binding.MaxReceivedMessageSize = 50000000;

             
             Uri moCSServiceUri = new Uri(serviceUri);


            WebChannelFactory<MoCS.Client.RestService.IMoCSService> channel = new WebChannelFactory<MoCS.Client.RestService.IMoCSService>(binding, moCSServiceUri);
            MoCS.Client.RestService.IMoCSService moCSService = channel.CreateChannel();


           

   


            return moCSService;
        }

    }
}
