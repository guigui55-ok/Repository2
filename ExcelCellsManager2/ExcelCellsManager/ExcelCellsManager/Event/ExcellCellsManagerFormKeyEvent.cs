using System;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelCellsManager.Event
{
    public class ExcellCellsManagerFormKeyEvent
    {
        protected ErrorManager.ErrorManager _error;
        protected ExcelCellsManagerForm _form;
        protected ExcelCellsManagerMain _excelCellsManagerMain;
        public ExcellCellsManagerFormKeyEvent(ErrorManager.ErrorManager error,
            ExcelCellsManagerForm form,ExcelCellsManagerMain excelCellsManagerMain)
        {
            _error = error;
            _form = form;
            _excelCellsManagerMain = excelCellsManagerMain;

            _form.KeyDown += new System.Windows.Forms.KeyEventHandler(_form_KeyDown);
        }

        private void _form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                switch (e.KeyData)
                {
                    case Keys.Control | Keys.S :
                        _error.AddLog("_form_KeyDown : Keys.Control | Keys.S");
                        _excelCellsManagerMain.SaveDataGridViewData(false);                   
                        break;
                    case Keys.Control | Keys.Shift | Keys.S:
                        _error.AddLog("_form_KeyDown : Keys.Control | Keys.Shift | Keys.S");
                        _excelCellsManagerMain.SaveDataGridViewData(true);
                        break;
                    case Keys.Control | Keys.O:
                        _error.AddLog("_form_KeyDown : Keys.Control | Keys.O");
                        _excelCellsManagerMain.OpenedFile.SetPathFromDilog();
                        _excelCellsManagerMain.OpenFile(_excelCellsManagerMain.OpenedFile.GetPath());
                        break;
                    case Keys.Control | Keys.N:
                        _error.AddLog("_form_KeyDown : Keys.Control | Keys.N");
                        _excelCellsManagerMain.NewFile();
                        break;
                    case Keys.Control | Keys.Shift | Keys.O:
                        _error.AddLog("_form_KeyDown : Keys.Control | Keys.Shift | Keys.O");
                        _excelCellsManagerMain.AppsSettingsFormManager.ShowForm(true);
                        break;
                    case Keys.Control | Keys.A:
                        _excelCellsManagerMain.Test();
                        break;
                    case Keys.F5:
                        _excelCellsManagerMain.UpdateList();
                        break;
                    default:
                        _error.AddLog("_form_KeyDown : " + e.KeyData.ToString());
                        break;
                }
                //if (e.KeyData == (Keys.Control | Keys.S ))
                //{
                //    .WriteLine("_form_KeyDown : Keys.Control | Keys.S");
                //}
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + "._form_KeyDown");
                _error.ClearError();
            }
        }
    }
}
