using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyVideoPlayer
{
    public partial class MainWindow : Window
    {
        private string _folderFilms;
        private Random _random = new Random();

        private PlayState _stateEnum;
        private PlayerState _state;

        private const string SETTINGS_FOLDER_FILMS = "FolderFilms";

        public MainWindow()
        {
            InitializeComponent();
            this.Cursor = Cursors.None;
            PART_MediaElement.MediaEnded += PART_MediaElement_MediaEnded;
            PART_MediaElement.MediaFailed += PART_MediaElement_MediaFailed;
            _stateEnum = PlayState.Stop;

            _folderFilms = ConfigurationManager.AppSettings[SETTINGS_FOLDER_FILMS];

            if (!Directory.Exists(_folderFilms))
            {
                MessageBox.Show(string.Format("Не найдена папка с фильмами: {0}", _folderFilms));
            }

            _state = new PlayerState()
            {
                Volume = 0.7
            };



            this.DataContext = _state;
        }

        private void Play()
        {
            switch (_stateEnum)
            {
                case PlayState.Stop:
                    var files = Directory.GetFiles(_folderFilms);

                    if (files.Length > 0)
                    {
                        var index = _random.Next(files.Length);
                        var fileName = files[index];
                        _state.FileTitle = System.IO.Path.GetFileNameWithoutExtension(fileName);
                        PART_MediaElement.Source = new Uri(fileName);
                        PART_MediaElement.Play();
                        _stateEnum = PlayState.Play;
                    }
                    break;

                case PlayState.Pause:
                    PART_MediaElement.Play();
                    _stateEnum = PlayState.Play;
                    break;
            }
        }

        private void Pause()
        {
            PART_MediaElement.Pause();
            _stateEnum = PlayState.Pause;
        }

        private void Stop()
        {
            PART_MediaElement.Stop();
            PART_MediaElement.Source = null;
            _stateEnum = PlayState.Stop;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    if (_stateEnum == PlayState.Play)
                    {
                        Pause();
                    }
                    else
                    {
                        Play();
                    }
                    break;

                case Key.Escape:
                    Stop();
                    break;

                case Key.Up:
                    _state.Volume += 0.05;
                    break;

                case Key.Down:
                    _state.Volume -= 0.05;
                    break;
            }
        }

        void PART_MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        void PART_MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Stop();
        }
    }

    public enum PlayState
    {
        Stop,
        Play,
        Pause
    }
}
