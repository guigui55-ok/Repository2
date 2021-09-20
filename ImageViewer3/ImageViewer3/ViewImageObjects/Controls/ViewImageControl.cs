using System;
using System.Windows.Forms;
using ErrorLog;
using System.Drawing;
using System.Runtime.InteropServices;
using ImageViewer.Values;

namespace ImageViewer.Controls
{
    // PictureBox
    public class ViewImageControl : IViewImageControl
    {
        private PictureBox _pictureBox;
        private Panel _parentControl;
        protected ErrorLog.IErrorLog _errorLog;
        private IViewControlState _state;
        private IViewImageSettings _settings;

        // PauseLayout用
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
            HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;
        //private const int WM_PAINT = 0x000F;

        public ViewImageControl(IErrorLog errorlog,
            Panel parentControl,PictureBox viewControl,IViewImageSettings settings,IViewControlState state)
        {
            _errorLog = errorlog;
            _state = state;
            _settings = settings;
            _parentControl = parentControl;
            _pictureBox = viewControl;
        }

        public IViewControlState State { get { return _state; } set { _state = value; } }
        public IViewImageSettings Settings { get { return _settings; } set { _settings = value; } }
        public Control GetControl() { return _pictureBox; }
        public int SetControl(Control pictureBox)
        {
            try
            {
                if (Object.ReferenceEquals(pictureBox.GetType(), new PictureBox().GetType()))
                {
                    _pictureBox = (PictureBox)pictureBox;
                } else
                {
                    return -1;
                }
                return 1;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, "setControl");
                return 0;
            }
        }
        public int SetParentControl(Control panel)
        {
            try {
                //if (Object.ReferenceEquals(form.GetType(), new Form().GetType()))
                if (!( panel.GetType().Equals(typeof( Panel ))))
                {
                    return -1;
                }
                else
                {
                    _parentControl = (Panel)panel;
                }
                return 1;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, "setParentControl");
                return 0;
            }
        }

        public int Initialize()
        {
            try
            {

                return 1;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, "Initialize");
                return 0;
            }
        }

        public bool ContorlIsNull()
        {
            try { 
                if (_pictureBox == null)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, "ContorlIsNull");
                return false;
            }
        }

        public void SetImageWithDispose(Image image)
        {
            try
            {
                if (_pictureBox == null)
                {
                    _pictureBox.Image.Dispose();
                }
                _pictureBox.Image = image;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, "setImageWithDispose");
            }
        }
        
        public void SetImageNotDispose(Image image)
        {
            try
            {
                _pictureBox.Image = image;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " setImageNotDispose");
            }
        }

        public Image GetImage()
        {
            try
            {
                if (_pictureBox != null)
                {
                    return _pictureBox.Image;
                }
                else
                {
                    _errorLog.AddErrorNotException(this.ToString() + "setImageNotDispose");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " getImage");
                return null;
            }
        }

        public void SetBitmapWithDispose(Bitmap bitmap)
        {
            try
            {
                if (_pictureBox != null)
                {
                    _pictureBox.Image.Dispose();
                    _pictureBox.Image = bitmap;
                }
                else
                {
                    _pictureBox.Image = bitmap;
                }
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " setBitmapWithDispose");
            }
        }

        public void SetBitmapNotDispose(Bitmap bitmap)
        {
            if (_pictureBox != null)
            {
                _pictureBox.Image = bitmap;
            }
            else
            {
                _errorLog.AddErrorNotException("setBitmapNotDispose");
            }
        }

        public Size GetSize()
        {
            try
            {
                if (_pictureBox != null)
                {
                    return _pictureBox.Size;
                }
                else
                {
                    return new Size(0, 0);
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, "getSize");
                return new Size(0, 0);
            }
        }

        public Point GetLocation()
        {
            if (ContorlIsNull())
            {
                _errorLog.AddErrorNotException("getLocation");
                return new Point(0, 0);
            }
            else
            {
                return _pictureBox.Location;
            }
        }
        public void ChangeSize(Size size)
        {
            try
            {
                if (ContorlIsNull()) { _errorLog.AddErrorNotException("changeSize");  }
                if (_pictureBox.Size.Equals(new Size(0,0))) {
                    _errorLog.AddErrorNotException("changeSize:Size is zero.");
                }
                _state.NowSizeUpdate = true;
                _pictureBox.Size = size;
                _state.NowSizeUpdate = false;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex,"changeSize");
            }
        }
        public void SetVisible(bool flag)
        {
            if (ContorlIsNull()) { _errorLog.AddErrorNotException("setVisible"); }
            _pictureBox.Visible = flag;
        }
        public void ChangeLocation(Point point)
        {
            try
            {
                if (ContorlIsNull()) { _errorLog.AddErrorNotException("changeLocation"); }
                _pictureBox.Location = point;

            }
            catch (Exception ex) { _errorLog.AddException(ex,"changeLocation"); }

        }
        public void SetImageLocation(String path)
        {
            try
            {
                _pictureBox.ImageLocation = path;
            }
            catch (Exception ex) { _errorLog.AddException(ex, "setImageLocation"); }
        }
        public PictureBox GetPictureBox()
        {
            return _pictureBox;
        }

        public Control GetParentControl()
        {
            try
            {
                 if (_parentControl == null) { return null; }
                return _parentControl;
            } 
            catch (Exception ex) { _errorLog.AddException(ex, this.ToString()+ ".getParentControl"); return null; }
        }

        public void RefreshPaint()
        {
            try
            {
                //Graphics g = Graphics.FromImage(pictureBox1.Image);
                //myPainting(g); // Bitmapオブジェクトに描画
                _pictureBox.Refresh();
            }
            catch (Exception ex) { _errorLog.AddException(ex, this.ToString() + ".RefreshPaint"); return; }
        }

        public void PausePaint(bool flag)
        {
            try
            {
                State.IsPausePaint = flag;
                if (flag)
                {
                    //_pictureBox.SuspendLayout();
                    SendMessage(new HandleRef(_pictureBox, _pictureBox.Handle),
                        WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
                } else
                {
                    //_pictureBox.ResumeLayout();
                    SendMessage(new HandleRef(_pictureBox, _pictureBox.Handle),
                        WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                    _pictureBox.Invalidate();
                }
            }
            catch (Exception ex) { _errorLog.AddException(ex, this.ToString() + ".PausePaint"); return; }
        }

        public void SuspenLayout()
        {
            if (_pictureBox == null) { return; }
            _pictureBox.SuspendLayout();
        }

        public void ResumeLayout()
        {
            if (_pictureBox ==null) { return; }
            _pictureBox.ResumeLayout();
        }
    }
}
