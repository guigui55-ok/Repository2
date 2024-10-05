using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;
using CommonModules;
using CommonModulesProject;
using ImageViewer5.ImageControl;
using JsonStreamModule;

namespace ImageViewer5
{
    public class FormMainSetting
    {
        public int formMainCount = 2;
        //#
        //Frame の配置の決め方（Frameが複数あるときに、配置するときに使用する）
        // ToRight , ToLeft
        // ToDonw , ToUp
        // Priority, Horizon, Vertical
        // FixFrameSize(Formのサイズを大きくする）、FixFormSize（Window内でFrameのサイズを調整する）
        //#
        public int frameDirectionHorizon = ImageViewerConstants.TO_RIGHT;
        public int frameDirectionVertical = ImageViewerConstants.TO_BOTTOM;
        public int framePreferredDirection = ImageViewerConstants.FRAME_PRIORITY_HORIZON;
        public int frameRowMax = ImageViewerConstants.FRAME_ROW_MAX_DEFAULT;
        public int frameColMax = ImageViewerConstants.FRAME_COL_MAX_DEFAULT;
        public int iIsFixFrameSize = ImageViewerConstants.FIX_FRAME_SIZE;
        //
        public bool isOutputSetting = true;

        //
        AppLogger _logger;
        JsonStream _jsonStream;
        FormMain _formMain;
        public SettingDictionary _settingDictionary;

        public FormMainSetting(AppLogger logger, FormMain formMain)
        {
            _logger = logger;
            _jsonStream = new JsonStream();
            _formMain = formMain;
            _settingDictionary = new SettingDictionary("SettingDict_FormMain");
        }

        /// <summary>
        /// 外部から読み込み、設定値を格納する
        /// </summary>
        /// <param name="formMain"></param>
        public void SetSettingDataByDict(Dictionary<string, object> settingDict)
        {
            _logger.PrintInfo("FormMainSetting > SetSettingDataByDict");
            _settingDictionary._settingDict = settingDict;

        }

        public void SaveSetting()
        {
            _logger.PrintError("FormMainSetting > SaveSetting");
            try
            {
                // Default値を設定して、Jsonファイルを書き込み
                var writeDict = GetSettingValueFromNowState_WithImageMainFrame();
                _jsonStream.WriteFile(writeDict);
                if (isOutputSetting)
                {
                    //CommonModule.CommonGeneral.PrintDict(settingDict);
                    string json = _jsonStream.DictToJson(writeDict);
                    _logger.PrintInfo("allInfoDict = ");
                    _logger.PrintInfo(json);
                }
                _logger.PrintInfo("SaveSetting Done.");
            } catch(Exception ex)
            {
                _logger.PrintError(ex, "FormMainSetting > SaveSetting Error");
            }
        }


        /// <summary>
        /// 外部から読み込み、設定値を格納する
        /// </summary>
        /// <param name="path"></param>
        public void ReadSettingFile(string path)
        {
            _logger.PrintInfo("FormMainSetting > ReadSettingFile");
            if (File.Exists(path))
            {
                _jsonStream._path = path;
                // readSetting
            }
            else
            {
                // Default値を設定して、Jsonファイルを書き込み
                //string json = ConstFileSender.DEFAULT_JSON;
                var defaultDict = GetSettingValueInitialize_WithImageMainFrame();
                _jsonStream._path = path;
                //Dictionary<string, object> dict = _jsonStream.JsonToDict(json);
                _jsonStream.WriteFile(defaultDict);
                _logger.PrintInfo("Create New Setting File");
            }
            Dictionary<string, object> settingDict = _jsonStream.ReadFile();
            // ここで読み込みエラーがある場合は、ファイルのバックアップを取ってデフォルト値からやり直し
            _settingDictionary._settingDict = settingDict;
            if (isOutputSetting)
            {
                //CommonModule.CommonGeneral.PrintDict(settingDict);
                string json = _jsonStream.DictToJson(settingDict);
                _logger.PrintInfo("settingDict = ");
                _logger.PrintInfo(json);
            }
        }


        public Dictionary<string, object> GetSettingValueFromNowState_WithImageMainFrame()
        {
            _logger.PrintInfo("FormMainSetting > GetSettingValueFromNowState_WithImageMainFrame");
            Dictionary<string, object> bufDict;
            SetSettingValueToThisFromNowState();
            bufDict = _settingDictionary._settingDict;
            foreach (ImageMainFrame imageMainFrame in _formMain._mainFrameManager._imageMainFrameList)
            {
                imageMainFrame._imageMainFrameSetting.SetSettingValueToThisFromNowState();
                string key = (string)imageMainFrame._imageMainFrameSetting._settingDictionary._settingDict[SettingKey.FRAME_NAME];
                //bufDict.Add(key, imageMainFrame._imageMainFrameSetting._settingDictionary._settingDict);//同一のキー
                bufDict[key] = imageMainFrame._imageMainFrameSetting._settingDictionary._settingDict;
                Console.WriteLine(" ########## ");
                //CommonModule.CommonGeneral.PrintDict(imageMainFrame._imageMainFrameSetting._settingDictionary._settingDict);
                CommonModule.CommonGeneral.PrintDict(bufDict);
            }
            return bufDict;
        }

