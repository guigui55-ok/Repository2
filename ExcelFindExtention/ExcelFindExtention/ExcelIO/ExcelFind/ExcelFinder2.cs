using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelIO.ExcelFind
{
    public class ExcelFinder2
    {
        protected ErrorManager.ErrorManager _Error;
        public Workbook Workbook = null;
        protected Range _currentFindRange;
        public List<ExcelFinderResult> ResultList = new List<ExcelFinderResult>();
        protected ExcelFinderAddress finderAddressUtil;

        public ExcelFinder2(ErrorManager.ErrorManager error)
        {
            _Error = error;
            finderAddressUtil = new ExcelFinderAddress(_Error);
        }

        public void FindNext(in Application application, ExcelFindSearchConditions conditions)
        {
            try
            {
                if(Workbook == null) { throw new Exception("Workbook Is Null"); }


            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".FindNext");
                return;
            }
        }

        public void FindNextMain(in Application application,ExcelFindSearchConditions conditions)
        {
            try
            {

            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".FindNextMain");
                return;
            }
        }
    }
}
