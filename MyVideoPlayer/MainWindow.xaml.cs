using System;
using System.Collections.Generic;
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
        private string _folderFiles = @"C:\Films";
        private Random _random = new Random();

        private PlayState _state;

        public MainWindow()
        {
            InitializeComponent();
            this.Cursor = Cursors.None;
            PART_MediaElement.Volume = 1;
            PART_MediaElement.MediaEnded += PART_MediaElement_MediaEnded;
            PART_MediaElement.MediaFailed += PART_MediaElement_MediaFailed;
            _state = PlayState.Stop;
        }

        private void Play()
        {
            switch (_state)
            {
                case PlayState.Stop:
                    var files = Directory.GetFiles(_folderFiles);

                    if (files.Length > 0)
                    {
                        var index = _random.Next(files.Length);
                        PART_MediaElement.Source = new Uri(files[index]);
                        PART_MediaElement.Play();
                        _state = PlayState.Play;
                    }
                    break;

                case PlayState.Pause:
                    PART_MediaElement.Play();
                    _state = PlayState.Play;
                    break;
            }
        }

        private void Pause()
        {
            PART_MediaElement.Pause();
            _state = PlayState.Pause;
        }

        private void Stop()
        {
            PART_MediaElement.Stop();
            PART_MediaElement.Source = null;
            _state = PlayState.Stop;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (_state == PlayState.Play)
                {
                    Pause();
                }
                else
                {
                    Play();
                }
            }

            if (e.Key == Key.Escape)
            {
                Stop();
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
