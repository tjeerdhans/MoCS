using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client.Model
{
    public class MonitorSubmitViewModel : ViewModel
    {
        public string TeamName { get; set; }
        public DateTime SubmitDate { get; set; }
        public DateTime StatusDate { get; set; }
        public bool IsFinished { get; set; }
        public string CurrentStatus { get; set; }
        public string TeamMembers { get; set; }

        private int _statusCode;

        public int CurrentStatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; SetCurrentStatus(); }
        }


        private void SetCurrentStatus()
        {
            CurrentStatus = ViewModelFactory.ConvertStatus(CurrentStatusCode);
        }
    }
}
