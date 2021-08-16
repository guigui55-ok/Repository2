using ExcelCellsManager.ExcelCellsManager.Settings;
using SettingsManager;
using System;

namespace ExcelCellsManager.Settings
{
    public class ExcelCellsManagerSettingsValue
    {
        protected ErrorManager.ErrorManager _error;
        protected int _registMode;
        public ISettingsRegister Register;
        public SettingsManager.SettingsManager SettingsManager;
        protected Utility.TypeUtility _typeUtil;

        public CellsManagerSettingManager SettingsManager2;

        // アドレスを登録した後にエクセル(Workbook)ウィンドウをアクティブにする
        public bool _isActivateWorkbookWindowAfterRegistAddress;
        public bool IsActivateWorkbookWindowAfterRegistAddress
        {
            get { return _isActivateWorkbookWindowAfterRegistAddress; }
            set { 
                _isActivateWorkbookWindowAfterRegistAddress = value;                 
            }
        }
        // 起動時に前回のファイルを開く
        public bool IsOpenLastFileWhenRunApplication;
        // 前回最後に開いていたファイル
        public string LastOpendFilePath;
        // 画面端にマウスポインタを移動したときにフォームをアクティブにする
        public bool IsActivateFormWhenMoveMouseScreenEdge;
        // DataList のカラム幅を記憶する
        public bool IsSaveColumnWidthOfDataList;
        // Column Width
        public int[] ColumnWidthOfDataList;

        // flag=0/1 , UnderLimit , UpperLimit , ScreenNo
        // 画面端にマウスポインタを移動したときにフォームをアクティブにする設定の詳細
        public int[] MouseCaptureSettingLeft;
        public int[] MouseCaptureSettingTop;
        public int[] MouseCaptureSettingRight;
        public int[] MouseCaptureSettingBottom;
        // エクセルを開いた時、閉じた時に WorkbookList を更新する
        public bool IsUpdateListWhenWorkbookOpendAndClosed;

        // 前回の表示位置
        // ウィンドウサイズ

        public ExcelCellsManagerSettingsValue(ErrorManager.ErrorManager error,int registerMode)
        {
            try
            {
                IsActivateWorkbookWindowAfterRegistAddress = true;
                _error = error;
                _registMode = registerMode;
                if (_registMode == 1)
                {
                    Register = new SettingsRegisterToReg(_error);
                }
                else
                {
                    Register = new SettingsRegisterToIni(_error);
                }
                SettingsManager = new SettingsManager.SettingsManager(_error);
                _typeUtil = new Utility.TypeUtility(_error);
                //-----------
                SettingsManager2 = new CellsManagerSettingManager(_error);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Constracta");
            }
        }


        public void Initialize(string iniPath,string regRootPath)
        {
            try
            {
                if (_registMode == 1)
                {
                    Register.SetPath(regRootPath);
                }
                else
                {
                    Register.SetPath(iniPath);
                }
                Register.ReadSettings();
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Initialize");
            }
        }

        public void SettingsValueToThisMember()
        {
            try
            {
                _error.AddLog(this.ToString()+ ".SettingsValueToThisMember");
                // 設定値を ini or Registory から読み込みメンバ変数へ格納する
                string value="";
                foreach(ISettingsObject setting in SettingsManager.SettingsList)
                {
                    _error.AddLog("SettingsValueToThisMember: Name=" + setting.Name);
                    GetSettingsValue(setting, ref value);
                }

                SettingsValueToThisMemberMain();
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Initialize");
            }
        }

