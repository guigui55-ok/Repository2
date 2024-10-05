using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;
using System.Drawing;
using ViewImageModule;
using CommonControlUtilityModule;
using ImageViewer5.CommonModules;
using CommonControlUtilityModuleB;
using TransportForm;

namespace ImageViewer5.ImageControl
{
    /// <summary>
    /// 1つのコントロールやイメージ処理を、取りまとめて扱うクラス
    /// ViewImage、外枠UserControl、内側PictureBoxの、機能や連携処理を扱う
    /// </summary>
    public class ImageMainClass
    {
        public PictureBox _pictureBox;
        public Control _parentControl;
        protected AppLogger _logger;
        public string Name;
        //# 
        // Member
        public IViewImage _viewImage;
        public IViewImageControl _viewImageControl;
        public IViewImageFrameControl _viewImageFrameControl;
        public ReadFileByDragDrop _readFileByDragDrop;
        public ViewImageFunction _viewImageFunction;

        ControlDraggerB _draggerFrame;
        ControlDraggerB _draggerInner_ToInner;
        ControlDraggerB _draggerInner_ToFrame;
        FormDragger _formDraggerByForm;
        FormDragger _formDraggerByFrame;
        //
        FormDragger _formDraggerByInner;

        TransparentFormSwitch _transparentFormSwitch;
        //
        public bool _isFocusFrame = false;

        /// <summary>
        /// ViewImageのコントロールや機能を取りまとめるクラス
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="formMain"></param>
        /// <param name="parentControl"></param>
        /// <param name="pictureBox"></param>
        public ImageMainClass(AppLogger logger,Form formMain, Control parentControl, PictureBox pictureBox, int componentNumber)
        {
            _logger = logger;
            _pictureBox = pictureBox;
            _parentControl = parentControl;
            SetImageSettings();
            _viewImage = new ViewImage(_logger);
            _viewImageControl = new ViewImageControlPictureBox(_logger, parentControl, pictureBox);
            _viewImageFrameControl = new ViewImageFrameControlForm(_logger, formMain);
            _viewImageFunction = new ViewImageFunction(
                _logger, _viewImage, _viewImageFrameControl, _viewImageControl, (ImageMainFrame)parentControl);
            this.Name = "ImageMainClass" + componentNumber;


        }
        private void SetImageSettings()
        {
            try
            {
                _pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "SetImageSettings");
            }
        }

