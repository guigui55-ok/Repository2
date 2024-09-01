using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlUtility.SelectFiles
{
    public interface IFiles
    {
        EventHandler ChangedFileList { get; set; }
        List<string> FileList { get; set; }
        void MoveNext();
        void MovePrevious();
        string GetCurrentValue();
        List<string> GetList();

    }
}
