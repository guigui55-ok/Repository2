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
    public partial class ControlUtilitySampleModalForm : Form
    {
        ErrorManager.ErrorManager _err;
        ControlUtility _util;
        Form _parentForm;
        public ControlUtilitySampleModalForm(ErrorManager.ErrorManager err,ControlUtility util,Form parentForm)
        {
            InitializeComponent();
            _err = err;
            _util = util;
            _parentForm = parentForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                List<Control> list = _util.GetControlListMatchType(_parentForm, typeof(Form));
                textBox1.Text = list.Count.ToString();

                textBox1.Text = Form.ActiveForm.Text;

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "button2_Click");
            }
        }
    }
}
