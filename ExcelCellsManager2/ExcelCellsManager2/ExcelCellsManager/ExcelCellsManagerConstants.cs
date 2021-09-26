

namespace ExcelCellsManager.ExcelCellsManager
{
    public class ExcelCellsManagerConstants
    {
        protected ErrorManager.ErrorManager _error;

        public string ApplicationTitle;
        public string DefalutNewFileName;
        public string DefaultFileType;
        public string IniFileDirectory;
        public string IniFileName;
        public string DialogFilterType;
        public string ErrorLogFileName;
        public string DebugLogFileName;
        public string SaveAsDefaultFileName;
        public string StatusBarTextInitialize;

        public ExcelCellsManagerErrorMessages ErrorMessage;
        public ExcelCellsManagerConstants(ErrorManager.ErrorManager error)
        {
            _error = error;
            this.ErrorMessage = new ExcelCellsManagerErrorMessages(this.ToString());
            ApplicationTitle = "Excel Cells Manager2";
            DefalutNewFileName = "*";
            DefaultFileType = ".tsv";
            DialogFilterType = "TSVファイル(*.tsv)|*.tsv;|すべてのファイル(*.*)|*.*";
            IniFileName = "settings.ini";
            ErrorLogFileName = "Error.Log";
            DebugLogFileName = "Debug.Log";
            SaveAsDefaultFileName = "NewFile";
            StatusBarTextInitialize = "Application Started.";
        }
        public string GetErrorMessage(ExcelCellsManagerErrorCodes code)
        {
            return ErrorMessage.GetMessage(code);
        }
    }
}
