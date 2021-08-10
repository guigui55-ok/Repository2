using FormUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MousePointCapture
{
    public partial class MousePointCaptureOnScreenEdgeForm : Form
    {
        TaskTray _taskTray;
        TaskTrayContextMenu _taskTrayContextMenu;
        //MouseCaptureInScreenEdgeEvents _mousePointCaptureMouseEvents;
        protected ErrorManager.ErrorManager _error;
        protected MousePointCaptureOnScreenEdgeManager _captureFormManager;
        protected MousePointCaptureOnScreenEdgeFormEvents _edgeFormEvents;
        public MousePointCaptureOnScreenEdgeForm()
        {
            try
            {
                InitializeComponent();
                _error = new ErrorManager.ErrorManager(1);
                _taskTray = new TaskTray(_error);
                if (_error.HasException())
                { MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "TaskTray.Constracta Failed."); }

                Console.WriteLine("====== Debug ======");
                string iconPath = System.IO.Directory.GetCurrentDirectory() + @"\clear_night_icon_188052.ico";
                Console.WriteLine("iconPath = " + iconPath);
                string iconText = "MousePointCapture";

                _taskTrayContextMenu = new TaskTrayContextMenu(_error);
                _taskTrayContextMenu.Initialize();
                if (_error.HasException())
                { MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "TaskTrayContextMenu.Initialize Failed."); }

                _taskTray.Initialize(iconPath,iconText);
                if (_error.HasException())
                { MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "TaskTray.Initialize Failed."); }

                _taskTray.GetNotifyIcon().ContextMenuStrip = _taskTrayContextMenu.GetContextMenuStrip();

                // MouseEvents を初期化
                //_mousePointCaptureMouseEvents = new MousePointCaptureMouseEvents(_error, this);

                // CaptureFormManager を初期化する
                _captureFormManager = new MousePointCaptureOnScreenEdgeManager(_error);
                _captureFormManager.InitializeForms();
                if (_error.HasException()) { throw new Exception("CaptureFormManager.Initialize Failed."); }

                // EdgeForm に Event を紐づける 
                _edgeFormEvents = new MousePointCaptureOnScreenEdgeFormEvents(_error, _captureFormManager, this);
                if (_error.HasException()) { throw new Exception("EdgeFormEvents Constracta Failed"); }

                // EdgeForm を表示する
                _captureFormManager.SetFormVisible(true,true,true,true);
                _captureFormManager.ShowForms();
                if (_error.HasException()) { throw new Exception("CaptureFormManager Method Failed"); }


                _captureFormManager.EdgeFormEvents = _edgeFormEvents;
                // Reset
                int[] left = new int[] {100,101 };
                int[] top = new int[] { 200,201 };
                int[] right = new int[] { 300, 301 };
                int[] bottom = new int[] { 400,401 };
                int[] scr = new int[] { 1, 1, 1, 1 };
                _captureFormManager.ResetSettings(scr,left,top,right,bottom);
                if (_error.HasException()) { throw new Exception("ResetSettings Failed"); }

            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".mousePointCaptureForm");
                MessageBox.Show(_error.GetExceptionMessageAndStackTrace(), "mousePointCaptureForm constractae Failed.");
            }
        }

        private void MousePointCaptureForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    Console.WriteLine("Keys.Enter");
                    //if (this.Visible) { this.Visible = false; } else { this.Visible = true; }
                    //this.Hide();
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.Visible = false;
            this.Hide();
        }



    }
}
