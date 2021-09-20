using ImageViewer.Events;
using ImageViewer.Functions;
using ImageViewer.ParentForms;
using ImageViewer.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ImageViewer
{
    public class MainControls
    {
        public ContentsControl ContentsControl;
        public ContentsControlFunction ContentsFunction;　//　外でNew
        // MainForm関連
        public Form MainForm;
        public MainFormManager MainFormManger;
        public MainFormFunction MainFormFunction;
        public MainKeyEvents MainKeyEvents;
        public SettingsFileFunction SettingsFileFunction;
    }
    public class ViewImageManager
    {
        readonly ErrorLog.IErrorLog _errorLog;


        public List<ViewImageObjects> ViewImageObjectList;
        public InvokeFunctionViewImageObjects Invoke;

        public MainControls MainControls; // まとめクラス
        public ViewImageBasicFunction BasicFunction; // 外でNewする
        public ImageViewerSettings Settings; // 外でNewする


        /// <summary>
        /// コンストラクタ
        /// <param name="errorlog">エラーログ</param>
        /// <param name="mainForm">メインフォーム</param>
        /// <param name="contentsControl">パネル</param>
        /// <param name="settings">設定</param>
        /// </summary>
        public ViewImageManager(ErrorLog.IErrorLog errorlog,Form mainForm,Panel contentsControl,ImageViewerSettings settings)
        {
            MainControls = new MainControls();
            _errorLog = errorlog;
            ViewImageObjectList = new List<ViewImageObjects>();
            Invoke = new InvokeFunctionViewImageObjects();

            Settings = settings;
            MainControls.MainForm = mainForm;
            MainControls.MainFormManger = new MainFormManager(errorlog,mainForm,settings,this);
            MainControls.ContentsControl = new ContentsControl(errorlog, contentsControl, mainForm, MainControls.MainFormManger);
            MainControls.MainFormManger.Initialize();

            MainControls.ContentsFunction = new ContentsControlFunction(errorlog, mainForm, MainControls.ContentsControl, this);

            MainControls.MainFormFunction = new MainFormFunction(errorlog, mainForm, MainControls.ContentsControl, this);
            MainControls.MainKeyEvents = new MainKeyEvents(errorlog, contentsControl, mainForm, this, settings);

            MainControls.SettingsFileFunction = new SettingsFileFunction(errorlog, 
                ImageViewerConstants.SETTINGS_INI, MainControls.MainFormManger);

        }


        public int Initialize()
        {
            try
            {
                // カレントディレクトリの親
                Settings.SettingsFormatFilePath = Directory.GetParent(Directory.GetCurrentDirectory().ToString()).ToString();
                // カレントディレクトリの親の親
                Settings.SettingsFormatFilePath = 
                    Directory.GetParent(Settings.SettingsFormatFilePath).ToString() + @"formatSettings.ini";
                // 設定ファイルを読み込む
                MainControls.SettingsFileFunction.ReadSettings();
                // 設定ファイルからオブジェクトへ
                SetSettingsFromFile();
                // 
                ApplySettings();
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " Initialize Failed");
                return 0;
            }
        }

        public int SetSettingsFromFile()
        {
            Common.Convert convert = new Common.Convert();
            try
            {
                string buf = MainControls.SettingsFileFunction.GetParameterValue("settings", "IsMenuBarVisibleAlways");
                Settings.IsMenuBarVisibleAlways = convert.StrintToBool(buf);

                buf = MainControls.SettingsFileFunction.GetParameterValue("settings", "IsMenuBarVisibleOnStartUp");
                Settings.IsMenuBarVisibleOnStartUp = convert.StrintToBool(buf);
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " Initialize Failed");
                return 0;
            }
            finally { }
        }

        public int ApplySettings()
        {
            try
            {
                this.MainControls.MainFormFunction.ChangeChecked_MenuStrip_MenuVisible_FromSettings();
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " Initialize Failed");
                return 0;
            }
        }

        public int Finlize()
        {
            try
            {
                int ret = MainControls.SettingsFileFunction.WriteSettingToFile();
                if (ret < 1)
                {
                    _errorLog.AddException(new Exception("WriteSettingToFile Failed"), this.ToString() + ".Finlize");
                }

                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " Initialize Failed");
                return 0;
            }
        }

        public ViewImageObjects MakeViewImageObjects(Form mainForm,Panel contentsPanel)
        {
            try
            {
                int n = 0;
                // 番号で管理するため、Newするときはこれで作る
                if (ViewImageObjectList == null)
                {
                    ViewImageObjectList = new List<ViewImageObjects>();
                    return new ViewImageObjects(_errorLog, 0, this,mainForm, contentsPanel, new Panel(), new Panel(), new PictureBox());
                }
                if (ViewImageObjectList.Count < 1)
                {
                    return new ViewImageObjects(_errorLog, 0,this, mainForm, contentsPanel, new Panel(), new Panel(), new PictureBox());
                } else
                {
                    n = ViewImageObjectList.Count;
                    return new ViewImageObjects(_errorLog, n,this, mainForm, contentsPanel, new Panel(), new Panel(), new PictureBox());
                }
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " MakeViewImageObjects Failed");
                return null;
            }
        }

        public void AddControlToMainContents(ViewImageObjects viObjects)
        {
            try
            {
                Panel contents = this.MainControls.ContentsControl.GetControl();
                Panel frame = (Panel)viObjects.Controls.ViewFrameControl.GetControl();
                Panel inner = (Panel)viObjects.Controls.ViewInnerControl.GetControl();
                PictureBox picturebox = (PictureBox)viObjects.Controls.ViewImageControl.GetControl();

                contents.Controls.Add(frame);
                frame.Controls.Add(inner);
                inner.Controls.Add(picturebox);

                if (this.ViewImageObjectList.Count == 1)
                {
                    frame.Size = MainControls.ContentsControl.GetSize();
                }

            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " AddControlToMainContents Failed");
            }
        }
        public List<ViewImageObjects> GetActiveControl()
        {
            try
            {
                List<ViewImageObjects> retlist = new List<ViewImageObjects>();
                if (ViewImageObjectList == null)
                { _errorLog.AddErrorNotException(this.ToString(), "getActiveControl list is null"); return null; }
                if (ViewImageObjectList.Count < 1)
                { _errorLog.AddErrorNotException(this.ToString(), "getActiveControl list count 0"); return null; }
                foreach (ViewImageObjects value in ViewImageObjectList)
                {
                    if (value.Controls.ViewFrameControl.State.IsActiveControl)
                    {
                        retlist.Add(value);
                    }
                }
                return retlist;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ViewImage");
                return null;
            }
        }

        // 指定したViewObjectに対して実行
        public void DoFunctionAll(Func<ViewImageObjects, int> func)
        {
            try
            {
                List<ViewImageObjects> list = this.ViewImageObjectList;
                // Listがnullまたはない
                if (((list == null) | (list.Count < 1)))
                { _errorLog.AddErrorNotException(this.ToString(), "DoFunctionAll List is Nothing"); return; }
                // 実行
                foreach (var viewImageObject in list)
                {
                    func(viewImageObject);
                }
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " DoFunctionAll");
            }
        }


        // GetActiveControlに対して実行
        public void DoFunction(Func<ViewImageObjects,int> func)
        {
            try
            {
                List<ViewImageObjects> list = this.GetActiveControl();
                if (!((list == null) | (list.Count < 1)))
                {
                    foreach (ViewImageObjects value in list)
                    {
                        func(value);
                    }
                }
                else
                {
                    _errorLog.AddErrorNotException(this.ToString() + " doFunction");
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " doFunction");
                return;
            }
        }
        public void TestFunction(string value)
        {
            MessageBox.Show(value);
        }

    }
}
