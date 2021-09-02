# Repository2

##### 最終更新日 2021.9.3

## ExcelFindExtention > ExcelCellsManger  
### Summary  

エクセルの任意のセルの位置(アドレス)を登録し、登録した位置を表示するアプリケーション  


#### 主な機能

* エクセルの任意のセルの位置(アドレス)を登録  
  
* 登録した位置を表示する 

　表示するワークブックが開いていないときは自動的にワークブックを開く  

* アプリケーションが前面にないとき、表示しているスクリーン淵の設定された位置へマウスポインタを移動することで、アプリケーションを前面に表示する  
* 登録アドレスリストをtsvファイルとして保存、保存したtsvファイルを読み込み表示  

* ショートカットキー  
Ctrl＋O：開く  
Ctrl＋S：ファイルを保存(上書き保存、ファイル名が設定されていないときは名前を付けて保存)  
Ctrl＋Shift＋S：名前を付けて保存  
Ctrl＋Shift＋O：設定ウィンドウを開く  

#### 経緯  
業務内で使用等が記載されたドキュメントとしてのエクセルを多量に開くことがあるため、本アプリケーションの作成に至った  

#### 課題  


## CommonUtility
## ErrorManager
## Summary  

開発アプリケーション内で発生する例外を管理する、開発時のログを管理する

## ExcelFindExtention

## ExcelManager
### Summary  
Comから提供されているExcel起動時のオブジェクト Excel.Aplication を管理する。  
具体的な内容としては、Excel.Application のイベントを検知して開いているWorkbookを取得・保持する

### ExcelWorkbookList
### Summary  
ExcelCellsMangerTest 内で使用している、上記 ExcelManager で保持しているWorkbookを表示するもの
ExcelCellsMangerTest のコード分割のため作成
(作成中)
