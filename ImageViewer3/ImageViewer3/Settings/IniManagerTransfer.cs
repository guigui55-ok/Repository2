using IniManager;
using System;
using System.Collections.Generic;

namespace ImageViewer.Settings
{
    // Transfer 移譲
    public class IniManagerTransfer : ISettingsManager
    {
        protected IniManager.IniManager _iniManager;
        protected ErrorLog.IErrorLog _errorLog;
        string _path;

        public IniManagerTransfer(ErrorLog.IErrorLog errorLog)
        {
            _errorLog = errorLog;
            _iniManager = new IniManager.IniManager(1);
        }

        public int SetPath(string iniPath) {
            int ret = _iniManager.SetPath(iniPath);
            if (ret < 1)
            { _errorLog.AddException(
                new Exception(_iniManager.Error.GetMessages()),this.ToString(), "SetPath Failed") ; }
            _path = iniPath;
            return ret;
        }
        public int ReadSetingsFile()
        {
            int ret = _iniManager.ReadIni();
            if (ret < 1)
            {
                _errorLog.AddException(
                  new Exception(_iniManager.Error.GetMessages()), this.ToString(), "ReadIni Failed");
            }
            return ret;
        }
        public int SetPathAndCreateFileIfPathNotExists(string iniPath, string iniFormatString)
        {
            int ret = _iniManager.SetPathAndCreateFileIfPathNotExists(iniPath,iniFormatString);
            if (ret < 1)
            {
                _errorLog.AddException(
                  new Exception(_iniManager.Error.GetMessages()), this.ToString(), "SetPathAndCreateFileIfPathNotExists Failed");
            }
            _path = iniPath;
            return ret;
        }
        public int SetSectionFromPath()
        {
            int ret = _iniManager.SetSectionFromPath();
            if (ret < 1)
            {
                _errorLog.AddException(
                  new Exception(_iniManager.Error.GetMessages()), this.ToString(), "SetSectionFromPath Failed");
            }
            return ret;
        }
        public string GetParameterValue(string sectionName, string parametersName)
        {
            string ret = _iniManager.GetParameterValue(sectionName,parametersName);
            if (_iniManager.Error.HasException())
            {
                _errorLog.AddException(
                  new Exception(_iniManager.Error.GetMessages()), this.ToString(), "GetParameterValue Failed");
            }
            return ret;
        }
        public int SetParameterValue(string sectionName, string parameterName, string value)
        {
            int ret = _iniManager.SetParameterValue(sectionName, parameterName, value);
            if (ret < 1)
            {
                _errorLog.AddException(
                  new Exception(_iniManager.Error.GetMessages()), this.ToString(), "SetParameterValue Failed");
            }
            return ret;
        }
        public List<string> GetSectionValues(string sectionName)
        {
            List<string> ret = _iniManager.GetSectionValues(sectionName);
            if (_iniManager.Error.HasException())
            {
                _errorLog.AddException(
                  new Exception(_iniManager.Error.GetMessages()), this.ToString(), "GetSectionValues Failed");
            }
            return ret;
        }
        public int CountSection(string sectionName)
        {
            int ret = _iniManager.CountSection(sectionName);
            if (ret < 1)
            {
                _errorLog.AddException(
                  new Exception(_iniManager.Error.GetMessages()), this.ToString(), "CountSection Failed");
            }
            return ret;
        }
        public int WriteAllData()
        {
            // list<string> にする
            List<string> writeList = _iniManager.ConvertSectionDataToListString();
            IniWriter writer = new IniWriter(_iniManager.Error);
            int ret = writer.WriteData(_path, writer.Constants.CREATE_FILE_WHEN_NOT_EXISTS, writeList);
            if (ret < 1)
            {
                _errorLog.AddException(
                  new Exception(_iniManager.Error.GetMessages()), this.ToString(), "WriteAllData Failed");
            }
            return ret;
        }

        public bool HasException()
        {
            return _iniManager.Error.HasException();
        }
        public string GetErrorMessages()
        {
            return _iniManager.Error.GetMessages();
        }

        public void ClearError()
        {
            _iniManager.Error.ClearError();
        }
    }
}
