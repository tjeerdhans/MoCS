using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoCS.Client.Model;
using MoCS.Client.Controls.Matrix;

namespace MoCS.Client.Page
{
    /// <summary>
    /// Interaction logic for PageAllSubmits.xaml
    /// </summary>
    public partial class PageAllSubmits : System.Windows.Controls.Page
    {
        public PageAllSubmits()
        {
            InitializeComponent();
        }
       
        MonitorSubmitsViewModel _model;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            _model = new MonitorSubmitsViewModel();

            this.matrix.DataContext = _model.Matrix;
            _model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_model_PropertyChanged);
         }

        void _model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Matrix")
            {
                this.matrix.DataContext = _model.Matrix;
            }
        }

       
    }


    public class SubmitsMatrix : MatrixBase<MatrixTeam, MatrixAssignment>
    {
        #region Constructor

        public SubmitsMatrix()
        {
            _teams = null;
            _assignments = null;
            _submits = null;
        }

        public MatrixTeam[] Teams
        {
            get { return _teams; }
            set { _teams = value; }
        }

        public MatrixAssignment[] Assignments
        {
            get { return _assignments; }
            set { _assignments = value; }
        }

     
        public Dictionary<string, MatrixSubmit> Submits
        {
            get { return _submits; }
            set { _submits = value; }
        }

        #endregion // Constructor

        #region Base Class Overrides

        protected override IEnumerable<MatrixAssignment> GetColumnHeaderValues()
        {
            return _assignments;
        }

        protected override IEnumerable<MatrixTeam> GetRowHeaderValues()
        {
            return _teams;
        }

        protected override object GetCellValue(MatrixTeam rowHeaderValue, MatrixAssignment columnHeaderValue)
        {
            string result = "0";

            string key = rowHeaderValue.Id + "_" + columnHeaderValue.Id;

            if (_submits.ContainsKey(key))
            {
                MatrixSubmit s = (_submits[key]);
                result = s.Value;
            }

            return result;
        }


        #endregion // Base Class Overrides

        #region Fields

        private MatrixTeam[] _teams;
        private MatrixAssignment[] _assignments;
        private Dictionary<string, MatrixSubmit> _submits;


        #endregion // Fields
    }


    public class MatrixAssignment
    {
        public string Id { get; private set; }
        public string AssignmentName { get; private set; }
        public bool Active { get; private set; }

        public MatrixAssignment(string id, string name, bool active)
        {
            Id = id;
            AssignmentName = name;
            Active = active;

        }
    }

    public class MatrixTeam : IComparable
    {
        public string Id { get; private set; }
        public string TeamName { get; private set; }
        public string Members { get; private set; }
        public bool Me { get; set; }
        public int SortingPoints { get; set; }

        public MatrixTeam(string id, string name, string members)
        {
            Id = id;
            TeamName = name;
            Members = members;
            Me = false;
        }


        #region IComparable Members

        public int CompareTo(object obj)
        {
            return ((MatrixTeam)obj).SortingPoints.CompareTo(this.SortingPoints);
        }

        #endregion

    }

    public class MatrixSubmit
    {
        public string TeamId { get; set; }
        public string AssignmentId { get; set; }
        public string Value { get; set; }

        public MatrixSubmit(string teamId, string assignmentId, string value)
        {
            TeamId = teamId;
            AssignmentId = assignmentId;
            Value = value;
        }

    
    }
}
