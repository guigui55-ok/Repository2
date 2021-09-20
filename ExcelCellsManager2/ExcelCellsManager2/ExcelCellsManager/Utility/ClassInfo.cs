using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ClassInfo
    {
        public class ClassData
        {
            public string MemberName;
            public Type Type;
            public object Value;
        }
        protected ErrorManager.ErrorManager _error;
        public ClassInfo(ErrorManager.ErrorManager error)
        {
            _error = error;
        }
        protected string[] _propertyNameList;

        public void SetPropertyNameList(Type type)
        {
            _propertyNameList = GetMemberFeildsNameList(type);
        }

        // Type type = typeof(ReflectionCS.TestClass);
        /// <summary>
        /// クラス内のフィールド名リストを取得する
        /// ※ Type type = typeof(ReflectionCS.TestClass);
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string[] GetMemberFeildsNameList(Type type)
        {
            string[] ret = { };
            try
            {
                //Public パブリックメンバ
                //NonPublic 非パブリックメンバ
                //Instance インスタンスメンバ
                //Static 静的メンバ
                //DeclaredOnly 継承メンバーを除外

                //メンバを取得する
                MemberInfo[] members = type.GetMembers(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.DeclaredOnly);

                foreach (MemberInfo mb in members)
                {
                    //.WriteLine("{0}", mb.Name);
                    if(mb.MemberType.ToString() == "Property")
                    {
                        //.WriteLine("{0}：{1}", mb.MemberType, mb.Name);
                        Array.Resize<string>(ref ret, ret.Length + 1);
                        ret[ret.Length-1] = mb.Name;
                    }
                }
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex,this.ToString() + ".GetMemberFeildsNameList");
                return null;
            }
        }

        public string[] GetList(Type type,MemberTypes memberTypes)
        {
            try
            {
                //メンバを取得する
                BindingFlags flags = 
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.DeclaredOnly;
                return GetList(flags, type, memberTypes);
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetMemberFeildsNameList");
                return null;
            }
        }

        public string[] GetList(BindingFlags flags,Type type,MemberTypes memberTypes)
        {
            string[] ret = { };
            try
            {
                //メンバを取得する
                MemberInfo[] members = type.GetMembers(flags);

                foreach (MemberInfo mb in members)
                {
                    //.WriteLine("{0}", mb.Name);
                    if (mb.MemberType.ToString() == memberTypes.ToString())
                    {
                        //.WriteLine("{0}：{1}", mb.MemberType, mb.Name);
                        Array.Resize<string>(ref ret, ret.Length + 1);
                        ret[ret.Length - 1] = mb.Name;
                    }
                }
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetList");
                return null;
            }
        }

        public List<ClassData> GetMemberList(Type type, MemberTypes memberTypes,Object value)
        {
            List<ClassData> ret = new List<ClassData>();
            try
            {
                //メンバを取得する
                BindingFlags flags =
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.DeclaredOnly;

                //メンバを取得する
                MemberInfo[] members = type.GetMembers(flags);

                ClassData buf;
                foreach (MemberInfo mb in members)
                {
                    buf = new ClassData();
                    //.WriteLine("{0}", mb.Name);
                    if (mb.MemberType.ToString() == memberTypes.ToString())
                    {
                        buf.MemberName = mb.Name;
                        buf.Type = mb.DeclaringType;
                        buf.Value = null;
                        //.WriteLine("{0}：{1}", mb.MemberType, mb.Name);
                        ret.Add(buf);
                    }
                }
                return ret;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".GetList");
                return null;
            }
        }




        public void SetValueToProperty(Type PropertyType,in object classObject,string propertyName,Type valueType,object value)
        {
            try
            {
                throw new Exception("Not implemented");
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".SetValueToProperty");
            }
        }
    }
}
