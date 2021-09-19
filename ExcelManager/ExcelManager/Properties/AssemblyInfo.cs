using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// 制御されます。アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更します。
[assembly: AssemblyTitle("ExcelManager")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ExcelManager")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、このアセンブリ内の型は COM コンポーネントから
// 参照できなくなります。COM からこのアセンブリ内の型にアクセスする必要がある場合は、
// その型の ComVisible 属性を true に設定します。
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID が typelib の ID になります
[assembly: Guid("bc570ab8-550e-43a8-bb30-12a5ba49876d")]

// アセンブリのバージョン情報は次の 4 つの値で構成されています:
//
//      メジャー バージョン
//      マイナー バージョン
//      ビルド番号
//      リビジョン
//
// すべての値を指定するか、次を使用してビルド番号とリビジョン番号を既定に設定できます
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("4.0.0.0")]
[assembly: AssemblyFileVersion("4.0.0.0")]

// 4.0.0.0
//2021/9/19
// クラス追加 ExcuteAfterWaitForExit (最後のワークブックを閉じるときに ExcelAppsList を更新するクラス)
// *Update 時の処理見直し
// Excel.Application が無く、エクセルファイルを起動したときの処理を修正
//  (Application を補足できなかったり、ExcelAppsList が重複して Add されていた)
// Excel.Application.Workbooks.Count =1 のときに WorkbookClose したときの処理を修正
// ExcelManager.ExcelAppsList<0 && ExcelWorkbookSyncer.IsSyncExcelWorkbook = true &&
// workbookClose 後に ListUpdate する処理を追加 (ExcelWorkbookSyncer クラスを追加)
// *IExcelAppsActivationEventBridge に以下メソッドを追加(イベントが2重登録されないための処理を追加した)
// int SetEventForApplication(in Application application);
// int SetEventForWorkbook(in Application application, string bookName);

// 3.0.0.0
//2021/9/18
// メソッドを追加
// UpdateExcelApps
// OverWriteExcelApps
// IsGhost=true 後、Workbook を開き Update すると Workbook が読み込まれなかった
// → Update 実行時、UpdateExcelApps、OverWriteExcelAppsメソッドで更新されるようにした
// ログ出力個所を追加
// GetExcelApplicationFromProcess メソッド内 IsGhost=false 処理を追加、再読み込み時 IsGhost=true のままのことがあったため

// 2.0.0.0
// ExcelWorkbookSyncer クラスをプロジェクト内に追加・作成
// ExcelManager に protected ExcelWorkbookSyncer を追加
// ExcelManger に public IsSyncWorkbook フラグを追加
// ExcelWorkbookSyncer.IsSyncWorkbook=true で、Workbook を開く・閉じる時、ExcelManager.ExcelAppsList を更新する
