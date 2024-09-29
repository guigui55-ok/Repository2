using CommonModule;
using FileSenderApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using CommonModulesProject;

namespace FileSenderAppSample
{
    public partial class FormFileSenderAppSample : Form
    {
        AppLogger _logger;
        List<string> _pathList;
        FormFileSenderApp _fileSenderApp;
        FileChangerSimple _filechangeSimple;
        public FormFileSenderAppSample()
        {
            InitializeComponent();
            _filechangeSimple = new FileChangerSimple();
        }

        private void FormFileSenderAppSample_Load(object sender, EventArgs e)
        {
            //左右のキーでファイルを変更（TextBox変更）　クラス化
            //srcにファイルをコピー
            // テスト用フォルダパスセット
            string copyDir = @"J:\ZMyFolder_2\jpgbest";
            string defaultSrcDirPath = @"J:\ZMyFolder_2\jpgbest\zz_src";
            string defaultDistDirPath = @"J:\ZMyFolder_2\jpgbest\zz_dist";
            _filechangeSimple.CopyFilesInDirectory(copyDir , defaultSrcDirPath);
            // ファイル一覧読み込み
            _filechangeSimple.SetFileListByPath(defaultSrcDirPath);
            //
            _pathList = CommonGeneral.GetPathList(defaultSrcDirPath);

            //FileSenderを生成して、保持しておく
            _fileSenderApp = new FormFileSenderApp();
            _fileSenderApp._isSubForm = true;
            _logger = _fileSenderApp._logger;

            // KeyDown紐づけ
            this.KeyPreview = true;

            //ボタンイベント紐づけ（FileSenderAppの）
            //ファイルリストのカレントファイルが変更されたときに、FileSenderAppのBridgeValueを変更する
            //　=値（パス）の入れ替え時は、fileSender.DataBridgeも更新（テキストボックス更新＞DataBridge更新）
            _filechangeSimple.ChangeFileEvent += ChangeFileEventRecieve;
            //このフォームのボタンでダイアログを表示
            //fileSenderのイベント成功を受け取り
            //削除したら、ファイルのリストも更新する
            _fileSenderApp.AnySendButton_Clicked += FileSenderApp_ClickedButton_RecieveEvent;

            _filechangeSimple.MoveIndex(0, true);
        }

        private void FileSenderApp_ClickedButton_RecieveEvent(object sender, EventArgs e)
        {
            //ボタンがクリックされた（ファイルが、削除orコピーされた）
            _logger.PrintInfo("FormFileSenderSample > FileSenderApp_ClickedButton_RecieveEvent");
            // UpdateFilePath
            _filechangeSimple.UpdateFileList();
            string filePath = _filechangeSimple.GetCurrentPath();
            _fileSenderApp._dataBridgeFromExternal.SetData(filePath);
            textBox1.Text = filePath;
        }

        private void ChangeFileEventRecieve(object sender, EventArgs e)
        {
            //カレントファイルが更新された
            _logger.PrintInfo("FormFileSenderSample > ChangeFileEventRecieve");
            string filePath = _filechangeSimple.GetCurrentPath();
            _fileSenderApp._dataBridgeFromExternal.SetData(filePath);
            textBox1.Text = filePath;
        }

        private void FileSenderAppSample_KeyDown(object sender, KeyEventArgs e)
        {
            _filechangeSimple.FileChangeSimple_KeyDown(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // show Dialog
            _fileSenderApp.Visible = true;
        }

        private void FormFileSenderSample_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void FormFileSenderSample_FormClosed(object sender, FormClosedEventArgs e)
        {
            _fileSenderApp.FormFileSenderApp_FormClosed(sender, e);
        }
    }
}
