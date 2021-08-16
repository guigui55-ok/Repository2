using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExcelFindExtention
{
    public class WindowControl
    {
        protected ErrorManager.ErrorManager _Error;
        public WindowControl(ErrorManager.ErrorManager error)
        {
            _Error = error;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_MAXIMIZE = 3;
        const int SW_MINIMIZE = 6;
        const int SW_RESTORE = 9;

        // 外部プロセスのメイン・ウィンドウを起動するためのWin32 API
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("kernel32.dll", SetLastError = true)]
        private extern static bool Beep(uint dwFreq, uint dwDuration);

        // メイン・ウィンドウが最小化されていれば元に戻す
        public void WakeupWindow(IntPtr hWnd)
        {
            try
            {
                // メイン・ウィンドウが最小化されていれば元に戻す
                if (IsIconic(hWnd))
                {
                    ShowWindowAsync(hWnd, SW_RESTORE);
                }
                ConsoleWriteLineWhenRiseErrorWin32();

                // メイン・ウィンドウを最前面に表示する
                SetForegroundWindow(hWnd);
                ConsoleWriteLineWhenRiseErrorWin32();

            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".WakeupWindow");
                return;
            }
        }

        public void ConsoleWriteLineWhenRiseErrorWin32()
        {
            int errCode = GetLastError();
            if (errCode != 0)
            {
                //Console.WriteLine("Win32エラー・コード：" + String.Format("{0:X8}", errCode));
                Console.WriteLine("Win32エラー・コード：" + String.Format("{0}", errCode));
            }
        }

        public int GetLastError()
        {
            try
            {
                // エラーが発生
                int errCode = Marshal.GetLastWin32Error();
                return errCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLastError Failed.");
                Console.WriteLine(ex);
                Console.WriteLine(ex.StackTrace);
                return 0;
            }
        }
    }
}
