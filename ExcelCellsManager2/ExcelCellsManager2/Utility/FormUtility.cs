using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonUtility.FormUtility
{
    public class FormUtility
    {
        protected ErrorManager.ErrorManager _error;
        public FormUtility(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        public string GetValueFromControl(Control control,Type type)
        {
            try
            {
                if (type == typeof(CheckBox))
                {
                    CheckBox chk = (CheckBox)control;
                    return chk.Checked.ToString();
                }
                else
                if (type == typeof(TextBox))
                {
                    TextBox chk = (TextBox)control;
                    return chk.Text;
                }
                else
                {
                    _error.AddLog("*** Control Type Not Support");
                    return "";
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetValueFromControl");
                return "";
            }
        }

        public void SetValueToControl(Control control,Type type,object value)
        {
            try
            {
                if (value == null)
                {
                    _error.AddLog("*** " + this.ToString() + ".SetValueToControl : (object)Value == null : type=" + type.ToString());
                    return;
                }
                if (type == typeof(CheckBox))
                {
                    CheckBox chk = (CheckBox)control;
                    chk.Checked = (bool)value;
                }
                else
                if (type == typeof(TextBox))
                {
                    TextBox chk = (TextBox)control;
                    chk.Text = (string)value;
                }
                else
                {
                    _error.AddLog("*** " + this.ToString() + ".SetValueToControl : Control Type Not Support : type="+ type.ToString()) ;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValueToControl");
            }
        }


        // Check する Type にキャストしてみて問題なければ True を返す
        public bool IsMatchControlType(Control control,Type type)
        {
            try
            {
                //if (type != control.GetType())
                //{
                //    //_error.AddLogAlert(this.ToString()+ ".IsMatchControlType : Type Is Invalid.");
                //    _error.AddLogAlert(new Exception(this.ToString() + ".IsMatchControlType : Type Is Invalid."));
                //    _error.AddLogAlert("control.GetType="+control.GetType().ToString() + " ,type="+type.ToString());
                //    return false;
                //}
                if (type == typeof(CheckBox))
                {
                    CheckBox chk = (CheckBox)control;
                    return true;
                } else 
                if (type == typeof(TextBox))
                {
                    TextBox chk = (TextBox)control;
                    return true;
                }
                if (type == typeof(Panel))
                {
                    Panel chk = (Panel)control;
                    return true;
                }
                else
                {
                    _error.AddException(new Exception("*** Control Type Not Support : type="+type.ToString()));                    
                    return false;
                }
                
            }
            catch (System.InvalidCastException)
            {
                return false;
            }
            catch 
            {
                //_error.AddExceptionNormal(ex, this.ToString() + ".GetControlFromControl");
                return false;
            }
        }
        public Control GetControlFromControl(Control control,Type type)
        {
            try
            {
                if (control == null) { throw new Exception("control is null"); }
                if (type == null) { throw new Exception("type is null"); }

                foreach (Control con in control.Controls)
                {
                    if (con.GetType().Equals(type))
                    {
                        return con;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetControlFromControl");
                return null;
            }
        }

        // Control が保持している対象の Name の control を取得する。探す対象は子コントロール内も含めるる
        public Control GetControlFirstMatchNameFromControlIncludeChild(Control control, string controlName)
        {
            return GetControlFirstMatchFromControlIncludeChild(control, 2, controlName);
        }
        // Control が保持している対象の Type の control を取得する。探す対象は子コントロール内も含める
        public Control GetControlFirstMatchTypeFromControlIncludeChild(Control control, Type type)
        {
            return GetControlFirstMatchFromControlIncludeChild(control, 1, type);
        }

        // Control が保持している対象の Type,Name の control を取得する。探す対象は子コントロール内も含める
        public Control GetControlFirstMatchFromControlIncludeChild(Control control,int mode ,object target)
        {
            try
            {
                if (control == null) { throw new Exception("Control is null"); }
                if (control.Controls.Count < 1) { _error.AddLog("Control Count < 1"); return null; }
                Control ret = null;
                bool isMatch = false;
                foreach (Control item in control.Controls)
                {
                    // 
                    if (item.HasChildren)
                    {
                        // 子コントロールがあれば再帰的に実行する
                        ret = GetControlFirstMatchFromControlIncludeChild(item, mode,target);
                        return ret;
                    } else
                    {
                        // not HasChildren
                        isMatch = isMatchControl(item, mode, target);
                        if (isMatch)
                        {
                            return item;
                        }
                    }
                }
                return null;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetControlFirstMatchFromControlIncludeChild");
                return null;
            }
        }
        public Control GetControlFromForm(Form form, string controlName)
        {
            return GetControlFromForm(form, 2, controlName);
        }

        // ----------------
        public Control GetControlFromForm(Form form,Type type)
        {
            return GetControlFromForm(form, 1, type);
        }

        public bool isMatchControl(Control control,int mode,object target)
        {
            try
            {
                bool ret = false;
                switch (mode)
                {
                    case 1:
                        // Control Type
                        //Console.WriteLine(" type =" + ((Type)target).ToString()); 
                        if (control.GetType().ToString() == ((Type)target).ToString())
                        {

                            if (control.GetType() == (Type)target)
                            {
                                return true;
                            }
                        }
                        break;
                    case 2:
                        if (target.GetType() == typeof(string))
                        {
                            if (control.Name == (string)target)
                            {
                                return true;
                            }
                        }
                        break;
                    default:
                        ret = false;
                        _error.AddLogAlert(this, "isMatch Type is Invalid");
                        break;
                }
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".isMatch");
                return false;
            }
        }
        // ----------------
        public Control GetControlFromForm(Form form, int mode,object target)
        {
            try
            {
                if (form == null) { throw new Exception("form is null"); }
                bool isMatch = false;
                foreach (Control formControl in form.Controls)
                {
                    isMatch = isMatchControl(formControl, mode, target);
                    if (isMatch)
                    {
                        return formControl;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetControlFromForm");
                return null;
            }
        }

        //private bool IsMatchControlName(Control control, object target)
        //{
        //    try
        //    {
        //        if (target == null) { throw new Exception("target is null"); }
        //        string targetName = "";
        //        if (target.GetType().Equals(typeof(string)))
        //        {
        //            targetName = (string)target;
        //        }

        //        if (control.Name.Equals(targetName))
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        _error.AddException(ex, this.ToString() + ".IsMatchControlName");
        //        return false;
        //    }
        //}

        //private bool IsMatchControlType(Control control,object target)
        //{
        //    try
        //    {
        //        if (target == null) { throw new Exception("type is null"); }

        //        if (control.GetType().Equals(target))
        //        {
        //            return true;
        //        }
        //        return false;
        //    } catch (Exception ex)
        //    {
        //        _error.AddException(ex, this.ToString() + ".IsMatchControlType");
        //        return false;
        //    }
        //}
    }
}
