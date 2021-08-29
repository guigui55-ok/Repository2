using System;
using System.Collections.Generic;
using ErrorLog;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using ImageViewer.Controls;
using ImageViewer.Values;

namespace ImageViewer.Functions
{
    public class ViewImageBasicFunction
    {
        protected ErrorLog.IErrorLog _errorLog;

        public IViewImage ViewImage;
        public FileList.FileListReader FileList;
        public FileList.FileListRegister FileRegister;

        public ViewImageManager ViewImageManager;

        // コンストラクタ
        public ViewImageBasicFunction(
            ErrorLog.IErrorLog errorlog,
            ViewImageManager viewImageManager)
        {
            _errorLog = errorlog;
            this.ViewImageManager = viewImageManager;
        }

        public void InitializeViewImageObjects(ViewImageObjects viewImageObjects) { viewImageObjects.Initialize(); }
        public void ViewImageNowIndex() { ViewImageManager.DoFunction(ViewImageNowIndex); }

        public int ViewImageNowIndex(ViewImageObjects viewImageObjects)
        {
            try
            {
                int ret = 0;
                // パス取得
                string path = viewImageObjects.FileList.GetNowValue();
                Debug.WriteLine("path (" + viewImageObjects.FileList.NowIndex + ")= " + path);
                // イメージ取得
                ret = viewImageObjects.ViewImage.SetPath(path);
                if (ret < 1)
                {
                    _errorLog.AddErrorNotException(this.ToString() + "setPath");
                    return -1;
                }
                // イメージをセット
                viewImageObjects.Controls.ViewFrameControl.ViewInnerControl.SetImageWithDispose(
                    viewImageObjects.ViewImage.GetImage());

                // InnerControl の位置とサイズを記憶する
                viewImageObjects.Controls.ViewFrameControl.ViewInnerControl.SaveRaitoSizeAndPositionFromFrameControl();
                return 1;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ViewImage");
                return 0;
            }
        }

        public void ViewNext(Object sender, MouseEventArgs e) { ViewNext();}
        public void ViewNext()
        {
            try
            {
                List<ViewImageObjects> list = ViewImageManager.GetActiveControl();
                if (!((list is null) | (list.Count < 1)))
                {
                    foreach (ViewImageObjects value in list)
                    {
                        NextView(value);
                        //ViewImageNowIndex(value); 
                    }
                }
            }
            catch (Exception ex) { _errorLog.AddException(ex, this.ToString() + " ViewNext"); }
        }
        private void NextView(ViewImageObjects viewImageObjects)
        {
            try
            {
                viewImageObjects.FileList.MoveNext();
                if (_errorLog.HasError())
                {
                    _errorLog.ShowErrorMessageAndClearError();
                    return;
                }
                ViewImageNowIndex(viewImageObjects);
                if (_errorLog.HasError())
                {
                    _errorLog.ShowErrorMessageAndClearError();
                }
                viewImageObjects.Controls.ViewFrameControl.ViewInnerControl.RefreshPaint();
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ViewNext");
            }
        }


        public void ViewPrevious() { ViewImageManager.DoFunction(ViewPrevious); }
        public void ViewPrevious(Object sender, MouseEventArgs e) { ViewImageManager.DoFunction(ViewPrevious); }
        private int ViewPrevious(ViewImageObjects viewImageObjects)
        {
            try
            {
                viewImageObjects.FileList.MovePrevious();
                if (_errorLog.HasError())
                {
                    _errorLog.ShowErrorMessageAndClearError();
                    return -1;
                }
                ViewImageNowIndex(viewImageObjects);
                if (_errorLog.HasError())
                {
                    _errorLog.ShowErrorMessageAndClearError();
                }
                viewImageObjects.Controls.ViewFrameControl.ViewInnerControl.RefreshPaint();
                return 1;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ViewNext");
                return 0;
            }
        }

        // 拡大縮小

