using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CommonUtility
{
    public class MyRandom
    {
        protected ErrorManager.ErrorManager _err;
        public MyRandom(ErrorManager.ErrorManager err)
        {
            _err = err;
        }
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
                _err.AddException(ex,this,"ListToRandom Failed");
                return list;
            }
        }
    }

}
