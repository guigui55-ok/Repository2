using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;
using CommonModulesProject;

namespace ImageViewer5.ImageControl
{
    public class ImageMainFrameSetting
    {
        public AppLogger _logger;
        ImageMainFrame _imageMainFrame;
        public SettingDictionary _settingDictionary;
        public ImageMainFrameSetting(AppLogger logger, ImageMainFrame imageMainFrame)
        {
            _logger = logger;
            _imageMainFrame = imageMainFrame;
            _settingDictionary = new SettingDictionary();
            _settingDictionary._name = "SettingDict_" + imageMainFrame.Name;
        }

        // ##############################
        // ## メインの設定値
        // ##############################
        // メインフォームにフィットさせる
        public bool _isFitFormMain = false;
        // メインフォームのウィンドウサイズ比と連動させる
        //Link the size ratio with the main form
        public bool _isLinkeControlSizeRadioWithFormMain = false;

        public bool _slideShowOn = false;
        public int _slideShowInterval = 1750;

        // ##############################
        /*
         * 
         */

        /// <summary>
        /// 設定値読み込み時に受け取る
        /// 　（適用はApplySettingsクラスで行う、引数の設定の方を優先する）
        /// </summary>
        /// <param name="formMainSetting"></param>
        public void SetSettingDatFromMain(FormMainSetting formMainSetting)
        {
            _logger.PrintInfo("ImageMainFrameSetting > SetSettingDatFromMain > " + _imageMainFrame.Name);
            _logger.PrintInfo("NotImplemented");

            var buf = GetSettingValueInitialize();
            _settingDictionary._settingDict = buf;

            //    // デフォルトに上書きしていく
            //string key = _imageMainFrame.Name;
            bool isFound = false;
            foreach(string key in formMainSetting._settingDictionary._settingDict.Keys)
            {
                if (key == _imageMainFrame.Name)
                {
                    object obj = formMainSetting._settingDictionary._settingDict[key];
                    _logger.PrintInfo(string.Format(" formMainSetting._settingDictionary._settingDict[key].GetType() = {0}", obj.GetType().ToString()));
                    //_settingDictionary._settingDict = (Dictionary<string,object>)obj;
                    _settingDictionary._settingDict = JsonStreamModule.JsonStream.ConvertJObjectToDict(obj);
                    isFound = true;
                    break;
                }
            }
            if (!(isFound))
            {
                _logger.PrintInfo(string.Format("# Not Fount Setting [{0}]", _imageMainFrame.Name));
                _settingDictionary._settingDict = GetSettingValueInitialize(_imageMainFrame.GetComponentNumber());
            }
            //System.Collections.Generic.KeyNotFoundException 指定されたキーはディレクトリ内に存在しませんでした。
            //_slideShowOn = (bool)_settingDictionary._settingDict[SettingKey.SLIDE_SHOW_ON];
            //_slideShowInterval = (int)_settingDictionary._settingDict[SettingKey.SLIDE_SHOW_INTERVAL];
        }



        public void SetSettingValueToThisFromNowState()
        {
            //string key;
            object value;
            //以下の値はそのまま使う
            // SettingKey.RESTORE_PREV_FRAME
            //#
            value = _imageMainFrame.Name;
            _settingDictionary.AddValue(SettingKey.FRAME_NAME, value);
            //#
            value = _imageMainFrame._formFileList._fileListManager._filesRegister.DirectoryPath;
            _settingDictionary.AddValue(SettingKey.RESTORE_PREV_DIR, value);
            //#
            value = _imageMainFrame.Size;
            _settingDictionary.AddValue(SettingKey.FRAME_SIZE, value);
            //#
            value = _imageMainFrame.Location;
            _settingDictionary.AddValue(SettingKey.FRAME_LOC, value);
            //#
            value = _imageMainFrame._imageViewerMain._viewImageControl.GetSize();
            _settingDictionary.AddValue(SettingKey.IMAGE_CONTROL_SIZE, value);
            //#
            value = _imageMainFrame._imageViewerMain._viewImageControl.GetLocation();
            _settingDictionary.AddValue(SettingKey.IMAGE_CONTROL_LOC, value);
            //#
            value = _imageMainFrame._formFileList._fileListManager._files.GetCurrentValue();
            _settingDictionary.AddValue(SettingKey.CURRENT_VALUE, value);
            //#
            value = _imageMainFrame._formFileList._fileListManager._files.GetCurrentIndex();
            _settingDictionary.AddValue(SettingKey.CURRENT_INDEX, value);
            //#
            value = _imageMainFrame._formFileList._fileListManager._fileListManagerSetting._isListRandom;
            _settingDictionary.AddValue(SettingKey.FILE_LIST_RANDOM, value);
            //#
            value = this._slideShowOn;
            _settingDictionary.AddValue(SettingKey.SLIDE_SHOW_ON, value);
            //#
            value = this._slideShowInterval;
            _settingDictionary.AddValue(SettingKey.SLIDE_SHOW_INTERVAL, value);
            //#
            value = _settingDictionary.GetValueBool(SettingKey.FIT_IMAGE_TO_FRAME);
            _settingDictionary.AddValue(SettingKey.FIT_IMAGE_TO_FRAME, value);
            //#
            value = this._isFitFormMain;
            _settingDictionary.AddValue(SettingKey.LINK_SIZE_WITH_FORM, value);
            //#
            value = _settingDictionary.GetValueBool(SettingKey.LINK_LOC_WITH_FORM);
            _settingDictionary.AddValue(SettingKey.LINK_LOC_WITH_FORM, value);
            //#
            value = _settingDictionary.GetValueBool(SettingKey.SHOW_LIST_SUB_WINDOW);
            _settingDictionary.AddValue(SettingKey.SHOW_LIST_SUB_WINDOW, value);
            //#
            //以下の値はそのまま使う
            // INCLUDE_SUB_DIR_FILE
            // FILE_LIST_RANDOM
            // SLIDE_SHOW_ON
            // FIT_IMAGE_TO_FRAME
            // LINK_SIZE_WITH_FORM
            // LINK_LOC_WITH_FORM
        }

