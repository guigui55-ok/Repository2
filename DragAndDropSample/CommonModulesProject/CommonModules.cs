using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CommonModules
{
    public static class CommonUtility
    {
        /// <summary>
        /// 任意の列挙型の値と名前を表示する汎用メソッド
        /// </summary>
        /// <param name="enumType">表示したい列挙型のタイプ</param>
        public static string DisplayEnumValues(Type enumType, int index)
        {
            // 列挙型であるか確認する
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("引数は列挙型でなければなりません。");
            }

            Array enumValues = Enum.GetValues(enumType);

            int count = 0;
            foreach (var value in enumValues)
            {
                string name = Enum.GetName(enumType, value);
                int intValue = (int)value;
                string output = $"{name} [{intValue}]";
                if (index <= count)
                {
                    return output;
                }
            }
            return "";
        }
    }

    public static class Debugger
    {
        public static void DebugPrint(string value)
        {
            Debug.Print(value);
        }
        public static void DebugPrint(params string[] values)
        {
            foreach (var value in values)
            {
                Debug.Print(value);
            }
        }
    }
}
