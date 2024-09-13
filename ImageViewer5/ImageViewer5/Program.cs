using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace ImageViewer5
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppLogger _logger = new AppLogger();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormMain _formMain = new FormMain(args);
            try
            {
                _logger = _formMain._logger;
                Application.Run(_formMain);
            } catch (ArgumentException ex)
            {
                _logger = _formMain._logger;
                _logger.PrintError(ex, "Program.Main > ArgumentException");
            }
            catch(Exception ex)
            {
                _logger = _formMain._logger;
                _logger.PrintError(ex, "Program.Main > Error");
            } 
        }
    }
}
