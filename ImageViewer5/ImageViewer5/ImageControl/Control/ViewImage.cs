using System;
using System.Drawing;
using AppLoggerModule;

namespace ViewImageModule
{
    public class ViewImage : IViewImage
    {
        public AppLogger _logger;
        private Image _image;
        String _path;

        public ViewImage(AppLogger logger)
        {
            _logger = logger;
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
                    _logger.AddLogAlert(this, "FileNotExists:" + path);
                    return -1;
                }
                return 1;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "setPath");
                return 0;
            }
        }

        public int SetImage(Image image)
        {
            try
            {
                _image = image;
                return 1;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "SetImage");
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
                _logger.AddException(ex, this, "isExistsImage");
                return false;
            }
        }

        public Image GetImage()
        {
            try
            {
                if (!IsExistsImage()) { _logger.AddLogAlert(this, "getImage : isExistsImage : false"); }
                return _image;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetImage");
                return null;
            }
        }

        public Size GetSize()
        {
            try
            {
                if (!IsExistsImage()) { _logger.AddLogAlert(this,"getSize : isExistsImage : false"); }
                return _image.Size;
            } catch (Exception ex)
            {
                _logger.AddException(ex, this, "GetSize");
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
                _logger.AddException(ex, this, "GetPath");
                return "";
            }
        }

        public bool ImageIsNull()
        {
            try
            {
                if (_image != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "ImageIsNull");
                return true;
            }
        }

        public bool DisposeImage()
        {
            try
            {
                _image?.Dispose();
                _image = null;
                return true;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "DisposeImage");
                return false;
            }
        }

    }
}