        public void GetSettingsValue(ISettingsObject setting,ref string value)
        {
            try
            {
                value = Register.GetValue(setting.SettingsTypeName, setting.Name);
                if (_error.HasException()) { return; }


                // 設定値がない場合は初期値を格納する
                if ((value == ""))
                {
                    string setValue;
                    setValue = _typeUtil.ConvertToString(setting.InitialValue);
                    Register.SetValue(setting.SettingsTypeName, setting.Name, setValue);
                    // クラス内変数に格納する
                    setting.Value = setting.InitialValue;
                    return;
                }

                // 設定値がある場合、値が有効か判定する
                // 判定方法は、初期値との型判定の比較で行う
                // 初期値
                object initVal = setting.InitialValue;
                // 設定から読み込んだ値
                object readsetVal = _typeUtil.ConvertToObject(setting.ValueType, value);
                // 設定から読み込んだ値と初期値の方を比較する
                bool isValid = false;
                if (readsetVal != null) // readSetVal 読み込んだ値が null なら無効扱い
                {
                    isValid = initVal.GetType() == readsetVal.GetType();
                }

                // 設定値が存在していても、それが無効な場合は初期値を格納する
                if (!isValid)
                {
                    string setValue;
                    setValue = _typeUtil.ConvertToString(setting.InitialValue);
                    Register.SetValue(setting.SettingsTypeName, setting.Name, setValue);
                } else
                {
                    // 値が有効な場合は、クラス内変数に格納する
                    setting.Value = _typeUtil.ConvertToObject(setting.ValueType,value);
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetSettingsValue");
            }
        }

        // 最後のファイルパスは個別で設定できるように
        // value => Member,SettingsObject
        public void SetLastFilePathToSettingsObject(string path)
        {
            try
            {
                // 起動時に前回のファイルを開く
                this.LastOpendFilePath = path;
                SettingsManager.SetValue("application", "LastOpendFilePath", path);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetSettingsValue");
            }
        }

        // SettingsManager SettingsObjectList -> this.Value 
        public void SettingsValueToThisMemberMain()
        {
            try
            {
                _error.AddLog(this.ToString() + ".SettingsValueToThisMemberMain");
                object buf;
                // アドレスを登録した後にエクセル(Workbook)ウィンドウをアクティブにする
                buf = SettingsManager.GetValue("application", "IsActivateWorkbookWindowAfterRegistAddress");
                bool bufbool = _typeUtil.ConvertToBoolean(buf);
                IsActivateWorkbookWindowAfterRegistAddress = bufbool;
                // 起動時に前回のファイルを開く
                buf = SettingsManager.GetValue("application", "IsOpenLastFileWhenRunApplication");
                bufbool = _typeUtil.ConvertToBoolean(buf);
                IsOpenLastFileWhenRunApplication = bufbool;
                // 前回最後に開いていたファイル
                buf = (string)SettingsManager.GetValue("application", "LastOpendFilePath");
                LastOpendFilePath = (string)buf;
                // 画面端にマウスポインタを移動したときにフォームをアクティブにする
                buf = SettingsManager.GetValue("application", "IsActivateFormWhenMoveMouseScreenEdge");
                bufbool = _typeUtil.ConvertToBoolean(buf);
                IsActivateFormWhenMoveMouseScreenEdge = bufbool;
                // DataList のカラム幅を記憶する
                buf = SettingsManager.GetValue("dataList", "IsSaveColumnWidthOfDataList");
                bufbool = _typeUtil.ConvertToBoolean(buf);
                IsSaveColumnWidthOfDataList = bufbool;
                // ColumnWidthOfDataList
                buf = SettingsManager.GetValue("dataList", "ColumnWidthOfDataList");
                int[] valintary = (int[])(object)buf;
                ColumnWidthOfDataList = valintary;
                // MouseCaptureSettingLeft
                buf = SettingsManager.GetValue("application", "MouseCaptureSettingLeft");
               valintary = (int[])(object)buf;
                MouseCaptureSettingLeft = valintary;
                // MouseCaptureSettingTop
                buf = SettingsManager.GetValue("application", "MouseCaptureSettingTop");
                valintary = (int[])(object)buf;
                MouseCaptureSettingTop = valintary;
                // MouseCaptureSettingRight
                buf = SettingsManager.GetValue("application", "MouseCaptureSettingRight");
                valintary = (int[])(object)buf;
                MouseCaptureSettingRight = valintary;
                // MouseCaptureSettingBottom
                buf = SettingsManager.GetValue("application", "MouseCaptureSettingBottom");
                valintary = (int[])(object)buf;
                MouseCaptureSettingBottom = valintary;
                // エクセルを開いた時、閉じた時に WorkbookList を更新する
                buf = SettingsManager.GetValue("application", "IsUpdateListWhenWorkbookOpendAndClosed");
                bufbool = _typeUtil.ConvertToBoolean(buf);
                IsUpdateListWhenWorkbookOpendAndClosed = bufbool;
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SettingsValueToThisMemberMain");
            }
        }

        // Member => SettingsIni/Register
        public void SetSettingsValueMemberToSettingsClass()
        {
            try
            {
                // アドレスを登録した後にエクセル(Workbook)ウィンドウをアクティブにする
                string buf = _typeUtil.ConvertToString(IsActivateWorkbookWindowAfterRegistAddress);
                Register.SetValue("application", "IsActivateWorkbookWindowAfterRegistAddress", buf);
                // 起動時に前回のファイルを開く
                buf = _typeUtil.ConvertToString(IsOpenLastFileWhenRunApplication);
                Register.SetValue("application", "IsOpenLastFileWhenRunApplication", buf);
                // 前回最後に開いていたファイル
                buf = LastOpendFilePath;
                Register.SetValue("application", "LastOpendFilePath", buf);
                // 画面端にマウスポインタを移動したときにフォームをアクティブにする
                buf = _typeUtil.ConvertToString(IsActivateFormWhenMoveMouseScreenEdge);
                Register.SetValue("application", "IsActivateFormWhenMoveMouseScreenEdge", buf);
                // DataList のカラム幅を記憶する
                buf = _typeUtil.ConvertToString(IsSaveColumnWidthOfDataList);
                Register.SetValue("dataList", "IsSaveColumnWidthOfDataList", buf);
                // ColumnWidthOfDataList
                buf = _typeUtil.ConvertToString(ColumnWidthOfDataList);
                Register.SetValue("dataList", "ColumnWidthOfDataList", buf);
                // MouseCaptureSettingLeft
                buf = _typeUtil.ConvertToString(MouseCaptureSettingLeft);
                Register.SetValue("application", "MouseCaptureSettingLeft", buf);
                // MouseCaptureSettingTop
                buf = _typeUtil.ConvertToString(MouseCaptureSettingTop);
                Register.SetValue("application", "MouseCaptureSettingTop", buf);
                // MouseCaptureSettingRight
                buf = _typeUtil.ConvertToString(MouseCaptureSettingRight);
                Register.SetValue("application", "MouseCaptureSettingRight", buf);
                // MouseCaptureSettingBottom
                buf = _typeUtil.ConvertToString(MouseCaptureSettingBottom);
                Register.SetValue("application", "MouseCaptureSettingBottom", buf);
                // エクセルを開いた時、閉じた時に WorkbookList を更新する
                buf = _typeUtil.ConvertToString(IsUpdateListWhenWorkbookOpendAndClosed);
                Register.SetValue("application", "IsUpdateListWhenWorkbookOpendAndClosed", buf);

            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetSettingsValueMemberToSettingsClass");
            }
        }
    }
}
