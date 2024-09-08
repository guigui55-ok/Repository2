using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using CommonModule;

namespace ImageViewer5.ImageControl.Function
{
    public class ViewImageSlideShow
    {
        AppLogger _logger;
        ImageMainFrame _imageMainFrame;
        public Timer _SlideShowTimer;
        public ViewImageSlideShow(AppLogger logger, ImageMainFrame imageMainFrame)
        {
            _logger = logger;
            _imageMainFrame = imageMainFrame;
            _SlideShowTimer = new Timer();
            _SlideShowTimer.Interval = _imageMainFrame._imageMainFrameSetting._slideShowInterval;
            /*
             * 240907
             * 考えられる追加機能
             * スライドショー実行時間の上限時間（*分後スライドショーを終了）
             *
             * 検討案
             * 何回切替で終了、何回切替で何かアクション
             * だんだん遅くなる、早くなる、Interval時間ランダム（設定範囲内）
             * 
             */
        }

        public void StartTimer()
        {
            _logger.PrintInfo("_SlideShowTimer.Start");
            _logger.PrintInfo("Interval = " + _SlideShowTimer.Interval.ToString());
            _SlideShowTimer.Start();
        }

        public void StopTimer()
        {
            _logger.PrintInfo("_SlideShowTimer.Stop");
            _SlideShowTimer.Stop();
        }

        public void ChangeOnOff()
        {
            _logger.PrintInfo("_SlideShowTimer.ChangeOnOff");
            _imageMainFrame._imageMainFrameSetting._slideShowOn = !_imageMainFrame._imageMainFrameSetting._slideShowOn;
            bool flag = _imageMainFrame._imageMainFrameSetting._slideShowOn;
            List<ToolStripMenuItem> bufList = CommonGeneral.GetMenuItemListIsMatchNameInMenuStrip(
                _imageMainFrame.ContextMenuStrip, "SlideShowOnOff_ToolStripMenuItem");
            // 1つだけしかないはず;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)bufList[0];
            if (flag)
            {
                menuItem.Text = "スライドショー停止";
                StartTimer();
            }
            else
            {
                StopTimer();
                menuItem.Text = "スライドショー開始";
            }
        }
    }
}
