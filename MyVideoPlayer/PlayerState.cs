using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace MyVideoPlayer
{
    public class PlayerState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Timer _timerVisibleTitle;

        public PlayerState()
        {
            _timerVisibleTitle = new Timer(4000);
            _timerVisibleTitle.Elapsed += timer_visibleTitle;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(FileTitle))
                    return string.Format("Громкость {0}%", Volume * 100);
                else
                    return string.Format("{0}. Громкость {1}%", FileTitle, Volume * 100);
            }
        }

        private string _fileTitle;
        public string FileTitle
        {
            get { return _fileTitle; }
            set
            {
                _fileTitle = value;
                RaisePropertyChanged("FileTitle");
                OnVisibleTitle();
            }
        }

        private double _volume;
        public double Volume
        {
            get { return _volume; }
            set
            {
                if (value < 0) 
                    value = 0;

                if (value > 1) 
                    value = 1;
                    
                _volume = Math.Round(value, 2);
                RaisePropertyChanged("Volume");
                OnVisibleTitle();
            }
        }

        private bool _isVisibleTitle;
        public bool IsVisibleTitle
        {
            get { return _isVisibleTitle; }
            set
            {
                _isVisibleTitle = value;
                RaisePropertyChanged("IsVisibleTitle");
            }
        }

        private void OnVisibleTitle()
        {
            RaisePropertyChanged("Title");
            IsVisibleTitle = true;
            _timerVisibleTitle.Stop();
            _timerVisibleTitle.Start();
        }

        void timer_visibleTitle(object sender, ElapsedEventArgs e)
        {
            IsVisibleTitle = false;
        }
    }
}
