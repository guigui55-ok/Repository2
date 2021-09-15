

namespace ExcelCellsManager.ExcelCellsManager.Settings
{
    public class CellsManagerSettingsValue2
    {
        // アドレスを登録した後にエクセル(Workbook)ウィンドウをアクティブにする
        public bool IsActivateWorkbookWindowAfterRegistAddress;
        // 前回最後に開いていたファイル
        public string LastOpendFilePath;
        // 画面端にマウスポインタを移動したときにフォームをアクティブにする
        public bool IsActivateFormWhenMoveMouseScreenEdge;
        // DataList のカラム幅を記憶する
        public bool IsSaveColumnWidthOfDataList;
        // Column Width
        public int[] ColumnWidthOfDataList;
    }
}
