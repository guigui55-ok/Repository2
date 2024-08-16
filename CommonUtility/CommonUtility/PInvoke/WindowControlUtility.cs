using ErrorUtility;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CommonUtility.Pinvoke
{
    public class WindowControlUtility
    {
        protected ErrorManager _Error;
        public WindowControlUtility(ErrorManager error)
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

        /// <summary>
        /// メイン・ウィンドウが最小化されていれば元に戻す
        /// </summary>
        /// <param name="pid"></param>
        public void WakeupWindow(int pid)
        {
            Process prs = null;
            try
            {
                prs = Process.GetProcessById(pid);
                WakeupWindow(prs.MainWindowHandle);
            } catch(Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".WakeupWindow ,pid="+pid);
                return;
            } finally
            {
                if (prs != null) { prs.Dispose();
                }
            }
        }
        /// <summary>
        /// メイン・ウィンドウが最小化されていれば元に戻す
        /// 戻り値：成功時は1、失敗時は1以外のエラーコードを返す、エラーコードが取得できない場合は0を返す
        /// </summary>
        /// <param name="pid"></param>
        public int WakeupWindowR(int pid)
        {
            Process prs = null;
            try
            {
                prs = Process.GetProcessById(pid);
                return WakeupWindowR(prs.MainWindowHandle);
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this, "WakeupWindowR ,pid=" + pid);
                return 0;
            }
            finally
            {
                if (prs != null)
                {
                    prs.Dispose();
                }
            }
        }
        /// <summary>
        /// メイン・ウィンドウが最小化されていれば元に戻す
        /// </summary>
        /// <param name="hWnd"></param>
        public void WakeupWindow(IntPtr hWnd)
        {
            try
            {
                _Error.AddLog("WakeupWindow");
                // メイン・ウィンドウが最小化されていれば元に戻す
                if (IsIconic(hWnd))
                {
                    ShowWindowAsync(hWnd, SW_RESTORE);
                    ConsoleWriteLineWhenRiseErrorWin32();
                }

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

        /// <summary>
        /// メイン・ウィンドウが最小化されていれば元に戻す
        /// 戻り値：成功時は1、失敗時は1以外のエラーコードを返す、エラーコードが取得できない場合は0を返す
        /// </summary>
        /// <param name="hWnd"></param>
        public int WakeupWindowR(IntPtr hWnd)
        {
            bool ret;
            int errCode = 0;
            try
            {
                _Error.AddLog("WakeupWindowR");
                // メイン・ウィンドウが最小化されていれば元に戻す
                if (IsIconic(hWnd))
                {
                    ret = ShowWindowAsync(hWnd, SW_RESTORE);
                    if (!ret) {
                        errCode = ConsoleWriteLineWhenRiseErrorWin32();
                        return errCode;
                    }
                }

                // メイン・ウィンドウを最前面に表示する
                ret = SetForegroundWindow(hWnd);
                if (!ret) { return ConsoleWriteLineWhenRiseErrorWin32(); }

                return 1;
            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this, "WakeupWindowR");
                return 0;
            }
        }

        /// <summary>
        /// ウィンドウを最小化する
        /// </summary>
        /// <param name="hWnd"></param>
        public void ShowWindowMinimize(int pid)
        {
            Process prs = null;
            try
            {
                _Error.AddLog("ShowWindowMinimize");
                prs = Process.GetProcessById(pid);

                IntPtr hWnd = prs.MainWindowHandle;
                // メイン・ウィンドウが最小化されていれば元に戻す
                if (IsIconic(hWnd))
                {
                    _Error.AddLog(" pid="+pid+" is Minimize now.");
                } else
                {
                    _Error.AddLog("ShowWindowAsync, hWnd=" + hWnd);
                    ShowWindowAsync(hWnd, SW_MINIMIZE);
                    ConsoleWriteLineWhenRiseErrorWin32();
                }

            }
            catch (Exception ex)
            {
                _Error.AddException(ex, this,"ShowWindowMinimize ,pid="+pid);
                return;
            }
            finally
            {
                if (prs != null)
                {
                    prs.Dispose();
                }
            }
        }
        public int ConsoleWriteLineWhenRiseErrorWin32()
        {
            int errCode = GetLastError();
            if (errCode != 0)
            {
                //.WriteLine("Win32エラー・コード：" + String.Format("{0:X8}", errCode));
                _Error.AddLog("Win32エラー・コード：" + String.Format("{0}", errCode));
                Exception ex = new Exception("Win32エラー・コード：" + String.Format("{0}", errCode));
                _Error.AddException(ex, this,  "ConsoleWriteLineWhenRiseErrorWin32");
                return errCode;
            }
            else
            {
                return 0;
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
                _Error.AddException(ex,this.ToString()+ ".GetLastError");
                return 0;
            }
        }
    }
}
