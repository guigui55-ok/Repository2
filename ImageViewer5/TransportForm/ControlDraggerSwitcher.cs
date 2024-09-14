using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportForm
{
    public class ControlDraggerSwitcher
    {
        IsEnableFlag _innerToInner;
        IsEnableFlag _innerToFrame;
        IsEnableFlag _innerToForm;
        IsEnableFlag _frameToFrame;
        IsEnableFlag _frameToForm;
        IsEnableFlag _FormToForm;
        public ControlDraggerSwitcher()
        {

        }

        public void setFlags(
            IsEnableFlag innerToinner,
            IsEnableFlag innerToFrame,
            IsEnableFlag innerToForm,
            IsEnableFlag frameToFrame,
            IsEnableFlag FrameToForm,
            IsEnableFlag FormToForm)
        {
            _innerToInner = innerToinner;
            _innerToFrame = innerToFrame;
            _innerToForm = innerToForm;
            _frameToFrame = frameToFrame;
            _frameToForm = FrameToForm;
            _FormToForm = FormToForm;
        }

        public void SwitchDefault()
        {
            _innerToInner._value = false;
            _innerToFrame._value = false;
            _innerToForm._value = true;
            _frameToFrame._value = false;
            _frameToForm._value = true;
            _FormToForm._value = true;
        }

    }
}
