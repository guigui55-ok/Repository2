﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppLoggerModule;

namespace CommonUtility
{
    public class MyRandom
    {
        public AppLogger _logger;
        protected ErrorManager.ErrorManager _err;
        public MyRandom(AppLogger logger, ErrorManager.ErrorManager err)
        {
            _logger = logger;
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
                _logger.AddException(ex,this,"ListToRandom Failed");
                return list;
            }
        }
    }

}