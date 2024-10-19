using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;
using ImageViewer5.ImageControl;
using CommonModuleImageViewer;
using CommonModulesProject;

namespace ImageViewer5
{
    public class ApplySettings
    {
        // 外部からの異常
        AppLogger _logger;
        ImageMainFrame _nowImageMainFrame;
        FormMain _formMain;
        // クラス内で生成
        // なし
        public ApplySettings(AppLogger logger, FormMain formMain)
        {
            _logger = logger;
            _formMain = formMain;
            _nowImageMainFrame = _formMain.GetNowImageMainFrame();
        }

        public void DisposeObjects()
        {
            try
            {
                //とくになし
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }


        /// <summary>
        /// 読み込んだ引数値・設定を適用させる (Setting.json用)
        /// <para></para>
        /// ImageMainFrameのフォルダパス設定は、Form_Loadのfinallyで行われるので
        /// それ以降にこのメソッドを実行するとよい
        /// または、影響ない範囲で適用させて、フォルダのみ後で適用させる
        /// </summary>
        /// <param name="imageViewerArgs"></param>
        public void ApplyArgsForSetting(SettingDictionary mainSettingObj)
        {
            try
            {
                object value;
                _logger.PrintInfo("## ApplySettings > ApplyArgsForSetting");
                value = (string)mainSettingObj._settingDict[SettingKey.MAIN_FORM_SIZE];
                Size winSize = FormMainSetting.ConvertStringToSize((string)value);
                if (!winSize.Equals(new Size()))
                {
                    _logger.PrintInfo(string.Format("Args winSize = {0}", winSize));
                    _formMain.Size = winSize;
                }
                //Point winLoc = imageViewerArgs.GetWindowLocation();
                value = (string)mainSettingObj._settingDict[SettingKey.MAIN_FORM_LOC];
                Point winLoc = FormMainSetting.ConvertStringToPoint((string)value);
                if (!winLoc.Equals(new Point()))
                {
                    _logger.PrintInfo(string.Format("Args winLoc = {0}", winLoc));
                    _formMain.Location = winLoc;
                }
                //#
                foreach(ImageMainFrame frame in _formMain._mainFrameManager._imageMainFrameList)
                {
                    ApplyArgsImageViewerFrameByDict(mainSettingObj, frame.GetComponentNumber());
                }
                //#
                value = (bool)_formMain._formMainSetting._settingDictionary._settingDict[SettingKey.SHOW_SENDER_DIALOG];
                bool isShowSenderDialog = (bool)value;
                if (isShowSenderDialog)
                {
                    _logger.PrintInfo(string.Format("Args isShowSenderDialog = {0}", isShowSenderDialog));
                    _formMain._fileSenderFunction._fileSenderApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".ApplyArgs");
            }
        }


        /// <summary>
        /// 読み込んだ引数・設定を適用させる (ImageViewerFrame)
        /// （Args引数の処理とは異なるので注意）
        /// </summary>
        /// <param name="imageViewerArgs"></param>
        public void ApplyArgsImageViewerFrameByDict(SettingDictionary mainSettingObj, int frameNumber)
        {
            try
            {
                _logger.PrintInfo("ApplyArgsImageViewerFrame > ApplyArgsImageViewerFrameByDict");
                string frameName = ImageViewerConstants.FRAME_NAME_BASE + frameNumber;
                _logger.PrintInfo(string.Format("frameNumber = {0}", frameNumber));
                //Dictionary<string, object> settingDict = (Dictionary<string,object>)mainSettingObj._settingDict[frameName];
                Dictionary<string, object> settingDict = JsonStreamModule.JsonStream.ConvertJObjectToDict(mainSettingObj._settingDict[frameName]);

                _logger.PrintInfo(string.Format("----- frameNumber = {0}", frameNumber));
                Dictionary<string, object> dict = settingDict;
                var dictStr = String.Join(",", dict.Select(kvp => kvp.Key + " : " + kvp.Value));
                _logger.PrintInfo(string.Format("dictStr = {0}", dictStr));
                //
                if (_formMain._mainFrameManager._imageMainFrameList.Count < frameNumber)
                {
                    _logger.PrintError(this.ToString() + ".ApplyArgsImageViewerFrame  # OutOfRange");
                    string msg = string.Format("_formMain._imageMainFrameList.Count[{0}]", _formMain._mainFrameManager._imageMainFrameList.Count);
                    msg += String.Format(" < frameNumber[{0}]", frameNumber);
                    _logger.PrintInfo(msg);
                    //msg = string.Format(" (imageViewerArgs_frameSettingsList.Count = {0})", imageViewerArgs._frameSettingsList.Count);
                    //_logger.PrintInfo(msg);
                    return;
                }
                ImageMainFrame imageMainFrame = _formMain._mainFrameManager._imageMainFrameList[frameNumber - 1];
                //#

                bool restore = GetDictValueBool(dict, SettingKey.RESTORE_PREV_FRAME);
                _logger.PrintInfo(string.Format("Args restore = {0}", restore));
                if (!restore)
                {
                    return;
                }
                //#
                string folderPath = (string)GetDictValueStr(dict, SettingKey.RESTORE_PREV_DIR);
                if (folderPath != "")
                {
                    _logger.PrintInfo(string.Format("Args folderPath = {0}", folderPath));
                    //フォルダのパスを変更する
                    imageMainFrame._formFileList.SetFilesFromPath(folderPath, null, null);
                    //フォルダパスが存在しなければ処理はしない
                }
                //#
                string sizeFrameStr = (string)GetDictValue(dict, SettingKey.FRAME_SIZE);
                if (sizeFrameStr != "")
                {
                    Size frameSize = FormMainSetting.ConvertStringToSize(sizeFrameStr);
                    _logger.PrintInfo(string.Format("Args frameSize = {0}", frameSize));
                    //Frameのサイズを変更する 処理を記載
                    imageMainFrame.Size = frameSize;
                }
                //#
                string frameLocationStr = (string)GetDictValue(dict, SettingKey.FRAME_LOC);
                if (frameLocationStr != "")
                {
                    Point frameLoc = FormMainSetting.ConvertStringToPoint(frameLocationStr);
                    _logger.PrintInfo(string.Format("Args frameLoc = {0}", frameLoc));
                    //FrameのLocationを変更する 処理を記載
                    imageMainFrame.Location = frameLoc;
                }
                //#
                bool readSubFolderBool = GetDictValueBool(dict, SettingKey.INCLUDE_SUB_DIR_FILE);
                if (readSubFolderBool)
                {
                    _logger.PrintInfo(string.Format("Args readSubFolderBool = {0}", readSubFolderBool));
                    //サブフォルダーも読み込む 処理を記載
                    imageMainFrame._formFileList._fileListManagerSettingForm._fileListManagerSetting.ChangeIncludeSubFolder(readSubFolderBool);
                }
                //#
                string currentPath = GetDictValueStr(dict, SettingKey.CURRENT_VALUE);
                if (currentPath != "")
                {
                    _logger.PrintInfo(string.Format("Args CURRENT_VALUE = {0}", currentPath));
                    //パス設定 処理を記載
                    imageMainFrame._formFileList._fileListManager._files.Move(currentPath);
                }
                //#
                bool randomBool = (bool)GetDictValue(dict, SettingKey.FILE_LIST_RANDOM);
                if (randomBool)
                {
                    _logger.PrintInfo(string.Format("Args randomBool = {0}", randomBool));
                    //リストをランダムにする 処理を記載
                    imageMainFrame._formFileList._fileListManagerSettingForm._fileListManagerSetting.ChangeListRandom(randomBool);
                }
                //#
                object value = GetDictValue(dict, SettingKey.SLIDE_SHOW_INTERVAL);
                //int slideShowIntervalInt = (int)GetDictValue(dict, SettingKey.SLIDE_SHOW_INTERVAL);
                int slideShowIntervalInt = Convert.ToInt32(value.ToString());
                if (slideShowIntervalInt >= 0)
                {
                    _logger.PrintInfo(string.Format("Args slideShowIntervalInt = {0}", slideShowIntervalInt));
                    //スライドショーのインターバルを変更する 処理を記載
                    imageMainFrame._imageViewerMain._viewImageFunction._viewImageSlideShow._SlideShowTimer.Interval = slideShowIntervalInt;
                }
                //#
                bool slideShowBool = (bool)GetDictValue(dict, SettingKey.SLIDE_SHOW_ON);
                if (slideShowBool)
                {
                    _logger.PrintInfo(string.Format("Args slideShowBool = {0}", slideShowBool));
                    //スライドショーをオンにする 処理を記載
                    imageMainFrame._imageViewerMain._viewImageFunction._viewImageSlideShow.ChangeOnOffByFlag(1);
                }
                //#
                bool fileListWIndow = GetDictValueBool(dict, SettingKey.SHOW_LIST_SUB_WINDOW);
                if (!(fileListWIndow))
                {
                    _logger.PrintInfo(string.Format("Args fileListtWindow = {0}", fileListWIndow));
                    //fileListWIndowを表示する 処理を記載
                    imageMainFrame._formFileList.Visible = false;
                }
                _logger.PrintInfo(" * ApplyArgsImageViewerFrame > ApplyArgsImageViewerFrameByDict END");
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".ApplyArgs");
            }
        }


