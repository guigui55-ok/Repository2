using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// 制御されます。アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
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
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID が typelib の ID になります
[assembly: Guid("b96ecbe2-b1e1-4883-8762-750cd9811876")]

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
[assembly: AssemblyVersion("2.4.0.0")]
[assembly: AssemblyFileVersion("2.4.0.0")]

//ErrorManager
//2021/10/02  
//2.4.0.0
//ExcelUtilityStaticMethod クラス追加
//  ystem.Runtime.InteropServices.COMException 例外発生時の Exception の処理をするメソッドを追加
//OpenFileAddToList メソッド修正、メソッド例外内の処理を変更
//System.Runtime.InteropServices.COMException 例外時のエラーメッセージ追加

//2021/09/30
//2.3.0.0
//エラーコードを追加
//メソッド戻り値を変更
//Class ExcelApps
//public int ActivateWorkbook(string workBookName)
//Class ExcelManagerErrorCodes
//エラーコードを追加

//メソッド戻り値を変更
//Class ExcelManager
//public int WindowActivate()

//2021/9/27  
////2.2.0.0
//ExcelManagerConst クラス追加
//enum ExcelManagerErrorCodes 追加
//ExcelApps.SelectAddress メソッド修正
//System.Runtime.InteropServices.COMException 例外時のエラーメッセージ追加

//ErrorManager
////2.1.0.0
//メソッド引数変更
//string[] GetLastErrorMessagesAsArray(int moreThanThisType = 4, bool orderIsRev = false, bool isAddExceptionMessage = false)
//string GetLastErrorMessagesAsString(
//            int moreThanThisType = 3, bool orderIsRev = false, bool isAddExceptionMessage = false)
//public string GetLastErrorMessagesAsString(
//            int moreThanThisType = 3, bool orderIsRev = false, bool isAddExceptionMessage = false)
//string[] GetLastErrorMessagesAsArray(int moreThanThisType = 4, bool orderIsRev = false, bool isAddExceptionMessage = false)
//List<string> GetLastErrorMessagesList(int[] errorTypes, bool orderRev = true, bool isAddExceptionMessage = false)

//privateメソッド追加
//private List<string> GetLastErrorMessagesListOrderRev(int[] targetErrorTypes, bool orderRev = true, bool isAddExceptionMessage = false)
//private void GetDataForDebugList(
//            DebugData data, ref List<string> list, int[] targetErrorTypes, bool isAddExceptionMessage)

//IErrorMessenger
//メソッド引数変更
//void ShowResultSuccessMessageAddToExisting(string msg,bool isBehind = true, string title = "")
//void ShowWarningMessageMessageAddToExisting(string msg, bool isBeHind = true, string title = "")
//void ShowAlertMessageMessageAddToExisting(string msg, bool isBehind = true, string title = "")

