using System;
using System.Diagnostics;

namespace ImageViewer.Common
{
    public class Convert
    {
        public bool StrintToBool(string value)
        {
            try
            {
                if (value.CompareTo("1") == 0) { return true; }
                if (value.CompareTo("true") == 0) { return true; }
                if (value.ToLower().CompareTo("true") == 0) { return true; }
                return false;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
