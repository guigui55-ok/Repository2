using System;

namespace ExcelCellsManager.ExcelCellsManager.Settings
{
    public class SettingsRegisterToIni : ISettingsRegister
    {
        protected ErrorManager.ErrorManager _error;
        protected IniManager.IniManager _iniManager;

        public SettingsRegisterToIni(ErrorManager.ErrorManager error)
        {
            _error = error;
            _iniManager = new IniManager.IniManager(_error);
        }

        public void SetPath(string path)
        {
            try
            {
                // Path をセットする、なければ作る
                _iniManager.SetPathAndCreateFileIfPathNotExists(path, "");
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetPath");
            }
        }

        public void ReadSettings()
        {
            try
            {
                int ret;
                // あったら読み込む
                ret = _iniManager.ReadIni();
                if (ret < 1) { throw new Exception("iniManager ReadIni Failed");}

                // 読み込んだら値をクラス変数にセット
                ret = _iniManager.SetSectionFromPath();
                if (ret == 0) { throw new Exception("iniManager SetSectionFromPath Failed"); }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ReadSettings");
            }
        }

        public string GetValue(string sectionName,string parameterName)
        {
            try
            {
                string ret = _iniManager.GetParamterValueWhenAddValueIsNothing(sectionName, parameterName);
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".GetValue");
                return "";
            }
        }

        public void SetValue(string sectionName,string parameterName ,string value)
        {
            try
            {
                _iniManager.SetParamterValueWhenAddValueIsNothing(sectionName, parameterName, value);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValue");
            }
        }

        public void SaveValuesToFile()
        {
            try
            {
                _iniManager.WriteAllData();
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SaveValuesToFile");
            }
        }

    }
}
