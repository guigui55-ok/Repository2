using ImageViewer.Functions;
using System.Windows.Forms;

namespace ImageViewer.Function
{
    public class CommonFunctions
    {
        public ErrorLog.IErrorLog ErrorLog;
        public ViewImageControlFunction ForControl;
        public ViewImageBasicFunction ForViewBasic;
        public MainFormFunction ForMainForm;
        public FileListFunction ForFileList;
        public ViewImageObjects ViewImageObjects;

        public ViewImageManager ViewImageManager;

        public CommonFunctions(ErrorLog.IErrorLog errorlog,
            ViewImageManager viewImageManager, ViewImageObjects viewImageObjects)
        {
            this.ErrorLog = errorlog;
            this.ViewImageManager = viewImageManager;
            this.ViewImageObjects = viewImageObjects;
            ForControl = new ViewImageControlFunction(
                errorlog,
                viewImageObjects.Controls.ViewInnerControl.ViewImageControl,
                viewImageObjects.Controls.ViewInnerControl,
                viewImageObjects.Controls.ViewFrameControl,
                viewImageObjects);

            ForViewBasic = new ViewImageBasicFunction(errorlog, viewImageManager);
            //ForViewBasic = viewImageManager.BasicFunction;
            ForMainForm = new MainFormFunction(errorlog,
                viewImageObjects.MainForm, viewImageManager.MainControls.ContentsControl, viewImageManager);
            ForFileList = new FileListFunction(errorlog, ViewImageObjects.FileList);


        }
        public void TestFunction(string value)
        {
            MessageBox.Show(value);
        }
    }
}
