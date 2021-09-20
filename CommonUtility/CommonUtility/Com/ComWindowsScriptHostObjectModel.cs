using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtility.ComUtility
{
    public class ComWindowsScriptHostObjectModel
    {
        protected ErrorManager.ErrorManager _err;
        protected dynamic _comInstance = null;   // COM インスタンス
        public ComWindowsScriptHostObjectModel(ErrorManager.ErrorManager err)
        {
            _err = err;
        }

        public dynamic ComInstance { get { return _comInstance; } }

        public void CreateInstance()
        {
            try
            {
                if (null == _comInstance)
                {
                    ///////////////////////////////////
                    // COM のインスタンスを作成する
                    const string progID = "WScript.Shell"; // ProgID を指定する

                    Type comType = Type.GetTypeFromProgID(progID);
                    _comInstance = Activator.CreateInstance(comType);
                    _err.AddLog("Instance created.");
                }
            } catch (Exception ex)
            {
                _err.AddException(ex,this, "CreateInstance");
            }
        }

        public void Dispose()
        {
            // （一般的な）COMオブジェクトの開放
            Marshal.ReleaseComObject(_comInstance);
            _comInstance = null;
        }
    }
}
