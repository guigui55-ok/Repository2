using CommonUtility.FileListUtility;
using System;

namespace CommonUtility.FileListUtility.FileListControl
{
    public interface IFileListControl
    {
        IFiles Files { get; set; }
        void UpdateFileListAfterEvent(object sender, EventArgs e);
        void SelectItem(object value);
        EventHandler SelectedItemEvent { get; set; }
    }
}
