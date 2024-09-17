using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PlayWebp
{
    public partial class FormPlayWebp : Form
    {
        public FormPlayWebp()
        {
            InitializeComponent();
        }

        private void FormPlayWebp_Load(object sender, EventArgs e)
        {
            _pathList = GetPlayPathList(@"J:\ZMyFolder_2\jpgbest\webp");
        }


        // 指定されたディレクトリからGIFファイルのパスリストを取得するメソッド
        private List<string> GetPlayPathList(string defaultDirPath)
        {
            List<string> gifPaths = new List<string>();

            if (Directory.Exists(defaultDirPath))
            {
                // ディレクトリ内のGIFファイルのみを取得
                gifPaths = Directory.GetFiles(defaultDirPath, "*.webp", SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                Debugger.DebugPrint($"指定されたディレクトリが存在しません: {defaultDirPath}");
            }

            return gifPaths;
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
