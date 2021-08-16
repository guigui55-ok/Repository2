
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace TestExcelIO
{
    public partial class Form1 : Form
    {
        protected ErrorManager.ErrorManager _Error;
        //protected ExcelIO _ExcelIO;
        public Form1()
        {
            InitializeComponent();
            string logFolder = System.IO.Directory.GetCurrentDirectory();
            _Error = new ErrorManager.ErrorManager(1,logFolder,"Error.Log","Log.Log");
            textBox1.Text = @"C:\Users\OK\source\repos\ExcelFindExtention\ExcelFindExtention\bin\Debug\test.xlsx";
            //_ExcelIO = new ExcelIO(_Error);
        }


#pragma warning disable IDE1006 // 命名スタイル
        private void button1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名スタイル
        {
            //bool ret = _ExcelIO.Open(textBox1.Text);
            if (!ret) { Debug.WriteLine("excelIO.Open Failed"); }
            else { { Debug.WriteLine("excelIO.Open Success"); } }
            if (_Error.HasException()) { _Error.GetExceptionMessage(); }
        }

#pragma warning disable IDE1006 // 命名スタイル
        private void button2_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名スタイル
        {
            bool ret=false;
            //bool ret = _ExcelIO.Close();
            if (!ret) { Debug.WriteLine("excelIO.Close Failed"); }
            else { { Debug.WriteLine("excelIO.Close Success"); } }
            if (_Error.HasException()) { _Error.GetExceptionMessage(); }
        }
#pragma warning disable IDE1006 // 命名スタイル
        private void label1_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // 命名スタイル
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
