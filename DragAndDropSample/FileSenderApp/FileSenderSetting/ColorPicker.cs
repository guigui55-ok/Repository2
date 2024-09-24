using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSenderApp.FileSenderSetting
{
    public class ColorPicker
    {
        ColorDialog _cd;
        //Color _defaultColor = SystemColors.Control;
        public Color _defaultColor = Color.White;
        public ColorPicker()
        {
            _cd = new ColorDialog();
            //はじめに選択されている色を設定
            _cd.Color = _defaultColor;
            //色の作成部分を表示可能にする
            _cd.FullOpen = true;
            //純色だけに制限しない
            _cd.SolidColorOnly = false;
            //[作成した色]に指定した色（RGB値）を表示する
            _cd.CustomColors = new int[] {
                0x33, 0x66, 0x99, 0xCC, 0x3300, 0x3333,
                0x3366, 0x3399, 0x33CC, 0x6600, 0x6633,
                0x6666, 0x6699, 0x66CC, 0x9900, 0x9933};
        }

        public Color ShowDialogAndGetSelectedColor(Color defaultColor)
        {
            Color ret = defaultColor;
            //ダイアログを表示する
            if (_cd.ShowDialog() == DialogResult.OK)
            {
                //選択された色の取得
                ret = _cd.Color;
            }
            return ret;
        }
    }
}
