using ImageViewer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer3
{
    static class ImageViewerMain
    {
        static ErrorLog.IErrorLog _errorLog;
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ImageViewerForm MainForm = new ImageViewerForm();

            // application initialize
            _errorLog = new ErrorLog.ErrorLog();
            MainForm.ErrorLog = _errorLog;
            // ------- Mainform、MainContents初期化　----------
            Panel mainContentsPanel = new Panel
            {
                Name = "MainContentsPanel",
                Size = ImageViewerConstants.CONTROL_NEW_SIZE()
            };
            MainForm.Controls.Add(mainContentsPanel);
            MainForm.Size = new System.Drawing.Size(480, 640);
            mainContentsPanel.Parent = MainForm;
            mainContentsPanel.Size = MainForm.ClientSize;
            // アンカー  左上に表示
            mainContentsPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            // フォームの大きさに追随して伸縮するようになる
            mainContentsPanel.Dock = DockStyle.Fill;
            //　--------------------------

            // ***** ViewImageManager *****
            ViewImageManager ViewImageManager = new ViewImageManager(
                _errorLog, MainForm, mainContentsPanel, new ImageViewerSettings(ImageViewerConstants.SETTINGS_INI));

            // BaseicFunctionは先にNewする
            ViewImageManager.BasicFunction = new ImageViewer.Functions.ViewImageBasicFunction(_errorLog, ViewImageManager);
            // ContentsControlFunctionは先にNewする
            ViewImageManager.MainControls.ContentsFunction = new ImageViewer.Functions.ContentsControlFunction(_errorLog,
                MainForm, ViewImageManager.MainControls.ContentsControl, ViewImageManager);

            // Initialize
            int ret = ViewImageManager.Initialize();
            if (ret < 1) { _errorLog.ShowErrorMessage(); return; }

            // ***** ViewImageObjects *****
            ViewImageObjects viewImageObjects = ViewImageManager.MakeViewImageObjects(MainForm, mainContentsPanel);
            ViewImageManager.ViewImageObjectList.Add(viewImageObjects);

            // 作った viewImageObjects を MainContents へ追加
            ViewImageManager.AddControlToMainContents(viewImageObjects);
            MainForm.ViewImageManager = ViewImageManager;

            //　--------------------------
            ViewImageManager.MainControls.MainFormManger.State.Initialize = false;
            // このあとMainForm_Loadへ
            Application.Run(MainForm);

            Debug.WriteLine("*** Finalize");
            ret = ViewImageManager.Finlize();
            if (ret < 1) { _errorLog.ShowErrorMessage(); return; }
        }
    }
}
