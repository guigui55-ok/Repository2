# Repository2
##### 最終更新日 2021.10.05

## ■ ExcelCellsManager2  
### Description  
エクセルの任意のセルの位置(アドレス)を登録し、登録した位置を表示するアプリケーションです。  
※プロジェクト： ExcelCellsManager2.sln > ExcelCellsManager2.csproj   

### BinayPath
https://github.com/guigui55-ok/Repository2_Binary/blob/master/ExcelCellsManager2_3.5.7z  
FileName : ExcelCellsManager2_3.5.zip  

#### 主な機能

* エクセルの任意のセルの位置(アドレス)を登録します。  
  
* 登録した位置を表示します。 
    表示するワークブックが開いていないときは自動的にワークブックを開きます。  

* アプリケーションが前面にないとき、表示しているスクリーン淵の設定された位置へマウスポインタを移動することで、アプリケーションを前面に表示します。  
* 保存・読込：登録アドレスリストをtsvファイルとして保存、保存したtsvファイルを読み込み表示します。  
    アプリケーションフォーム上にドラッグ＆ドロップすることでファイルを読み込みます。 
    
* 実行されているエクセルのプロセス(EXCEL.EXE)および開いているワークブックを取得し、その一覧を表示します。  
  また、アプリケーション実行中は、ワークブックの実行・終了を検知し、自動的に更新します。  
* DataGridView の RowHeader ダブルクリックで登録した位置を表示します。  
* 動作時のログ保存、出力をします。  
* CheckedListBox で選択されているワークブックを閉じます。  
* アプリケーション設定をファイル(iniファイル)に保存します。  
  
* ショートカットキー  
Ctrl＋O：開く  
Ctrl＋S：ファイルを保存(上書き保存、ファイル名が設定されていないときは名前を付けて保存)  
Ctrl＋Shift＋S：名前を付けて保存
Ctrl＋Shift＋S：名前を付けて保存  
Ctrl＋Shift＋O：設定ウィンドウを開く  

#### 経緯  
ドキュメントとしてのエクセルを多量に開くことがあり、ワークブックやシート内の複数のセルの位置を頻繁に切り替えて使うことが多く、
その往来に時間を要することがあったため、本アプリケーションの作成に至りました。  

#### 課題  
* ヘルプ作成(chmファイル作成中)
* ユーザー向けエラーメッセージの表示(実装中) 
* リファクタリング(機能など追加していくうちに、ソースが煩雑になってしまったため)(考案中)

#### 更新履歴  
ExcelCellsManager2  
3.5.0.0 - 2021/10/05 - SettingsFrom 表示時の MouseCapture の動作を修正、ツールバーを作成する予定でしたが機能が少ないため保留  
3.4.0.0 - 2021/10/02  
 ExcelCellsManagerMain > public void CopyCellsValue(bool isShowError = true) メソッド  
 System.Runtime.InteropServices.COMException 例外時の処理修正(表示するエラーについて修正)  
3.3.0.0 - 2021/09/30  
 Class ExcelCellsManagerForm , private void Button1_Click(object sender, EventArgs e)  
例外発生時の処理を変更(表示するエラーについて修正)   
 3.2.0.0 - 2021/09/27  
  ExcelCellsManager.MakeAddValue メソッド  
 System.Runtime.InteropServices.COMException 例外時のエラーメッセージ追加  
3.1.0.0 - 2021/09/20  
 メニューバーを追加  
2.1.0.0 - 2021/09/19  
 ExcelManager.dll,WorkbookList.dll を修正  
1.0.0.0  
 ExcelCellsManagerTest から ExcelCellsManager2 にソリューション、プロジェクトを変更  
 ExcelManager.dll,WorkbookList.dll を追加  
  
## ■ CommonUtility
## Description  
いろいろなクラスで使用する汎用的な機能をまとめたクラスです。  

## ■ ErrorManager (LogManager)
## Description  
開発アプリケーション内で発生する例外管理します。また、開発時の(任意の)ログを管理します。
#### 更新履歴  
// 2.2.0.0 - 2021/10/04 - class DebugData メンバ変数追加 public Type ExceptionType

## ■ ExcelFindExtention
### Description  
開いている Excelへ詳細な検索を行うために作りましたが、実装を大幅に見直しているためリファクタリング予定です。

