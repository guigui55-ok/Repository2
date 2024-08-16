using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.FileListUtility
{
    public enum FileListManagerConstants : int
    {
        RESULT_NONE,
        RESULT_SUCCESS
    }
    public enum FileListManagerErrorCodes : int
    {
        UNEXPECTED_ERROR,
        UNEXPECTED_ERROR_M,
        UNDEFINED_ERROR,
    }

    static class FileListManagerErrorMessages
    {
        public static readonly string UNEXPECTED_ERROR = "不明なエラーです。(Unexpected Error)";
        static public readonly string[] ErrorMessages = new string[]{
            "不明なエラーです。(Unexpected Error)",
            "不明なエラーです。(Unexpected Error[-1])",
            "未定義のエラーです。(Undefined Error)",
        };
    }

    public class FileListManagerConstConvert
    {
        public static string GetErrorMessage(FileListManagerErrorCodes code)
        {
            return GetErrorMessageInExcelManager(code);
        }
        public static string GetErrorMessageInExcelManager(FileListManagerErrorCodes code)
        {
            for (int i = 0; i < FileListManagerErrorMessages.ErrorMessages.Length; i++)
            {
                if ((int)code == i)
                {
                    return FileListManagerErrorMessages.ErrorMessages[i];
                }
            }
            return "FileListManagerErrorCodes.UNEXPECTED ERROR";
        }
    }

}
