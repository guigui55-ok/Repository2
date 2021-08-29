using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Common
{
    public class MyRandom
    {
        public List<int> ListToRandom(List<int> list)
        {
            try
            {
                //シャッフルする
                int[] ary = list.ToArray().OrderBy(i => Guid.NewGuid()).ToArray();

                return new List<int>(ary);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + " : ListToRandom Failed");
                Debug.WriteLine(ex.Message);
                return list;
            }
        }
    }

}
