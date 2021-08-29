using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    static class ImageViewerConstants
    {
        public static Size CONTROL_NEW_SIZE() { return new Size(200, 200); }
        // 拡大倍率
        public static readonly double EXPANTION_RAITO = 1.05;
        // 縮小倍率
        public static readonly double SHRINK_RAITO = 0.952;
        // Settings 読み込みファイルの種類
        static public readonly int SETTINGS_INI = 1;
        static public readonly int SETTINGS_JSON = 2;

        static public SettingsName SettingsName = new SettingsName();


        // エラー	CS0710	静的クラスにはコンストラクターを指定できません
        //public ImageViewerConstants() { settingsName = new settingsName(); }

    }

    class SettingsName
    {
        public readonly string IsMenuBarVisibleAlways = "IsMenuBarVisibleAlways";
        public readonly string IsMenuBarVisibleOnStartUp = "IsMenuBarVisibleOnStartUp";
    }
}
