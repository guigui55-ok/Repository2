using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
//using AppLoggerModule;

namespace PlayWebp
{
    /*
     * C# WinFormsでWebP画像を表示するためにImageProcessorライブラリを使用しようとしている場合、ImageProcessorはWebP形式をネイティブにサポートしていないため、問題が発生することがあります。代わりに、WebPをサポートしているSkiaSharpやImageMagick、またはWebP.NETといったライブラリを使用する方法があります。
     * 
     */
    public partial class FormPlayWebp : Form
    {
        List<string> _pathList;
        private int _currentIndex;
        //private AppLogger _logger;
        PlayAnimatedWebp _playWebp;
        public FormPlayWebp()
        {
            InitializeComponent();
            string path = @"J:\ZMyFolder_2\jpgbest\webp";
            _pathList = GetPlayPathList(path);
            _currentIndex = 0;
            pictureBox1.Dock = DockStyle.Fill;
            _playWebp = new PlayAnimatedWebp(pictureBox1);
        }

        private void FormPlayWebp_Load(object sender, EventArgs e)
        {
            string testJpgPath = @"J:\ZMyFolder_2\jpgbest\test.jpeg";
            pictureBox1.Image = Image.FromFile(testJpgPath);
            LoadImageAtCurrentIndex();
        }

        // 現在のインデックスに基づいて画像をロードする
        private void LoadImageAtCurrentIndex()
        {
            if (_pathList.Count > 0 && _currentIndex >= 0 && _currentIndex < _pathList.Count)
            {
                Debugger.DebugPrint(string.Format("PlayPath = {0}", _pathList[_currentIndex]));
                _playWebp.SetImageFromFile(_pathList[_currentIndex]);
            }
            else
            {
                Debugger.DebugPrint("_pathList.Count <= 0");
            }
        }


        // キーイベントの処理
        private void FormPlayWebp_OnKeyDown(object sender, KeyEventArgs e)        
        {
            if (e.KeyCode == Keys.Right)
            {
                // インデックスを次に進めるが、最大を超えないようにする
                if (_currentIndex < _pathList.Count - 1)
                {
                    _currentIndex++;
                    LoadImageAtCurrentIndex();
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                // インデックスを前に戻すが、最小を超えないようにする
                if (_currentIndex > 0)
                {
                    _currentIndex--;
                    LoadImageAtCurrentIndex();
                }
            }
        }

        // 指定されたディレクトリから再生用ファイルのパスリストを取得するメソッド
        private List<string> GetPlayPathList(string defaultDirPath)
        {
            List<string> retPaths = new List<string>();

            if (Directory.Exists(defaultDirPath))
            {
                // ディレクトリ内の目的の拡張子ファイルのみを取得
                retPaths = Directory.GetFiles(defaultDirPath, "*.webp", SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                Debugger.DebugPrint($"指定されたディレクトリが存在しません: {defaultDirPath}");
            }

            return retPaths;
        }

    }

    public static class Debugger
    {
        public static void DebugPrint(string value)
        {
            Debug.WriteLine(value);
        }
    }
}
