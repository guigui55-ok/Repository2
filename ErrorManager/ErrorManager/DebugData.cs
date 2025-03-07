﻿using System;

namespace ErrorUtility
{
    public class DebugData
    {
        public int DataType = 0;
        public long ErrorCode = 0;
        public Exception Exception = null;
        public Type ExceptionType = null;
        public string MessageForDebug = null;
        public string MessageForUser = null;
        public DateTime DateTime;
        public bool IsAccecced = false;
        public string GetErrorTypeName(int type)
        {
            switch (type)
            {
                case 0: return "";
                case 1: return "[UEXER]";
                case 2: return "[ERROR]";
                case 3: return "[WARNG]";
                case 4: return "[ALERT]";
                default: return "[OTHER]";
            }
        }
    }

}
