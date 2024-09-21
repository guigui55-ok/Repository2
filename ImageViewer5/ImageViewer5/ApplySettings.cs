using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;
using ImageViewer5.ImageControl;
using CommonModule;

namespace ImageViewer5
{
    public class ApplySettings
    {
        AppLogger _logger;
        ImageMainFrame _nowImageMainFrame;
        FormMain _formMain;
        public ApplySettings(AppLogger logger, FormMain formMain)
        {
            _logger = logger;
            _formMain = formMain;
            _nowImageMainFrame = _formMain.GetNowImageMainFrame();
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
                _logger.PrintInfo("## ApplySettings > ApplyArgs");
                Size winSize = imageViewerArgs.GetWindowSize();
                if (!winSize.Equals(new Size()))
                {
                    _logger.PrintInfo(string.Format("Args winSize = {0}", winSize));
                    _formMain.Size = winSize;
                }
                Point winLoc = imageViewerArgs.GetWindowLocation();
                if (!winLoc.Equals(new Point()))
                {
                    _logger.PrintInfo(string.Format("Args winLoc = {0}", winLoc));
                    _formMain.Location = winLoc;
                }
                ApplyArgsImageViewerFrame(imageViewerArgs);
            } catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".ApplyArgs");
            }
        }

        /// <summary>
        /// 読み込んだ引数・設定を適用させる
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
                    if (folderPath != "")
                    {
                        _logger.PrintInfo(string.Format("Args folderPath = {0}", folderPath));
                        //フォルダのパスを変更する
                        imageMainFrame._formFileList.SetFilesFromPath(folderPath, null, null);
                        //フォルダパスが存在しなければ処理はしない（未実装）
                    }
                    string sizeFrameStr = (string)GetDictValue(dict, SETTINGS_KEYS.FRAME_SIZE);
                    if (sizeFrameStr!="")
                    {
                        Size frameSize = imageViewerArgs.CnvWindowSize(dict, SETTINGS_KEYS.FRAME_SIZE);
                        _logger.PrintInfo(string.Format("Args frameSize = {0}", frameSize));
                        //Frameのサイズを変更する 処理を記載
                        imageMainFrame.Size = frameSize;
                    }


                    string frameLocationStr = (string)GetDictValue(dict, SETTINGS_KEYS.FRAME_LOC);
                    if (frameLocationStr != "")
                    {
                        Point frameLoc = imageViewerArgs.CnvWindowLocation(dict, SETTINGS_KEYS.FRAME_LOC);
                        _logger.PrintInfo(string.Format("Args frameLoc = {0}", frameLoc));
                        //FrameのLocationを変更する 処理を記載
                        imageMainFrame.Location = frameLoc;
                    }



                    //string readSubFolderStr = (string)GetDictValue(dict, SETTINGS_KEYS.SUBFOLDERS);
                    bool readSubFolderBool = (bool)GetDictValue(dict, SETTINGS_KEYS.SUBFOLDERS);
                    if (readSubFolderBool)
                    {
                        //bool readSubFolder = CommonGeneral.AnyToBool(readSubFolderStr);
                        _logger.PrintInfo(string.Format("Args readSubFolderBool = {0}", readSubFolderBool));
                        //サブフォルダーも読み込む 処理を記載
                        imageMainFrame._formFileList._fileListManagerSettingForm._fileListManagerSetting.ChangeIncludeSubFolder(readSubFolderBool);
                    }
                    //string randomrStr = (string)GetDictValue(dict, SETTINGS_KEYS.RANDOM);
                    bool randomBool = (bool)GetDictValue(dict, SETTINGS_KEYS.RANDOM);
                    if (randomBool)
                    {
                        //bool randomBool = CommonGeneral.AnyToBool(randomrStr);
                        _logger.PrintInfo(string.Format("Args randomBool = {0}", randomBool));
                        //リストをランダムにする 処理を記載
                        imageMainFrame._formFileList._fileListManagerSettingForm._fileListManagerSetting.ChangeListRandom(randomBool);
                    }
                    int slideShowIntervalInt = (int)GetDictValue(dict, SETTINGS_KEYS.INTERVAL);
                    if (slideShowIntervalInt >= 0)
                    {
                        //int.TryParse(slideShowIntervalStr,out slideShowIntervalInt);
                        _logger.PrintInfo(string.Format("Args slideShowIntervalInt = {0}", slideShowIntervalInt));
                        //スライドショーのインターバルを変更する 処理を記載
                        imageMainFrame._imageViewerMain._viewImageFunction._viewImageSlideShow._SlideShowTimer.Interval = slideShowIntervalInt;
                    }
                    bool slideShowBool = (bool )GetDictValue(dict, SETTINGS_KEYS.SLIDESHOW);
                    if (slideShowBool)
                    {
                        _logger.PrintInfo(string.Format("Args slideShowBool = {0}", slideShowBool));
                        //スライドショーをオンにする 処理を記載
                        //imageMainFrame._imageViewerMain._viewImageFunction._viewImageSlideShow.StartTimer();
                        imageMainFrame._imageViewerMain._viewImageFunction._viewImageSlideShow.ChangeOnOffByFlag(1);
                    }
                    int fileListWIndow = (int)GetDictValueInt(dict, SETTINGS_KEYS.FILE_LIST_WINDOW);
                    if (fileListWIndow >= 0)
                    {
                        _logger.PrintInfo(string.Format("Args fileListtWindow = {0}", fileListWIndow));
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
        private int GetDictValueBool(Dictionary<string, object> dict, string key)
        {
            try
            {
                return (int)dict[key];
            }
            catch (KeyNotFoundException ex)
            {
                Debugger.DebugPrint(ex.Message);
                return 0;
            }
        }
    }
}
