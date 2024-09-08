using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppLoggerModule;

namespace SlideShowImage
{
    public class SlideShowFunction
    {
        AppLogger _logger;
        public Timer _timer;
        Control _control;
        //スライドショーを使用するPictureBoxが複数あるときに、ログ出力の呼び出し元を判別するために使用する
        //デフォルトでControl.Nameを格納する
        public string _itemName; 
        public SlideShowFunction(AppLogger logger, Control control)
        {
            _logger = logger;
            _control = control;
            _itemName = _control.Name;
            _timer = new Timer();
            _timer.Interval = 2000;
        }

        public void StartTimer()
        {
            _logger.PrintInfo(_itemName + " > Start");
            _timer.Start();
        }

        public void SetTimerTick(EventHandler eventHandler)
        {
            _timer.Tick += eventHandler;
        }

        public void StopTimer()
        {
            _logger.PrintInfo(_itemName + " > StopTimer");
            _timer.Stop();
        }
    }
}
