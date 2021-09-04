# Repository2

##### 最終更新日 2021.9.3

## ■ ExcelFindExtention > ExcelCellsManger  
### Summary  

エクセルの任意のセルの位置(アドレス)を登録し、登録した位置を表示するアプリケーションです。  


#### 主な機能

* エクセルの任意のセルの位置(アドレス)を登録します。  
  
* 登録した位置を表示します。 

　表示するワークブックが開いていないときは自動的にワークブックを開きます  

* アプリケーションが前面にないとき、表示しているスクリーン淵の設定された位置へマウスポインタを移動することで、アプリケーションを前面に表示します。  
* 登録アドレスリストをtsvファイルとして保存、保存したtsvファイルを読み込み表示します。  

* ショートカットキー  
Ctrl＋O：開く  
Ctrl＋S：ファイルを保存(上書き保存、ファイル名が設定されていないときは名前を付けて保存)  
Ctrl＋Shift＋S：名前を付けて保存  
Ctrl＋Shift＋O：設定ウィンドウを開く  

#### 経緯  
業務内でキュメントとしてのエクセルを多量に開くことがあるため、本アプリケーションの作成に至りました。  

#### 課題  
* ツールバー作成
* メニューバー作成
* ユーザー向けエラーメッセージの表示(実装方法を考え中)
  
  
## ■ CommonUtility
## Summary  
汎用的な機能をまとめたクラスです。  
(作成中)

## ■ ErrorManager (LogManager)
## Description  
開発アプリケーション内で発生する例外管理します。また、開発時の(任意の)ログを管理します。

## ■ ExcelFindExtention
### Description  
開いている Excelへ詳細な検索をお香なうために作りましたが、実装を大幅に見直しているためリファクタリング予定です。

## ■ ExcelManager
### Description  
Comから提供されているExcel起動時のオブジェクト Excel.Aplication を管理します。  
具体的な内容としては、Excel.Application のイベントを検知して開いているWorkbookを取得・保持します。
※サンプルプロジェクト： IniManagerSample.sln > IniManagerSample.csproj 

### ■ ExcelWorkbookList
### Description  
ExcelCellsMangerTest 内で使用している、上記 ExcelManager で保持しているWorkbookを表示します。
ExcelCellsMangerTest のコード分割のため後発的に作成しました。
(作成中)

### ■ ImageViewer4
### Description  
簡易的に画像を表示します。
(作成中)
### 作成経緯  
Windows フォトの動作が重いときがあり、軽量はビューワが欲しいと思ったためです。

### ■ iniManager2 (IniManagerSample)
### Description  
設定ファイルを読み下記するためのモジュールです。
※サンプルプロジェクト： IniManagerSample.sln > IniManagerSample.csproj  
### そのた
反省点：Win32 APIのGetPrivateProfileString関数、GetPrivateProfileInt関数を使えばもっと簡単に実装できました。。  

### ■ MousePointCaptureOnScreenEdte
### Description  
WindowsForms の任意のフォームのウィンドウが非アクティブの時、かつ、デスクトップのスクリーンの淵にマウスポインタを移動した時に
ウィンドウをアクティブにするモジュール  
デフォルトでスクリーンの左端上から 100～101 ポイントの位置を設定してある
※サンプルプロジェクト： MousePointCaptureOnScreenEdge.sln > MousePointCaptureOnScreenEdge.csproj  

### ■ ProgressDialogManager
### Description  
時間を要するとき可視化 or ユーザーに作業中であることを知らせるためのものです。
※サンプルプロジェクト：ProgressDialogManager.sln > ProgressDialogSample.csproj  
### その他  
問題点：ProgressDialogManager を通して処理を行うと、単に処理するより時間を要しているように見受けられるため、改善が必要。  
