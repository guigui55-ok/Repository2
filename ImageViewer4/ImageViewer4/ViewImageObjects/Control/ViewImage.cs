using System;
using System.Drawing;

namespace ViewImageObjects
{
    public class ViewImage : IViewImage
    {
        private Image _image;
        String _path;
        protected ErrorManager.ErrorManager _err;

        public ViewImage(ErrorManager.ErrorManager err)
        {
            _err = err;
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
                    _err.AddLogAlert(this, "FileNotExists:" + path);
                    return -1;
                }
                return 1;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "setPath");
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
                _err.AddException(ex, this, "isExistsImage");
                return false;
            }
        }

        public Image GetImage()
        {
            try
            {
                if (!IsExistsImage()) { _err.AddLogAlert(this, "getImage : isExistsImage : false"); }
                return _image;
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "GetImage");
                return null;
            }
        }

        public Size GetSize()
        {
            try
            {
                if (!IsExistsImage()) { _err.AddLogAlert(this,"getSize : isExistsImage : false"); }
                return _image.Size;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetSize");
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
                _err.AddException(ex, this, "GetPath");
                return "";
            }
        }
    }
}
