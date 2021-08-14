using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;

namespace ExcelUtility.ExcelObject
{
    public class ExcelWorkbook 
    {
        protected ErrorManager.ErrorManager _Error;
        public Microsoft.Office.Interop.Excel.Workbook MainObject = null;
        public ExcelWorkbook(ErrorManager.ErrorManager error)
        {
            _Error = error;
        }

        public List<string> GetSheetList()
        {
            List<string> sheetList = new List<string>();
            try
            {
                if(MainObject == null) { throw new Exception("MainObject Is Null"); }

                Worksheet Sheet;
                for (int i =1; i<=MainObject.Sheets.Count; i++)
                {
                    Sheet = MainObject.Worksheets[i];
                    sheetList.Add(Sheet.Name);
                }
                return sheetList;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".GetSheetList");
                return sheetList;
            } finally
            {

            }
        }

        public void Close()
        {
            try
            {
                if (MainObject != null)
                {
                    MainObject.Close();
                    MainObject = null;
                }
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".Close");
                return;
            }
        }
    }
}
