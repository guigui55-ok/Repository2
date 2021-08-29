using ImageViewer.Controls;
using ImageViewer.Events;
using ImageViewer.Function;
using ImageViewer.Values;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ImageViewer
{
    public class ViewImageObjects
    {
        protected ErrorLog.IErrorLog _errorLog;
        public FileList.FileListReader FileList;
        public FileList.FileListRegister FileRegister;

        public ViewImage ViewImage;
        public ViewImageControls Controls;
        public int Number;
        public CommonFunctions Functions;
        public ViewImageControlEvents ControlEvents;

        public Form MainForm;
        public Panel MainContentsControl;
        public readonly MainControls MainControls;

        // Panel2,Panel3,PictureBox
        public ViewImageObjects(ErrorLog.IErrorLog errorlog,int number,ViewImageManager viewImageManger,
            Form mainForm,Panel contentsControl, Panel frameControl,Panel innerControl,PictureBox viewControl)
        {
            try
            {
                _errorLog = errorlog;
                Number = number;
                // FileList
                FileList = new FileList.FileListReader(_errorLog,new List<string>());
                FileRegister = new FileList.FileListRegister(_errorLog);
                ViewImage = new ViewImage();
                // controls initialize
                frameControl.Name = "FrameControl" + Number;
                frameControl.Size = ImageViewerConstants.CONTROL_NEW_SIZE();
                innerControl.Name = "InnerControl" + Number;
                innerControl.Size = ImageViewerConstants.CONTROL_NEW_SIZE();
                viewControl.Name = "ViewControl" + Number;
                viewControl.Size = ImageViewerConstants.CONTROL_NEW_SIZE();
                Controls = new ViewImageControls(_errorLog, contentsControl,frameControl, innerControl,viewControl);
                // mainForm
                this.MainForm = mainForm;
                this.MainContentsControl = contentsControl;
                this.MainControls = viewImageManger.MainControls;

                Functions = new CommonFunctions(errorlog, viewImageManger, this);
                ControlEvents = new ViewImageControlEvents(errorlog, this);

            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ViewImageObjects failed");
            }
        }

        public void Initialize()
        {
            try
            {
                // Frame Raito Save
                Functions.ForControl.SaveRatioFromContentscControl();

                //MainContentsControl.SizeChanged += this.Controls.ViewFrameControl.SaveRaitoSizeAndPositionFromContentsControl;

            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "Initialize failed");
            }
        }

    }

    public class ViewImageControls
    {
        public ErrorLog.IErrorLog _errorLog;
        public IViewFrameControl ViewFrameControl;
        public IViewInnerControl ViewInnerControl;
        public IViewImageControl ViewImageControl;
        public IViewControlState State;
        public IViewImageSettings Settings;
        public IViewImage ViewImage;

        public ViewImageControls(ErrorLog.IErrorLog errorlog,
            Panel contentsControl, Panel frameControl, Panel innerControl, PictureBox viewControl)
        {
            try
            {
                frameControl.Size = contentsControl.Size;

                // DragAndDrop を許可
                viewControl.AllowDrop = true;

                // アンカー  左上に表示
                viewControl.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                // 画像にフィット
                viewControl.SizeMode = PictureBoxSizeMode.Zoom;
                // フォームの大きさに追随してピクチャボックスが伸縮するようになる
                viewControl.Dock = DockStyle.Fill;

                _errorLog = errorlog;
                State = new ViewControlState();
                Settings = new ViewImageSettings();
                ViewFrameControl = new ViewFrameControl(_errorLog,contentsControl,frameControl,Settings,State);
                ViewInnerControl = new ViewInnerControl(_errorLog,frameControl,innerControl,Settings,State);
                ViewImageControl = new ViewImageControl(_errorLog, innerControl, viewControl,Settings,State);
                ViewFrameControl.ViewInnerControl = ViewInnerControl;
                ViewInnerControl.SetViewImageControl(ViewImageControl);
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ViewImageControls failed");
            }
        }
    }
}
