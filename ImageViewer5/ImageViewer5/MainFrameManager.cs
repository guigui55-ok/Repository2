using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
//using CommonModulesImageViewer;
using ImageViewer5.ImageControl;
using ViewImageModule;

namespace ImageViewer5
{
    /// <summary>
    /// ImageMainFrameのリストを扱うクラス
    /// 　、FrameListすべてに処理が必要なときなどにも使用する
    /// </summary>
    public class MainFrameManager
    {
        // 外部からの移譲
        AppLogger _logger;
        FormMain _formMain;
        public List<ImageMainFrame> _imageMainFrameList;
        int _nowIndex = 0;
        ImageMainFrame _nowImageMainFrame;
        // クラス内で生成
        // なし
        ViewImageFunction_FitInnerToFrame _fitFunction;
        public MainFrameManager(AppLogger logger, FormMain formMain)
        {
            _logger = logger;
            _formMain = formMain;

            List<Control> conList = CommonModuleFileSenderApp.CommonGeneral.GetControlListIsMatchType(_formMain, typeof(ImageMainFrame));
            _imageMainFrameList = ConvertControlListToImageMainFrameList(conList);
            _nowIndex = 0;
            _nowImageMainFrame = GetCurrentFrame();
        }

        public void DisposeObjects()
        {
            try
            {
                for (int i = 0; i < _imageMainFrameList.Count; i++)
                {
                    _imageMainFrameList[i].Dispose();
                    _imageMainFrameList[i] = null;
                }
                _imageMainFrameList.Clear();
                _imageMainFrameList = null;
                if (_fitFunction != null)
                {
                    _fitFunction.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString() + ".Dispose Error");
                Console.WriteLine(ex.ToString() + ":" + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public ImageMainFrame GetCurrentFrame()
        {
            return _imageMainFrameList[_nowIndex];
        }

        /// <summary>
        /// List＜Control＞ から List＜ImageMainFrame＞ に変換する
        /// </summary>
        /// <param name="controlList"></param>
        /// <returns></returns>
        public static List<ImageMainFrame> ConvertControlListToImageMainFrameList(List<Control> controlList)
        {
            return controlList.OfType<ImageMainFrame>().ToList();
        }

        public ImageMainFrame CreateImageMainFrame(int number)
        {
            _logger.PrintInfo("MainFrameManager > CreateImageMainFrame, " + number.ToString());
            ImageMainFrame retFrame = null;
            try
            {
                retFrame = new ImageViewer5.ImageControl.ImageMainFrame();
                retFrame.Location = new System.Drawing.Point(0, 1);
                retFrame.Name = String.Format("imageMainFrame{0}", number);
                retFrame.Size = new System.Drawing.Size(414, 419);
                retFrame.TabIndex = 0;
                retFrame._logger = _logger;
                retFrame.Parent = _formMain;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "CreateImageMainFrame, " + number.ToString());
            }
            return retFrame;
        }
        public void AdjustSizeAndLocation(int frameNum=0)
        {
            _logger.PrintInfo("MainFrameManager > AdjustSizeAndLocation");
            //#
            //（未対応）
            //配置の決め方の設定が必要
            // ToRight , ToLeft
            // ToDonw , ToUp
            // Priority, Horizon, Vertical
            // FixFrameSize(Formのサイズを大きくする）、FixFormSize（Window内でFrameのサイズを調整する）
            //#
            // Frameの非表示設定などがあったら、非表示を除外する処理が必要（未対応）
            int count = _imageMainFrameList.Count;
            int baseWidth;
            int baseHeight;
            Size afterWinSize;
            if (_formMain._formMainSetting.iIsFixFrameSize == ImageViewerConstants.FIX_FORM_SIZE)
            {
                baseWidth = (int)(_formMain.ClientSize.Width / count);
                baseHeight = (int)(_formMain.ClientSize.Height / count);
                afterWinSize = _formMain.ClientSize;
            }
            else
            {
                baseWidth = _imageMainFrameList[frameNum].Size.Width;
                baseHeight = _imageMainFrameList[frameNum].Size.Height;
                afterWinSize = _formMain.ClientSize;
            }
            _logger.PrintInfo(string.Format("cnt={0} ,w={1}, h={2} ", count, baseWidth, baseHeight));
            //foreach(ImageMainFrame bufFrame in _imageMainFrameList)
            //{
            //    bufFrame.
            //}
            int maxRow = _formMain._formMainSetting.frameRowMax;
            if (maxRow <= 0) { maxRow = count; }
            int maxCol = _formMain._formMainSetting.frameColMax;
            if (maxCol <= 0) { maxCol = count; }
            //カウントを追加していって、ループ内のtempCountと合致したときの、width,Heightを使用する
            //
            // 0番目は無条件に設定
            //int nowCount = 0;
            int tempCount = 0;
            int rowCount = 0; //afterWinSizeで使用する
            int colCount = 0; //afterWinSizeで使用する
            List<Size> newSizeList = new List<Size> { };
            List<Point> newLocList = new List<Point> { };
            //横優先
            if (_formMain._formMainSetting.framePreferredDirection == ImageViewerConstants.FRAME_PRIORITY_HORIZON)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    for (int row = 0; row < maxRow; row++)
                    {
                        //
                        int x = (int)(baseWidth * row);
                        int y = (int)(baseHeight * col);
                        int w = (int)(baseWidth);
                        int h = (int)(baseHeight);
                        newSizeList.Add(new Size(w, h));
                        newLocList.Add(new Point(x, y));
                        tempCount++;
                        if (rowCount < row) { rowCount = row; }
                        if (count < tempCount){break;}
                    }
                    if (count < tempCount) { break; }
                    // rowが抜けたときはカウントしない
                    if (colCount < col) { colCount = col; }
                }
            }
            else
            {
                // _formMain._formMainSetting.framePreferredDirection == ImageViewerConstants.FRAME_PRIORITY_VERTICAL
                //縦優先

            }
            _logger.PrintInfo(string.Format("Win, size={0}, loc={1} ", _formMain.Size, _formMain.Location));
            for (int i = 0; i < count; i++)
            {
                //int x = (int)(baseWidth * i);
                //int y = (int)(baseHeight * i);
                //int w = (int)(x + baseWidth);
                //int h = (int)(y + baseHeight);
                //Size newSize = new Size(w, h);
                //Point newLoc = new Point(x, y);
                Size newSize = newSizeList[i];
                Point newLoc = newLocList[i];
                _logger.PrintInfo(string.Format("before, size={0}, loc={1} ", _imageMainFrameList[i].Size, _imageMainFrameList[i].Location));
                _logger.PrintInfo(string.Format("i={0} ,size={1}, loc={2} ", i, newSize, newLoc));
                _imageMainFrameList[i].ChangeSizeAndLocation(newSize, newLoc);
                //_imageMainFrameList[i]._imageViewerMain._viewImageFunction_

                ImageMainFrame frameParts = _imageMainFrameList[i];
                ImageMainClass frameObj = frameParts._imageViewerMain;
                //ViewImageFunction_FitInnerToFrame fitFunction =
                //    new ViewImageFunction_FitInnerToFrame(
                //        _logger,
                //        frameObj._viewImageFrameControl, frameObj._viewImageControl, frameObj._viewImage);
                ViewImageFunction_FitInnerToFrame fitFunction = frameObj._viewImageFunction._viewImageFunction_FitInnerToFrame;
                fitFunction.FitImageToControl(frameParts._imageMainFrameSetting._isFitFormMain);
            }
            if (_formMain._formMainSetting.iIsFixFrameSize == ImageViewerConstants.FIX_FORM_SIZE)
            {
                //_formMain.ClientSize = afterWinSize;
            }
            else
            {
                rowCount++;
                colCount++;
                _formMain.ClientSize = new Size(baseWidth * rowCount, baseHeight * colCount);
            }
            for (int i = 0; i < count; i++)
            {
                _logger.PrintInfo(string.Format("after, size={0}, loc={1} ", _imageMainFrameList[i].Size, _imageMainFrameList[i].Location));
            }
            _logger.PrintInfo(string.Format("Win, size={0}, loc={1} ", _formMain.Size, _formMain.Location));
        }



