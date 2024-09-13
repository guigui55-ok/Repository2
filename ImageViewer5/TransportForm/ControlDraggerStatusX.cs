using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppLoggerModule;

namespace TransportForm
{
    public class IsDragEnable
    {
        public bool _value = true;
        public IsDragEnable(bool value)
        {
            _value = value;
        }
    }


    public class ControlDraggerStatusX
    {
        AppLogger _logger;
        //#
        //　構成はフォーム＞Frame パネル＞Inner PictureBox とする
        //#
        // フォームのタイトルが表示されている
        bool _isShowFormTitle = false;
        // フォームが透過状態
        // この状態だとフォームの背景がつかめない
        // 
        bool _isShowFormBackColor = false;
        //#
        // メインフォームが移動可能（Formの操作）
        // 　（メインフォーム背景のドラッグで移動する）
        // 　（Frame_パネルの背景のドラッグで移動する）
        // 　（Frame_パネルの中のInner_PicuterBoxの背景のドラッグで移動する）
        //　　（Frame、Innerは移動しない）
        // タイトルあり・なし両方
        // フォームの背景・あり
        // （Frameパネルの移動はしない、Frameパネルの中のInner_PictureBoxの移動はしない）
        bool _isEnableMoveForm = false;
        //#
        // Frameパネルの移動が可能（Frameの操作）
        // 　（Frame_パネルの背景のドラッグで移動する）
        // 　（Frame_パネルの中のInner_PicuterBoxの背景のドラッグで移動する/しない両方あり得る）
        // 　（メインフォームは移動はする/しない両方あり得る）
        bool _isEnableMoveFrameConrol = false;
        //#
        // Inner_PictureBoxの移動が可能（Innerの操作）
        // 　（Inner_PicuterBoxの背景のドラッグで移動する）
        // 　（Frame_パネルは移動する/しない両方あり得る）
        // 　（メインフォームは移動する/しない両方あり得る）
        bool _isEnableMoveInnerConrol = false;

        //ドラッグ前ドラッグ後の位置を記憶しておいて、前に戻せるようにする
        public ControlDraggerStatusX(AppLogger logger)
        {
            _logger = logger;
        }
    }
}
