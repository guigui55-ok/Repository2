using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUtility
{
    public class ExcelActiveCellsNotifer
    {
        protected ErrorManager.ErrorManager _err;
        protected ExcelManager _excelManager;
        public ExcelActiveCellsNotifer(ErrorManager.ErrorManager err,ExcelManager excelManager)
        {
            _err = err;
            _excelManager = excelManager;
            _excelManager.UpdateExcelAppsListAfterEvent += UpdateExcelAppsListAfterEvent;
        }

        public void UpdateExcelAppsListAfterEvent(object sender,EventArgs e)
        {
            try
            {

            } catch (Exception ex)
            {
                _err.AddLogAlert(this, "UpdateExcelAppsListAfterEvent", "UpdateExcelAppsListAfterEvent Failed",ex);
            }
        }
    }
}
