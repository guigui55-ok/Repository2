using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ErrorLog
{
    public class ErrorLog　: IErrorLog
    {
        List<Exception> _exList;
        List<string> _funcList;
        public int ErrState = 0;

        public void ResetError()
        {
            _exList = null;
            _exList = new List<Exception>();
            _funcList = null;
            _funcList = new List<string>();
            ErrState = 0;
        }
        public ErrorLog()
        {
            _exList = new List<Exception>();
            _funcList = new List<string>();
        }

        // エラーをリストに追加
        public void AddException(Exception exception, string FunctionName)
        {
            Debug.WriteLine(FunctionName);
            Debug.WriteLine(exception.Message);
            ErrState = 1;
            _exList.Add(exception);
            _funcList.Add(FunctionName);
        }

        public void AddException(Exception exception, string ClassName, string FunctionName)
        {
            AddException(exception, ClassName + " : " + FunctionName);
        }

        public void ShowErrorMessage(Exception exception)
        {
            // using System.Windows.Forms;
            MessageBox.Show(exception.Message, exception.StackTrace);

        }

        // エラーメッセージを表示、メッセージボックスで
        //public void ShowErrorMessage()
        //{
        //    string exstr = ExceptionListToString();
        //    if (exstr != "")
        //    {
        //        ErrState = 0;
        //        MessageBox.Show(exstr, "Error");
        //    }
        //}

        public bool ShowErrorMessage()
        {
            string exstr = ExceptionListToString();
            if (exstr != "")
            {
                ErrState = 0;
                MessageBox.Show(exstr, "Error");
                return true;
            } else
            {
                return false;
            }

        }

        public void ShowErrorMessageAndClearError()
        {
            ShowErrorMessage();
            ClearError();
        }

        public void ClearError()
        {
            try
            {
                _exList = new List<Exception>();
                _funcList = new List<string>();
                ErrState = 0;
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".ClearError Failed");
                Debug.WriteLine(ex.Message);
            }
        }

        //　今リストに持っているエラーを文字列へ、出力前処理
        private string ExceptionListToString()
        {
            try
            {
                string exstr = "";

                //for (int i =0; i < _exList.Count; i++)
                //{
                //    exstr += _exList[i].Message + "\n" + _exList[i].StackTrace + "\n";
                //}
                int countmax = _exList.Count;
                int count = 0;
                int lastindex = countmax - 1;
                while ((count <= countmax) && (lastindex >= 0))
                {
                    if (_exList[lastindex] != null)
                    {
                        exstr += _exList[lastindex].Message + "\n" + _exList[lastindex].StackTrace + "\n";
                        _exList.RemoveAt(lastindex);
                        lastindex = _exList.Count - 1;
                    }
                    count++;
                    if (count >= countmax) { break; }
                }

                if (exstr != "")
                {
                    string func = "";
                    for (int i = _funcList.Count - 1; i >= 0; i--)
                    {
                        func += _funcList[i] + "\n";
                    }
                    exstr += "\n" + func;
                }

                return exstr;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.StackTrace);
                return ex.Message + ex.StackTrace;
            }
        }

        // 渡された文字列をメッセージボックスに表示するだけのもの
        // コンソールプログラムなので使う用
        public void MessageBoxShow(string str)
        {
            try
            {
                MessageBox.Show(str);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "ErrorLog Error");
            }
        }

        public void AddErrorNotException(string FunctionName)
        {
            ErrState = 1;
            _exList.Add(new NotException());
            _funcList.Add(FunctionName);
        }
        public void AddErrorNotException(string ClassName, string FunctionName)
        {
            AddErrorNotException(ClassName + " : " + FunctionName);
        }

        public bool HasError()
        {
            try
            {
                if (_exList.Count> 0)
                {
                    return true;
                }
                return false;
            } catch             {
                return false;
            }
        }
    }
}
