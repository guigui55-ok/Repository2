using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelIO;
using ProcessUtility;

namespace ExcelFindExtention
{
    public partial class Form1 : Form
    {
        protected ErrorManager.ErrorManager _Error;
        //protected ExcelIO.ExcelIO _ExcelIO;
        //protected ProcessUtility.ProcessUtility _ProcUtil;
        protected ExcelIO.ExcelManager _ExcelManager;
        protected ExcelFinder _ExcelFinder;
        protected ExcelFinderForExcelApps _ExcelFinderForApps;
        public Form1()
        {
            _Error = new ErrorManager.ErrorManager(1);
            //_ProcUtil = new ProcessUtility.ProcessUtility(_Error);
            _ExcelManager = new ExcelIO.ExcelManager(_Error);
            //_ExcelIO = new ExcelIO.ExcelIO(_Error);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = @"C:\Users\OK\source\repos\ExcelFindExtention\ExcelFindExtention\bin\Debug\test.xlsx";
        }

        // Open
        private void Button1_Click_1(object sender, EventArgs e)
        {            
            _ExcelManager.OpenFile(textBox1.Text);
            if (_Error.HasException()) { 
                MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return; }
        }

        // 閉じる(保存)
        private void Button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                // 保存する=true、確認する=false
                _ExcelManager.CloseWorkbook(textBox1.Text,true,false);
                if (_Error.HasException())
                {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Button2 Save and Close Failed.");
            }
        }

