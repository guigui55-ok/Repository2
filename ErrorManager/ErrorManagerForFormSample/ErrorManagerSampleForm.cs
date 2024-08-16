using ErrorUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ErrorManagerForFormSample
{
    public partial class ErrorManagerSampleForm : Form
    {
        protected ErrorManager _err;
        protected IErrorMessenger _errorMessenger;
        public ErrorManagerSampleForm()
        {
            InitializeComponent();
            _err = new ErrorManager(1);
            _errorMessenger = new ErrorMessengerMessageBox(_err);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                RiseException();
                if (_err.hasAlert) { 
                    _errorMessenger.ShowAlertMessages();
                    //AppendText( _err.GetLastErrorMessagesAsString());
                    _err.ClearError();
                }
            } catch (Exception ex)
            {
                _err.AddException(ex,this,"button1_Click Failed");
            }
        }

        private void RiseException()
        {
            try
            {
                throw new Exception("RiseException Throw Exception");
            } catch (Exception ex)
            {
                _err.AddException(ex,this, "RiseException Faild");
            }
        }

        private void AppendText(string text)
        {
            try
            {
                richTextBox1.AppendText(text);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Error - AppendText Failed");
            }
        }
    }
}
