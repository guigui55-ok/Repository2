using CommonUtility.ControlUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlUtlitySample
{
    public partial class ControlUtilitySampleForm : Form
    {
        ErrorManager.ErrorManager _err;
        ControlUtility _util;
        public ControlUtilitySampleForm()
        {
            InitializeComponent();
            _err = new ErrorManager.ErrorManager(1);
            _util = new ControlUtility(_err);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form modalForm = new ControlUtilitySampleModalForm(_err,_util,this);
            modalForm.ShowDialog();
        }
    }
}
