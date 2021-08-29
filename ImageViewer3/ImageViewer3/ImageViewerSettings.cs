using System.IO;

namespace ImageViewer
{
    public class ImageViewerSettings
    {
        public string SettingsFilePath = "";
        public string SettingsFormatFilePath = "";
        public bool IsMenuBarVisibleOnStartUp = false;
        public bool IsMenuBarVisibleAlways = false;

        public ImageViewerSettings(int IniFileMode)
        {
            string settingsFolder = Directory.GetCurrentDirectory();
            // 基本は同じ場所
            SettingsFormatFilePath = Directory.GetCurrentDirectory() + @"\settingsFormat.txt";
            SettingsFilePath = settingsFolder + @"\settings";
            switch (IniFileMode)
            {
                case 1: SettingsFilePath += ".ini"; break;
                case 2: SettingsFilePath += ".json"; break;
                default:  break;
            }
        }
    }
}
