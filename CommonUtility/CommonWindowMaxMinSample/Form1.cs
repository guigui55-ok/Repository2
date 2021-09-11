using CommonUtility;
using CommonUtility.Pinvoke;
using ErrorManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonWindowMaxMinSample
{
    public partial class Form1 : Form
    {
        ErrorManager.ErrorManager _err;
        WindowControlUtility _windowUtility;
        ProcessUtility _processUtility;
        public Form1()
        {
            InitializeComponent();
            _err = new ErrorManager.ErrorManager(1);
            _windowUtility = new WindowControlUtility(_err);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(textBox1.Text == "") { Console.WriteLine("TextBox1.Text == \"\""); return; }
                _processUtility = new ProcessUtility(_err);
                List<int> pidList = _processUtility.GetPidListContainsProcessNameInNow(textBox1.Text);
                if(pidList.Count < 1) { MessageBox.Show("pidList.Count < 1"); return; }
                _windowUtility.WakeupWindow(pidList[0]);
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "button1_Click");
                MessageBox.Show(ex.Message, "button1_Click Failed");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") { Console.WriteLine("TextBox1.Text == \"\""); return; }
                _processUtility = new ProcessUtility(_err);
                List<int> pidList = _processUtility.GetPidListContainsProcessNameInNow(textBox1.Text);
                if (pidList.Count < 1) { MessageBox.Show("pidList.Count < 1"); return; }
                _windowUtility.ShowWindowMinimize(pidList[0]);
            }
            catch (Exception ex)
            {
                _err.AddException(ex, this, "button1_Click");
                MessageBox.Show(ex.Message, "button1_Click Failed");
            }
        }
    }
}
