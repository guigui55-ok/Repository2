

namespace ExcelCellsManager.ExcelCellsManager.Settings
{
    public interface ISettingsRegister
    {
        string GetValue(string kindName, string subItemName);
        void SetPath(string path);
        void ReadSettings();
        void SetValue(string kindName, string subItemName, string value);
        void SaveValuesToFile();
    }
}
