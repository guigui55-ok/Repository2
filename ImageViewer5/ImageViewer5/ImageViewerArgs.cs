using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;
using CommonModulesProject;

namespace ImageViewer5
{

    /// <summary>
    /// 引数で受け取るときの設定値クラス
    /// 、（設定ファイルのKey名とは別となる）
    /// </summary>
    public static class SETTINGS_KEYS
    {
        // MainFormの設定キー
        public const string WINDOW_SIZE = "window-size";
        public const string WINDOW_LOCATION = "window-location";
        public const string FRAME_COUNT = "frame-count";

        // ImageViewFrameの設定キー
        public const string FRAME_NUMBER = "frame-number"; //番号記録用
        public const string FRAME_SETTINGS = "frame{0}-settings";
        public const string FOLDER = "folder";
        public const string SUBFOLDERS = "subfolders";
        public const string RANDOM = "random";
        public const string SLIDESHOW = "slideshow";
        public const string INTERVAL = "interval";

        public const string FILE_LIST_WINDOW = "file-list-window";
        public const string FRAME_SIZE = "frame-size";
        public const string FRAME_LOC = "frame-location";
    }

    public class ImageViewerArgs
    {
        AppLogger _logger;
        string[] _baseArgs;
        public Dictionary<string, object> _settings;
        public Dictionary<string, object> _settingDict; // 240926 新しいほうに置き換える（Settingクラス導入のため）　古いほう_settingsは廃止予定
        public List<Dictionary<string, object>> _frameSettingsList;
        //
        public List<SettingDictionary> _frameSettingObjectList;
        public ImageViewerArgs(AppLogger logger, string[] args)
        {
            _logger = logger;
            _baseArgs = args;
            _settings = new Dictionary<string, object> { };
            _settingDict = new Dictionary<string, object> { };
            _frameSettingsList = new List<Dictionary<string, object>> { };
            _frameSettingObjectList = new List<SettingDictionary>();
        }

