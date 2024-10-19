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
using CommonUtility.FileListUtility;

namespace FileListManagerSample
{
    public partial class FileListManagerSettingForm : Form
    {
        AppLogger _logger;
        public FileListManagerSetting _fileListManagerSetting;
        // 親フォームで押されたか判定する用
        // 設定欄が開き、変更されたら、Applyボタンを有効にする
        public bool _isPshedShowButton;
        // ウィンドを開いた後、1度以上設定が変更されたか
        public bool _isChangedSettingValue;
        public FileListManagerSettingForm(AppLogger logger, FileListManagerSetting setting)
        {
            InitializeComponent();

            _logger = logger;
            _fileListManagerSetting = setting;
            SetEventChangeValue();
        }

        public void DisposeObjects()
        {
            try
            {
                checkBox_FixedDirectory.CheckedChanged -= SettingValueChanged;
                checkBox_FixedFileList.CheckedChanged -= SettingValueChanged;
                checkBox_ReadSubDirFiles.CheckedChanged -= SettingValueChanged;
                checkBox_EnableRandomList.CheckedChanged -= SettingValueChanged;
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void SetEventChangeValue()
        {
            checkBox_FixedDirectory.CheckedChanged += SettingValueChanged;
            checkBox_FixedFileList.CheckedChanged += SettingValueChanged;
            checkBox_ReadSubDirFiles.CheckedChanged += SettingValueChanged;
            checkBox_EnableRandomList.CheckedChanged += SettingValueChanged;
        }

        private void SettingValueChanged(object sender, EventArgs e)
        {
            if (!_isChangedSettingValue)
            {
                buttonApply.Enabled = true;
            }
        }

        private void FileListManagerSettingForm_Load(object sender, EventArgs e)
        {
        }

        private void FileManagerSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("SettingForm > buttonClose_Click");
            this.Visible = false;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            _logger.PrintInfo("SettingForm > buttonApply_Click");
            bool isChanged;
            bool isChangedB;
            isChanged = ApplyValueCheckBox(ref checkBox_FixedDirectory, ref _fileListManagerSetting._isFixedDirectory);
            isChangedB = ApplyValueCheckBox(ref checkBox_FixedFileList, ref _fileListManagerSetting._isFixedFileList);
            //ファイル固定と、ディレクトリ固定は検討中
            //とりあえずどちらかが更新されていたら、リストを固定にしてディレクトリを通常移動不可にする
            if (isChanged || isChangedB)
            {
                _fileListManagerSetting.ChangeFixedDirectoryFlag(checkBox_FixedDirectory.Checked);
                _fileListManagerSetting.ChangeFixedDirectoryFlag(checkBox_FixedFileList.Checked);
            }
            //ApplyValueCheckBox(ref checkBox_ReadSubDirFiles, ref _fileListManagerSetting._isLoadEverythingUnderSubfoldersFiles);
            _fileListManagerSetting.ChangeIncludeSubFolder(checkBox_ReadSubDirFiles.Checked);
            //ApplyValueCheckBox(ref checkBox_EnableRandomList, ref _fileListManagerSetting._isListRandom);
            _fileListManagerSetting.ChangeListRandom(checkBox_EnableRandomList.Checked);

            buttonApply.Enabled = false;
            _isChangedSettingValue = false;
        }

        private void FileListManagerSetting_Activated(object sender, EventArgs e)
        {
            _logger.PrintInfo("SettingForm > FileListManagerSetting_Activated");
            if (_isPshedShowButton)
            {
                //ウィンドウを開いた直後は適用ボタンと値変更フラグを無効に
                buttonApply.Enabled = false;
                _isChangedSettingValue = false;
                _isPshedShowButton = false;
            }
        }


        private bool ApplyValueCheckBox(ref CheckBox checkbox, ref bool flag)
        {
            if (flag != checkbox.Checked)
            {
                flag = checkbox.Checked;
                // 以前の値から変更されていたらTrue、変更されていなければFalseを返す
                return true;
            }
            return false;
        }
    }
}
