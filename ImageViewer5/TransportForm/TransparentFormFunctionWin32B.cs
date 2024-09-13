using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AppLoggerModule;

namespace TransportForm
{

    public class TransparentFormFunctionWin32B
    {
        private Form _targetForm;
        private AppLogger _logger;

        // WinAPI関数を使ってウィンドウの属性を設定する
        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        const int GWL_EXSTYLE = -20;
        const uint WS_EX_LAYERED = 0x80000;
        const uint WS_EX_TRANSPARENT = 0x20;
        const uint LWA_ALPHA = 0x2;

        public TransparentFormFunctionWin32B(AppLogger logger, Form targetForm)
        {
            _logger = logger;
            _targetForm = targetForm;
            _targetForm.FormBorderStyle = FormBorderStyle.None;  // 枠線を非表示
            _targetForm.Load += new EventHandler(OnLoad);
            _targetForm.Paint += new PaintEventHandler(OnPaint);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            // 透明部分でもクリックを有効にするため、透明度を設定
            uint exStyle = GetWindowLong(_targetForm.Handle, GWL_EXSTYLE);
            SetWindowLong(_targetForm.Handle, GWL_EXSTYLE, exStyle | WS_EX_LAYERED);

            // フォームを半透明にする。ここでは透明度200で設定（255で不透明）
            SetLayeredWindowAttributes(_targetForm.Handle, 0, 100, LWA_ALPHA);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            // フォームの背景を透明に設定（実際は何も描画しない）
            e.Graphics.Clear(Color.Transparent);
        }

        // WndProcの処理を委譲するメソッド（ドラッグなどができるように）
        public void ProcessWndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;

            if (m.Msg == WM_NCHITTEST)
            {
                if ((int)m.Result == HTCLIENT)
                {
                    m.Result = (IntPtr)HTCAPTION; // ウィンドウ全体をドラッグ可能に
                }
            }
        }

    }
}
