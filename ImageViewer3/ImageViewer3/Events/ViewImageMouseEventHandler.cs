using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageViewer.Events
{
    public class ViewImageMouseEventHandler : IDisposable
    {
        public event EventHandler<MouseEventArgs> OnRightClick;
        public event EventHandler<MouseEventArgs> OnLeftClick;
        public event EventHandler<DragEventArgs> OnDragDrop;
        public event EventHandler<MouseEventArgs> OnMouseWheel;
        public event EventHandler<MouseEventArgs> OnMouseMove;
        public event EventHandler<MouseEventArgs> OnMouseDown;
        public event EventHandler<MouseEventArgs> OnMouseUp;

        public void MouseUp(object sender, MouseEventArgs e) {
            if (OnMouseUp != null) { OnMouseUp.Invoke(sender, e); }
        }
        public void MouseDown(object sender, MouseEventArgs e) {
            if (OnMouseDown != null) { OnMouseDown.Invoke(sender, e); }
        }
        public void MouseMove(object sender, MouseEventArgs e) {
            if (OnMouseMove != null) { OnMouseMove.Invoke(sender, e); }
        }
        public void MouseWheel(object sender,MouseEventArgs e)
        {
            if (OnMouseWheel != null) { OnMouseWheel.Invoke(sender, e); }
        }
        public void RightClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (OnRightClick != null) { OnRightClick.Invoke("1", e); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RightClick");
                Debug.WriteLine(ex.Message);
            }
        }
        public void LeftClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (OnLeftClick != null) { OnLeftClick.Invoke("2", e); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LeftClick");
                Debug.WriteLine(ex.Message);
            }
        }
        public void DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (OnDragDrop != null) { OnDragDrop.Invoke(sender, e); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OnDragDrop");
                Debug.WriteLine(ex.Message);
            }
        }
        public void Dispose()
        {
            //④Dispose()実行
            if (OnDragDrop != null) { OnDragDrop.Invoke(0, null); }
        }
    }
}