        public void InitializeMainFrame(ImageMainFrame imageMainFrame, string path, List<string> filterList)
        {
            _logger.PrintInfo("MainFrameManager > InitializeMainFrame");
            //#
            //テスト値
            //List<string> ignoreList = new List<string> { "_4a44d3" };
            List<string> ignoreList = new List<string> { "" };
            //string path = @"C:\Users\OK\source\repos\test_media_files\test_jpg";
            //List<string> filterList = ImageViewerConstants.SUPPORTED_IMAGE_EXTENTION_DEFAULT_LIST;
            //#
            imageMainFrame.InitializeValues(filterList);
            // タプルを作成して渡す
            var parameters = Tuple.Create(
                path,
                filterList,
                ignoreList,
                false);
            imageMainFrame.ShowSubFormFileList(parameters);
            imageMainFrame._imageViewerMain.ShowImageAfterInitialize(path);
            //ファイルリスト設定後に実行する
            imageMainFrame._imageViewerMain._viewImageFunction.InitializeValue_LoadAfter();
            ////引数設定を適用
            ////（スライドショーインスタンス生成などがあるため）
            //_applySettings.ApplyArgs(_imageViewerArgs);
            // 240920
            // Anchorがどこかで Top,Left となる
            imageMainFrame.Anchor = AnchorStyles.None;
            // デザイナの設定と重複する可能性があるので、ログ出力
            // 240926
            _formMain._fileSenderFunction.AddEventHandler(imageMainFrame);
            _logger.PrintInfo(imageMainFrame.Name + " > Set Anchor.None");
        }

        public void ClearFrameFocusFlag()
        {
            foreach(ImageMainFrame frame in this._imageMainFrameList)
            {
                frame._imageViewerMain._isFocusFrame = false;
            }
        }
    }
}
