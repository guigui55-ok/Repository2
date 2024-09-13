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

    public class TransparentFormFunctionB
    {
        private Form _targetForm;
        private AppLogger _logger;

        // WinAPI関数を使って透明度を設定する
        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        const int GWL_EXSTYLE = -20;
        const uint WS_EX_LAYERED = 0x80000;
        const uint LWA_COLORKEY = 0x1;

        const uint LWA_ALPHA = 0x2;

        public TransparentFormFunctionB(AppLogger logger, Form targetForm)
        {
            _logger = logger;
            _targetForm = targetForm;
            _targetForm.FormBorderStyle = FormBorderStyle.None;  // 枠線を非表示
            _targetForm.BackColor = Color.Magenta;  // 透明にしたい色
            _targetForm.TransparencyKey = Color.Empty;  // TransparencyKeyの使用を防ぐ
            _targetForm.Load += new EventHandler(OnLoad);
            //_targetForm.Paint += new PaintEventHandler(OnPaint);
        }

        //private void OnLoad(object sender, EventArgs e)
        //{
        //    // 透明部分のクリックを有効にするためのウィンドウスタイル設定
        //    uint exStyle = GetWindowLong(_targetForm.Handle, GWL_EXSTYLE);
        //    SetWindowLong(_targetForm.Handle, GWL_EXSTYLE, exStyle | WS_EX_LAYERED);

        //    // Magentaの部分を透明化
        //    SetLayeredWindowAttributes(_targetForm.Handle, (uint)Color.Magenta.ToArgb(), 0, LWA_COLORKEY);
        //}
        private void OnLoad(object sender, EventArgs e)
        {
            // 透明部分のクリックを有効にするためのウィンドウスタイル設定
            uint exStyle = GetWindowLong(_targetForm.Handle, GWL_EXSTYLE);
            SetWindowLong(_targetForm.Handle, GWL_EXSTYLE, exStyle | WS_EX_LAYERED);

            // 完全に透明にするための設定
            SetLayeredWindowAttributes(_targetForm.Handle, 0, 0, LWA_ALPHA); // 透明度を0に設定
        }


        //private void OnPaint(object sender, PaintEventArgs e)
        //{
        //    // フォームのデザインを描画する（透明部分は描画しない）
        //    using (Brush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0))) // 半透明の色
        //    {
        //        e.Graphics.FillRectangle(brush, _targetForm.ClientRectangle); // 描画領域
        //    }
        //}

        // WndProcの処理を委譲するメソッド
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

        //// フォーム全体をドラッグ可能にするメソッド
        //public void HandleWndProc(ref Message m)
        //{
        //    const int WM_NCHITTEST = 0x84;
        //    const int HTCLIENT = 1;
        //    const int HTCAPTION = 2;

        //    if (m.Msg == WM_NCHITTEST)
        //    {
        //        _targetForm.WndProc(ref m);
        //        if ((int)m.Result == HTCLIENT)
        //        {
        //            m.Result = (IntPtr)HTCAPTION; // ウィンドウ全体をドラッグ可能に
        //        }
        //        return;
        //    }

        //    _targetForm.WndProc(ref m);
        //}
    }

}
