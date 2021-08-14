using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelWorkbookList
{
    public class WorkbookListCheckedListBox : IWorkbookListControl
    {
        protected ErrorManager.ErrorManager _err;
        protected CheckedListBox CheckedListBox;
        public Control Control { get => (Control)CheckedListBox; }
        public WorkbookListCheckedListBox (ErrorManager.ErrorManager err,CheckedListBox checkedListBox)
        {
            _err = err;
            this.CheckedListBox = checkedListBox;
        }

        public void ClearItems()
        {
            try
            {
                _err.AddLog(this, "ClearItems");
                if (this.CheckedListBox == null) { _err.AddLogWarning("GetIndexListSelecteditem"); return; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return; }

                while(CheckedListBox.Items.Count > 0)
                {
                    CheckedListBox.Items.RemoveAt(0);
                }

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "ClearItems");
            }
        }
        public void SetWorkbookList(List<string> list)
        {
            try
            {
                _err.AddLog(this, "SetWorkbookList");
                ClearItems();
                if (_err.hasAlert) { return; }
                if (list == null) { _err.AddLogWarning(" list == null"); return; }
                if(list.Count < 1) { _err.AddLogWarning(" list.Count < 1"); return; }
                this.CheckedListBox.Items.AddRange(list.ToArray());
            } catch(Exception ex)
            {
                _err.AddException(ex,this, "SetWorkbookList");
            }
        }

        public List<int> GetIndexListSelectedItem()
        {
            List<int> retList = new List<int>();
            try
            {
                _err.AddLog(this, "GetIndexListSelecteditem");
                if (this.CheckedListBox == null) { _err.AddLogWarning("GetIndexListSelecteditem"); return retList; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return retList; }

                for (int i = 0; i < CheckedListBox.Items.Count; i++)
                {
                    if (CheckedListBox.GetItemChecked(i))
                    {
                        retList.Add(i);
                    }
                }
                return retList;
            } catch(Exception ex)
            {
                _err.AddException(ex, this, "GetIndexListSelecteditem");
                return retList;
            }
        }


    }
}
