using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace ImageViewer.ParentForms
{
    public class ToolStripLiistForRegist : List<ToolStripItemForRegist>
    {
        public ToolStripLiistForRegist GetListCopied()
        {
            try
            {
                ToolStripLiistForRegist retlist = new ToolStripLiistForRegist();
                if (this.Count < 1)
                {
                    return retlist;
                }
                foreach (var val in this)
                {
                    retlist.Add(val);
                }
                return retlist;
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".Add failed");
                Debug.WriteLine(ex.Message);
                return new ToolStripLiistForRegist();
            }
        }
        public void Add(string value, Keys keys, bool shorCut)
        {
            try
            {
                this.Add(new ToolStripItemForRegist(value, keys, shorCut));
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString()+".Add failed");
                Debug.WriteLine(ex.Message);
            }
        }

        public void Add(string value,Keys keys)
        {
            try
            {
                this.Add(new ToolStripItemForRegist(value,keys));
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".Add failed");
                Debug.WriteLine(ex.Message);
            }
        }

        public void Add(string value)
        {
            try
            {
                this.Add(new ToolStripItemForRegist(value));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".Add failed");
                Debug.WriteLine(ex.Message);
            }
        }
    }

    public class ToolStripItemForRegist
    {
        public string Value;
        public Keys Keys;
        public bool IsShortCutKeys = false;

        public ToolStripItemForRegist(string value,Keys keys,bool shortCut)
        {
            Value = value;
            RegistValue(value, keys);
            if (shortCut) { }
        }
        public ToolStripItemForRegist(string value,Keys keys)
        {
            try
            {
                RegistValue(value, keys);
            } catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".ToolStripItemForRegist failed");
                Debug.WriteLine(ex.Message);
            }
        }
        public ToolStripItemForRegist(string value)
        {
            Value = value;
            Keys = Keys.None;
            IsShortCutKeys = false;
        }
        private void RegistValue(string value, Keys keys)
        {
            try
            {
                Value = value;
                if (value is null) { Debug.Print(keys.ToString()); }
                if (Keys.None == Keys)
                {
                    this.IsShortCutKeys = false;
                } else
                {
                    this.IsShortCutKeys = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".registValue failed");
                Debug.WriteLine(ex.Message);
            }
        }
    }

    
}
