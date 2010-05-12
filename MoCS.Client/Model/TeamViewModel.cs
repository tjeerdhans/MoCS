using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MoCS.Client.Model
{
    public class TeamViewModel : ViewModel
    {

        private string _name;
        private string _teamMembers;
        private int _points=0;
        public bool Me { get; set; }

        public TeamViewModel()
        {
        }


        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public string TeamMembers
        {
            get { return _teamMembers; }
            set { _teamMembers = value; OnPropertyChanged("TeamMembers"); }
        }

        public int Points
        {
            get { return _points; }
            set { _points = value; OnPropertyChanged("Points"); }

        }



    }
}
