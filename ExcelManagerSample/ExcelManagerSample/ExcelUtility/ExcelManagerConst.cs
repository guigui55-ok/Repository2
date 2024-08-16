using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUtility
{
    public enum ExcelManagerConstants :int
    {
        RESULT_NONE,
        RESULT_SUCCESS
    }
    public enum ExcelManagerErrorCodes : int
    {
        UNEXPECTED_ERROR,
        UNEXPECTED_ERROR_M,
        UNDEFINED_ERROR,
        RPC_E_CALL_REJECTED,
        INTEROP_SERVICES_COM_EXCEPTION,
        WORKBOOK_IS_NOTHING,
        ACCESS_DENIED,
        ACCESS_DENIED_0X800AC472
    }
    public static class ExcelManagerConst
    {
        public static readonly string UNEXPECTED_ERROR = "不明なエラーです。(Unexpected Error)";
        public static readonly string RPC_E_CALL_REJECTED_MSG =
            "アクセスが拒否されました。Excelの設定ダイアログなどが開いている可能性があります。";
        public static readonly string ACCESS_DENIED_0X800AC472 = "HRESULT からの例外:0x800AC472";

        static readonly string[] ErrorMessages = new string[]{
            "不明なエラーです。(Unexpected Error)",
            "不明なエラーです。(Unexpected Error[-1])",
            "未定義のエラーです。(Undefined Error)",
            "アクセスが拒否されました。Excelの設定ダイアログなどが開いている可能性があります。",
            "不明なエラーです。(InteropServieces.COM Exception)",
            "対象のワークブックが見つかりません。",
            "アクセスが拒否されました。",
            "アクセスが拒否されました。(0x800AC472)"
        };

        public static string GetErrorMessage(ExcelManagerErrorCodes code)
        {
            return GetErrorMessageInExcelManager(code);
        }
        public static string GetErrorMessageInExcelManager(ExcelManagerErrorCodes code)
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