        // SetPath + ViewImage
        public void ViewImageAfterSetPath(ViewImageObjects viewImageObjects,List<string> list)
        {
            try
            {
                SetPaths(viewImageObjects,list);
                ViewImageDefault(viewImageObjects);

            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ViewImageAfterSetPath");
            }
        }

        public void SetPaths(ViewImageObjects viewImageObjects,List<string> list)
        {
            int ret = viewImageObjects.FileRegister.SetFileListWithApplyConditions(list);
            if (ret < 1)
            {
                MessageBox.Show("Control_DragDrop.setFileListWithApplyConditions Failed");
            }
            viewImageObjects.FileList.FileList = viewImageObjects.FileRegister.GetList();
            if (_errorLog.HasError())
            {
                _errorLog.ShowErrorMessageAndClearError();
            }
        }


        // サイズ Panel.Size、Location00
        // 画像を表示
        public void ViewImageDefault(ViewImageObjects viewImageObjects)
        {
            try
            {
                int ret = 1;
                Panel frameControl = (Panel)viewImageObjects.Controls.ViewFrameControl.GetControl();
                // 外側のサイズ
                if (frameControl == null)
                {
                    _errorLog.AddErrorNotException(this.ToString(), ".ViewImageDefault frameControl is null"); return;
                }
                Size frameSize = frameControl.Size;
                // SetPath
                string NowPath = viewImageObjects.FileList.GetNowValue();
                if (!(((NowPath == null)|(viewImageObjects.ViewImage.GetPath() == null))))
                {
                    // これからの Path と以前の Path のどちらかが空 null ではない 
                    // イメージの読み込みはしない
                } else if (NowPath.CompareTo(viewImageObjects.ViewImage.GetPath()) == 0)
                {
                    // 両方とも値がある
                    // 今表示しているものと同じ、この場合イメージの読み込みはしない
                }
                else
                {
                    // 今表示しているものと違う、イメージを読み込みなおす
                    ret = viewImageObjects.ViewImage.SetPath(NowPath);
                }

                if (ret < 1)
                {
                    _errorLog.AddErrorNotException(this.ToString() + "ViewImageDefault setPath"); return;
                }

                if (viewImageObjects.ViewImage.GetImage() == null)
                {
                    _errorLog.AddErrorNotException(this.ToString() + "ViewImageDefault ViewImage is null"); return;
                }

                // ImageSize
                Size ImageSize = viewImageObjects.ViewImage.GetImage().Size;

                // CalcObjectSet
                SizeLocationCalc Calclator = new SizeLocationCalc();
                // Assert
                Calclator.SizeIsNull(frameControl.Size, viewImageObjects.ViewImage.GetImage().Size);
                // サイズ計算 Panel にフィットさせる
                Size newSize = Calclator.GetSizeFitFrame(frameControl.Size, viewImageObjects.ViewImage.GetImage().Size);
                //Size newSize = Calclator.getSizeFitFrame(_parentControl.Size, ViewImageControl.getSize());

                // Location
                if (viewImageObjects.Controls.ViewInnerControl.Settings.IsViewAlwaysCenter)
                {
                    // 常に中央に表示する場合
                    // サイズ変更、中央計算
                } else
                {
                    // PictureBox 非固定

                }
                // Default は00
                //Point location = new Point(0, 0);

                viewImageObjects.Controls.ViewInnerControl.SetVisible(false);

                // イメージをセット
                viewImageObjects.Controls.ViewInnerControl.SetImageWithDispose(viewImageObjects.ViewImage.GetImage());
                // SetSize
                //_viewImageControl.changeSize(newSize);
                viewImageObjects.Controls.ViewInnerControl.ChangeSize(frameControl.Size);
                // SetLocation
                //_viewImageControl.changeLocation(location);

                // visible
                viewImageObjects.Controls.ViewInnerControl.SetVisible(true);

                // save size and location
                viewImageObjects.Controls.ViewInnerControl.SaveRaitoSizeAndPositionFromFrameControl();

            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " ViewImageDefault");
            }
        }

