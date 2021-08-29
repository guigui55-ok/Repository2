using System;
using System.Diagnostics;
using System.Windows.Forms;
using ErrorLog;
using ImageViewer.Function;

namespace ImageViewer.Events
{
    public class MainKeyEvents
    {
        protected ErrorLog.IErrorLog _errorLog;
        protected Control _contentsControl;
        protected Control _recieveEventControl;
        //public Function.CommonFunctions Functions;
        protected ViewImageManager _viewImageManager;
        protected ImageViewerSettings _settings;

        public MainKeyEvents(IErrorLog errorLog, 
            Control panel,Control recieveControl, ViewImageManager manager,ImageViewerSettings settings) {
            _errorLog = errorLog;
            _contentsControl = panel;
            _recieveEventControl = recieveControl;
            _viewImageManager = manager;
            _settings = settings;

            Initialize();
        }

        public int SetRecieveEventControl(Control control)
        {
            try
            {
                if (control is null)
                { _errorLog.AddErrorNotException(this.ToString(), "initialize control is null"); return -1; }
                _recieveEventControl = control;
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "initialize Failed");
                return 0;
            }
        }

        public int Initialize()
        {
            try
            {
                if (_contentsControl is null)
                { _errorLog.AddErrorNotException(this.ToString(), "initialize control is null"); return -1; }

                _contentsControl.KeyDown += FrameControl_KeyDown;
                _recieveEventControl.KeyDown += FrameControl_KeyDown;

                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "initialize Failed");
                return 0;
            }
        }

        private void FrameControl_KeyDown(object sender,KeyEventArgs e)
        {
            try
            {
                //Debug.WriteLine("FrameControl_KeyDown");
                if (e.Alt)
                {
                    if (!(_settings.IsMenuBarVisibleAlways))
                    {
                        _viewImageManager.MainControls.MainFormFunction.ChangeVisibleMenuStrip();

                    } else
                    {
                        // もし非表示なら表示する
                        _viewImageManager.MainControls.MainFormFunction.ChangeVisibleToTrueMenuStripIfFalse();
                    }
                }
            } catch ( Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "FrameControl_KeyDown Failed");
                return;
            }
        }

    }
}
