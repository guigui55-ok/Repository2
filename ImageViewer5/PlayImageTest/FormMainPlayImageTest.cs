using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlayImageModule;
using AppLoggerModule;
using System.IO;


namespace PlayImageTest
{
    public partial class FormMainPlayImageTest : Form
    {
        private PlayGif _playGif;
        private AppLogger _logger;
        private List<string> _pathList;
        private int _currentIndex;
        public FormMainPlayImageTest()
        {
            InitializeComponent();
            _logger = new AppLogger();
            pictureBox1.Dock = DockStyle.Fill;
            _playGif = new PlayGif(_logger, pictureBox1);

            // 指定ディレクトリからGIFのパスリストを取得;
            // GIFファイルが保存されているディレクトリのパスを指定してください
            string defaultDirPath = @"J:\ZMyFolder_2\jpgbest\gif_png_bmp\gif";
            _pathList = GetGifPathList(defaultDirPath);

            _currentIndex = 0;
        }

        private void FormMainPlayImageTest_Load(object sender, EventArgs e)
        {
            string path = @"J:\ZMyFolder_2\jpgbest\gif_png_bmp\gif\1-2.gif";
            _playGif.DisplayGif(path, 50);
        }
        // 現在のインデックスに基づいてGIFをロードする
        private void LoadGifAtCurrentIndex()
        {
            if (_pathList.Count > 0 && _currentIndex >= 0 && _currentIndex < _pathList.Count)
            {
                _playGif.DisplayGif(_pathList[_currentIndex], 100);
            }
        }

        // キーイベントの処理
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Right)
            {
                // インデックスを次に進めるが、最大を超えないようにする
                if (_currentIndex < _pathList.Count - 1)
                {
                    _currentIndex++;
                    LoadGifAtCurrentIndex();
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                // インデックスを前に戻すが、最小を超えないようにする
                if (_currentIndex > 0)
                {
                    _currentIndex--;
                    LoadGifAtCurrentIndex();
                }
            }
        }

        private void FormMainPlayImageTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            _logger.PrintInfo("Form1_FormClosing");
            _playGif.StopGif(); // フォームを閉じる際にGIF再生を停止
        }

        // 指定されたディレクトリからGIFファイルのパスリストを取得するメソッド
        private List<string> GetGifPathList(string defaultDirPath)
        {
            List<string> gifPaths = new List<string>();

            if (Directory.Exists(defaultDirPath))
            {
                // ディレクトリ内のGIFファイルのみを取得
                gifPaths = Directory.GetFiles(defaultDirPath, "*.gif", SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                _logger.PrintError($"指定されたディレクトリが存在しません: {defaultDirPath}");
            }

            return gifPaths;
        }
    }
}
