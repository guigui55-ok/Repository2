using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// 制御されます。アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更します。
[assembly: AssemblyTitle("ExcelCellsManager2")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ExcelCellsManager2")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、このアセンブリ内の型は COM コンポーネントから
// 参照できなくなります。COM からこのアセンブリ内の型にアクセスする必要がある場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID が typelib の ID になります
[assembly: Guid("3dc63708-3a10-4ea5-be80-581bba225f7c")]

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      メジャー バージョン
//      マイナー バージョン
//      ビルド番号
//      リビジョン
//
// すべての値を指定するか、次を使用してビルド番号とリビジョン番号を既定に設定できます
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("3.5.0.0")]
[assembly: AssemblyFileVersion("3.5.0.0")]

//ExcelCellsManager2
// 3.5.0.0 - 2021/10/05 - SettingsFrom 表示時の MouseCapture の動作を修正

//3.4.0.0
//2021/10/02
// ExcelCellsManagerMain > public void CopyCellsValue(bool isShowError = true) メソッド
//System.Runtime.InteropServices.COMException 例外時の処理修正

//3.2.0.0
//ExcelCellsManager.MakeAddValue メソッド
//System.Runtime.InteropServices.COMException 例外時のエラーメッセージ追加

//ErrorManager
////2.2.0.0
//ExcelManagerConst クラス追加
//enum ExcelManagerErrorCodes 追加
//ExcelApps.SelectAddress メソッド修正
//System.Runtime.InteropServices.COMException 例外時のエラーメッセージ追加

// 3.1.0.0
// 2021/09/20
// メニューバーを追加

// 2.1.0.0
// ExcelManager.dll,WorkbookList.dll を修正

