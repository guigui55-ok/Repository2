using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace ExcelIO
{
    public class ExcelFindSearchConditions
    {
        public string KeyWord;
        public string FindRange;
        public string BeginAddress;
        public object After;
        public string AfterAddress;
        public XlFindLookIn LookIn;
        public XlLookAt LookAt;
        public XlSearchOrder SearchOrder;
        public XlSearchDirection SearchDirection;
        public bool MatchCase;
        public bool MatchByete;
        public bool SearchFormat;
        public List<string> TargetAddressListFromTargetRange; // sepalatedAddressListByComma と同義
        public List<string> sepalatedAddressListBySearchOrder; // 検索直前用で縦または横に分割されたアドレスリスト
    }
}
