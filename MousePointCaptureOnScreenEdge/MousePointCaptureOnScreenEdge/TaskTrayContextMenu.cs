using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormUtility
{
    public class TaskTrayContextMenu
    {
        protected ErrorManager.ErrorManager _error;
        ContextMenuStrip _contextMenuStrip;
        ToolStripMenuItem _toolStripMenuItem;
        public TaskTrayContextMenu(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        public ContextMenuStrip GetContextMenuStrip()
        {
            return _contextMenuStrip;
        }

        public void Initialize()
        {
            try
            {
                _contextMenuStrip = new ContextMenuStrip();
                _toolStripMenuItem = new ToolStripMenuItem();
                _toolStripMenuItem.Text = "&終了";
                _toolStripMenuItem.Click += ToolStripMenuItem_Click;
                _contextMenuStrip.Items.Add(_toolStripMenuItem);                

            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+ ".Initialize");
            }
        }

        private void ToolStripMenuItem_Click(object sender,EventArgs e)
        {
            Application.Exit();
        }

    }
}
