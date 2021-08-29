using System.Collections.Generic;

namespace ImageViewer.Settings
{
    public interface ISettingsManager
    {
        int SetPath(string iniPath);
        int ReadSetingsFile();
        int SetPathAndCreateFileIfPathNotExists(string iniPath, string iniFormatString);
        int SetSectionFromPath();
        string GetParameterValue(string sectionName, string parametersName);
        int SetParameterValue(string sectionName, string parameterName, string value);
        List<string> GetSectionValues(string sectionName);
        int CountSection(string sectionName);
        int WriteAllData();

        bool HasException();
        string GetErrorMessages();
        void ClearError();

    }
}
