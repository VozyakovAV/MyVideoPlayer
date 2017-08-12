using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
            this.Volume = 0.7;
            this.Status = PlayerStatus.Stop;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private PlayerStatus _status;
        public PlayerStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                switch(value)
                {
                    case PlayerStatus.Play: OnVisibleTitle(); break;
                    case PlayerStatus.Pause: break;
                    case PlayerStatus.Stop: OnHideTitle(); break; 
                }
            }
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

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
            }
        }

        public string FileTitle
        {
            get { return string.IsNullOrEmpty(FileName) ? string.Empty : Path.GetFileNameWithoutExtension(FileName); }
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

        private void OnHideTitle()
        {
            IsVisibleTitle = false;
            _timerVisibleTitle.Stop();
        }

        void timer_visibleTitle(object sender, ElapsedEventArgs e)
        {
            OnHideTitle();
        }
    }

    public enum PlayerStatus
    {
        Stop,
        Play,
        Pause
    }
}
