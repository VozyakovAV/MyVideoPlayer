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

        private PlayerState _state;

        private const string SETTINGS_FOLDER_FILMS = "FolderFilms";

        public MainWindow()
        {
            InitializeComponent();
            this.Cursor = Cursors.None;
            PART_MediaElement.MediaEnded += PART_MediaElement_MediaEnded;
            PART_MediaElement.MediaFailed += PART_MediaElement_MediaFailed;

            _folderFilms = ConfigurationManager.AppSettings[SETTINGS_FOLDER_FILMS];

            if (!Directory.Exists(_folderFilms))
            {
                MessageBox.Show(string.Format("Не найдена папка с фильмами: {0}", _folderFilms));
            }

            _state = new PlayerState();
            this.DataContext = _state;
        }

        private void Play()
        {
            if (_state.Status == PlayerStatus.Stop)
            {
                LoadNextFilm();
            }

            if (PART_MediaElement.Source != null)
            {
                PART_MediaElement.Play();
                _state.Status = PlayerStatus.Play;
            }
        }

        private void LoadNextFilm()
        {
            var files = Directory.GetFiles(_folderFilms);

            if (files.Length > 0)
            {
                var index = _random.Next(files.Length);
                _state.FileName = files[index];
                PART_MediaElement.Source = new Uri(_state.FileName);
            }
        }

        private void Pause()
        {
            PART_MediaElement.Pause();
            _state.Status = PlayerStatus.Pause;
        }

        private void Stop()
        {
            PART_MediaElement.Stop();
            PART_MediaElement.Source = null;
            _state.Status = PlayerStatus.Stop;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    if (_state.Status == PlayerStatus.Play)
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
}
