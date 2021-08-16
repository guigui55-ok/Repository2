using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class TypeUtility
    {
        protected ErrorManager.ErrorManager _error;
        public TypeUtility(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        // string 100,100,100,100,100,100
        // Type int[]
        // convert type type string value
        // Type not match


        public object ConvertToObject(Type convertType, string value, char delimiter = ',')
        {
            object ret = null;
            try
            {
                if (convertType == typeof(int[]))
                {
                    ret = ConvertToIntArray(value,delimiter);
                } else if (convertType == typeof(bool))
                {
                    ret = ConvertToBoolean(value);
                }
                else if(convertType == typeof(string))
                {
                    ret = value;
                } else
                {
                    string TypeName;
                    if (convertType == null) { TypeName = "null"; } else { TypeName = convertType.ToString(); };
                    _error.AddLog(this.ToString()+ ".Convert :ConvertType Not Supported. Type="+ TypeName);
                }
                return ret;
            } catch (Exception ex)
            {
                string TypeName="";
                if (convertType == null) { TypeName = "null"; } else { TypeName = convertType.ToString(); };
                _error.AddException(ex, this.ToString() + ".ConvertToObject [" + TypeName + "] to object");
                return null;
            }
        }

        public bool ConvertToBoolean<T>(T value)
        {
            try
            {
                if (value == null) { _error.AddLog("ConvertToBoolean<T>: Value is Null"); return false; }
                if (value.GetType() == typeof(string))
                {
                    if ((value == null)||((string)(object)value == "")) {
                        _error.AddLog("ConvertToBoolean Value Is Null.");
                        return false; 
                    }
                    string val = (string)(object)value;
                    if(String.Compare(val, "true", true) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                } else if (value.GetType() == typeof(bool))
                {
                    return (bool)(object)value;
                }
                else
                {
                    string TypeName;
                    if (value == null) { TypeName = "null"; } else { TypeName = value.GetType().ToString(); };
                    _error.AddLog(this.ToString() + ".Convert :ConvertType Not Supported. Type=" + TypeName);
                    return false;
                }
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertToBoolean");
                return false;
            }
        }

        public int[] ConvertToIntArray(string value,char delimiter = ',')
        {
            try
            {
                if((value == "")||(value == null)) { _error.AddLog("Convert[str=>int[]] value==null"); return null; }
                string[] splitVal = value.Split(delimiter);
                if(splitVal.Length < 1) { _error.AddLog("Convert[str=>int[]] value.Split.Length<1"); return null; }

                int[] ret = new int[] { };
                int n=0;
                for(int i=0; i< splitVal.Length; i++)
                {
                    if (int.TryParse(splitVal[i],out n))
                    {
                        n = int.Parse(splitVal[i]);
                    }
                    else
                    {
                        n = 0;
                    }
                    Array.Resize(ref ret, ret.Length + 1);
                    ret[ret.Length - 1] = n;
                }
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertToIntArray string to int[]");
                return null;
            }
        }

        //public string ConvertToString(object value, Type type)
        //{
        //    try
        //    {
        //        return ConvertToString(())
        //    }
        //    catch (Exception ex)
        //    {
        //        _error.AddException(ex, this.ToString() + ".ConvertToString");
        //    }
        //}

        public string ConvertToString<T>(T value,char delimiter = ',')
        {
            string ret = "";
            try
            {
                if (value.GetType() == typeof(string))
                {
                    ret = value.ToString();
                } else if (value.GetType() == typeof(int[]))
                {
                    ret = ConvertIntArrayToString((int[])(object)value);
                }
                else if (value.GetType() == typeof(bool))
                {
                    ret = ((bool)(object)value).ToString();
                } else 
                {
                    string typeName;
                    if(value == null) { typeName = value.GetType().ToString(); } else { typeName = "null";  }
                    _error.AddLog("ConvertToString<T> T to String:value Type Is Not Supported. Type=" + typeName);
                }
                return ret;

            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ConvertToString<T> T to string");
                return "";
            }
        }

        public string ConvertIntArrayToString(int[] values,char delimiter=',')
        {
            try
            {
                if ((values == null)||(values.Length < 1)) { return ""; }
                string[] result = values.Select(x => x.ToString()).ToArray();
                string ret="";
                foreach(string value in result)
                {
                    ret += value + delimiter;
                }
                ret = ret.Substring(0,ret.Length-1);
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString() + ".ConvertToString int[] to string");
                return "";
            }
        }
    }
}