        //private Size GetViewSizeByCalc()
        //{
        //    try
        //    {
        //        return new Size();
        //    } catch (Exception ex)
        //    {
        //        _errorLog.AddException(ex, this.ToString() + " getViewSizeByCalc");
        //        return new Size();
        //    }
        //}
        public class SizeLocationCalc
        {
            ErrorLog.IErrorLog _errorLog;
            public void SetErrorLog(IErrorLog errorlog) { _errorLog = errorlog; }
            // Image を Frameに合わせる
            public Size GetSizeFitFrame(Size frameSize, Size imageSize)
            {
                try
                {
                    // ※ sizeIsNull を外部で実行しておく
                    //'PictureBoxのサイズをPanelサイズにFitさせる
                    //'＝PictureBoxのサイズをPanelサイズからはみ出さないようにする

                    //'サイズを合わせるための基準 Criteria for matching sizes
                    //'Public Overloads Sub setFlagCriteriaForMatchSize(argFlag As Integer)
                    //'0 Image OuterFit
                    //'1 Inner_Horizontal 幅はInnerで高さにOuterの比率をかける
                    //'2 Inner_Vertical　高さはInnerで幅にOuterの比率をかける
                    //'FlagCriteriaForMatchSize = argFlag
                    // 縦横比 1:AspectRaito
                    double AspectRaito = (double)imageSize.Width / imageSize.Height;
                    // Frame と Image の縦比
                    double VerticalRaito = (double)frameSize.Height / imageSize.Height;
                    // Frame と Image の縦比
                    double HorizontalRaito = (double)frameSize.Width / imageSize.Width;

                    double retRaito = 0, NewHeight = 0, NewWidth = 0;
                    // 倍率が小さいほうを設定する
                    if (VerticalRaito < HorizontalRaito)
                    {
                        // Frame の縦に合わせる
                        //NewHeight = frameSize.Height * 1;
                        //retRaito = NewHeight / imageSize.Height; // 倍率を計算する
                        //NewWidth = imageSize.Width * retRaito; //* AspectRaito; // 上記倍率から縦横比を掛けて Width を決定

                        // 縦のほうが倍率が小さい→横の余白が長い→縦に合わせる
                        NewHeight = frameSize.Height * 1;
                        // 縦の倍率を求める imageSize が frameSize になる
                        retRaito = (double)frameSize.Height / imageSize.Height; // VerticalRaito
                        // 倍率を Width にかける
                        NewWidth = imageSize.Width * retRaito;
                    }
                    else
                    {
                        // Frame の横に合わせる
                        NewWidth = frameSize.Width * 1;
                        retRaito = NewWidth / frameSize.Width; // 倍率を計算する
                        NewHeight = imageSize.Height * retRaito; // 上記倍率から縦横比を掛けて Height を決定
                    }
                    return new Size((int)NewWidth, (int)NewHeight);
                }
                catch (Exception ex)
                {
                    _errorLog.AddException(ex, this.ToString() + " getSizeFitFrame");
                    return new Size();
                }
            }

            public bool SizeIsNull(Size frameSize, Size imageSize)
            {
                try
                {
                    if (frameSize == null)
                    {
                        _errorLog.AddErrorNotException(this.ToString() + " getPointLocationCenter ,frameSize null");
                        return false;
                    }
                    if (imageSize == null)
                    {
                        _errorLog.AddErrorNotException(this.ToString() + " getPointLocationCenter ,imageSize null");
                        return false;
                    }
                    return true;
                } catch (Exception ex)
                {
                    _errorLog.AddException(ex, this.ToString() + " getPointLocationCenter");
                    return false;
                }
            }

            // 中央に表示
            public Point GetPointLocationCenter(Size frameSize,Size imageSize)
            {
                try
                {
                    // sizeIsNull を外部で実行しておく
                    double width = (frameSize.Width - imageSize.Width) / 2;
                    double height = (frameSize.Height - imageSize.Height) / 2;
                    return new Point((int)width, (int)height);
                } catch (Exception ex)
                {
                    _errorLog.AddException(ex, this.ToString() + " getPointLocationCenter");
                    return new Point();
                }
            }
        }

    }
}
