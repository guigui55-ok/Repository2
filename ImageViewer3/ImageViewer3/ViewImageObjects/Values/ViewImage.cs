using System;
using System.Drawing;
using ErrorLog;

namespace ImageViewer.Values
{
    public class ViewImage : IViewImage
    {
        private Image _image;
        String _path;
        private ErrorLog.IErrorLog _errorLog;

        public ViewImage()
        {
            _errorLog = GlobalErrloLog.ErrorLog;
        }
        public void SetErrorLog(ErrorLog.IErrorLog errorLog)
        {
            _errorLog = errorLog;
        }
        public int SetPath(String path)
        {
            try
            {
                this._path = path;
                if (System.IO.File.Exists(path))
                {
                    _image = new Bitmap(path, true);
                }
                else
                {
                    _errorLog.AddErrorNotException(this.ToString() + "FileNotExists:" + path);
                    return -1;
                }
                return 1;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " setPath");
                return 0;
            }
        }

        public bool IsExistsImage()
        {
            try
            {
                if (this._image == null)
                {
                    return false;
                } else {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " isExistsImage");
                return false;
            }
        }

        public Image GetImage()
        {
            try
            {
                if (!IsExistsImage()) { _errorLog.AddErrorNotException(this.ToString() + " getImage : isExistsImage : false"); }
                return _image;
            }
            catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " getImage");
                return null;
            }
        }

        public Size GetSize()
        {
            try
            {
                if (!IsExistsImage()) { _errorLog.AddErrorNotException(this.ToString() + " getSize : isExistsImage : false"); }
                return _image.Size;
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " isExistsImage");
                return new Size(0,0);
            }
        }

        public string GetPath()
        {
            try
            {
                if (_path != "")
                {
                    return _path;
                }
                return "";
            } catch (Exception ex)
            {
                _errorLog.AddException(ex, this.ToString() + " getPath");
                return "";
            }
        }
    }
}
