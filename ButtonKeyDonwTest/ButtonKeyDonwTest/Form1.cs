using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ButtonKeyDonwTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Form1_Load");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("button2_Click");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("button1_Click");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(String.Format("Form1_KeyDown, value={0}", e.KeyValue));
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("button1_KeyDown");
            Form1_KeyDown(sender, e);
        }

        private void button1_KeyUp(object sender, KeyEventArgs e)
        {
            Console.WriteLine("button1_KeyDown");
            Form1_KeyDown(sender, e);
        }
    }
}
