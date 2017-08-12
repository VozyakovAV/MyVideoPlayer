using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
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

        #region WindowTopmost
        // Приложение всегда поверх остальных

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            m_KeepActiveWorker.CancelAsync();
            m_KeepActiveWorker.Dispose();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = (new WindowInteropHelper(this)).Handle;
            m_KeepActiveWorker.DoWork += KeepActive;
            m_KeepActiveWorker.WorkerSupportsCancellation = true;
            m_KeepActiveWorker.RunWorkerAsync(new object[] { this, PART_MediaElement });
        }
        static BackgroundWorker m_KeepActiveWorker = new BackgroundWorker();
        static void KeepActive(object sender, DoWorkEventArgs e)
        {
            if (m_KeepActiveWorker.CancellationPending)
                e.Cancel = true;

            while (!m_KeepActiveWorker.CancellationPending)
            {
                Window window = (e.Argument as object[])[0] as Window;
                UIElement focus = (e.Argument as object[])[1] as UIElement;
                bool isActive = false;
                do
                {
                    Thread.Sleep(1000);

                    if (!m_KeepActiveWorker.CancellationPending && window != null)
                    {
                        window.Dispatcher.Invoke((Action)(() =>
                        {
                            isActive = window.IsActive;
                            window.Activate();
                            Keyboard.Focus(focus);
                        }));
                    }

                } while (!isActive);
            }
        }

        #endregion
    }
}
