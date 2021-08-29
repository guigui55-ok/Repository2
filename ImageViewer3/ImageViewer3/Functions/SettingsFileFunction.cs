using ErrorLog;
using ImageViewer.ParentForms;
using System;

namespace ImageViewer.Settings
{
    public class SettingsFileFunction
    {
        protected ErrorLog.IErrorLog _errorLog;
        protected ISettingsManager _settingsFileManager;
        protected MainFormManager _mainformManager;

    public SettingsFileFunction(IErrorLog errorLog,int mode,MainFormManager mainFormManager)
        {
            _errorLog = errorLog;
            _mainformManager = mainFormManager;
            _ = SetSettingsFileKind(mode);
        }

        public int SetSettingsFileKind(int mode)
        {
            try
            {
                switch (mode)
                {
                    case 1: // ImageViewer.ImageViewerConstants.SETTINGS_INI
                        _settingsFileManager = new IniManagerTransfer(_errorLog);
                        break;
                    case 2: //
                        break;
                    default:
                        break;
                }
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ReadSettings Failed");
                return 0;
            }
        }
        public int ReadSettings()
        {
            try
            {
                // フォーマットを作成 -> プロジェクトにインポートしておく
                string iniFormatString = new Common.FileIO().ExcuteRead(
                    _mainformManager.ViewImageManager.Settings.SettingsFormatFilePath);

                // Path をセットする、なければ作る
                int ret = _settingsFileManager.SetPathAndCreateFileIfPathNotExists(
                    _mainformManager.ViewImageManager.Settings.SettingsFilePath,
                    _mainformManager.ViewImageManager.Settings.SettingsFormatFilePath);
                if (_settingsFileManager.HasException()) 
                { _errorLog.AddException(
                        new Exception(_settingsFileManager.GetErrorMessages()),this.ToString(),".ReadSettings");return -1; }

                // あったら読み込む
                ret = _settingsFileManager.ReadSetingsFile();
                if (_settingsFileManager.HasException()) {
                    _errorLog.AddException(
                          new Exception(_settingsFileManager.GetErrorMessages()), this.ToString(), ".ReadSettings"); return -2; }

                // 読み込んだら値をクラス変数にセット
                ret = _settingsFileManager.SetSectionFromPath();
                if (_settingsFileManager.HasException())
                { _errorLog.AddException(
                          new Exception(_settingsFileManager.GetErrorMessages()), this.ToString(), ".ReadSettings"); return -3; }

                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ReadSettings Failed");
                return 0;
            }
        }

        public string GetParameterValue(string sectionName,string parameterName)
        {
            return  _settingsFileManager.GetParameterValue(sectionName, parameterName);
            
        }

        public void SetParameterValue(string sectionName, string parameterName,string value)
        {
            _settingsFileManager.SetParameterValue(sectionName, parameterName, value);
        }

        public int ReadSettingsFormat()
        {
            try
            {                
                string iniFormatString = new Common.FileIO().ExcuteRead(
                    _mainformManager.ViewImageManager.Settings.SettingsFormatFilePath);
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ReadSettingsFormat Failed");
                return 0;
            }
        }
        public int ReadSettingsFormatFromResources()
        {
            try
            {
                // 現在実行しているAssemblyを取得する
                System.Reflection.Assembly asm;
                asm = System.Reflection.Assembly.GetExecutingAssembly();
                // ResourceManagerオブジェクトの作成
                //リソースファイル名が"Resource1.resources"だとする
                System.Resources.ResourceManager rm =
                    new System.Resources.ResourceManager(
                        asm.GetName().Name + ".Resource1", asm);

                // リソースファイルから文字列を取り出す
                string s = rm.GetString("SettingsFormat");
                return 1;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ReadSettingsFormatFromResources Failed");
                return 0;
            }
        }

        public int UpdateSettings()
        {
            try
            {

                return 1;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ReadSettings Failed");
                return 0;
            }
        }

        public int WriteSettingToFile()
        {
            try
            {
                // クラスで保持している値をすべて書き込む
                int ret = _settingsFileManager.WriteAllData();
                if (_settingsFileManager.HasException())
                {
                    _errorLog.AddException(
                            new Exception(_settingsFileManager.GetErrorMessages()), this.ToString(), ".WriteSettingToFile"); return -3;
                }

                return 1;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ReadSettings Failed");
                return 0;
            }
        }
    }
}