        // 更新
        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> excelList = _ExcelManager.GetWorkbookNameListAndGhostProcessNameList();
                if (_Error.HasException())
                {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }
                ClearAllCheckBox();
                if (excelList.Count < 1) { 
                    MessageBox.Show("Excel Not Found");
                    return; 
                }
                checkedListBox1.Items.AddRange(excelList.ToArray());

            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Button Update Failed.");
            }
        }

        private void ClearAllCheckBox()
        {
            try
            {
                if(checkedListBox1.Items.Count < 1) { return; }
                int max = checkedListBox1.Items.Count;
                for(int i = 0; i<= max; i++)
                {
                    checkedListBox1.Items.RemoveAt(0);
                    max = checkedListBox1.Items.Count;
                    if (max < 1) { break; }
                    i = 0;
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "ClearAllCheckBox Failed");
            }
        }

        // CheckedBoxList から
        private List<AppsInfo> GetAppsInfoListFromCheckedBoxList(CheckedListBox checkedListBox)
        {
            List<AppsInfo> retList = new List<AppsInfo>();
            try
            {
                if (checkedListBox.Items.Count < 1) { return retList; }
                List<int> indexList = new List<int>();
                string buf = "";
                AppsInfo info;
                // チェックされている項目のインデックスをすべて保存する
                for (int i = 0; i < checkedListBox.Items.Count; i++)
                {
                    if (checkedListBox.GetItemChecked(i))
                    {
                        buf = checkedListBox.Items[i].ToString();
                        if (buf != "")
                        {
                            info = new AppsInfo();
                            string str = "] ";
                            // [0000] FileName.xls 形式からファイル名を取得・設定する
                            info.FileName = buf.Substring(buf.IndexOf(str) + str.Length);

                            // [0000] FileName.xls 形式から ProcessId を取得・設定する
                            buf = buf.Substring(1, buf.IndexOf("] ") - 1);
                            int ret;
                            bool canConvert = int.TryParse(buf, out ret);
                            if (!canConvert)
                            {
                                info.ProcessId = 0;
                            } else
                            {
                                info.ProcessId = ret;
                            }
                            // index
                            info.Index = i;
                            // info Save
                            retList.Add(info);
                        }
                    }
                }
                return retList;
            } catch (Exception ex)
            {
                _Error.AddException(ex, this.ToString() + ".CreateApplication");
                return retList;
            }
        }
        // 接続 Activate
        private void Button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkedListBox1.Items.Count < 1) { return; }
                List<int> indexList = new List<int>();
                // チェックされている項目のインデックスをすべて保存する
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.GetItemChecked(i))
                    {
                        indexList.Add(i);
                    }
                }

            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Button Connect Failed");
            }
        }

        // テスト
        private void Button5_Click(object sender, EventArgs e)
        {
            try
            {
                _ExcelManager.GetSheetTest(textBox1.Text);
                if (_Error.HasException())
                {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Button5 Test Failed");
            }
        }

        // 閉じる(非保存)
        private void Button6_Click(object sender, EventArgs e)
        {
            try
            {
                // 保存する=false、確認する=false
                _ExcelManager.CloseWorkbook(textBox1.Text, false, false);
                if (_Error.HasException())
                {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Button6 Close Not Save Failed");
            }
        }

        // 閉じる(保存)
        private void Button7_Click(object sender, EventArgs e)
        {
            try
            {
                // checkedBoxList からファイル名 or プロセス名とプロセス ID を取得する
                // pid processName
                // pid FileName
                // CheckedBoxList からチェックされている FileName、pid、index を取得する
                List<AppsInfo> infoList = GetAppsInfoListFromCheckedBoxList(checkedListBox1);
                if (_Error.HasException()) {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }           
                if (infoList.Count < 1) {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }
                foreach (AppsInfo info in infoList)
                {
                    // filename、pid、index で指定した Workbook を閉じる
                    _ExcelManager.CloseWorkbookByPidAndBookName(
                        info.ProcessId, info.FileName, info.Index, true, false);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;

                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Buttonu Close And Save Failed");
            }
        }

        // テスト
        private void Button9_Click(object sender, EventArgs e)
        {
            try
            {
                // CheckedBoxList からチェックされている FileName、pid、index を取得する
                List<AppsInfo> infoList = GetAppsInfoListFromCheckedBoxList(checkedListBox1);
                if (_Error.HasException())
                {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }
                if (infoList.Count < 1)
                {
                    MessageBox.Show("CheckedBoxList.Item.Checked.Count is Zero", "Error"); return;
                }
                foreach (AppsInfo info in infoList)
                {
                    // 実行するエクセルのファイルパスを取得する
                    string path = _ExcelManager.GetPathFromAppsInfo(info);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }

                    // テストを実行する
                    // テスト：シート一覧を取得する                   
                    //_ExcelManager.GetSheetTest(path, 0);
                    ExcuteFunctionThatArgumentStringToCheckedFileName(_ExcelManager.GetSheetTest);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Buttonu Close And Save Failed");
            }
        }

        private void ExcuteFunctionThatArgumentExcelAppsInfoToCheckedFileName(Action<AppsInfo> func)
        {
            try
            {
                // CheckedBoxList からチェックされている FileName、pid、index を取得する
                List<AppsInfo> infoList = GetAppsInfoListFromCheckedBoxList(checkedListBox1);
                if (_Error.HasException())
                {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }
                if (infoList.Count < 1)
                {
                    MessageBox.Show("CheckedBoxList.Item.Checked.Count is Zero", "Error"); return;
                }
                foreach (AppsInfo info in infoList)
                {
                    // 実行するエクセルのファイルパスを取得する
                    string path = _ExcelManager.GetPathFromAppsInfo(info);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }

                    // テストを実行する
                    func(info);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Buttonu Close And Save Failed");
            }
        }
        private void ExcuteFunctionThatArgumentStringToCheckedFileName(Action<string> func)
        {
            try
            {
                // CheckedBoxList からチェックされている FileName、pid、index を取得する
                List<AppsInfo> infoList = GetAppsInfoListFromCheckedBoxList(checkedListBox1);
                if (_Error.HasException())
                {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }
                if (infoList.Count < 1)
                {
                    MessageBox.Show("CheckedBoxList.Item.Checked.Count is Zero", "Error"); return;
                }
                foreach (AppsInfo info in infoList)
                {
                    // 実行するエクセルのファイルパスを取得する
                    string path = _ExcelManager.GetPathFromAppsInfo(info);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }

                    // テストを実行する
                    // テスト：シート一覧を取得する
                    func(path);
                    //_ExcelManager.GetSheetTest(path);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Buttonu Close And Save Failed");
            }
        }

        // Find all
        private void Button10_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = "将棋２";
                textBox3.Text = "銀";
                // CheckedBoxList からチェックされている FileName、pid、index を取得する
                List<AppsInfo> infoList = GetAppsInfoListFromCheckedBoxList(checkedListBox1);
                if (_Error.HasException())
                {
                    MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                }
                if (infoList.Count < 1)
                {
                    MessageBox.Show("CheckedBoxList.Item.Checked.Count is Zero", "Error"); return;
                }

                if (_ExcelFinder == null)
                {

                    // 初回実行
                    AppsInfo info = infoList[0];
                    ExcelFinder finder = new ExcelFinder(_Error);
                    ExcelApps apps = _ExcelManager.GetExcelAppsFromAppsInfo(info);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }
                    if (apps == null)
                    {
                        Debug.Write("*** Excel Apps Is Null");
                    }
                    else
                    {
                        finder.Workbook = apps.Application.Workbooks[info.FileName];
                        ExcelFindSearchConditions conditions = new ExcelFindSearchConditions()
                        {
                            KeyWord = textBox3.Text
                                                        
                        };
                        // 検索範囲アドレス
                        conditions.BeginAddress = "";
                        // 検索開始アドレス
                        conditions.After = Type.Missing;
                        // 検索情報の種類。コメント、値、書式
                        conditions.LookIn = Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues;
                        // 部分一致、完全一致
                        conditions.LookAt = Microsoft.Office.Interop.Excel.XlLookAt.xlPart;
                        // 検索順序、Rows,Columns
                        conditions.SearchOrder = Microsoft.Office.Interop.Excel.XlSearchOrder.xlByColumns;
                        // 検索方向 Next,Privious
                        conditions.SearchDirection = Microsoft.Office.Interop.Excel.XlSearchDirection.xlNext;
                        // 大文字小文字を区別するか
                        conditions.MatchCase = false;
                        //  2 バイト文字にのみ一致させる場合は true
                        conditions.MatchByete = false;
                        // 書式を検索する
                        conditions.SearchFormat = false;

                        //List<string> addressList = finder.GetAddressListByFindInSheet(textBox2.Text, conditions);

                        conditions.FindRange = apps.Application.Workbooks[info.FileName].Sheets[textBox2.Text].UsedRange.Address;
                        List<string> addressList = finder.GetAddressListByFindInSheet(textBox2.Text, conditions);
                        if (_Error.HasException())
                        {
                            MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                        }
                        if (addressList.Count < 1)
                        {
                            MessageBox.Show("Keyword Not Found.");
                            return;
                        }
                        Console.WriteLine(" -------- " + info.FileName + " >> " + textBox2.Text);
                        foreach (string buf in addressList)
                        {
                            Console.WriteLine("  " + buf);
                        }
                        Console.WriteLine(" -------- ");
                        finder.Close();
                    }


                } else
                {
                    throw new Exception("Keyword Is Null"); 
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Buttonu Close And Save Failed");
            }
        }



        // activate
        private void Button11_Click(object sender, EventArgs e)
        {
            ExcuteFunctionThatArgumentExcelAppsInfoToCheckedFileName(_ExcelManager.ActivateWorkbook);
        }

        // 次を検索
        private void Button12_Click(object sender, EventArgs e)
        {
            // appslist 取得
            // ※ ExcelAppsList が必要
            // 現在のApplication、ActiveWorkbook が含まれいるか
            // 含まれていなければ、AppsListの最初のBookのActiveSheetに設定する
            // FindedFlagで管理する？ or 検索最初のシートインデックスとworkbookNameの一つ前までにする？
            // 次へ検索
            // なければ次のシートへ
            try
            {
                textBox3.Text = "ReadMe";
                ExcelFindSearchConditions conditions = new ExcelFindSearchConditions()
                {
                    KeyWord = textBox3.Text,
                    // 検索範囲アドレス
                    BeginAddress = "",
                    // 検索開始アドレス
                    After = Type.Missing,
                    // 検索情報の種類。コメント、値、書式
                    LookIn = Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues,
                    // 部分一致、完全一致
                    LookAt = Microsoft.Office.Interop.Excel.XlLookAt.xlPart,
                    // 検索順序、Rows,Columns
                    SearchOrder = Microsoft.Office.Interop.Excel.XlSearchOrder.xlByColumns,
                    // 検索方向 Next,Privious
                    SearchDirection = Microsoft.Office.Interop.Excel.XlSearchDirection.xlNext,
                    // 大文字小文字を区別するか
                    MatchCase = false,
                    //  2 バイト文字にのみ一致させる場合は true
                    MatchByete = false,
                    // 書式を検索する
                    SearchFormat = false
                };
                FindNextAndSelectCell(conditions);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Button12_Clickt Failed");
            }
        }

        private void FindNextAndSelectCell(ExcelFindSearchConditions conditions)
        {
            try
            {
                // appslist 取得
                // ※ ExcelAppsList が必要
                // 現在のApplication、ActiveWorkbook が含まれいるか
                // 含まれていなければ、AppsListの最初のBookのActiveSheetに設定する
                // FindedFlagで管理する？ or 検索最初のシートインデックスとworkbookNameの一つ前までにする？
                // 次へ検索
                // なければ次のシートへ
                    // CheckedBoxList からチェックされている FileName、pid、index を取得する
                    List<AppsInfo> infoList = GetAppsInfoListFromCheckedBoxList(checkedListBox1);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }
                    if (infoList.Count < 1)
                    {
                        MessageBox.Show("CheckedBoxList.Item.Checked.Count is Zero", "Error"); return;
                    }

                    // Finder Class を New する
                    _ExcelFinderForApps = new ExcelFinderForExcelApps(
                        _Error, _ExcelManager.GetExcelAppsList(), infoList);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }
                    // 検索する
                    _ExcelFinderForApps.ExcuteFind(conditions);
                    if (_Error.HasException())
                    {
                        MessageBox.Show(_Error.GetExceptionMessageAndStackTrace(), "Error"); return;
                    }
                    else
                    { Console.WriteLine("Find Next Success!"); }
                } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Buttonu Find Next Failed");
            }
        }

        // getselected
        private void Button14_Click(object sender, EventArgs e)
        {
            ExcuteFunctionThatArgumentExcelAppsInfoToCheckedFileName(_ExcelManager.GetActivateCell);
        }

        private void From1_Closed(object sender, FormClosedEventArgs e)
        {
            try
            {

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "From1_Closed Failed");
            }
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            // FindOnly


        }

        private void Button13_Click(object sender, EventArgs e)
        {
            // 前へ検索
            textBox3.Text = "ReadMe";
            ExcelFindSearchConditions conditions = new ExcelFindSearchConditions()
            {
                KeyWord = textBox3.Text,
                // 検索範囲アドレス
                BeginAddress = "",
                // 検索開始アドレス
                After = Type.Missing,
                // 検索情報の種類。コメント、値、書式
                LookIn = Microsoft.Office.Interop.Excel.XlFindLookIn.xlValues,
                // 部分一致、完全一致
                LookAt = Microsoft.Office.Interop.Excel.XlLookAt.xlPart,
                // 検索順序、Rows,Columns
                SearchOrder = Microsoft.Office.Interop.Excel.XlSearchOrder.xlByColumns,
                // 検索方向 Next,Privious
                SearchDirection = Microsoft.Office.Interop.Excel.XlSearchDirection.xlPrevious,
                // 大文字小文字を区別するか
                MatchCase = false,
                //  2 バイト文字にのみ一致させる場合は true
                MatchByete = false,
                // 書式を検索する
                SearchFormat = false
            };
            FindNextAndSelectCell(conditions);

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