        public void DisposeObjects()
        {
            try
            {
                _settings.Clear();
                _settings = null;
                _settingDict.Clear();
                _settingDict = null;
                if (0<_frameSettingsList.Count)
                {
                    for (int i = 0; i < _frameSettingsList.Count; i++)
                    {
                        _frameSettingsList[i].Clear();
                        _frameSettingsList[i] = null;
                    }
                }
                _frameSettingsList.Clear();
                _frameSettingsList = null;
                _frameSettingObjectList.Clear();
                _frameSettingObjectList = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        // ##########

        public SettingDictionary GetSettingDictionaryByNumber(int number)
        {
            foreach(SettingDictionary buf in _frameSettingObjectList)
            {
                string lastNumStr = GetLastNumberRegix(buf._name);
                if (lastNumStr == number.ToString())
                {
                    return buf;
                }
            }
            return null;
        }

        private string GetLastNumberRegix(string value)
        {
            // 正規表現で文字列の末尾の連続する数字を抽出
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(value, @"\d+$");

            // マッチする数字がなければ空文字を返す
            if (!match.Success)
            {
                return string.Empty;
            }

            // マッチした数字部分を文字列として返す
            return match.Value;
        }

        // ##########

        public void ReadArgs()
        {
            var size = (string)_settings["window-size"];
            string[] dimensions = size.Split('x');
        }


        /// <summary>
        /// 新しい設定値用 240926追加
        /// 　、引数の値をセットするときに使用する
        /// </summary>
        /// <param name="frameNum"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void SetArgsToFrameSettingList(int frameNum, string key, object value)
        {
            bool isExistsFrameSetting = false;
            SettingDictionary targetSetting = new SettingDictionary();

            foreach (SettingDictionary bufSetting in _frameSettingObjectList)
            {
                string name = (string)bufSetting._settingDict[SettingKey.FRAME_NAME];
                if (name.EndsWith(ImageViewerConstants.FRAME_NAME_BASE + frameNum))
                {
                    isExistsFrameSetting = true;
                    targetSetting = bufSetting;
                    break;
                }
            }
            if (!(isExistsFrameSetting))
            {
                targetSetting = new SettingDictionary("SettingDict_" + ImageViewerConstants.FRAME_NAME_BASE + frameNum);
                _frameSettingObjectList.Add(targetSetting);
            }
            targetSetting.AddValue(key, value);
        }


        /// <summary>
        /// 引数から値を取得する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Dictionary<string, object> ParseArguments(string[] args)
        {
            var settings = new Dictionary<string, object>();
            object value = "";
            try
            {
                _logger.PrintInfo("ImageViewerArgs.ParseArguments");
                foreach (string arg in args)
                {
                    if (arg.StartsWith("--" + SETTINGS_KEYS.WINDOW_SIZE + "="))
                    {
                        value = arg.Substring(("--" + SETTINGS_KEYS.WINDOW_SIZE + "=").Length);
                        settings[SETTINGS_KEYS.WINDOW_SIZE] = value;
                        _settingDict[SettingKey.MAIN_FORM_SIZE] = value;
                    }
                    else if (arg.StartsWith("--" + SETTINGS_KEYS.WINDOW_LOCATION + "="))
                    {
                        value = arg.Substring(("--" + SETTINGS_KEYS.WINDOW_LOCATION + "=").Length);
                        settings[SETTINGS_KEYS.WINDOW_LOCATION] = value;
                        _settingDict[SettingKey.MAIN_FORM_LOC] = value;
                    }
                    else if (arg.StartsWith("--" + SETTINGS_KEYS.FRAME_COUNT + "="))
                    {
                        value = int.Parse(arg.Substring(("--" + SETTINGS_KEYS.FRAME_COUNT + "=").Length));
                        settings[SETTINGS_KEYS.FRAME_COUNT] = value;
                        _settingDict[SettingKey.FRAME_COUNT] = value;
                    }
                    else if (arg.StartsWith("--frame"))
                    {
                        var frameNumber = GetFrameNumber(arg);
                        var frameKey = string.Format(SETTINGS_KEYS.FRAME_SETTINGS, frameNumber);
                        Dictionary<string, object> subSettings;
                        if (!settings.ContainsKey(frameKey))
                        {
                            subSettings = new Dictionary<string, object>();
                        }
                        else
                        {
                            subSettings = (Dictionary<string, object>)settings[frameKey];
                        }

                        //var frameSettings = (Dictionary<string, object>)settings[frameKey];

                        subSettings[SETTINGS_KEYS.FRAME_NUMBER] = frameNumber;
                        UpdateFrameSettingsDict(_frameSettingsList, frameNumber, SETTINGS_KEYS.FRAME_NUMBER, frameNumber);

                        if (arg.Contains("-folder="))
                        {
                            string buf = arg.Split('=')[1];
                            UpdateFrameSettingsDict(_frameSettingsList, frameNumber, SETTINGS_KEYS.FOLDER, buf);
                            subSettings[SETTINGS_KEYS.FOLDER] = buf;
                            SetArgsToFrameSettingList(frameNumber, SettingKey.RESTORE_PREV_DIR, buf);
                        }
                        if (arg.Contains("-size="))
                        {
                            string buf = arg.Split('=')[1];
                            UpdateFrameSettingsDict(_frameSettingsList, frameNumber, SETTINGS_KEYS.FRAME_SIZE, buf);
                            subSettings[SETTINGS_KEYS.FRAME_SIZE] = buf;
                            SetArgsToFrameSettingList(frameNumber, SettingKey.FRAME_SIZE, buf);
                        }
                        if (arg.Contains("-location="))
                        {
                            string buf = arg.Split('=')[1];
                            UpdateFrameSettingsDict(_frameSettingsList, frameNumber, SETTINGS_KEYS.FRAME_LOC, buf);
                            subSettings[SETTINGS_KEYS.FRAME_LOC] = buf;
                            SetArgsToFrameSettingList(frameNumber, SettingKey.FRAME_LOC, buf);
                        }
                        else if (arg.Contains("-subfolders="))
                        {
                            bool buf = bool.Parse(arg.Split('=')[1]); ;
                            UpdateFrameSettingsDict(_frameSettingsList, frameNumber, SETTINGS_KEYS.SUBFOLDERS, buf);
                            subSettings[SETTINGS_KEYS.SUBFOLDERS] = buf;
                            SetArgsToFrameSettingList(frameNumber, SettingKey.INCLUDE_SUB_DIR_FILE, buf);
                        }
                        else if (arg.Contains("-random="))
                        {
                            bool buf = bool.Parse(arg.Split('=')[1]); ;
                            UpdateFrameSettingsDict(_frameSettingsList, frameNumber, SETTINGS_KEYS.RANDOM, buf);
                            subSettings[SETTINGS_KEYS.RANDOM] = buf;
                            SetArgsToFrameSettingList(frameNumber, SettingKey.FILE_LIST_RANDOM, buf);
                        }
                        else if (arg.Contains("-slideshow="))
                        {
                            bool buf = bool.Parse(arg.Split('=')[1]); ;
                            UpdateFrameSettingsDict(_frameSettingsList, frameNumber, SETTINGS_KEYS.SLIDESHOW, buf);
                            subSettings[SETTINGS_KEYS.SLIDESHOW] = buf;
                            SetArgsToFrameSettingList(frameNumber, SettingKey.SLIDE_SHOW_INTERVAL, buf);
                        }
                        else if (arg.Contains("-interval="))
                        {
                            int buf = int.Parse(arg.Split('=')[1]); ;
                            UpdateFrameSettingsDict(_frameSettingsList, frameNumber, SETTINGS_KEYS.INTERVAL, buf);
                            subSettings[SETTINGS_KEYS.INTERVAL] = buf;
                            SetArgsToFrameSettingList(frameNumber, SettingKey.SLIDE_SHOW_INTERVAL, buf);
                        }
                        else if (arg.Contains("-listwindow="))
                        {
                            int buf = int.Parse(arg.Split('=')[1]); ;
                            UpdateFrameSettingsDict(_frameSettingsList, frameNumber, SETTINGS_KEYS.FILE_LIST_WINDOW, buf);
                            subSettings[SETTINGS_KEYS.FILE_LIST_WINDOW] = buf;
                            SetArgsToFrameSettingList(frameNumber, SettingKey.SHOW_LIST_SUB_WINDOW, buf);
                        }

                        //var dictStr = String.Join(",", frameSettings.Select(kvp => kvp.Key + " : " + kvp.Value));
                        //_logger.PrintInfo(string.Format("frameSettings　DictStr = {0}", dictStr));
                        //_frameSettingsList.Add(frameSettings);
                        //設定変更後元に戻す
                        settings[frameKey] = subSettings;
                        //if (!settings.ContainsKey(frameKey))
                        //{
                        //    subSettings = new Dictionary<string, object>();
                        //}
                        //else
                        //{
                        //    subSettings = (Dictionary<string, object>)settings[frameKey];
                        //}
                    }
                }
                _logger.PrintInfo("read args");
                //foreach(KeyValuePair<string, object> dict in settings)
                //{
                //    if (dict.Key.Contains("-setting"))
                //    {
                //        _frameSettingsList.Add((Dictionary<string, object>)dict.Value);
                //    }
                //}
                _settings = settings;
            } catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".ParseArguments");
            }
            return settings;
        }

