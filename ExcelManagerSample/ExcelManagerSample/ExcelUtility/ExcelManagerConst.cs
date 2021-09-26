using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUtility
{
    public enum ExcelManagerErrorCodes : int
    {
        TEST_ERROR,
        RPC_E_CALL_REJECTED
    }
    public static class ExcelManagerConst
    {
        public static readonly string TEST_ERROR_MSG = "テストエラー";
        public static readonly string RPC_E_CALL_REJECTED_MSG =
            "アクセスが拒否されました。Excelの設定ダイアログなどが開いている可能性があります。";

        static string[] ErrorMessages = new string[]{
            "テストエラー",
            "アクセスが拒否されました。Excelの設定ダイアログなどが開いている可能性があります。"
        };

        static public string GetErrorMessage(ExcelManagerErrorCodes code)
        {
            return GetErrorMessageInExcelManager(code);
        }
        static public string GetErrorMessageInExcelManager(ExcelManagerErrorCodes code)
        {
            Console.WriteLine("ExcelManagerConst.GetErrorMessageInExcelManager");
            Console.WriteLine("code="+(int)code + " / lenth="+ ExcelManagerConst.ErrorMessages.Length);
            for(int i=0; i<ExcelManagerConst.ErrorMessages.Length; i++)
            {
                if ((int)code == i)
                {
                    return ExcelManagerConst.ErrorMessages[i];
                }
            }
            return "ExcelManagerConst.UNEXPECTED ERROR";
        }

    }
}
