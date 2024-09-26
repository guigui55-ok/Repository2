using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace FileSenderApp
{    
    public class DataBridgeFromExternal
    {
        AppLogger _logger;
        public int _mode;
        //public Dictionary<string, object> _value;
        public object _value = null;
        public DataBridgeFromExternal(AppLogger logger)
        {
            _logger = logger;
            _mode = ConstDataBridge.MODE_PATH;
        }

        public void SetData(object value)
        {
            //_value = new Dictionary<string, object>()
            //{
            //    { _mode.ToString(), value }
            //};
            _value = value;
        }

        public T GetData<T>()
        {
            if (_mode == ConstDataBridge.MODE_PATH)
            {
                string ret;
                if(_value == null) { ret = ""; } else { ret = (string)_value;  }
                return (T)(object)ret;
            }
            else
            {
                // other
                return (T)_value;
            }
        }
        /*
         * 
呼び出し側の例：
csharp
コードをコピーする
// _mode == ConstDataBridge.MODE_PATH の場合
string value = GetData<string>();

// 他のモードの場合
object objValue = GetData<object>();
         * 
         */

    }

    public static class ConstDataBridge    {
        public static readonly int MODE_PATH = 0;
        public static readonly int MODE_OTHER = 99;
    }

}
