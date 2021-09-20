using System;
using System.Windows.Forms;

namespace ExcelCellsManager.ExcelCellsManager.Event
{
    public class MouseEvent
    {
        protected ErrorManager.ErrorManager _error;
        protected string _formName = "";
        public MouseEventHandler MouseEventHandler;
        public MouseEvent(ErrorManager.ErrorManager error,string formName)
        {
            _error = error;
            _formName = formName;
            this.MouseEventHandler = new MouseEventHandler(MouseMove);
        }

        public void MouseMove(object obj,MouseEventArgs e)
        {
            try
            {
                //画面座標でマウスポインタの位置を取得する
                //System.Drawing.Point sp = System.Windows.Forms.Cursor.Position;
                //.WriteLine("MouseMove : " + sp.X + "," + sp.Y);
            } catch (Exception ex)
            {
                _error.AddException(ex, _formName + "." + this.ToString() + ".MouseMove");
            }
        }


    }
}
