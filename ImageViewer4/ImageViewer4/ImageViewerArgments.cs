using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer4
{
    class ImageViewerArgments
    {
        private ErrorManager.ErrorManager _err;
        public string[] args;
        public string Path;
        public ImageViewerArgments(ErrorManager.ErrorManager err,string[] args)
        {
            _err = err;
            this.setValue(args);
        }

        public void setValue(string[] args)
        {
            try
            {
                //Console.WriteLine(args[0]);
                if (args.Length > 0)
                {
                    if (System.IO.File.Exists(args[0]))
                    {
                        this.Path = args[0];
                    }
                }
                return;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "SetValue");
                return;
            }
        }
    }
}
