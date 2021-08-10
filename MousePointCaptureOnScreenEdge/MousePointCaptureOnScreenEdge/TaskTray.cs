using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FormUtility
{
    public class TaskTray
    {
        protected ErrorManager.ErrorManager _error;
        NotifyIcon notifyIcon;
        public TaskTray(ErrorManager.ErrorManager error)
        {
            try
            {
                _error = error;
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString()+".TaskTray Constracta");
            }
        }

        public NotifyIcon GetNotifyIcon()
        {
            return notifyIcon;
        }

        public void Initialize(string iconPath,string iconText)
        {
            try
            {
                SetComponents(iconPath, iconText);
                if (_error.HasException()) { return; }

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".Initialize");
            }

        }

        private void SetComponents(string iconPath,string iconText)
        {
            try
            {
                notifyIcon = new NotifyIcon();
                // アイコンの設定
                notifyIcon.Icon = new Icon(iconPath);
                // 1アイコンを表示する
                notifyIcon.Visible = true;
                // アイコンにマウスポインタを合わせたとき
                notifyIcon.Text = iconText;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".setComponents");
            }
        }
    }
}
