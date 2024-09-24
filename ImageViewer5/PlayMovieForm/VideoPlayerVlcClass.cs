using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using LibVLCSharp.WinForms;
using LibVLCSharp.Shared;


namespace PlayMovieForm
{
    // NuGet パッケージ LibVLCSharp.WinForms と VideoLAN.LibVLC.Windows をインストールします。

    public class VideoPlayerVlc : IDisposable
    {
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;

        private Timer _timer;
        private VideoView _videoView;
        private bool _isPlaying;
        private string _moviePath = "";

        private AppLogger _logger;
        public VideoPlayerVlc(VideoView videoView, AppLogger logger)
        {
            _videoView = videoView;
            _logger = logger;
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
        }
        public void SetMovie(string moviePath)
        {
            this._moviePath = moviePath;

        }


        public void Play(string path="")
        {
            if (path == "") { path = _moviePath; }
            if (_isPlaying)
                return;

            _isPlaying = true;
            _logger.PrintInfo("Play");
            _logger.PrintInfo(string.Format("path = {0}", path));
            _mediaPlayer.Play(new Media(_libVLC, path, FromType.FromPath));

            //int interval = (int)(1000 / _frameRate);

            //_timer = new Timer();
            //_timer.Interval = interval > 0 ? interval : 33; // intervalが0以下の場合は33msに設定
            //_timer.Tick += Timer_Tick;
            //_timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isPlaying)
                return;

            try
            {
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, $"Error in Timer_Tick: {ex.Message}");
                Stop();
            }
        }

        public void Stop()
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            _mediaPlayer.Stop();
        }

        private void DisposeSourceReader()
        {
            //if (_sourceReader != null)
            //{
            //    _sourceReader.Dispose();
            //    _sourceReader = null;
            //}
        }

        public void Dispose()
        {
            Stop();
            DisposeSourceReader();
            //MediaManager.Shutdown();
        }
    }
}
