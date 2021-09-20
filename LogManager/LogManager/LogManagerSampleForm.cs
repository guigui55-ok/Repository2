using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Log
{
    public partial class LogTest : Form
    {
        LogManager logManager;
        public LogTest()
        {
            InitializeComponent();
        }

        private void LogTest_Load(object sender, EventArgs e)
        {
            string folderPath = System.IO.Directory.GetCurrentDirectory();
            string filePath = "Log.Log";
            string path = folderPath + "\\" + filePath;
            ErrorManager.ErrorManager _err = new ErrorManager.ErrorManager(1);
            logManager = new LogManager(_err,1, path);

            logManager.Add("log value 1","log notes");
            logManager.Add("log value 2", "");
            logManager.Add(1, 2, "log value 3", "", "", new Exception("new Exception")); ;

            Console.WriteLine("-----------\n"+logManager.GetLogDataListAtString());
        }
    }
}
