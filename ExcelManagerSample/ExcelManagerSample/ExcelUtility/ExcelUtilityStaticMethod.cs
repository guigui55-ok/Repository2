using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelUtility;

namespace ExcelUtility
{
    static class ExcelUtilityStaticMethod
    { 

        public static int AddExceptionForCOMException(ErrorManager.ErrorManager err, Exception ex,object classObject, string methodName)
        {
            try
            {
                string className;
                if (classObject != null)
                {
                    className = classObject.GetType().ToString();
                }
                else
                {
                    className = "_";
                }
                err.AddLog("ex.GetType().ToString() = " + ex.GetType().ToString());
                if (ex.GetType().Equals(typeof(System.Runtime.InteropServices.COMException)))
                {
                    if (ex.Message.IndexOf(ExcelManagerErrorCodes.RPC_E_CALL_REJECTED.ToString()) >= 0)
                    {
                        string msg = ExcelManagerConst.GetErrorMessage(ExcelManagerErrorCodes.RPC_E_CALL_REJECTED);
                        err.AddException(ex, className.GetType().ToString(), methodName + " Failed(System.Runtime.InteropServices.COMException)", msg);
                        return -(int)ExcelManagerErrorCodes.RPC_E_CALL_REJECTED;
                    }
                    else if (ex.Message.IndexOf(ExcelManagerConst.ACCESS_DENIED_0X800AC472) >= 0)
                    {
                        string msg = ExcelManagerConst.GetErrorMessage(ExcelManagerErrorCodes.ACCESS_DENIED_0X800AC472);
                        err.AddException(ex, className, methodName + " Failed(System.Runtime.InteropServices.COMException)", msg);
                        return -(int)ExcelManagerErrorCodes.ACCESS_DENIED_0X800AC472;
                    }
                    else
                    {
                        err.AddException(ex, className, methodName + " COMException");
                        return -(int)ExcelManagerErrorCodes.INTEROP_SERVICES_COM_EXCEPTION;
                    }
                }
                else if (ex.GetType().Equals(typeof(Exception)))
                {
                    err.AddException(ex, className, methodName + " Failed Exception");
                    return -(int)ExcelManagerErrorCodes.UNEXPECTED_ERROR_M;
                }
                else
                {
                    err.AddException(ex, className, methodName + " Failed");
                    return -(int)ExcelManagerErrorCodes.UNDEFINED_ERROR;
                }
                //return -(int)ExcelManagerErrorCodes.UNDEFINED_ERROR;
            } catch (Exception exception)
            {
                err.AddException(exception, "ExcelUtilityStaticMethod" , "AddExceptionForCOMException Failed");
                return -(int)ExcelManagerErrorCodes.UNEXPECTED_ERROR_M;
            }
        }
    }
}