        //public void ResetSettingValue()
        //{
        //    _logger.PrintInfo("ImageMainFrameSetting > ResetSettingValue > " + _imageMainFrame.Name);
        //    var buf = GetSettingValueInitialize();
        //    _settingDictionary._settingDict = buf;

        //    // デフォルトに上書きしていく
        //}




        static public Dictionary<string, object> GetSettingValueInitialize(int frameNum = 1)
        {
            Dictionary<string, object> bufDict = new Dictionary<string, object>();
            string key;
            object value;
            //KeyValuePair<string, object> bufItem;
            //bufDict = new KeyValuePair<string, object>(SettingKey.RESTORE_PREV_FRAME, SettingValueDefault.RESTORE_PREV_FRAME);
            //#
            value = SettingValueDefault.FRAME_NAME + frameNum.ToString();
            bufDict.Add(SettingKey.FRAME_NAME, value);
            //#
            value = SettingValueDefault.RESTORE_PREV_FRAME;
            bufDict.Add(SettingKey.RESTORE_PREV_FRAME, value);
            //#
            value = Directory.GetCurrentDirectory();
            bufDict.Add(SettingKey.RESTORE_PREV_DIR, value);
            //#
            value = SettingValueDefault.FRAME_SIZE;
            bufDict.Add(SettingKey.FRAME_SIZE, value);
            //#
            value = SettingValueDefault.FRAME_LOC;
            bufDict.Add(SettingKey.FRAME_LOC, value);
            //#
            value = SettingValueDefault.IMAGE_CONTROL_SIZE;
            bufDict.Add(SettingKey.IMAGE_CONTROL_SIZE, value);
            //#
            value = SettingValueDefault.IMAGE_CONTROL_LOC;
            bufDict.Add(SettingKey.IMAGE_CONTROL_LOC, value);
            //#
            value = SettingValueDefault.CURRENT_VALUE;
            bufDict.Add(SettingKey.CURRENT_VALUE, value);
            //#
            value = SettingValueDefault.CURRENT_INDEX;
            bufDict.Add(SettingKey.CURRENT_INDEX, value);
            //#
            value = SettingValueDefault.INCLUDE_SUB_DIR_FILE;
            bufDict.Add(SettingKey.INCLUDE_SUB_DIR_FILE, value);
            //#
            value = SettingValueDefault.FILE_LIST_RANDOM;
            bufDict.Add(SettingKey.FILE_LIST_RANDOM, value);
            //#
            value = SettingValueDefault.SLIDE_SHOW_ON;
            bufDict.Add(SettingKey.SLIDE_SHOW_ON, value);
            //#
            value = SettingValueDefault.SLIDE_SHOW_INTERVAL;
            bufDict.Add(SettingKey.SLIDE_SHOW_INTERVAL, value);
            //#
            value = SettingValueDefault.FIT_IMAGE_TO_FRAME;
            bufDict.Add(SettingKey.FIT_IMAGE_TO_FRAME, value);
            //#
            value = SettingValueDefault.LINK_SIZE_WITH_FORM;
            bufDict.Add(SettingKey.LINK_SIZE_WITH_FORM, value);
            //#
            value = SettingValueDefault.LINK_LOC_WITH_FORM;
            bufDict.Add(SettingKey.LINK_LOC_WITH_FORM, value);
            //#
            value = SettingValueDefault.SHOW_LIST_SUB_WINDOW;
            bufDict.Add(SettingKey.SHOW_LIST_SUB_WINDOW, value);
            return bufDict;
        }


    }

}
