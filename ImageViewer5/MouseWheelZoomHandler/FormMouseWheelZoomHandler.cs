using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonControlUtilityModuleB;
using AppLoggerModule;

namespace MouseWheelZoomHandler
{
    public partial class FormMouseWheelZoomHandler : Form
    {
        ChangeSizeByMouseWheelWithMousePointer _changeSizeByMouseWheelWithMousePointer;
        AppLogger _logger;
        public FormMouseWheelZoomHandler()
        {
            InitializeComponent();
            _logger = new AppLogger();
        }

        private void FormMouseWheelZoomHandler_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            string path = @"J:\ZMyFolder_2\jpgbest\test.jpeg";
            pictureBox1.Image = System.Drawing.Image.FromFile(path);
            // Panel が MouseWheel を受けたとき、PictureBox の大きさを変更する
            _changeSizeByMouseWheelWithMousePointer = new ChangeSizeByMouseWheelWithMousePointer(
                _logger, pictureBox1, panel1);
        }

        private void FormMouseWheelZoomHandler_FormClosed(object sender, FormClosedEventArgs e)
        {
            _logger.Dispose();
            _changeSizeByMouseWheelWithMousePointer.Dispose();
        }
    }
}
