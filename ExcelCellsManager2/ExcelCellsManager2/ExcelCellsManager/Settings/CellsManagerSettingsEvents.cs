using CommonUtility.FormUtility;
using SettingsManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelCellsManager.Settings
{
    public class CellsManagerSettingsEvents
    {
        protected ErrorManager.ErrorManager _error;
        protected FormUtility _formUtil;
        public List<ISettingsObject> SettingsList;
        

        public CellsManagerSettingsEvents(ErrorManager.ErrorManager error,List<ISettingsObject> settingsObjects)
        {
            _error = error;
            SettingsList = settingsObjects;
            _formUtil = new FormUtility(_error);
        }

        public void Initialize()
        {
            try
            {
                _error.AddLog(this.ToString()+ ".Initialize");
                if (SettingsList == null) { throw new Exception("SettingsList == null"); }
                if (SettingsList.Count < 1) { throw new Exception("SettingsList.Count < 1"); }
                Control control;
                foreach(ISettingsObject item in SettingsList)
                {
                    if (item.Control != null)
                    {
                        control = (Control)item.Control;
                        if (_formUtil.IsMatchControlType(control,typeof(CheckBox)))
                        {
                            CheckBox chk = (CheckBox)control;
                            chk.CheckedChanged += Chk_CheckedChanged;
                        } else 
                        if (_formUtil.IsMatchControlType(control, typeof(Panel))){
                            // Panel の場合は TextBox を保持している
                            TextBox txt = (TextBox)_formUtil.GetControlFirstMatchTypeFromControlIncludeChild(control, typeof(TextBox));
                            txt.TextChanged += Txt_TextChanged;
                        }
                    } else
                    {
                        _error.AddLog(" SettingsObject.Control is null");
                    }
                }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Initialize");
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
                _error.AddLog(this.ToString()+ ".Chk_CheckedChanged");


            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Chk_CheckedChanged");
            }
        }


        // 閉じるとき、適用時に実行する
        private void ApplyValue()
        {
            try
            {
                // List ISettings.Control からすべて値を取得して
                // ISettings.Value に入れなおす
                _error.AddLog(this.ToString() + ".ApplyValue");
                if (SettingsList == null) { throw new Exception("SettingsList == null"); }
                if (SettingsList.Count < 1) { throw new Exception("SettingsList.Count < 1"); }

                Control control;
                foreach (ISettingsObject item in SettingsList)
                {
                    if (item.Control != null)
                    {
                        control = (Control)item.Control;
                        if (_formUtil.IsMatchControlType(control, typeof(CheckBox)))
                        {
                            CheckBox chk = (CheckBox)control;
                            bool val = chk.Checked;
                            // Name を取得、Name から ISettigsObjectList を特定して、Value に値を格納する
                        }
                        else
                        if (_formUtil.IsMatchControlType(control, typeof(Panel)))
                        {
                            // Panel の場合は TextBox を保持している
                            TextBox txt = (TextBox)_formUtil.GetControlFirstMatchTypeFromControlIncludeChild(control, typeof(TextBox));
                            string val = txt.Text;
                        }
                    }
                    else
                    {
                        _error.AddLog(" SettingsObject.Control is null");
                    }
                }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ApplyValue");
            }
        }

        //private void SetValueToSettingValueFromSettingControl(string name,object value)
        //{

        //}
    }
}