        /// <summary>
        /// クラス内の状態を初期化する
        /// （FormMain_Load（またはそのすぐあと）で実行される想定）
        /// （各種イベントメソッドとの紐づけを行っている）
        /// </summary>
        /// <param name="initPath"></param>
        public void InitializeValues(List<string> SupportedImageExtList)
        {
            _logger.PrintInfo(this.Name + " > InitializeValues");

            // PictureBox をドラッグする
            //ControlDragger dragger = new ControlDragger(_logger, _pictureBox, _pictureBox);
            _draggerInner_ToInner = new ControlDraggerB(_logger, _pictureBox, _pictureBox, new SwitchKeys(Keys.Space));
            Control frame = _viewImageControl.GetParentControl();
            Form form = _viewImageFrameControl.GetParentForm();
            _draggerInner_ToFrame = new ControlDraggerB(_logger, frame, _pictureBox, null);
            //_draggerInner_ToFrame = new ControlDraggerB(_logger, frame, _pictureBox, new SwitchKeys(Keys.None, Keys.Control, true));
            //SwitchKeys bufKey = new SwitchKeys(Keys.None, Keys.Control, true); //Controlではなく ControlKey 240920
            SwitchKeys bufKey = new SwitchKeys(Keys.None, Keys.ControlKey, true);
            _draggerFrame = new ControlDraggerB(_logger, frame, frame, bufKey);
            _formDraggerByForm = new FormDragger(_logger, form, form);
            _formDraggerByFrame = new FormDragger(_logger, form, frame);
            //
            _formDraggerByInner = new FormDragger(_logger, form, _pictureBox);

            _transparentFormSwitch = new TransparentFormSwitch(_logger, form, frame);
            _transparentFormSwitch.SetDraggerFlags(
                ref _formDraggerByForm._isDragEnable,
                ref  _formDraggerByFrame._isDragEnable,
                ref _draggerFrame._isDragEnable,
                ref _draggerInner_ToInner._isDragEnable,
                ref _formDraggerByInner._isDragEnable,
                ref _draggerInner_ToFrame._isDragEnable);
            _transparentFormSwitch._moveInnerKey = _draggerInner_ToInner._switchKeys;
            _transparentFormSwitch._moveFrameKey = _draggerFrame._switchKeys;
            //_transparentFormSwitch._moveFrameKey = _draggerInner_ToFrame._switchKeys;

            // Panel のクリックされた位置が右側半分か左側半分か判定する
            JudgeClickRightOrLeft judgeClickRightOrLeft = new JudgeClickRightOrLeft(_logger, _parentControl);

            // PictureBox のクリックされた位置が Panel の右側半分か左側半分か判定する
            JudgeClickRightOrLeftChild judgeClickRightOrLeftChild =
                new JudgeClickRightOrLeftChild(_logger, _pictureBox, _parentControl);
            judgeClickRightOrLeftChild.ClickRight += PictureBox_ClickRightEvent;
            judgeClickRightOrLeftChild.ClickLeft += PictureBox_ClickLeftEvent;

            // Panel が MouseWheel を受けたとき、PictureBox の大きさを変更する
            //ChangeSizeByMouseWheel changeSizeByMouseWheel = new ChangeSizeByMouseWheel(
            //    _logger, _pictureBox, _parentControl);
            ChangeSizeByMouseWheelWithMousePointer _changeSizeByMouseWheel = new ChangeSizeByMouseWheelWithMousePointer(
                _logger, _pictureBox, _parentControl);
            //ChangeSizeByMouseWheelWithMousePointer changeSizeByMouseWheelWithMousePointer;

            _viewImageFunction.InitializeValue();
            _transparentFormSwitch.SwitchFlagsByTransparencyKey(false);
            _transparentFormSwitch.SwitchDefault();
        }

        public void ShowImageThisPath()
        {
            _logger.PrintInfo(this.Name + " > ShowImageThisPath");
            //
            _viewImageFunction._imagePlayer.ViewImageMain();
        }

        /// <summary>
        /// Initialize処理が終わった後に、最初に画像を表示する
        /// FormMain_Loadの最後か、すぐ後に実行される想定
        /// </summary>
        /// <param name="path"></param>
        public void ShowImageAfterInitialize(string path)
        {
            _logger.PrintInfo(this.Name + " > ShowImageAfterInitialize");
            _viewImageFunction._imagePlayer.ViewImageMain();
        }

        public void PictureBox_ClickRightEvent(object sender, EventArgs e)
        {
            //_readFileByDragDrop.FileListManager.MoveNextFileWhenLastFileNextDirectory();
            ImageMainFrame imageMainFrame = (ImageMainFrame)_parentControl;
            imageMainFrame._formFileList._fileListManager.MoveNextFileWhenLastFileNextDirectory();
            ReadFileEvent(null, EventArgs.Empty);
        }
        public void PictureBox_ClickLeftEvent(object sender, EventArgs e)
        {
            //_readFileByDragDrop.FileListManager.MoveProviousFileWhenFirstFilePreviousDirectory();
            ImageMainFrame imageMainFrame = (ImageMainFrame)_parentControl;
            imageMainFrame._formFileList._fileListManager.MoveProviousFileWhenFirstFilePreviousDirectory();
            ReadFileEvent(null, EventArgs.Empty);
        }

        public void ReadFileEvent(object sender, EventArgs e)
        {
            try
            {
                _logger.PrintInfo(this.Name + " > ReadFileEvent");
                _viewImageFunction._imagePlayer.ViewImageMain();
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, "ReadFileEvent Error");
            }
        }

        public int ChangeImageByPath(String path)
        {
            try
            {
                int flag = _viewImage.SetPath(path);
                SetImageWithDispose(_viewImage.GetImage());
                return 1;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "setPath");
                return 0;
            }
        }

        public void SetImageWithDispose(Image image)
        {
            try
            {
                if (_pictureBox == null)
                {
                    _pictureBox.Image.Dispose();
                }
                _pictureBox.Image = image;
            }
            catch (Exception ex)
            {
                _logger.AddException(ex, this, "SetImageWithDispose");
            }
        }

    }
}
