using CommonUtility;
using CommonUtility.Shortcut;
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

namespace CommonUtilitySample
{
    public partial class CommonUtilitySampleForm : Form
    {
        ErrorManager _err;
        DragAndDrop dragAndDrop;
        public CommonUtilitySampleForm()
        {
            InitializeComponent();
            _err = new ErrorManager(1);
            dragAndDrop = new DragAndDrop(_err, this);
            dragAndDrop.DragDropAfterEvent += DragAndDropAfterEvent;
            dragAndDrop.AddControlsDragEvent(new Control[]{ this.richTextBox1});
        }

        private void OutputData(object data)
        {
            try
            {
                string buf = "";
                if (data.GetType().Equals(typeof(string)))
                {
                    buf = (string)data;
                    Console.WriteLine(buf);
                } else if (data.GetType().Equals(typeof(string[])))
                {
                    buf = string.Join(", ", (string[])data);
                    Console.WriteLine(buf); 
                } else
                {
                    buf = "OutputData: data type is invalid";
                    Console.WriteLine(buf);                    
                }
                richTextBox1.Text = buf;
            } catch (Exception ex)
            {
                _err.AddException(ex,this,"OutputData");
            }
        }

        private void DragAndDropAfterEvent(object sender,DragEventArgs e)
        {
            try
            {
                string[] files = dragAndDrop.getFilesByDragAndDrop(e);
                //Console.WriteLine(string.Join(", ", files)); 
                if (files.Length < 0) { OutputData("files.Length < 0"); }

                ShortcutUtility shortcutUtility = new ShortcutUtility(_err);
                foreach (string value in files)
                {
                    string buf = shortcutUtility.GetFullName(value);
                    OutputData(buf);
                    buf = shortcutUtility.GetTargetPath(value);
                    OutputData(buf);
                }

            } catch (Exception ex)
            {
                _err.AddException(ex,this, "DragAndDropAfterEvent");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Not implementation");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
