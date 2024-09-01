using System.Drawing;
using System.Windows.Forms;

namespace ViewImageModule
{
    // PictureBox
    /// <summary>
    /// ViewImageの、外側UserControl(Control)と、内側PictureBoxの機能などを実装
    /// ViewImageControlPictureBox で使用される
    /// <para></para>
    /// 240824
    /// ViewImageControlPictureBox
    /// 以前のものから移行したが（まだ組み込んでいない）
    /// 描画停止、再描画などの、処理が実装されている
    /// ErrorManager>AppLogger,  その枠をPanelからUserControlに変更
    /// </summary>
    public interface IViewImageControl
    {
        //IViewImageSettings Settings { get; set; }
        //IViewControlState State { get; set; }

        int SetControl(Control ViewControl);
        int SetParentControl(Control ParentControl);
        int Initialize();
        void SetImageWithDispose(Image image);
        void SetImageNotDispose(Image image);
        Size GetSize();
        Point GetLocation();
        void ChangeSize(Size size);
        void SetVisible(bool flag);
        void ChangeLocation(Point point);
        void RefreshPaint();
        void PausePaint(bool flag);
        Control GetParentControl();
        Control GetControl();
        void SuspenLayout();
        void ResumeLayout();
    }
}
