using System;
using System.Collections.Generic;

namespace SettingsManager
{
    public class SettingsManager
    {
        protected ErrorManager.ErrorManager _error;
        public List<ISettingsObject> SettingsList;
        public SettingsManager(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        public void SetValue(string settingsTypeName, string settingsName,object value)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".SetValue");
                if (value == null) { _error.AddLog("  value == null"); return; }
                _error.AddLog("settingName="+ settingsTypeName + " ,settingsName="+settingsName + " ,value="+value.ToString());
                ISettingsObject setting = GetSetting(settingsTypeName, settingsName);
                if (setting != null)
                {
                    setting.SetValue(value);
                } else
                {
                    _error.AddLog(this.ToString() + ".SetValue: setting == Null");
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValue");
            }
        }

        public object GetValue(string settingsTypeName, string settingsName)
        {
            try
            {
                ISettingsObject setting = GetSetting(settingsTypeName, settingsName);
                if (setting != null)
                {
                    if (setting.Value == null)
                    {
                        _error.AddLog(this.ToString() + ".GetValue is null");
                        return null;
                    } else
                    {
                        _error.AddLog(this.ToString() + ".GetValue: setting.Value.GetType()=" + setting.Value.GetType().ToString());
                        return (object)setting.Value;
                    }
                } else
                {
                    _error.AddLog(this.ToString() + ".GetValue: setting.Value.GetType()= setting.Value = Null");
                    return (object)null;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".GetValue");
                return null;
            }
        }

        public ISettingsObject GetSetting(string settingsTypeName, string settingsName)
        {
            try
            {
                if (this.SettingsList == null) { throw new Exception("SettingsList is Null"); }
                if (this.SettingsList.Count < 1) { throw new Exception("SettingsList.Count < 1"); }

                foreach(ISettingsObject setting in SettingsList)
                {
                    if ((setting.SettingsTypeName.Equals(settingsTypeName))
                        && (setting.Name.Equals(settingsName)))
                    {
                        return setting;
                    }
                }
                return null;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetSetting");
                return null;
            }
        }

    }
}
