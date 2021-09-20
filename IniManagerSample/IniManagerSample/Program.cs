using System;
using System.IO;
using IniManager;

namespace TestIniManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string iniPath = Directory.GetCurrentDirectory() + @"\settings.ini";
            if (args.Length > 0)
            {
                iniPath = args[0];
            }
            int ret;
            IniManager.IniManager manager = new IniManager.IniManager(1);

            // Ini Path
            string FormatPath = Directory.GetCurrentDirectory() + @"\iniFormat.txt";
            // フォーマットを作成 -> プロジェクトにインポートしておく
            string iniFormatString = new Common().ExcuteRead(FormatPath);

            // Path をセットする、なければ作る
            manager.SetPathAndCreateFileIfPathNotExists(iniPath, iniFormatString);
            //if (ret < 1) { manager.ShowErrorMessages(); return; }
            if (manager.Error.HasException()) { manager.ShowErrorMessages(); manager.Error.ClearError(); }

            // あったら読み込む
            ret = manager.ReadIni();
            if (ret < 1) { manager.ShowErrorMessages(); return; }

            // 読み込んだら値をクラス変数にセット
            ret = manager.SetSectionFromPath();
            if (ret < 1) { manager.ShowErrorMessages(); return; }

            string buf;
            string section = "TestSection";
            string param = "TestProperty1";
            // 値を読み込む
            buf = manager.GetParameterValue(section, param);
            if (manager.Error.HasException()) { manager.ShowErrorMessages(); return; }
            // テスト出力
            Console.WriteLine("GetParametersValue : [" + section + "] " + param + " = " + buf);

            // 値をクラスへ保存する
            string value = "TestProperty1-2";
            ret = manager.SetParameterValue(section, param, value);
            if (ret < 1) { manager.ShowErrorMessages(); return; }

            // 値を読み込む
            buf = manager.GetParameterValue(section, param);
            if (manager.Error.HasException()) { manager.ShowErrorMessages(); return; }
            // テスト出力
            Console.WriteLine("GetParametersValue : [" + section + "] " + param + " = " + buf);

            // クラスで保持している値をすべて書き込む
            ret = manager.WriteAllData();
            if (manager.Error.HasException()) { manager.ShowErrorMessages(); return; }

            // 結果出力
            if (ret > 0) { Console.WriteLine("Success."); }
            else { Console.WriteLine("Failed."); }
            Console.ReadKey();
        }
    }
}
