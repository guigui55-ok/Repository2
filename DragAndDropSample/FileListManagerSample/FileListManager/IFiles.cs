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
        EventHandler SelectedFileEvent { get; set; }

        void ChangeFileRecieve(object sender, EventArgs e);
        void SelectedFileEventInThis(object sender, EventArgs e);
        void UpdateFileList(List<string> list);
        List<string> FileList { get; set; }

        //240901 現状で不要なのでコメントアウト。外部で絞込をする。
        //List<string> SupportedImageExtensionList { get; set; }
        void Move(string value);
        void Move(int index);
        void MoveNext();
        void MovePrevious();
        string GetCurrentValue();
        List<string> GetList();

        string StringJoinList(int index=-1);

        int Count();

        bool IsLastIndex();
        bool IsFirstIndex();

    }
}
