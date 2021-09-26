using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExcelWorkbookList
{
    public class WorkbookListCheckedListBox : IWorkbookListControl
    {
        protected ErrorManager.ErrorManager _err;
        protected CheckedListBox CheckedListBox;
        public UpdateControlAsync _updateControlAsync;
        public EventHandler DoubleClickedEvent;
        protected List<string> _ListforAdd;
        public Control Control { get { return (Control)CheckedListBox; } }
        public WorkbookListCheckedListBox (ErrorManager.ErrorManager err,Form form,CheckedListBox checkedListBox)
        {
            _err = err;
            this.CheckedListBox = checkedListBox;
            _updateControlAsync = new UpdateControlAsync(_err, form);
            _updateControlAsync.UpdateControlAction = this.AddItemToCheckdListBoxItems;
        }

        public List<string> GetValueList()
        {
            List<string> retList = new List<string>();
            try
            {
                _err.AddLog(this, "GetValueList");
                if (this.CheckedListBox == null) { _err.AddLogWarning(" GetIndexListSelecteditem"); return retList; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return retList; }

                for (int i = 0; i < CheckedListBox.Items.Count; i++)
                {
                    retList.Add(CheckedListBox.Items[i].ToString());
                }
                return retList;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetValueList");
                return retList;
            }
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
                if (_err.hasAlert) { _err.AddLog("ClearItems Failed. return");  return; }
                if (list == null) { _err.AddLogWarning(" list == null"); return; }
                if(list.Count < 1) { _err.AddLogWarning(" list.Count < 1"); return; }
                _ListforAdd = list;
                //this.CheckedListBox.Items.AddRange(list.ToArray());
                // Action <= AddItemToCheckdListBoxItems
                _updateControlAsync.ExcuteUpdateControlBySubThread();
                _err.AddLog(" CheckedListBox.Items.AddRange List.count="+list.Count);
            } catch(Exception ex)
            {
                _err.AddException(ex,this, "SetWorkbookList");
            }
        }

        private void AddItemToCheckdListBoxItems()
        {
            try
            {
                _err.AddLog(this, "AddItemToCheckdListBoxItems");
                ClearItems();
                if (_err.hasError) { _err.ReleaseErrorState(); }
                CheckedListBox.Items.AddRange(this._ListforAdd.ToArray());
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "AddItemToCheckdListBoxItems");
            }
        }

        public string GetItemValue(int index)
        {
            try
            {
                _err.AddLog(this, "GetIndexListSelecteditem");
                if (this.CheckedListBox == null) { _err.AddLogWarning("GetIndexListSelecteditem"); return ""; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return ""; }

                return CheckedListBox.Items[index].ToString();

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetItemValue");
                return "";
            }
        }

        public ExcelUtility.AppsInfo ConvertToAppsInfoFromItemValue(string value)
        {
            try
            {
                _err.AddLog(this, "ConvertToAppsInfoFromItemValue");
                ExcelUtility.AppsInfo info = new ExcelUtility.AppsInfo();
                int pos = value.IndexOf(']');
                string pidstr = value.Substring(1, pos-1-1);
                int pid;
                if(!int.TryParse(pidstr,out pid))
                {
                    pid = 0;
                }
                pos += 2;
                string bookname = value.Substring(pos,value.Length - pos);
                info.FileName = bookname;
                info.ProcessId = pid;

                return info;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetIConvertToAppsInfoFromItemValuetemValue");
                return null;
            }
        }

        public string[] GetSelectedItems()
        {
            string[] retAry = new string[] { };
            try
            {
                _err.AddLog(this, "GetSelectedItems");
                if (this.CheckedListBox == null) { _err.AddLogWarning("GetIndexListSelecteditem"); return retAry; }
                if (this.CheckedListBox.Items.Count < 1) { _err.AddLogWarning(" CheckedListBox.Items.Count < 1)"); return retAry; }

                for (int i = 0; i < CheckedListBox.Items.Count; i++)
                {
                    if (CheckedListBox.GetItemChecked(i))
                    {
                        Array.Resize(ref retAry, retAry.Length + 1);
                        retAry[retAry.Length-1] = CheckedListBox.Items[i].ToString();
                    }
                }
                return retAry;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "GetSelectedItems");
                return retAry;
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
