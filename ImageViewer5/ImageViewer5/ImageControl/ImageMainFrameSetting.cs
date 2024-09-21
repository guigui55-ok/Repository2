using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer5.ImageControl
{
    public class ImageMainFrameSetting
    {
        // メインフォームにフィットさせる
        public bool _isFitFormMain = false;
        // メインフォームのウィンドウサイズ比と連動させる
        //Link the size ratio with the main form
        public bool _isLinkeControlSizeRadioWithFormMain = false;

        public bool _slideShowOn = false;
        public int _slideShowInterval = 1750;

        /*
         * 
         */
    }
}
