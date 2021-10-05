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
        /// <summary>
        /// Control の中 (子 Control を含む) から Control.GetType に合致した Control を取得する
        /// </summary>

        /// <summary>
        /// Control の中 (子 Control を含む) から Control.Text に合致した Control を取得する
        /// </summary>
        /// <param name="control"></param>
        /// <param name="textArray"></param>
        /// <param name="index"></param>
        /// <returns></returns>
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

        public bool FormIsActiveWithModal(Form form)
        {
            try
            {
                if (Form.ActiveForm == form)
                {
                    return true;
                }

                return false;
            } catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + ".FormIsActiveWithModal");
                return false;
            }
        }

        /// <summary>
        /// 子 Control を含む Control と type が一致したものすべてを List<Control> として返す
        /// </summary>
        /// <param name="control"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Control> GetControlListMatchType(Control control,Type type)
        {
            List<Control> retList = new List<Control>();
            try
            {
                if (control == null) { throw new Exception("control is null"); }
                if (type == null) { throw new Exception("type is null"); }

                foreach (Control con in control.Controls)
                {
                    // 一致したときリストに追加する
                    if (ControlIsMatch((int)ControlUtilityConstants.MODE_TYPE,control,type))
                    {
                        retList.Add(con);
                    }
                    // HasChildren=true の時は子コントロールもチェックする
                    if (con.HasChildren)
                    {
                        foreach (Control con2 in con.Controls)
                        {
                            // 子コントロールも HasChildren=True のとき、さらに子コントロールもチェックする
                            retList.AddRange(GetControlListMatchType(con2, type));
                        }
                    }
                }
                return retList;
            } catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + ".GetControlListMatchType");
                return retList;
            }
        }

        /// <summary>
        /// Control が指定した値と一致するか判定する
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="control"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ControlIsMatch(int Mode,Control control,object value)
        {
            try
            {
                switch (Mode)
                {
                    case (int)ControlUtilityConstants.MODE_TEXT:
                        if (value.GetType().Equals(typeof(string)))
                        {
                            if (control.Text.Equals(value))
                            {
                                return true;
                            } else
                            {
                                return false;
                            }
                        } else
                        {
                            _err.AddLogAlert(this, "ControlIsMatch : object value Type Is Invalid.");
                            return false;
                        }
                    case (int)ControlUtilityConstants.MODE_TYPE:
                        if (value.GetType().Equals(typeof(Type)))
                        {
                            if (control.GetType().Equals(value))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            _err.AddLogAlert(this, "ControlIsMatch : object value Type Is Invalid.");
                            return false;
                        }
                    default:
                        break;
                }
                _err.AddLogAlert(this, "Mode is Invalid");
                return false;
            } catch (Exception ex)
            {
                _err.AddException(ex, this.ToString() + ".ControlIsMatch");
                return false;
            }
        }



    }
    public enum ControlUtilityConstants
    {
        MODE_TEXT,
        MODE_TYPE,
        TYPE_FORM
    }
}