        public void SetSettingValueToThisFromNowState()
        {
            _logger.PrintInfo("FormMainSetting > SetSettingValueFromNowState");
            object value;
            //#
            value = CommonGeneral.GetApplicationPath();
            _settingDictionary.AddValue(SettingKey.APP_PATH, value);
            //#
            // RESTORE_PREV_STATE
            //#
            value = _formMain.Size;
            _settingDictionary.AddValue(SettingKey.MAIN_FORM_SIZE, value);
            //#
            value = _formMain.Location;
            _settingDictionary.AddValue(SettingKey.MAIN_FORM_LOC, value);
            //#
            // FRAME_DIRECTION_HORIZON
            // FRAME_DIRECTION_VERTICAL
            // FRAME_PREFERRED_DIRECTION
            // FRAME_ROW_MAX
            // FRAME_COL_MAX
            //#
            value = _formMain._mainFrameManager._imageMainFrameList.Count;
            _settingDictionary.AddValue(SettingKey.FRAME_COUNT, value);
            //#
            value = _formMain._fileSenderFunction._fileSenderApp.Visible;
            _settingDictionary.AddValue(SettingKey.SHOW_SENDER_DIALOG, value);
        }

        /// <summary>
        /// 設定値のデフォルトを取得する
        /// （MainFormとImageMainFrame1つ分）
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetSettingValueInitialize_WithImageMainFrame()
        {
            Dictionary<string, object> bufDict = GetSettingValueInitialize();
            Dictionary<string, object> bufDictB = ImageMainFrameSetting.GetSettingValueInitialize();
            string key = (string)bufDictB[SettingKey.FRAME_NAME];
            bufDict.Add(key, bufDictB);
            return bufDict;
        }

        /// <summary>
        /// 設定値のデフォルトを取得する（FormMainのみ）
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetSettingValueInitialize()
        {
            Dictionary<string, object> bufDict = new Dictionary<string, object>();
            object value;
            //#
            value = CommonGeneral.GetApplicationPath();
            bufDict.Add(SettingKey.APP_PATH, value);
            //#
            value = SettingValueDefault.RESTORE_PREV_STATE;
            bufDict.Add(SettingKey.RESTORE_PREV_STATE, value);
            //#
            value = SettingValueDefault.MAIN_FORM_SIZE;
            bufDict.Add(SettingKey.MAIN_FORM_SIZE, value);
            //#
            value = SettingValueDefault.MAIN_FORM_LOC;
            bufDict.Add(SettingKey.MAIN_FORM_LOC, value);
            //#
            //ImageViewerConstants
            value = ImageViewerConstants.TO_RIGHT;
            bufDict.Add(SettingKey.FRAME_DIRECTION_HORIZON, value);
            //#
            //ImageViewerConstants
            value = ImageViewerConstants.TO_BOTTOM;
            bufDict.Add(SettingKey.FRAME_DIRECTION_VERTICAL, value);
            //#
            //ImageViewerConstants
            value = ImageViewerConstants.FRAME_PRIORITY_VERTICAL;
            bufDict.Add(SettingKey.FRAME_PREFERRED_DIRECTION, value);
            //#
            value = SettingValueDefault.FRAME_ROW_MAX;
            bufDict.Add(SettingKey.FRAME_ROW_MAX, value);
            //#
            value = SettingValueDefault.FRAME_COL_MAX;
            bufDict.Add(SettingKey.FRAME_COL_MAX, value);
            //#
            value = 1;
            bufDict.Add(SettingKey.FRAME_COUNT, value);
            //#
            value = false;
            bufDict.Add(SettingKey.SHOW_SENDER_DIALOG, value);
            return bufDict;
        }


        /// <summary>
        /// サイズの文字列”000，000”をSize型に変更する
        /// </summary>
        /// <returns></returns>
        static public Size ConvertStringToSize(string value)
        {
            Size retSize = new Size();
            try
            {
                // （"100 , 100"を想定）
                var size = value;
                string[] dimensions = value.Split(',');
                int width = int.Parse(dimensions[0]);
                int height = int.Parse(dimensions[1]);
                // MainFormのWidthとHeightを設定
                retSize = new Size(width, height);
            }
            catch (Exception ex)
            {
                Debugger.DebugPrint("ConvertStringToSize");
                Debugger.DebugPrint(ex.Message);
            }
            return retSize;
        }


        static public Point ConvertStringToPoint(string value)
        {
            Point ret = new Point();
            try
            {
                // （"100 , 100"を想定）
                var size = value;
                string[] dimensions = value.Split(',');
                int width = int.Parse(dimensions[0]);
                int height = int.Parse(dimensions[1]);
                // MainFormのWidthとHeightを設定
                ret = new Point(width, height);
            }
            catch (Exception ex)
            {
                Debugger.DebugPrint("ConvertStringToPoint");
                Debugger.DebugPrint(ex.Message);
            }
            return ret;
        }
    }
}
