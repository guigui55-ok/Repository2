using SettingsManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace ExcelCellsManager.ExcelCellsManager.SettingsForm
{
    public class EcmSettingsFormManager
    {
        protected ErrorManager.ErrorManager _error;
        public EcmSettingsForm SettingsForm;
        public List<ISettingsObject> SettingsList;
        protected List<Control> BaseControls;
        protected Form _parentForm;
        public int SetttingsPanelWidth;
        public int TabLocationY;
        public bool IsFirstShow = true;
        public EventHandler ButtonApply_ClickEvent;
        public EcmSettingsFormManager(ErrorManager.ErrorManager error,Form parentForm)
        {
            try
            {
                _error = error;
                SettingsForm = new EcmSettingsForm();
                BaseControls = new List<Control>();
                _parentForm = parentForm;
                Initialize();
                // Button_SettingsApply Event Set
                FormUtility formUtil = new FormUtility(_error);
                Control con = formUtil.GetControlFirstMatchNameFromControlIncludeChild(SettingsForm, "Button_SettingsApply");
                Button applyButton;
                if (con.GetType() == typeof(Button))
                {
                    applyButton = (Button)con;
                    applyButton.Click += ApplyButton_Click;
                    //applyButton.Click += ButtonApply_ClickEvent;
                }

                SettingsForm.VisibleChanged += SettingsForm_VisibleChanged;
                
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".EcmSettingsFormManager Constracta");
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            ButtonApply_ClickEvent?.Invoke(sender, e);
        }

        // 設定ダイアログを開く時のイベント
        private void SettingsForm_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (SettingsForm.Visible == false)
                {
                    // SettingsObject から値を取得しコントロールへセットする
                    SetValueToSettingsObjectMemberToControl();
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SettingsForm_VisibleChanged");
            }
        }
        // 設定ダイアログを開く
        public void ShowForm(bool visible)
        {
            try
            {

                if (!((SettingsForm == null)||(SettingsForm.IsDisposed)))
                {
                    if (visible)
                    {
                        SetValueToControlFromSettingsObjectMember();
                        if (IsFirstShow)
                        {
                            SettingsForm.ShowDialog(_parentForm);
                            IsFirstShow = false;
                        } else
                        {
                            SettingsForm.Visible = visible;
                        }
                    }
                } else
                {
                    SettingsForm = new EcmSettingsForm();
                    Initialize();
                    PlaceControlSettingsListToForm(0);
                    SettingsForm.ShowDialog(_parentForm);
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ShowForm");
            }
        }
        // 設定値を Member 変数からコントロールへ反映させる
        private void SetValueToSettingsObjectMemberToControl()
        {
            try
            {
                _error.AddLog(this.ToString() + ".SetValueToSettingsObjectMemberToControl");
                if (SettingsList == null) { _error.AddLog("  SettingsList == null"); return; }
                if (SettingsList.Count < 1) { _error.AddLog("  SettingsList.Count < 1"); return; }
                foreach (ISettingsObject item in SettingsList)
                {
                    item.SetValueToMemberFromControl();
                    if (_error.HasException()) { _error.AddLog(_error.GetExceptionMessageAndStackTrace()); }
                }
            }
            catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValueToSettingsObjectMemberToControl");
            }
        }
        // 設定値をコントロールから SettingsObject のメンバ変数へ格納する
        private void SetValueToControlFromSettingsObjectMember()
        {
            try
            {
                _error.AddLog(this.ToString()+ ".SetValueToControlFromSettingsObjectMember");
                if (SettingsList == null) { _error.AddLog("  SettingsList == null"); return; }
                if (SettingsList.Count < 1) { _error.AddLog("  SettingsList.Count < 1"); return; }
                foreach(ISettingsObject item in SettingsList)
                {
                    item.SetValueToControlFromMember();
                    if (_error.HasException()) { _error.AddLog(_error.GetExceptionMessageAndStackTrace()); }
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValueToControlFromSettingsObjectMember");
            }
        }
        // 初期化する
        public void Initialize()
        {
            try
            {
                // サイズを変更できないようにする
                SettingsForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                // 最大、最小サイズを固定
                SettingsForm.MaximumSize = SettingsForm.Size;
                SettingsForm.MinimumSize = SettingsForm.Size;

                FormUtility formUtil = new FormUtility(_error);
                ///////////////////////////////////
                SettingsForm.Text = "Settings";
                // とりあえず Form > TabControl > TabPage > Panel という構成が前提とする
                TabControl tab = (TabControl)formUtil.GetControlFromForm(SettingsForm, typeof(TabControl));
                if (tab == null) { _error.AddLog("SettingsForm TabControl Is Null"); }

                TabPage tabpage = (TabPage)formUtil.GetControlFromControl(tab, typeof(TabPage));
                tabpage.Text = "General";
                //tabpage.AutoScroll = true;
                // TabControl 内の Panel を取得する
                Panel panel = (Panel)formUtil.GetControlFromControl(tabpage, typeof(Panel));
                panel.Padding = new Padding(5);
                panel.AutoScroll = true;
                //
                panel.HorizontalScroll.Maximum = 0; 
                panel.AutoScroll = false;
                panel.VerticalScroll.Visible = false;
                panel.AutoScroll = false;

                panel.Width = tabpage.Width - tabpage.Padding.Left - tabpage.Padding.Right;

                SetttingsPanelWidth = panel.Width;
                TabLocationY = tab.Location.Y;



                BaseControls.Add(panel);

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Initialize");
            }
        }
        // 設定ダイアログへコントロールを配置する
        public void PlaceControlSettingsListToForm(int controlIndex)
        {
            try
            {
                _error.AddLog(this.ToString()+ ".PlaceControlSettingsListToForm");
                Control targetControl = this.BaseControls[controlIndex];
                if (SettingsList == null) { _error.AddLogAlert(" SettingsList is null"); return; }
                if (SettingsList.Count < 1) { _error.AddLogAlert(" SettingsList.Count < 1"); return; }

                int pheight = targetControl.Padding.All *2;
                foreach (ISettingsObject value in SettingsList)
                {
                    if (value != null)
                    {
                        //if (value.GetType() == typeof(Control))
                        if (value != null)
                        {
                            targetControl.Controls.Add((Control)value.Control);
                            pheight += ((Control)value.Control).Height;
                        } else
                        {
                            _error.AddLogAlert("  Settings ValueControl Type Invalid. name=" + value.Name 
                                + " ,type=" + value.Control.GetType().ToString()); ;
                        }
                    } else
                    {
                        _error.AddLogAlert(" Settings Value is null.");
                    }
                }

                ((Panel)targetControl).Height = pheight;
                _error.AddLog("  Panel Height = "+pheight);

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".PlaceControlSettingsListToForm");
            }
        }


    }
}
