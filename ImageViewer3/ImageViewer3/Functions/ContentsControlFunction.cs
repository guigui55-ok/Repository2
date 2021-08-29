using ImageViewer.Function;
using ImageViewer.ParentForms;
using System;
using System.Windows.Forms;

namespace ImageViewer.Functions
{
    public class ContentsControlFunction
    {
        protected ErrorLog.IErrorLog _errorLog;
        readonly public ViewImageManager ViewImageManager;
        readonly Panel _contentsPanel;
        protected Form _mainForm;
        protected ContentsControl _contentsControl;
        public ContentsControlFunction(ErrorLog.IErrorLog errorlog, Form mainForm, ContentsControl contentsControl, ViewImageManager manager)
        {
            _errorLog = errorlog;
            _mainForm = mainForm;
            _contentsControl = contentsControl;
            _contentsPanel = contentsControl.GetControl();
            ViewImageManager = manager;

            _contentsPanel.SizeChanged += ContentsPanel_SizeChanged;
        }

        private void ContentsPanel_SizeChanged(object sender,EventArgs e)
        {
            try
            {
                // ContentsControl単体で済まない処理なのでここで処理する
                // ContentsPanel の サイズ変更時(≒MainForm サイズ変更時)に FramePanel がついついしない場合、
                // 位置と倍率を保持する ON なら、それを復元する
                ViewImageManager.DoFunctionAll(ViewImageManager.Invoke.MaintainSizeAndPositionRatioContentsAndFrame);
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ContentsPanel_SizeChanged Failed");
            }
        }
    }
}
