using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer3
{
    public partial class ImageViewerForm : Form
    {
        public ErrorLog.IErrorLog ErrorLog;
        public ImageViewer.ViewImageManager ViewImageManager;
        public ImageViewerForm()
        {
            InitializeComponent();
        }



        private void ImageViewerForm_Load(object sender, EventArgs e)
        {

            // setPath
            string path = @"J:\ZMyFolder_2\jpgbest\To LOVEる ダークネス 第05巻00175.jpeg";
            //path = @"J:\ZMyFolder_2\jpgbest\gif_png_bmp\gif\160501001.gif";

            List<string> list = new List<string>
                {
                    path
                };
            ViewImageManager.BasicFunction.ViewImageAfterSetPath(
                ViewImageManager.ViewImageObjectList[0], list);

            ViewImageManager.BasicFunction.InitializeViewImageObjects(ViewImageManager.ViewImageObjectList[0]);
        }
    }
}
