using ErrorManager;
using IniManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniManager2Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Test03(args);
            Console.ReadKey();
        }

        static void Test03(string[] args)
        {
            try
            {
                // iniファイルの内容が空の時、終了しない
                ErrorManager.ErrorManager _error = new ErrorManager.ErrorManager(1);
                string iniPath = System.IO.Directory.GetCurrentDirectory() + @"\settings.ini";
                _error.AddLog("iniPath = " + iniPath);
                if (args.Length > 0)
                {
                    iniPath = args[0];
                }
                int ret;
                IniManager.IniManager manager = new IniManager.IniManager(_error);

                // Ini Path
                // フォーマットはなしにする、機能としては残してある
                //string FormatPath = System.IO.Directory.GetCurrentDirectory() + @"\iniFormat.txt";
                // フォーマットを作成 -> プロジェクトにインポートしておく
                //string iniFormatString = new Common().ExcuteRead(FormatPath);

                // Path をセットする、なければ作る
                _ = manager.SetPathAndCreateFileIfPathNotExists(iniPath, "");
                //if (ret < 1) { manager.ShowErrorMessages(); return; }
                if (manager._error.HasException()) { manager.ShowErrorMessages(); manager._error.ClearError(); }

                // あったら読み込む
                ret = manager.ReadIni();
                if (ret < 1) { manager.ShowErrorMessages(); return; }

                // 読み込んだら値をクラス変数にセット
                ret = manager.SetSectionFromPath();
                if (ret == 0) { manager.ShowErrorMessages(); return; }


                string buf;
                string section = "TestSection";
                string param = "TestProperty1";
                // 値を読み込む
                buf = manager.GetParamterValueWhenAddValueIsNothing(section, param);
                if (manager._error.HasException())
                {
                    Console.WriteLine("ERROR : " + manager.GetErrorMessages());
                }
                // テスト出力
                Console.WriteLine("GetParametersValue : [" + section + "] " + param + " = " + buf);

                // 値をクラスへ保存する
                string value = "TestProperty1-2";
                 manager.SetParamterValueWhenAddValueIsNothing(section, param, value);
                if (_error.HasException()) { Console.WriteLine("* ERROR : " + manager.GetErrorMessages()); }
                Console.WriteLine("WetParameter : [" + section + "] " + param + " = " + buf + " ,value="+value);

                // 値を読み込む
                buf = manager.GetParamterValueWhenAddValueIsNothing(section, param);
                if (manager._error.HasException()) { Console.WriteLine("* ERROR : " + manager.GetErrorMessages()); }
                // テスト出力
                Console.WriteLine("GetParametersValue : [" + section + "] " + param + " = " + buf);

                // クラスで保持している値をすべて書き込む
                ret = manager.WriteAllData();
                if (manager._error.HasException()) { Console.WriteLine("* ERROR : " + manager.GetErrorMessages()); }

                // 結果出力
                if (ret > 0) { Console.WriteLine("Success."); }
                else { Console.WriteLine("* Error WriteAllData Failed."); }
                Console.ReadKey();
            } catch (Exception ex)
            {

            }
        }

        static void Test02(string[] args)
        {
            try
            {
                // iniファイルの内容が空の時、終了しない
                ErrorManager.ErrorManager _error = new ErrorManager.ErrorManager(1);
                string iniPath = System.IO.Directory.GetCurrentDirectory() + @"\settings.ini";
                _error.AddLog("iniPath = " + iniPath);
                if (args.Length > 0)
                {
                    iniPath = args[0];
                }
                int ret;
                IniManager.IniManager manager = new IniManager.IniManager(_error);

                // Ini Path
                // フォーマットはなしにする、機能としては残してある
                //string FormatPath = System.IO.Directory.GetCurrentDirectory() + @"\iniFormat.txt";
                // フォーマットを作成 -> プロジェクトにインポートしておく
                //string iniFormatString = new Common().ExcuteRead(FormatPath);

                // Path をセットする、なければ作る
                _ = manager.SetPathAndCreateFileIfPathNotExists(iniPath, "");
                //if (ret < 1) { manager.ShowErrorMessages(); return; }
                if (manager._error.HasException()) { manager.ShowErrorMessages(); manager._error.ClearError(); }

                // あったら読み込む
                ret = manager.ReadIni();
                if (ret < 1) { manager.ShowErrorMessages(); return; }

                // 読み込んだら値をクラス変数にセット
                ret = manager.SetSectionFromPath();
                if (ret == 0) { manager.ShowErrorMessages(); return; }
                

                string buf;
                string section = "TestSection";
                string param = "TestProperty1";
                // 値を読み込む
                buf = manager.GetParameterValue(section, param);
                if (manager._error.HasException()) { 
                    Console.WriteLine("ERROR : " + manager.GetErrorMessages());  
                }
                // テスト出力
                Console.WriteLine("GetParametersValue : [" + section + "] " + param + " = " + buf);

                // 値をクラスへ保存する
                string value = "TestProperty1-2";
                ret = manager.SetParameterValue(section, param, value);
                if (ret < 1) { Console.WriteLine("ERROR : " + manager.GetErrorMessages()); }

                // 値を読み込む
                buf = manager.GetParameterValue(section, param);
                if (manager._error.HasException()) { Console.WriteLine("ERROR : " + manager.GetErrorMessages()); }
                // テスト出力
                Console.WriteLine("GetParametersValue : [" + section + "] " + param + " = " + buf);

                // クラスで保持している値をすべて書き込む
                ret = manager.WriteAllData();
                if (manager._error.HasException()) { Console.WriteLine("ERROR : " + manager.GetErrorMessages()); }

                // 結果出力
                if (ret > 0) { Console.WriteLine("Success."); }
                else { Console.WriteLine("WriteAllData Failed."); }
                Console.ReadKey();
            }
            catch
            {
                Console.ReadKey();
            }

        }

        static void Test01(string[] args)
        {
            // iniファイルの内容が空の時、終了する
            ErrorManager.ErrorManager _error = new ErrorManager.ErrorManager(1);
            string iniPath = System.IO.Directory.GetCurrentDirectory() + @"\settings.ini";
            _error.AddLog("iniPath = " + iniPath);
            if (args.Length > 0)
            {
                iniPath = args[0];
            }
            int ret;
            IniManager.IniManager manager = new IniManager.IniManager(_error);

            // Ini Path
            // フォーマットはなしにする、機能としては残してある
            //string FormatPath = System.IO.Directory.GetCurrentDirectory() + @"\iniFormat.txt";
            // フォーマットを作成 -> プロジェクトにインポートしておく
            //string iniFormatString = new Common().ExcuteRead(FormatPath);

            // Path をセットする、なければ作る
            _ = manager.SetPathAndCreateFileIfPathNotExists(iniPath, "");
            //if (ret < 1) { manager.ShowErrorMessages(); return; }
            if (manager._error.HasException()) { manager.ShowErrorMessages(); manager._error.ClearError(); }

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
            if (manager._error.HasException()) { manager.ShowErrorMessages(); return; }
            // テスト出力
            Console.WriteLine("GetParametersValue : [" + section + "] " + param + " = " + buf);

            // 値をクラスへ保存する
            string value = "TestProperty1-2";
            ret = manager.SetParameterValue(section, param, value);
            if (ret < 1) { manager.ShowErrorMessages(); return; }

            // 値を読み込む
            buf = manager.GetParameterValue(section, param);
            if (manager._error.HasException()) { manager.ShowErrorMessages(); return; }
            // テスト出力
            Console.WriteLine("GetParametersValue : [" + section + "] " + param + " = " + buf);

            // クラスで保持している値をすべて書き込む
            ret = manager.WriteAllData();
            if (manager._error.HasException()) { manager.ShowErrorMessages(); return; }

            // 結果出力
            if (ret > 0) { Console.WriteLine("Success."); }
            else { Console.WriteLine("Failed."); }
            Console.ReadKey();
        }
    }
}
