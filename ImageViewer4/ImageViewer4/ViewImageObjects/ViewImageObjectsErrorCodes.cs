using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewImageObjects
{
    public enum ViewImageObjectsErrorCodes : int
    {
        UNEXPECTED_ERROR,
        UNEXPECTED_ERROR_M,
        UNDEFINED_ERROR,
        ACCESS_DENIED
    }
    class ViewImageObjectsErrorConstants
    {
        public static readonly string UNEXPECTED_ERROR = "不明なエラーです。(Unexpected Error)";

        static readonly string[] ErrorMessages = new string[]{
            "不明なエラーです。(Unexpected Error)",
            "不明なエラーです。(Unexpected Error[-1])",
            "未定義のエラーです。(Undefined Error)",
            "アクセスが拒否されました。"
        };

        public static string GetErrorMessage(ViewImageObjectsErrorCodes code)
        {
            return GetErrorMessageInExcelManager(code);
        }
        public static string GetErrorMessageInExcelManager(ViewImageObjectsErrorCodes code)
        {
            Console.WriteLine("ExcelManagerConst.GetErrorMessageInExcelManager");
            Console.WriteLine("code=" + (int)code + " / lenth=" + ViewImageObjectsErrorConstants.ErrorMessages.Length);
            for (int i = 0; i < ViewImageObjectsErrorConstants.ErrorMessages.Length; i++)
            {
                if ((int)code == i)
                {
                    return ViewImageObjectsErrorConstants.ErrorMessages[i];
                }
            }
            return "ViewImageObjectsErrorConstants.UNEXPECTED ERROR";
        }
    }
}
