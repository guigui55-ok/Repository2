using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ExcelCellsManagerMain.AppsSettingsFormManager.ShowForm(true);
        }
    }
}
