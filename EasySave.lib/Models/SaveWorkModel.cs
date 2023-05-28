

using System.ComponentModel;

namespace EasySave.lib.Models
{
    public class SaveWorkModel : INotifyPropertyChanged
    {
        public string NameSaveWork { get; set; }

        public int TypeSaveWork { get; set; }

        public string SourcePathSaveWork { get; set; }

        public string DestinationPathSaveWork { get; set; }

        private string _ProgressState = "Inactive";
        public string ProgressState
        {
            get { return _ProgressState; }
            set
            {
                if (_ProgressState != value)
                {
                    _ProgressState = value;
                    OnPropertyChanged("ProgressState");
                }
            }
        }

        private ProgressStateModel _ProgressStateModel { get; set; } = new ProgressStateModel();

        public ProgressStateModel ProgressStateModel
        {
            get { return _ProgressStateModel; }
            set
            {
                if (_ProgressStateModel != value)
                {
                    _ProgressStateModel = value;
                    OnPropertyChanged("ProgressStateModel");
                }
            }
        }

        private float _percentage;
        public float Percentage
        {
            get { return _percentage; }
            set
            {
                if (_percentage != value)
                {
                    _percentage = value;
                    OnPropertyChanged("Percentage");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AutoResetEvent PauseEvent = new AutoResetEvent(true);
        public AutoResetEvent StopEvent = new AutoResetEvent(false);
    }
}