        /// <summary>
        /// dictList[parentKey]があれば、そのindexのDictに{updateKey,updateValue}を入れる
        /// なければADDして追加する。
        /// numberは1番目から開始する
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="parentKey"></param>
        /// <param name="updateKey"></param>
        /// <param name="updateValue"></param>
        private void UpdateFrameSettingsDict(List<Dictionary<string, object>> dictList, int number, string updateKey, object updateValue)
        {
            if (dictList.Count< number)
            {
                dictList.Add(new Dictionary<string, object> { });
            }
            dictList[number-1][updateKey] = updateValue;
        }

        public Size GetWindowSize()
        {
            Size retSize = new Size();
            try
            {
                // MainFormのサイズや位置設定を引数から取得 （"100 , 100"を想定）
                if (_settings.ContainsKey(SETTINGS_KEYS.WINDOW_SIZE))
                {
                    var size = (string)_settings[SETTINGS_KEYS.WINDOW_SIZE];
                    //string[] dimensions = size.Split('x');
                    string[] dimensions = size.Split(',');
                    int width = int.Parse(dimensions[0]);
                    int height = int.Parse(dimensions[1]);
                    // MainFormのWidthとHeightを設定
                    retSize = new Size(width, height);
                }
            } catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".GetWindowSize");
            }
            return retSize;
        }

        public Size CnvWindowSize(Dictionary<string, object>setting, string key)
        {
            Size retSize = new Size();
            try
            {
                // MainFormのサイズや位置設定を引数から取得 （"100 , 100"を想定）
                if (setting.ContainsKey(key))
                {
                    var size = (string)setting[key];
                    string[] dimensions = size.Split(',');
                    int width = int.Parse(dimensions[0]);
                    int height = int.Parse(dimensions[1]);
                    // MainFormのWidthとHeightを設定
                    retSize = new Size(width, height);
                }
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".CnvWindowSize");
            }
            return retSize;
        }

        public Point GetWindowLocation()
        {
            Point retPoint = new Point();
            try
            {
                if (_settings.ContainsKey(SETTINGS_KEYS.WINDOW_LOCATION))
                {
                    var location = (string)_settings[SETTINGS_KEYS.WINDOW_LOCATION];
                    string[] coords = location.Split(',');
                    int x = int.Parse(coords[0]);
                    int y = int.Parse(coords[1]);
                    // MainFormのXとYを設定
                    retPoint = new Point(x, y);
                }
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".GetWindowLocation");
            }
            return retPoint;
        }

        public Point CnvWindowLocation(Dictionary<string, object>setting, string key)
        {
            Point retPoint = new Point();
            try
            {
                if (setting.ContainsKey(key))
                {
                    var location = (string)setting[key];
                    string[] coords = location.Split(',');
                    int x = int.Parse(coords[0]);
                    int y = int.Parse(coords[1]);
                    // MainFormのXとYを設定
                    retPoint = new Point(x, y);
                }
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".CnvWindowLocation");
            }
            return retPoint;
        }
        public Point GetFrameNumber()
        {
            Point retPoint = new Point();
            try
            {
                if (_settings.ContainsKey(SETTINGS_KEYS.FRAME_COUNT))
                {
                    int frameCount = (int)_settings[SETTINGS_KEYS.FRAME_COUNT];
                    // フレーム数を処理
                }
            }
            catch (Exception ex)
            {
                _logger.PrintError(ex, this.ToString() + ".GetFrameNumber");
            }
            return retPoint;
        }

        static int GetFrameNumber(string arg)
        {
            var frameNumber = 1;
            if (arg.Contains("frame"))
            {
                var start = arg.IndexOf("frame") + "frame".Length;
                var end = arg.IndexOf("-", start);
                frameNumber = int.Parse(arg.Substring(start, end - start));
            }
            return frameNumber;
        }
    }
}
