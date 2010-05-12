using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;

namespace MoCS.Client.Model
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        protected Dispatcher Dispatcher {get;private set;}
        private ModelState _state;

        public ViewModel()
        {
             Dispatcher = Dispatcher.CurrentDispatcher;
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        public ModelState State
        {
            get
            {
                VerifyCalledOnUIThread();
                return _state;
            }
            set
            {
                VerifyCalledOnUIThread();
                _state = value;
                OnPropertyChanged("State");
            }
        }

        [Conditional("Debug")]
        protected void VerifyCalledOnUIThread()
        {
            Debug.Assert(Dispatcher.CurrentDispatcher == this.Dispatcher,
                "Call must be made on UI thread.");
        }

    }
}