        /// <summary>
        /// 読み込んだ引数値・設定を適用させる
        /// <para></para>
        /// ImageMainFrameのフォルダパス設定は、Form_Loadのfinallyで行われるので
        /// それ以降にこのメソッドを実行するとよい
        /// または、影響ない範囲で適用させて、フォルダのみ後で適用させる
        /// </summary>
        /// <param name="imageViewerArgs"></param>
        public void ApplyArgs(ImageViewerArgs imageViewerArgs)
        {
            try
            {
                object value;
                _logger.PrintInfo("## ApplySettings > ApplyArgs");
                //Size winSize = imageViewerArgs.GetWindowSize();
                value = (string)_formMain._formMainSetting._settingDictionary._settingDict[SettingKey.MAIN_FORM_SIZE];
                Size winSize = FormMainSetting.ConvertStringToSize((string)value);
                if (!winSize.Equals(new Size()))
                {
                    _logger.PrintInfo(string.Format("Args winSize = {0}", winSize));
                    _formMain.Size = winSize;
                }
                //Point winLoc = imageViewerArgs.GetWindowLocation();
                value = (string)_formMain._formMainSetting._settingDictionary._settingDict[SettingKey.MAIN_FORM_LOC];
                Point winLoc = FormMainSetting.ConvertStringToPoint((string)value);
                if (!winLoc.Equals(new Point()))
                {
                    _logger.PrintInfo(string.Format("Args winLoc = {0}", winLoc));
                    _formMain.Location = winLoc;
                }
                //#
                ApplyArgsImageViewerFrame(imageViewerArgs);
                //#
                value = (bool)_formMain._formMainSetting._settingDictionary._settingDict[SettingKey.SHOW_SENDER_DIALOG];
                bool isShowSenderDialog = (bool)value;
                if (isShowSenderDialog)
                {
                    _logger.PrintInfo(string.Format("Args isShowSenderDialog = {0}", isShowSenderDialog));
                    _formMain._fileSenderFunction._fileSenderApp.Visible = true;
                }
            } catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".ApplyArgs");
            }
        }

        /// <summary>
        /// 読み込んだ引数・設定を適用させる (ImageViewerFrame)
        /// </summary>
        /// <param name="imageViewerArgs"></param>
        public void ApplyArgsImageViewerFrame(ImageViewerArgs imageViewerArgs)
        {
            try
            {
                //throw new Exception("ループの処理が重複している、　foreach imageViewerArgs._frameSettingsList が9回実行されている");
                _logger.PrintInfo("ApplyArgsImageViewerFrame > ApplyArgs");
                int count = 0;
                foreach (Dictionary<string, object> dict in imageViewerArgs._frameSettingsList)
                {
                    _logger.PrintInfo(string.Format("count = {0}", count));
                    int frameNumber = (int)GetDictValue(dict, SETTINGS_KEYS.FRAME_NUMBER);
                    _logger.PrintInfo(string.Format("----- frameNumber = {0}", frameNumber));
                    var dictStr = String.Join(",", dict.Select(kvp => kvp.Key + " : " + kvp.Value));
                    _logger.PrintInfo(string.Format("dictStr = {0}", dictStr));
                    //
                    SettingDictionary bufSettingObj = imageViewerArgs.GetSettingDictionaryByNumber(frameNumber);
                    //
                    if (_formMain._mainFrameManager._imageMainFrameList.Count < frameNumber)
                    {
                        _logger.PrintError(this.ToString() + ".ApplyArgsImageViewerFrame  # OutOfRange");
                        string msg = string.Format("_formMain._imageMainFrameList.Count[{0}]", _formMain._mainFrameManager._imageMainFrameList.Count);
                        msg += String.Format(" < frameNumber[{0}]", frameNumber);
                        _logger.PrintInfo(msg);
                        msg = string.Format(" (imageViewerArgs_frameSettingsList.Count = {0})", imageViewerArgs._frameSettingsList.Count);
                        _logger.PrintInfo(msg);
                        return;
                    }
                    ImageMainFrame imageMainFrame = _formMain._mainFrameManager._imageMainFrameList[frameNumber-1];
                    string folderPath = (string)GetDictValue(dict, SETTINGS_KEYS.FOLDER);
                    folderPath = bufSettingObj.GetValueString(SettingKey.RESTORE_PREV_DIR);
                    if (folderPath != "")
                    {
                        _logger.PrintInfo(string.Format("Args folderPath = {0}", folderPath));
                        //フォルダのパスを変更する
                        imageMainFrame._formFileList.SetFilesFromPath(folderPath, null, null);
                        //フォルダパスが存在しなければ処理はしない（未実装）
                    }
                    string sizeFrameStr = (string)GetDictValue(dict, SETTINGS_KEYS.FRAME_SIZE);
                    sizeFrameStr = bufSettingObj.GetValueString(SettingKey.FRAME_SIZE);
                    if (sizeFrameStr!="")
                    {
                        //Size frameSize = imageViewerArgs.CnvWindowSize(dict, SETTINGS_KEYS.FRAME_SIZE);
                        Size frameSize = FormMainSetting.ConvertStringToSize(sizeFrameStr);
                        _logger.PrintInfo(string.Format("Args frameSize = {0}", frameSize));
                        //Frameのサイズを変更する 処理を記載
                        imageMainFrame.Size = frameSize;
                    }

                    string frameLocationStr = (string)GetDictValue(dict, SETTINGS_KEYS.FRAME_LOC);
                    frameLocationStr = bufSettingObj.GetValueString(SettingKey.FRAME_LOC);
                    if (frameLocationStr != "")
                    {
                        //Point frameLoc = imageViewerArgs.CnvWindowLocation(dict, SETTINGS_KEYS.FRAME_LOC);
                        Point frameLoc = FormMainSetting.ConvertStringToPoint(frameLocationStr);
                        _logger.PrintInfo(string.Format("Args frameLoc = {0}", frameLoc));
                        //FrameのLocationを変更する 処理を記載
                        imageMainFrame.Location = frameLoc;
                    }



                    //string readSubFolderStr = (string)GetDictValue(dict, SETTINGS_KEYS.SUBFOLDERS);
                    bool readSubFolderBool = (bool)GetDictValue(dict, SETTINGS_KEYS.SUBFOLDERS);
                    readSubFolderBool = bufSettingObj.GetValueBool(SettingKey.INCLUDE_SUB_DIR_FILE);
                    if (readSubFolderBool)
                    {
                        //bool readSubFolder = CommonGeneral.AnyToBool(readSubFolderStr);
                        _logger.PrintInfo(string.Format("Args readSubFolderBool = {0}", readSubFolderBool));
                        //サブフォルダーも読み込む 処理を記載
                        imageMainFrame._formFileList._fileListManagerSettingForm._fileListManagerSetting.ChangeIncludeSubFolder(readSubFolderBool);
                    }

                    string currentPath = GetDictValueStr(dict, SettingKey.CURRENT_VALUE);
                    if (currentPath!="")
                    {
                        _logger.PrintInfo(string.Format("Args CURRENT_VALUE = {0}", currentPath));
                        //パス設定 処理を記載
                        imageMainFrame._formFileList._fileListManager._files.Move(currentPath);
                    }

                    //string randomrStr = (string)GetDictValue(dict, SETTINGS_KEYS.RANDOM);
                    bool randomBool = (bool)GetDictValue(dict, SETTINGS_KEYS.RANDOM);
                    randomBool = bufSettingObj.GetValueBool(SettingKey.FILE_LIST_RANDOM);
                    if (randomBool)
                    {
                        //bool randomBool = CommonGeneral.AnyToBool(randomrStr);
                        _logger.PrintInfo(string.Format("Args randomBool = {0}", randomBool));
                        //リストをランダムにする 処理を記載
                        imageMainFrame._formFileList._fileListManagerSettingForm._fileListManagerSetting.ChangeListRandom(randomBool);
                    }
                    int slideShowIntervalInt = (int)GetDictValue(dict, SETTINGS_KEYS.INTERVAL);
                    slideShowIntervalInt = bufSettingObj.GetValueInt(SettingKey.SLIDE_SHOW_INTERVAL);
                    if (slideShowIntervalInt >= 0)
                    {
                        //int.TryParse(slideShowIntervalStr,out slideShowIntervalInt);
                        _logger.PrintInfo(string.Format("Args slideShowIntervalInt = {0}", slideShowIntervalInt));
                        //スライドショーのインターバルを変更する 処理を記載
                        imageMainFrame._imageViewerMain._viewImageFunction._viewImageSlideShow._SlideShowTimer.Interval = slideShowIntervalInt;
                    }
                    bool slideShowBool = (bool )GetDictValue(dict, SETTINGS_KEYS.SLIDESHOW);
                    slideShowBool = bufSettingObj.GetValueBool(SettingKey.SLIDE_SHOW_ON);
                    if (slideShowBool)
                    {
                        _logger.PrintInfo(string.Format("Args slideShowBool = {0}", slideShowBool));
                        //スライドショーをオンにする 処理を記載
                        //imageMainFrame._imageViewerMain._viewImageFunction._viewImageSlideShow.StartTimer();
                        imageMainFrame._imageViewerMain._viewImageFunction._viewImageSlideShow.ChangeOnOffByFlag(1);
                    }
                    int fileListWIndow = (int)GetDictValueInt(dict, SETTINGS_KEYS.FILE_LIST_WINDOW);
                    bool fileListWindowbool = bufSettingObj.GetValueBool(SettingKey.SHOW_LIST_SUB_WINDOW);
                    if (!(fileListWindowbool))
                    {
                        _logger.PrintInfo(string.Format("Args fileListtWindow = {0}", fileListWIndow));
                        _logger.PrintInfo(string.Format("Args fileListWindowbool = {0}", fileListWindowbool));
                        //fileListWIndowを表示する 処理を記載
                        imageMainFrame._formFileList.Visible = false;
                    }
                }
            } catch(Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".ApplyArgs");
            }
        }

        private object GetDictValue(Dictionary<string, object> dict, string key)
        {
            try
            {
                return dict[key];
            } catch(KeyNotFoundException ex)
            {
                Debugger.DebugPrint(ex.Message);
                return null;
            }
        }
        private string GetDictValueStr(Dictionary<string, object> dict, string key)
        {
            try
            {
                return (string)dict[key];
            }
            catch (KeyNotFoundException ex)
            {
                Debugger.DebugPrint(ex.Message);
                return "";
            }
        }
        private int GetDictValueInt(Dictionary<string, object> dict, string key)
        {
            try
            {
                return (int)dict[key];
            } catch (KeyNotFoundException ex)
            {
                Debugger.DebugPrint(ex.Message);
                return 0;
            }
        }
        private bool GetDictValueBool(Dictionary<string, object> dict, string key)
        {
            try
            {
                return (bool)dict[key];
            }
            catch (KeyNotFoundException ex)
            {
                Debugger.DebugPrint(ex.Message);
                return false;
            }
        }
    }
}
