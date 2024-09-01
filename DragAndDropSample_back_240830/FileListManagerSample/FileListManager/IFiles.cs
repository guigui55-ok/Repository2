using System;
using System.Collections.Generic;
using AppLoggerModule;

namespace CommonUtility.FileListUtility
{
    public interface IFiles
    {
        AppLogger Logger { get; set; }
        string DirectoryPath { get; set; }
        EventHandler ChangedFileListEvent { get; set; }
        void SelectedFileEvent(object sender, EventArgs e);
        List<string> FileList { get; set; }
        void Move(string value);
        void Move(int index);
        void MoveNext();
        void MovePrevious();
        string GetCurrentValue();
        List<string> GetList();

        int Count();

        bool IsLastIndex();
        bool IsFirstIndex();

    }
}
