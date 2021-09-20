using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelCellsManager.Settings
{
    public class ExcelCellsManagerState
    {
        public bool IsEdited = false;
        public bool IsWorkbookBeforeClose = false;
        public bool IsNotUpdateExcelAppsListAfterOpenWorkbook = false;
        public bool IsNowUpdateExcelAppsList = false;
        public bool IsInitialize = true;
        public ExcelCellsManagerState()
        {

        }
    }
}
