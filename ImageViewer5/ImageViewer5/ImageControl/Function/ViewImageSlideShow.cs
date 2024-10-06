using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
//using CommonModule;
using CommonModulesProject;

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
            int objectId = this.GetHashCode();
            _logger.PrintInfo(_imageMainFrame.Name + " > SlideShow ID: " + objectId.ToString());
            // _imageMainFrame._formFileList._fileListManage がnullの時がある
            //_SlideShowTimer = GetNewTimer();
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

        public void Initialize_LoadAfter()
        {
            _logger.PrintInfo(_imageMainFrame.Name + " > _SlideShowTimer.Initialize_LoadAfter");
            _SlideShowTimer = GetNewTimer();
        }

        private Timer GetNewTimer()
        {
            Timer retTimer = new Timer();
            retTimer.Interval = _imageMainFrame._imageMainFrameSetting._slideShowInterval;
            //retTimer.Tick += _imageMainFrame._formFileList._fileListManager.MoveNextFileWhenLastFileNextDirectoryEvent;
            // 既にイベントが登録されていないか確認してから追加する
            if (_imageMainFrame._formFileList._fileListManager != null)
            {
                retTimer.Tick -= _imageMainFrame._formFileList._fileListManager.MoveNextFileWhenLastFileNextDirectoryEvent;
                retTimer.Tick += _imageMainFrame._formFileList._fileListManager.MoveNextFileWhenLastFileNextDirectoryEvent;
            }
            // オブジェクトのハッシュコードを取得
            int objectId = retTimer.GetHashCode();
            _logger.PrintInfo(_imageMainFrame.Name + " > Timer ID: " + objectId.ToString());
            objectId = this.GetHashCode();
            _logger.PrintInfo(_imageMainFrame.Name + " > SlideShow ID: " + objectId.ToString());
            return retTimer;
        }

        public void StartTimer()
        {
            _logger.PrintInfo(_imageMainFrame.Name + " > _SlideShowTimer.Start");
            //// 古いタイマーが存在する場合、停止・破棄する
            //if (_SlideShowTimer != null)
            //{
            //    StopTimer();
            //}
            if (_SlideShowTimer == null)
            {
                _SlideShowTimer = GetNewTimer();
            }
            else
            {
                // オブジェクトのハッシュコードを取得
                _logger.PrintInfo("StartTimer  Timer != null");
                int objectId = _SlideShowTimer.GetHashCode();
                _logger.PrintInfo(_imageMainFrame.Name + " > Timer ID: " + objectId.ToString());
                objectId = this.GetHashCode();
                _logger.PrintInfo(_imageMainFrame.Name + " > SlideShow ID: " + objectId.ToString());
            }
            _logger.PrintInfo("Interval = " + _SlideShowTimer.Interval.ToString());
            _SlideShowTimer.Start();
        }

        public void StopTimer()
        {
            _logger.PrintInfo(_imageMainFrame.Name + " > _SlideShowTimer.Stop");
            // _formFileList オブジェクトのハッシュコードを取得
            int objectId = _SlideShowTimer.GetHashCode();
            _logger.PrintInfo(_imageMainFrame.Name + " > Timer ID: " + objectId.ToString());
            objectId = this.GetHashCode();
            _logger.PrintInfo(_imageMainFrame.Name + " > SlideShow ID: " + objectId.ToString());
            _SlideShowTimer.Stop();
            _SlideShowTimer.Enabled = false;
            _SlideShowTimer.Tick -= _imageMainFrame._formFileList._fileListManager.MoveNextFileWhenLastFileNextDirectoryEvent;
            _SlideShowTimer.Dispose();
            _SlideShowTimer = null;
        }

        public void ChangeOnOff()
        {
            ChangeOnOffByFlag(0);
        }

        /// <summary>
        /// スライドショーをスイッチする
        /// , 0=現在と反対、1=ON, 2=OFF
        /// </summary>
        /// <param name="flagInt"></param>
        public void ChangeOnOffByFlag(int flagInt)
        {
            _logger.PrintInfo(_imageMainFrame.Name + " > _SlideShowTimer.ChangeOnOffByFlag");
            bool afterFlag;
            if (flagInt == 0)
            {
                afterFlag = !_imageMainFrame._imageMainFrameSetting._slideShowOn;
            } else if(flagInt == 1)
            {
                afterFlag = true;
            }
            else
            {
                afterFlag = false;
            }
            _imageMainFrame._imageMainFrameSetting._slideShowOn = afterFlag;
            List<ToolStripMenuItem> bufList = CommonModuleFileSenderApp.CommonGeneral.GetMenuItemListIsMatchNameInMenuStrip(
                _imageMainFrame.ContextMenuStrip, "SlideShowOnOff_ToolStripMenuItem");
            // 1つだけしかないはず;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)bufList[0];
            if (afterFlag)
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