## ■ ExcelManager
### Description  
Comから提供されているExcel起動時のオブジェクト Excel.Aplication を管理します。  
具体的な内容としては、Excel.Application のイベントを検知して開いているWorkbookを取得・保持します。  
※参照設定：Microsoft.Office.Interop.Excel(バージョン：15.0.0.0、ランタイム バージョン：v2.0.50727)  
※サンプルプロジェクト： ExcelUtility.sln > ExcelManagerConsoleSample.csproj   
### 作成経緯  
Excel でのデータ処理を自動化しようと以前は VBA を使用していましたが、コードのボリュームが大きくなり管理が難しくなったので、機能が充実した他の IDE で同様のものを作成使用と思ったのがきっかけです。  
Java で Excel を扱うのは少し問題があるとわかったため、C# を選択しました。
ExcelManager は Excel 内のいろいろな値やオブジェクトを扱うための前段階の処理を行うものとして作成しました。

#### 更新履歴   
2.4.0.0 - 2021/10/02 - クラス追加、メソッド修正(Class ExcelUtilityStaticMethod、Class ExcelApps)  
2.3.0.0 - 2021/09/30 - エラーコードを追加、メソッド戻り値を変更(Class ExcelApps、Class ExcelManagerErrorCodes、Class ExcelManager)  
2.2.0.0 - 2021/9/27 - ExcelManagerConst クラス追加,enum ExcelManagerErrorCodes 追加、例外発生時の処理修正  

### ■ ExcelWorkbookList
### Description  
ExcelCellsMangerTest 内で使用している、上記 ExcelManager で保持しているWorkbookを表示します。   
※サンプルプロジェクト： ExcelWorkbookList.sln > ExcelWorkbookList.csproj   
### 作成経緯  
ExcelCellsMangerTest のコード分割のため後発的に作成しました。

### ■ ImageViewer4
### Description  
簡易的に画像を表示します。
(作成中)

### BinayPath  
https://github.com/guigui55-ok/Repository2_Binary/blob/master/ImageViewer4_0.2.7z  
FileName : ImageViewer4_0.2.zip 

### 作成経緯  
Windows フォトの動作が重いときがあり、軽量なビューワが欲しいと思ったためです。
#### 更新履歴   
// 0.2.0.0 - 21/10/04 - 機能変更：D&D がショートカットの時はショートカット先の Directory を読み込むように修正  
// 0.1.0.0 - 21/10/02 - 機能追加：読み込み時センターに表示する、読み込み時、Control にサイズ合わせる  
// 0.0.0.0 - 21/08/27 - Create Solution  

### ■ iniManager2 (IniManagerSample)
### Description  
設定ファイルを読み下記するためのモジュールです。  
※サンプルプロジェクト： IniManagerSample.sln > IniManagerSample.csproj  
### その他
反省点：作成した後に気付きましたが、Win32 APIのGetPrivateProfileString関数、GetPrivateProfileInt関数を使えばもっと簡単に実装できました。。  

### ■ MousePointCaptureOnScreenEdte
### Description  
WindowsForms の任意のフォームのウィンドウが非アクティブの時、かつ、デスクトップのスクリーンの淵にマウスポインタを移動した時に
ウィンドウをアクティブにするモジュールです。  
デフォルトでスクリーンの左側のみで、左上端からその下の 100～101 ポイントの位置を設定してあります。  
※サンプルプロジェクト： MousePointCaptureOnScreenEdge.sln > MousePointCaptureOnScreenEdge.csproj  

### ■ ProgressDialogManager
### Description
何らかの処理に時間を要するとき、可視化 and ユーザーに作業中であることを知らせるため、また、処理をキャンセルできるようにするためのものです。  
※サンプルプロジェクト：ProgressDialogManager.sln > ProgressDialogSample.csproj  
### その他
問題点：ProgressDialogManager を通して処理を行うと、軽い処理の場合、単に処理するより時間を要しているように見受けられるため、改善が必要。  

### ■ DragAndDropSample
### Description
DragAndDrop で ファイルを読み込むためのサンプルです。  
※サンプルプロジェクト：DragAndDropSample.sln > DragAndDropSample.csproj  
#### 更新履歴   
1.0.0.0 - 2021/10/05 - Create Solution


### ■ 作成環境  
VisualStudio 2019
Windows 10 バージョン 20H2