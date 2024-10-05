using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer5
{
    public static class ImageViewerConstants
    {
        public static readonly List<string> SUPPORTED_IMAGE_EXTENTION_DEFAULT_LIST = new List<string>{
            @"\.jpg$",
            @"\.jpeg$",
            @"\.png$",
            @"\.bmp$",   // Bitmap Image
            @"\.gif$",   // GIF Image
            @"\.tiff$",  // TIFF Image
            @"\.tif$",   // TIFF Image (alternate extension)
            @"\.webp$",   // WebP Image
        };
        // @"\.ico",

        public static readonly string IMAGE_VIEWER_SETTING_FILE_NAME = "settingImageViewer.json";
        /*
         * 複数のFrameを表示するときの設定値
         */
        // 配置する方向
        public static readonly int TO_RIGHT = 0;
        public static readonly int TO_LEFT = 1;
        public static readonly int TO_BOTTOM = 0;
        public static readonly int TO_TOP = 1;
        // 列方向、行方向どちらを優先して廃止していくか（上記で足りるかも）
        public static readonly int FRAME_PRIORITY_HORIZON = 0;
        public static readonly int FRAME_PRIORITY_VERTICAL = 1;
        // 行の表示Max、列の表示Max
        public static readonly int FRAME_ROW_MAX_DEFAULT = 2;
        public static readonly int FRAME_COL_MAX_DEFAULT = -1;
        // frameを再配置するとき、Frameのサイズを固定する、Formのサイズを固定する
        public static readonly int FIX_FRAME_SIZE = 0;
        public static readonly int FIX_FORM_SIZE = 1;

        //
        public static readonly string FRAME_NAME_BASE = "imageMainFrame";

    }

    public static class SettingValueDefault
    {
        // #####
        // MainForm
        public static readonly bool RESTORE_PREV_STATE = true;
        public static readonly Size MAIN_FORM_SIZE = new Size(480, 640); //VGA 640x480
        public static readonly Point MAIN_FORM_LOC = new Point(100, 100);
        public static readonly int FRAME_ROW_MAX = 2;
        public static readonly int FRAME_COL_MAX = -1;
        // #####
        // ImageMainFrame
        public static readonly string FRAME_NAME = "imageMainFrame";
        public static readonly bool RESTORE_PREV_FRAME = true;
        public static readonly Size FRAME_SIZE = new Size(480, 640);
        public static readonly Point FRAME_LOC = new Point(0, 0);
        public static readonly Size IMAGE_CONTROL_SIZE = new Size(480, 640); // ImageControl=PictureBox
        public static readonly Point IMAGE_CONTROL_LOC = new Point(0, 0);
        public static readonly string CURRENT_VALUE = "";
        public static readonly int CURRENT_INDEX = 0;
        public static readonly bool INCLUDE_SUB_DIR_FILE = false;
        public static readonly bool FILE_LIST_RANDOM = false;
        public static readonly bool SLIDE_SHOW_ON = false;
        public static readonly int SLIDE_SHOW_INTERVAL = 3000;
        public static readonly bool FIT_IMAGE_TO_FRAME = true;
        public static readonly bool LINK_SIZE_WITH_FORM = true;
        public static readonly bool LINK_LOC_WITH_FORM = true;
        public static readonly bool SHOW_LIST_SUB_WINDOW = false;
    }


    public static class SettingKey
    {

        public static readonly string IS_LOGOUT_SETTING_JSON = "IsLogoutSettingJson";
        // ##########
        // FormMain
        public static readonly string APP_PATH = "AppPath";
        public static readonly string RESTORE_PREV_STATE = "RestorePreviousState";
        public static readonly string MAIN_FORM_SIZE = "MainFormSize";
        public static readonly string MAIN_FORM_LOC = "MainFormLoc";
        //
        public static readonly string FRAME_DIRECTION_HORIZON = "FrameDirectionHorizon"; //右or左
        public static readonly string FRAME_DIRECTION_VERTICAL = "FrameDirectionVertical";  //上or下
        public static readonly string FRAME_PREFERRED_DIRECTION = "FramePreferredDirection"; //優先する方向（縦、横）
        public static readonly string FRAME_ROW_MAX = "FrameRowMax";
        public static readonly string FRAME_COL_MAX = "FrameColMax";

        //public static readonly string FIX_FRAME_SIZE = "FixFrameSize";
        public static readonly string FRAME_COUNT = "FrameCount";

        public static readonly string SHOW_SENDER_DIALOG = "ShowSenderDialog";

        // ##########
        // ImageMainFrame用 (Frame=ImageMainFrame=UserControl)
        public static readonly string FRAME_NAME = "FrameName";
        public static readonly string RESTORE_PREV_FRAME = "RestorePreviousStateFrame";　//元のディレクトリを復元する
        public static readonly string RESTORE_PREV_DIR = "RestorePreviousDir"; 
        public static readonly string FRAME_SIZE = "FrameSize";
        public static readonly string FRAME_LOC = "FrameLoc";
        public static readonly string IMAGE_CONTROL_SIZE = "ImageControlSize"; // ImageControl=PictureBox
        public static readonly string IMAGE_CONTROL_LOC = "ImageControlLoc";
        public static readonly string CURRENT_VALUE = "CurrentValue"; //CurrentPath
        //ファイルが増えたり移動しているかもしれないので使用する予定はないが、念のため
        public static readonly string CURRENT_INDEX = "CurrentIndex";
        public static readonly string INCLUDE_SUB_DIR_FILE = "IncludeSubDirFile";
        public static readonly string FILE_LIST_RANDOM = "FileListRandom";
        public static readonly string SLIDE_SHOW_ON = "SlideShowON";
        public static readonly string SLIDE_SHOW_INTERVAL = "SlideShowInterval";
        //
        public static readonly string FIT_IMAGE_TO_FRAME = "FitImageToFrame"; // ??FixFrameSize
        public static readonly string LINK_SIZE_WITH_FORM = "LinkSizeWithForm";
        public static readonly string LINK_LOC_WITH_FORM = "LinkLocWithForm";

        public static readonly string SHOW_LIST_SUB_WINDOW = "ShowListSubWindow";



    }
}
