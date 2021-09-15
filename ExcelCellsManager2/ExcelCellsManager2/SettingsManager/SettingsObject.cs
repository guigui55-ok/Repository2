using CommonUtility;
using CommonUtility.FormUtility;
using System;
using System.Windows.Forms;
using Utility;

namespace SettingsManager
{
    public class SettingsObject : ISettingsObject
    {
        protected ErrorManager.ErrorManager _error;
        protected FormUtility _formUtil;
        protected TypeUtility _typeUtil;
        protected string _settingsTypeName = "";
        protected string _name = "";
        protected string _description = "";
        protected Type _valueType;
        protected object _value;
        protected object _control;
        protected string _regKey = "";
        protected object _initialValue;
        protected Keys _shortCutKeys;

        public Keys ShortCutKeys { get => _shortCutKeys; set => _shortCutKeys = value; }
        public object InitialValue { get => _initialValue; set => _initialValue = value; }
        public string SettingsTypeName { get => _settingsTypeName; set => _settingsTypeName = value; }
        string ISettingsObject.Name { get => _name; set => _name = value; }
        string ISettingsObject.Description { get => _description; set => _description = value; }
        Type ISettingsObject.ValueType { get => _valueType; set => _valueType = value; }
        object ISettingsObject.Value {
            get => _value;
            set {
                _value = value;
            }
        }
        object ISettingsObject.Control {
            get => _control;
            set {
                _control = value;
                SetControlEvent();
            }
        }
        string ISettingsObject.RegKey { get => _regKey; set => _regKey = value; }

        private Control saveValueControl;
        private Type saveValueControlType;



        public SettingsObject(ErrorManager.ErrorManager error, FormUtility formUtility,TypeUtility typeUtility)
        {
            _settingsTypeName = "";
            _error = error;
            _formUtil = formUtility;
            _typeUtil = typeUtility;
        }

        private void SetControlEvent()
        {
            try
            {
                _error.AddLog(this.ToString() + ".SetControlEvent");
                ISettingsObject item = this;
                Control control;
                if (item.Control != null)
                {
                    control = (Control)item.Control;
                    if (_formUtil.IsMatchControlType(control, typeof(CheckBox)))
                    {
                        CheckBox chk = (CheckBox)control;
                        chk.CheckedChanged += Chk_CheckedChanged;
                        this.saveValueControl = chk;
                        this.saveValueControlType = chk.GetType();
                    }
                    else
                    if (_formUtil.IsMatchControlType(control, typeof(Panel)))
                    {
                        // Panel の場合は TextBox を保持している
                        TextBox txt = (TextBox)_formUtil.GetControlFirstMatchTypeFromControlIncludeChild(control, typeof(TextBox));
                        txt.TextChanged += Txt_TextChanged;
                        this.saveValueControl = txt;
                        this.saveValueControlType = txt.GetType();
                    }
                }
                else
                {
                    _error.AddLog(" SettingsObject.Control is null");
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetControlEvent");
            }
        }

        // フォーム初期化時は SettingsFile/Registry から取得した value をフォームに反映していない
        private void SetValueToControlFromValue()
        {
            try
            {
                if ((saveValueControl != null)&&(saveValueControlType != null))
                {
                    _formUtil.SetValueToControl(saveValueControl, saveValueControlType, _value);
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValueToControlFromValue");
            }
        }

         private void Txt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString() + ".Txt_TextChanged");
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Txt_TextChanged");
            }
        }

        private void Chk_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                _error.AddLog(this.ToString() + ".Chk_CheckedChanged");
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Chk_CheckedChanged");
            }
        }

        // Member -> Control
        public void SetValueToControlFromMember()
        {
            try
            {
                _error.AddLog(this.ToString() + ".SetValueToControlFromMember: Name=" + _name);
                if (saveValueControlType == typeof(CheckBox))
                {
                    if (_valueType == typeof(bool))
                    {
                        ((CheckBox)saveValueControl).Checked = (bool)_value;
                    }
                    else
                    {
                        ((CheckBox)saveValueControl).Checked = _typeUtil.ConvertToBoolean(_value);
                    }
                }
                else
                if (saveValueControlType == typeof(TextBox))
                {
                    if (_valueType == typeof(int[]))
                    {
                        ((TextBox)saveValueControl).Text = _typeUtil.ConvertToString(_value);
                    }
                    else
                    if (_valueType == typeof(string))
                    {
                        ((TextBox)saveValueControl).Text = (string)_value;
                    }
                }


            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValueToControlFromMember");
            }
        }

        public void SetValue(object value)
        {
            try
            {
                // いったん string で受ける
                string val = _typeUtil.ConvertToString(value);
                // string をそれぞれの方に再変換する
                if (this._valueType == typeof(string))
                {
                    this._value = val;
                }
                else
                if (this._valueType == typeof(bool))
                {
                    _value = _typeUtil.ConvertToBoolean(val);
                }
                else
                if (this._valueType == typeof(int[]))
                {
                    _value = _typeUtil.ConvertToIntArray(val);
                }
                else
                {
                    _error.AddLog("  *** " + this.ToString() + ".SetValueToMemberFromControl : SetValue : Value Type Not Supported.");
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValue");
            }
        }

        // Control -> Member
        public void SetValueToMemberFromControl()
        {
            try
            {
                // いったん string で受ける
                string val = _formUtil.GetValueFromControl(saveValueControl,saveValueControlType);
                // string をそれぞれの方に再変換する
                if (this._valueType == typeof(string))
                {
                    this._value = val;
                } else
                if (this._valueType == typeof(bool))
                {
                    _value = _typeUtil.ConvertToBoolean(val);
                } else
                if (this._valueType == typeof(int[]))
                {
                    _value = _typeUtil.ConvertToIntArray(val);
                } else
                {
                    _error.AddLog("  *** " + this.ToString() + ".SetValueToMemberFromControl : SetValue : Value Type Not Supported.");
                }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValueToMemberFromControl");
            }
        }

        //private void AddException(Exception exception, string functionName,int type = 0)
        //{
        //    try
        //    {
        //        if(_error != null)
        //        {
        //            _error.AddException(exception, functionName,type);
        //        } else
        //        {
        //            Console.WriteLine("*** AddException Error : ClassName = " + this.ToString()); ;
        //            Console.WriteLine(exception.Message);
        //            Console.WriteLine(exception.StackTrace);
        //        }
        //    } catch (Exception ex)
        //    {
        //        Console.WriteLine("*** AddException Error : ClassName = " + this.ToString()); ;
        //        Console.WriteLine(ex.Message);
        //        Console.WriteLine(ex.StackTrace);
        //    }
        //}
    }
}
