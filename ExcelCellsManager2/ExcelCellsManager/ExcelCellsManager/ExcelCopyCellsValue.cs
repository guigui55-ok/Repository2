using ExcelIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelCellsManager.ExcelCellsManager
{
    public class ExcelCopyCellsValue
    {
        protected ErrorManager.ErrorManager _err;
        protected ExcelCellsInfo2 info;
        protected ExcelApps apps;
        public ExcelCopyCellsValue(ErrorManager.ErrorManager err,ExcelCellsInfo2 excelCellsInfo2,ExcelApps excelApps)
        {
            _err = err;
            info = excelCellsInfo2;
            apps = excelApps;
        }
        public void Excute()
        {
            try
            {
                ExcelCellsController controller = new ExcelCellsController(_err);
                controller.Copy(apps.Application, info.BookName, info.SheetName, info.Address);
            } catch (System.Threading.ThreadStateException ex)
            {
                _err.AddLogWarning(this.ToString(), "Excute", ex);
            } catch (Exception ex)
            {
                _err.AddException(ex,this,"Excute");
            }
        }
    }
}
