using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonUtility.ControlUtility
{
    public class ControlUtility
    {
        ErrorManager.ErrorManager _err;
        public ControlUtility(ErrorManager.ErrorManager err)
        {
            _err = err;
        }

        public Control GetControlFromControlByControlTextMultipleHierarchies(
            Control control,string[] textArray,int index=0)
        {
            Control ret = null;
            try
            {
                if(textArray == null) { _err.AddLogWarning(this, "textArray is Null"); return null; }
                if(textArray.Length < 1) { _err.AddLogWarning(this,"textArray.Length<1"); return null; }
                int n = index;
                foreach (Control con in control.Controls)
                {
                    if (con.Text.Equals(textArray[n]))
                    {
                        n++;
                        if(n == textArray.Length)
                        {
                            return con;
                        } else if(n < textArray.Length)
                        {
                            ret =  GetControlFromControlByControlTextMultipleHierarchies(con, textArray, n);
                            if (ret != null) { return ret; }
                        }
                    }
                }
                if (index >= n)
                {
                    _err.AddLogWarning(this, "Control is Nothing. i=" + index + " , " + textArray[index]);
                }
                return ret;
            } catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + ".GetControlFromControlByControlTextMultipleHierarchies");
                return null;
            }
        }

        /// <summary>
        /// Control.Text と合致したものを取得する
        /// </summary>
        /// <param name="control"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public Control GetControlFromControlByControlText(Control control, string text)
        {
            Control ret=null;
            try
            {
                if (control == null) { throw new Exception("control is null"); }
                if (text == null) { throw new Exception("text is null"); }
                if (text == "") { _err.AddLogAlert(this,"text.Length < 1"); }

                foreach (Control con in control.Controls)
                {
                    if (con.Text.Equals(text))
                    {
                        return con;
                    } else
                    {
                        if (con.HasChildren)
                        {
                            foreach(Control con2 in con.Controls)
                            {
                                ret = GetControlFromControlByControlText(control, text);
                                if (ret != null) { return ret; }
                            }
                        }
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + ".GetControlFromControlByControlText");
                return null;
            }
        }

    }
}
