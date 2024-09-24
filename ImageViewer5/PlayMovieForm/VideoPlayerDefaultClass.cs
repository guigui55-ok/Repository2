using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace PlayMovieForm
{
    // インターフェース的なもの

    public class VideoPlayerDefault : IDisposable
    {
        private Timer _timer;
        private Control _videoControl;
        private bool _isPlaying;
        private string _moviePath = "";

        private AppLogger _logger;
        public VideoPlayerDefault(Control videoControl, AppLogger logger)
        {
            _videoControl = videoControl;
            _logger = logger;
            //MediaManager.Startup();
        }
        public void SetMovie(string moviePath)
        {
            this._moviePath = moviePath;

        }


        public void Play()
        {
            if (_isPlaying)
                return;

            _isPlaying = true;

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
        }

        private void DisposeSourceReader()
        {
        }

        public void Dispose()
        {
            Stop();
            DisposeSourceReader();
        }
    }
}
