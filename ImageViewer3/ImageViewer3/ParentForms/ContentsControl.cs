using ErrorLog;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageViewer.ParentForms
{
    public class ContentsControl
    {
        protected ErrorLog.IErrorLog _errorLog;
        protected Panel _contentsPanel;
        protected Form _parentControl;
        readonly public MainFormManager MainformManager;
        public ContentsControl(IErrorLog errorlog, Panel contentsPanel,Form parentControl,MainFormManager mainFormManager) 
        {
            _errorLog = errorlog;
            _contentsPanel = contentsPanel;
            _parentControl = parentControl;
            this.MainformManager = mainFormManager;
            _contentsPanel.SizeChanged += ContentsPanel_SizeChanged;
        }

        public Panel GetControl() { return _contentsPanel; }
        public Size GetSize() { return _contentsPanel.Size; }
        public void ChangeSize(Size size)
        {
            try
            {
                _contentsPanel.Size = size;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "setPrentControl Failed");
                return;
            }
        }

        public void ChangeLocation(Point point)
        {
            try
            {
                _contentsPanel.Location = point;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "setPrentControl Failed");
                return;
            }
        }
        private void ContentsPanel_SizeChanged(object sender,EventArgs e)
        {
            try
            {
                MainformManager.State.IsContentsControlSizeChanged = true;
                // frame size location ajust

            } catch(Exception ex)
            {
                _errorLog.AddException(ex, this.ToString(), "ContentsPanel_SizeChanged Failed");
            } finally
            {
                MainformManager.State.IsContentsControlSizeChanged = false;
            }
        }
    }
}
