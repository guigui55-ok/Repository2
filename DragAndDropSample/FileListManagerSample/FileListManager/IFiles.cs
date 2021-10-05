using System;
using System.Collections.Generic;

namespace CommonUtility.FileListUtility
{
    public interface IFiles
    {
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

        bool IsLastIndex();
        bool IsFirstIndex();

    }
}
