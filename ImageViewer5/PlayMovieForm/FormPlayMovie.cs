using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AppLoggerModule;
using CommonModules;
using System.IO;

namespace PlayMovieForm
{
    public partial class FormMainPlayMoview : Form
    {
        private AppLogger _logger;
        private VideoPlayer _videoPlayer;
        private List<string> _movieList;
        private int _currentMovieIndex = 0;
        private int _volume = 30; // デフォルト音量

        public FormMainPlayMoview()
        {
            InitializeComponent();
            _logger = new AppLogger();
            _logger.LogOutPutMode = OutputMode.DEBUG_WINDOW | OutputMode.FILE;
            _logger.FilePath = Directory.GetCurrentDirectory() + @"\__test_log.log";
            _logger.PrintInfo("####################");
            _logger.PrintInfo("Logger.FilePath = " + _logger.FilePath);
            Console.WriteLine(_logger.FilePath);
            _movieList = GetFileList();
        }


        private void FormMainPlayMoview_Load(object sender, EventArgs e)
        {
            _videoPlayer = new VideoPlayer(panel1, _logger);
            if (_movieList.Count > 0)
            {
                _videoPlayer.SetMovie(_movieList[_currentMovieIndex]); // 最初の動画を設定
                _videoPlayer.SetVolume(_volume); // デフォルト音量を設定
                _logger.PrintInfo($"Default volume set to {_volume}");

                _logger.PrintInfo(String.Format("Panel.Size = {0}", panel1.Size));
                _logger.PrintInfo(String.Format("Panel.Location = {0}", panel1.Location));
                panel1.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            _videoPlayer.Play(); // 動画の再生
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _videoPlayer.Stop(); // 動画の停止
        }

        public List<string> GetFileList()
        {
            SimpleFileListManager manager = new SimpleFileListManager();

            // 対象フォルダパス
            string folderPath = @"J:\ZMyFolder_2\00movie";

            // 正規表現条件のリスト
            List<string> conditionsRegix = new List<string>
            {
                @"\.avi$",
                @"\.mp4$",
                @"\.mkv$"
            };

            // フィルタされたファイルリストを取得
            List<string> filteredFiles = manager.GetFilteredFileList(folderPath, conditionsRegix);

            // 結果を表示
            foreach (var file in filteredFiles)
            {
                Console.WriteLine(file);
            }

            return filteredFiles;
        }
        // キーイベント処理
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right: // 次の動画
                    PlayNextMovie();
                    return true;
                case Keys.Left: // 前の動画
                    PlayPreviousMovie();
                    return true;
                case Keys.Up: // 音量を上げる
                    IncreaseVolume();
                    return true;
                case Keys.Down: // 音量を下げる
                    DecreaseVolume();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void PlayNextMovie()
        {
            if (_movieList.Count > 0)
            {
                _currentMovieIndex = (_currentMovieIndex + 1) % _movieList.Count;
                _videoPlayer.SetMovie(_movieList[_currentMovieIndex]);
                _logger.PrintInfo($"Playing next movie: {_movieList[_currentMovieIndex]}");
                _videoPlayer.Play();
            }
        }

        private void PlayPreviousMovie()
        {
            if (_movieList.Count > 0)
            {
                _currentMovieIndex = (_currentMovieIndex - 1 + _movieList.Count) % _movieList.Count;
                _videoPlayer.SetMovie(_movieList[_currentMovieIndex]);
                _logger.PrintInfo($"Playing previous movie: {_movieList[_currentMovieIndex]}");
                _videoPlayer.Play();
            }
        }

        private void IncreaseVolume()
        {
            if (_volume < 100) // 音量の最大値は100とする
            {
                _volume++;
                _videoPlayer.SetVolume(_volume);
                _logger.PrintInfo($"Increased volume to {_volume}");
            }
        }

        private void DecreaseVolume()
        {
            if (_volume > 0) // 音量の最小値は0とする
            {
                _volume--;
                _videoPlayer.SetVolume(_volume);
                _logger.PrintInfo($"Decreased volume to {_volume}");
            }
        }
    }
}
