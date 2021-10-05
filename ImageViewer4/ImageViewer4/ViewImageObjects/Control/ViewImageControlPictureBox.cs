using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ViewImageObjects
{
    // PictureBox
    public class ViewImageControlPictureBox : IViewImageControl
    {
        private PictureBox _pictureBox;
        private Panel _parentControl;
        protected ErrorManager.ErrorManager _err;
        protected bool IsSizeChanging = false;
        //private IViewControlState _state;
        //private IViewImageSettings _settings;

        // PauseLayout用
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
            HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;
        //private const int WM_PAINT = 0x000F;

        //public ViewImageControlPictureBox(ErrorManager.ErrorManager err,
        //    Panel parentControl, PictureBox viewControl, IViewImageSettings settings, IViewControlState state)
        // ↑Control からの通知を受けたり、このオブジェクトを保持する形でほかのクラスで管理する

        public ViewImageControlPictureBox(ErrorManager.ErrorManager err ,Panel parentControl,PictureBox viewControl)
        {
            _err = err;
            _parentControl = parentControl;
            _pictureBox = viewControl;
        }

        //public IViewControlState State { get { return _state; } set { _state = value; } }
        //public IViewImageSettings Settings { get { return _settings; } set { _settings = value; } }
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
                _err.AddException(ex, this,"setControl");
                return 0;
            }
        }
        public int SetParentControl(Control panel)
        {
            try {
                //if (Object.ReferenceEquals(form.GetType(), new Form().GetType()))
                if (!( panel.GetType().Equals(typeof(Panel))))
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
                _err.AddException(ex, this, "SetParentControl");
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
                _err.AddException(ex, this, "Initialize");
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
                _err.AddException(ex, this, "ContorlIsNull");
                return false;
            }
        }

        private void SetImageSettings()
        {
            try
            {
                _pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "SetImageSettings");
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
                SetImageSettings();

            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "SetImageWithDispose");
            }
        }
        
        public void SetImageNotDispose(Image image)
        {
            try
            {
                _pictureBox.Image = image;
                SetImageSettings();
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "SetImageNotDispose");
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
                    _err.AddLogAlert(this,"setImageNotDispose");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "GetImage");
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
                }
                else
                {
                }
                _pictureBox.Image = bitmap;
                SetImageSettings();
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "SetBitmapWithDispose");
            }
        }

        public void SetBitmapNotDispose(Bitmap bitmap)
        {
            if (_pictureBox != null)
            {
                _pictureBox.Image = bitmap;
                SetImageSettings();
            }
            else
            {
                _err.AddLogAlert(this, "SetBitmapWithDispose");
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
                _err.AddException(ex, this, "GetSize");
                return new Size(0, 0);
            }
        }

        public Point GetLocation()
        {
            if (ContorlIsNull())
            {
                _err.AddLogAlert(this,"GetLocation");
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
                if (ContorlIsNull()) { _err.AddLogAlert(this,"changeSize");  }
                if (_pictureBox.Size.Equals(new Size(0,0))) {
                    _err.AddLogAlert(this,"changeSize:Size is zero.");
                }
                IsSizeChanging = true;
                _pictureBox.Size = size;
                IsSizeChanging = false;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "ChangeSize");
            }
        }
        public void SetVisible(bool flag)
        {
            if (ContorlIsNull()) { _err.AddLogAlert(this,"setVisible"); }
            _pictureBox.Visible = flag;
        }
        public void ChangeLocation(Point point)
        {
            try
            {
                if (ContorlIsNull()) { _err.AddLogAlert(this,"changeLocation"); }
                _pictureBox.Location = point;

            }
            catch (Exception ex) { _err.AddException(ex,this,"changeLocation"); }

        }
        public void SetImageLocation(String path)
        {
            try
            {
                _pictureBox.ImageLocation = path;
            }
            catch (Exception ex) { _err.AddException(ex,this, "setImageLocation"); }
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
            catch (Exception ex) { _err.AddException(ex, this, "getParentControl"); return null; }
        }

        public void RefreshPaint()
        {
            try
            {
                //Graphics g = Graphics.FromImage(pictureBox1.Image);
                //myPainting(g); // Bitmapオブジェクトに描画
                _pictureBox.Refresh();
            }
            catch (Exception ex) { _err.AddException(ex, this,"RefreshPaint"); return; }
        }

        public void PausePaint(bool flag)
        {
            try
            {
                //State.IsPausePaint = flag;
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
            catch (Exception ex) { _err.AddException(ex, this,"PausePaint"); return; }
        }

        public void SuspenLayout()
        {
            if (_pictureBox == null) { return; }
            _pictureBox.SuspendLayout();
        }

        public void ResumeLayout()
        {
            if (_pictureBox == null) { return; }
            _pictureBox.ResumeLayout();
        }
    }
}
