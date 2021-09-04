using ControlUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewImageObjects;

namespace ImageViewer4
{
    public partial class ImageViewerTestForm : Form
    {
        protected ErrorManager.ErrorManager _err;
        public ImageViewerTestForm()
        {
            InitializeComponent();
            _err = new ErrorManager.ErrorManager(1);
        }

        private void ImageViewerTestForm_Load(object sender, EventArgs e)
        {
            try
            {
                TestViewImage();
            } catch (Exception ex)
            {
                Console.WriteLine("ImageViewerTestForm_Load");
                Console.WriteLine(ex.Message);
            }
        }

        IViewImage viewImage;
        IViewImageControl ViewImageControl;
        ReadFileByDragDrop readFileByDragDrop;
        public void TestViewImage()
        {
            try
            {
                Console.WriteLine("TestViewImage");
                // panel
                Panel panel1 = new Panel();
                panel1.Parent = this;
                panel1.Size = this.Size;
                panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                //panel1.Anchor = AnchorStyles.None;
                panel1.Dock = DockStyle.Fill;
                panel1.BackColor = Color.Gray;

                // picturebox
                PictureBox pictureBox1 = new PictureBox();
                pictureBox1.Parent = panel1;
                pictureBox1.Size = panel1.Size;
                pictureBox1.Anchor = AnchorStyles.None;

                // setPath
                string dir = @"C:\Users\OK\source\repos\ImageViewer4\TestData\test_jpg";
                string filename = "01_4a44d3544f1f00b7d1ba958974b0d9eb_w.jpg";
                string path = dir + "\\" + filename;

                // viewImageObject 画像をコントロールに表示する
                viewImage = new ViewImage(_err);
                viewImage.SetPath(path);
                ViewImageControl = new ViewImageControlPictureBox(_err, panel1, pictureBox1);
                // 画像を表示する
                ViewImageControl.SetImageWithDispose(viewImage.GetImage());
                //ViewImageControl.SetImageNotDispose(viewImage.GetImage());

                // PictureBox をドラッグする
                ControlDragger dragger = new ControlDragger(_err, pictureBox1, pictureBox1);

                // Panel のクリックされた位置が右側半分か左側半分か判定する
                JudgeClickRightOrLeft judgeClickRightOrLeft = new JudgeClickRightOrLeft(_err, panel1);

                // PictureBox のクリックされた位置が Panel の右側半分か左側半分か判定する
                JudgeClickRightOrLeftChild judgeClickRightOrLeftChild =
                    new JudgeClickRightOrLeftChild(_err, pictureBox1, panel1);

                // Panel が MouseWheel を受けたとき、PictureBox の大きさを変更する
                ChangeSizeByMouseWheel changeSizeByMouseWheel = new ChangeSizeByMouseWheel(_err, pictureBox1, panel1);

                // DragAndDrop でファイルを読み込む
                readFileByDragDrop = new ReadFileByDragDrop(_err, this, ReadFileEvent);
                // Control をイベントに追加
                Control[] controls = new Control[] { this, pictureBox1, panel1 };
                readFileByDragDrop.AddMethodToEventHandler(controls);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
        }
        public void ReadFileEvent(object sender, EventArgs e)
        {
            try
            {
                _err.AddLog(this, "ReadFileEvent");
                viewImage.SetPath(readFileByDragDrop.files.GetCurrentValue());
                // 画像を表示する
                ViewImageControl.SetImageWithDispose(viewImage.GetImage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
