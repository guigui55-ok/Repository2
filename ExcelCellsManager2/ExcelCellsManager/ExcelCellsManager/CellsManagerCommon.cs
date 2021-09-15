using System;

namespace ExcelCellsManager.ExcelCellsManager
{
    public static class CellsManagerCommon
    {
        private static readonly ErrorManager.ErrorManager _error = null;
        public static void ConsoleAddLog(string value,string notes)
        {
            Console.WriteLine("** " + value);
            Console.WriteLine(notes);
            _error.AddLog(value,notes); 
        }
        public static void ConsoleAddLog(string value)
        {
            Console.WriteLine("** " + value);
            _error.AddLog(value); 
        }
        public static void ConsoleAddLog(int priority,int logType,string value,string notes,Exception ex = null)
        {
            _error.AddLog(priority,logType,value,notes,ex);
        }
        public static void ConsoleAddEx(Exception ex,string msg)
        {
            Console.WriteLine("*** " + msg);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            _error.AddException(ex, msg);
        }
    }
}
