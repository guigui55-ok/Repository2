using System;

namespace ExcelCellsManager.ExcelCellsManager.Settings
{
    public class SettingsRegisterToReg : ISettingsRegister
    {
        protected ErrorManager.ErrorManager _error;

        public SettingsRegisterToReg(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        public string GetValue(string keyPath,string KeyName)
        {
            try
            {
                throw new Exception("未実装");
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".GetValue");
                return "";
            }
        }

        public void ReadSettings()
        {
            throw new NotImplementedException();
        }

        public void SaveValuesToFile()
        {
            throw new NotImplementedException();
        }

        public void SetPath(string path)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string kindName, string subItemName, string subItemValue)
        {
            throw new NotImplementedException();
        }
    }
}
