﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// 制御されます。アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更します。
[assembly: AssemblyTitle("MousePointCapture")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("MousePointCapture")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、このアセンブリ内の型は COM コンポーネントから
// 参照できなくなります。COM からこのアセンブリ内の型にアクセスする必要がある場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID が typelib の ID になります
[assembly: Guid("7daa1fb8-e524-41bc-9c0b-016dc786ccd8")]

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
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]
// 1.2.0.0 - 2021/10/02
//MousePointCaptureOnScreenEdgeFormEvents Add Method private void ActivateForm()
//MousePointCaptureOnScreenEdgeManager
//Add Member public Form LastActivateForm;
//Add Method public void SaveFormActivateEvent(object sender, EventArgs e)
// _form がアクティブでないときに ～EdgeManager.LastActivateForm をアクティブにするように修正

// 1.1.0.0
// 2021/10/02
// Class MousePointCaptureOnScreenEdgeFormEvents
// メソッド追加
// private void ActivateForm()
// 実装変更・動作変更
// モーダルウィンドウ表示時に、メインウィンドウをアクティブにすると子ウィンドウがモーダルウィンドウの動作ではなくなるので
// モーダルウィンドウ表示時にはメインウィンドウをアクティブにしないように修正

// 1.0.0.0