using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CommonModules
{
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
