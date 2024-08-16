using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// 制御されます。アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更します。
[assembly: AssemblyTitle("ErrorManager")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ErrorManager")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、このアセンブリ内の型は COM コンポーネントから
// 参照できなくなります。COM からこのアセンブリ内の型にアクセスする必要がある場合は、
// その型の ComVisible 属性を true に設定します。
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID が typelib の ID になります
[assembly: Guid("3bc49dc8-87f7-4bc9-aede-467448961e10")]

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
[assembly: AssemblyVersion("5.4.0.0")]
[assembly: AssemblyFileVersion("5.4.0.0")]

// 5.4.0.0 - 2021/10/10
//Add Method : public void AddLogAlert(Exception exception, object argObject, string logValue, string messageToUser = "")

// 5.3.0.0 - 2021/10/10
// Cange Class static public class Constants
// Add Method IErrorMessenger
// void ShowMessageAddToExistingStringToBehind(FontStyle style, Color color, string msg, string delimiter = "\n", string title = "");
// Add Project ErrorManagerSampleForm

// 4.2.0.0 - 2021/10/10
// change namespace 

// 3.2.0.0 - 2021/10/10
// class ErrorManager - Add Meghod : void AddLogAlert(object argObject, Exception exception, string logValue, string messageToUser = "")

//2.2.0.0
//2021/10/04
// class DebugData メンバ変数追加 public Type ExceptionType

//2.1.0.0
//メソッド引数変更
//ErrorManager.cs
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

//IErrorMessenger.cs
//メソッド引数変更
//void ShowResultSuccessMessageAddToExisting(string msg,bool isBehind = true, string title = "")
//void ShowWarningMessageMessageAddToExisting(string msg, bool isBeHind = true, string title = "")
//void ShowAlertMessageMessageAddToExisting(string msg, bool isBehind = true, string title = "")