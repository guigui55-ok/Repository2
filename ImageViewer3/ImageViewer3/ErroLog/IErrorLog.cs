using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorLog
{
    public interface IErrorLog
    {
        //void ErrorLog();
        void ResetError();
        void AddException(Exception exception, string FunctionName);
        void AddException(Exception exception, string ClassName,string FunctionName);
        void ShowErrorMessage(Exception exception);
        //void ShowErrorMessage();
        bool ShowErrorMessage();
        void ShowErrorMessageAndClearError();
        void AddErrorNotException(string FunctionName);
        void AddErrorNotException(string ClassName, string FunctionName);
        bool HasError();
    }
}
