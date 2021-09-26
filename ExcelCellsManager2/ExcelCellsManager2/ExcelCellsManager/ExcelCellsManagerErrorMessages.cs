namespace ExcelCellsManager.ExcelCellsManager
{
    //public class ExcelCellsMangerErrorCodes
    //{
    //    public readonly string ClassName = "ExcelCellsManagerErrorMessages";
    //    private int[] codes;
    //    public ExcelCellsMangerErrorCodes(string className)
    //    {
    //        this.ClassName = className;
    //    }
    //    public readonly int OBJECT_IS_NULL = -1;
    //    public readonly int WORKBOOKLIST_IS_NOT_CHECKED = -2;
    //    public readonly int APPLICATION_INITIALIZE = -3;
    //    public readonly int FILE_OPEN_ERROR = -4;
    //    public readonly int WORKBOOK_GET_LIST_ERROR = -5;
    //    public readonly int WORKBOOK_GET_LIST_ERROR2 = -6;
    //    public readonly int WORKBOOK_GET_CHECKED_ERROR = -7;
    //}

    public enum ExcelCellsManagerErrorCodes : int
    {
        TEST_ERROR,
        OBJECT_IS_NULL,
        WORKBOOKLIST_IS_NOT_CHECKED,
        APPLICATION_INITIALIZE,
        FILE_OPEN_ERROR,
        WORKBOOK_GET_LIST_ERROR,
        WORKBOOK_GET_LIST_ERROR2,
        WORKBOOK_GET_CHECKED_ERROR,
        EXCEL_DIALOG_OPEN,
        RPC_E_CALL_REJECTED
    }
    public class ExcelCellsManagerErrorMessages
    {
        class ErrorInfo
        {
            public ErrorInfo(ExcelCellsManagerErrorCodes code,string msg)
            {
                Message = msg; ErrorCode = (int)code;
            }
            public string Message;
            public int ErrorCode;
        }
        // ===================================================================

        public ExcelCellsManagerErrorCodes ErrorCode = new ExcelCellsManagerErrorCodes();
        public readonly string ClassName = "ExcelCellsManagerErrorMessages";
        public ExcelCellsManagerErrorMessages(string className)
        {
            this.ClassName = className;
            Initialize();
        }
        private ErrorInfo[] errorInfoList;
        
        public int GetCode(ExcelCellsManagerErrorCodes code)
        {
            return (int)code;
        }

        public string GetMessage(ExcelCellsManagerErrorCodes code)
        {
            foreach(ErrorInfo info in errorInfoList)
            {
                if ((int)code==(info.ErrorCode))
                {
                    return info.Message;
                }
            }
            return "";
        }

        private void Initialize()
        {
            errorInfoList = new ErrorInfo[] {
                new ErrorInfo( ExcelCellsManagerErrorCodes.TEST_ERROR, "テストエラー" ),
            new ErrorInfo( ExcelCellsManagerErrorCodes.WORKBOOKLIST_IS_NOT_CHECKED, "ワークブックがチェックされていません。1つ以上チェックしてください。" ),
            new ErrorInfo( ExcelCellsManagerErrorCodes.APPLICATION_INITIALIZE, "アプリケーション初期化エラー" ),
            new ErrorInfo( ExcelCellsManagerErrorCodes.FILE_OPEN_ERROR, "ファイルオープンエラー" ),
            new ErrorInfo( ExcelCellsManagerErrorCodes.WORKBOOK_GET_LIST_ERROR, "WorkbookList 取得エラー1" ),
            new ErrorInfo( ExcelCellsManagerErrorCodes.WORKBOOK_GET_LIST_ERROR, "WorkbookList 取得エラー2" ),
            new ErrorInfo( ExcelCellsManagerErrorCodes.WORKBOOK_GET_CHECKED_ERROR, "WorkbookList CheckList 取得エラー" ),
            new ErrorInfo( 
                ExcelCellsManagerErrorCodes.EXCEL_DIALOG_OPEN,
                "アクセスが拒否されました。Excelの設定ダイアログなどが開いている可能性があります。"),
            new ErrorInfo(
                ExcelCellsManagerErrorCodes.RPC_E_CALL_REJECTED,
                "アクセスが拒否されました。Excelの設定ダイアログなどが開いている可能性があります。")
            }; 
        }
        //public readonly int OBJECT_IS_NULL = 1;
        //public readonly string WORKBOOKLIST_IS_NOT_CHECKED = "ワークブックがチェックされていません。1つ以上チェックしてください。";
        //public readonly string APPLICATION_INITIALIZE = "アプリケーション初期化エラー";
        //public readonly string FILE_OPEN_ERROR = "ファイルオープンエラー";
        //public readonly string WORKBOOK_GET_LIST_ERROR = "WorkbookList 取得エラー1";
        //public readonly string WORKBOOK_GET_LIST_ERROR2 = "WorkbookList 取得エラー2";
        //public readonly string WORKBOOK_GET_CHECKED_ERROR = "WorkbookList CheckList 取得エラー";

    }
}
