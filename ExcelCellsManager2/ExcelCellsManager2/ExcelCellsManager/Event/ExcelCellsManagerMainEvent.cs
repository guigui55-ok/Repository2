using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelCellsManager
{
    public class ExcelCellsManagerMainEvent
    {
        protected ErrorManager.ErrorManager _err;
        public ExcelCellsManagerMain ExcelCellsManagerMain;
        public ExcelCellsManagerMainEvent(ErrorManager.ErrorManager err
            , ExcelCellsManagerMain excelCellsManagerMain)
        {
            _err = err;
            this.ExcelCellsManagerMain = excelCellsManagerMain;
            Form form = excelCellsManagerMain.GetMainForm();
            if(form != null) { form.Activated += MainForm_Activated; }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            try
            {
                if (ExcelCellsManagerMain.AppsSettingsFormManager.SettingsForm.Visible)
                {
                    ExcelCellsManagerMain.AppsSettingsFormManager.SettingsForm.Activate();
                }
                //if (Form.ActiveForm != null)
                //{
                //}
            } catch (Exception ex)
            {
                _err.AddLogAlert(this, "MainForm_Activated Failed", "MainForm_Activated Failed",ex);
            }
        }

        public void OpenFileDialogEvent(object sender,EventArgs e)
        {
            try
            {
                _err.AddLog(this, "OpenFileDialogEvent");
                ExcelCellsManagerMain.OpenedFile.SetPathFromDilog();
                ExcelCellsManagerMain.OpenFile(ExcelCellsManagerMain.OpenedFile.GetPath());
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "OpenFileDialogEvent");
            }
        }
        public void SaveDataOfGridViewDataEvent(object sender, EventArgs e)
        {
            _err.AddLog(this, "SaveDataOfGridViewDataEvent");
            ExcelCellsManagerMain.SaveDataGridViewData(false);
        }
        public void SaveAsDataOfGridViewDataEvent(object sender, EventArgs e)
        {
            _err.AddLog(this, "SaveAsDataOfGridViewDataEvent");
            ExcelCellsManagerMain.SaveDataGridViewData(true);
        }
        public void ExitApplicationEvent(object sender, EventArgs e)
        {
            _err.AddLog(this, "ExitApplicationEvent");
            ExcelCellsManagerMain.Close();
        }
        public void ShowSettingsWindowEvent(object sender, EventArgs e)
        {
            _err.AddLog(this, "ShowSettingsWindowEvent");
            ExcelCellsManagerMain.ShowSettingsForm();
        }
    }
}
