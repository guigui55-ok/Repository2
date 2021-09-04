using ControlUtility.SelectFiles;
using CotnrolUtility.SelectFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SelectFileSample
{
    public partial class SelectFileSampleForm : Form
    {
        ErrorManager.ErrorManager _err;
        Thread subThread;
        bool WriteToEditBox = false;
        object _sender;
        EventArgs _e;
        bool IsClosingForm = false;
        string ToRichtextValue = "";
        public SelectFileSampleForm()
        {
            _err = new ErrorManager.ErrorManager(1);
            InitializeComponent();
            this.FormClosing += SelectFileSampleForm_FormClosing;
        }

        private void SelectFileSampleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsClosingForm = true;
        }

        Files files;
        FileListManager fileListManager;
        SelectFileByDragDrop fileDragDrop;
        List<string> list;
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // DragDrop をするためのクラス
                this.AllowDrop = true;
                fileDragDrop = new SelectFileByDragDrop(_err, this);
                // Control を追加する
                this.richTextBox1.AllowDrop = true;
                fileDragDrop.AddRecieveControl(this.richTextBox1);
                // ファイルリストを扱うクラス
                list = new List<string>();
                files = new Files(_err,list);
                // ファイルリストを登録するクラス
                fileListManager = new FileListManager(_err, files);
                // FileDragDrop イベントを紐づけする
                this.DragDrop += fileListManager.RegistFileByDragDrop;
                this.richTextBox1.DragDrop += fileListManager.RegistFileByDragDrop;
                // ファイルリストを取得した後にコントロールなどに表示する場合、以下に紐づける
                fileListManager.UpdateFileListAfterEvent += AddValueToControlFromUpdateList;


                // richTextBox1 に書き込んでいる間 DragDrop から UI 処理が戻ってこないので別スレッドで処理する
                // スレッドを生成して開始する
                subThread = new Thread(new ThreadStart(DoLoopSubThread));
                subThread.Start();
            } catch (Exception ex)
            {
                _err.AddException(ex,this,"Form1_Load");
            }
        }

        public void AddValueToControlFromUpdateList(object sender,EventArgs e)
        {
            try
            {
                _sender = sender;
                _e = e;
                WriteToEditBox = true;
            } catch (Exception ex)
            {
                _err.AddException(ex, this, "AddValueToControlFromUpdateList");
            }
        }
        //public delegate void AddValueToControlFromUpdateList_Del(object sender, EventArgs e);
        public delegate void DelegateSubThreadMethod();



        private void SubThreadMethod()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new DelegateSubThreadMethod(this.SubThreadMethod));
                    return;
                }
                this.richTextBox1.AppendText(ToRichtextValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {

            }
        }
        public void DoLoopSubThread()
        {
            try
            {
                bool swFlag = false;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    if (WriteToEditBox)
                    {
                        AddValueToControlFromUpdateList_Main(_sender, _e);
                        WriteToEditBox = false;
                        SubThreadMethod();
                    }
                        if (sw.ElapsedMilliseconds % 10000 == 0)
                    {
                        if (!swFlag) { Console.WriteLine("SubThread Working"); swFlag = true; }                        
                    } else { swFlag = false; }
                    if (IsClosingForm) { Console.WriteLine("SubThread Loop End"); break; }
                }
            } catch (Exception ex)
            {
                Console.WriteLine("  Thread=" + Thread.CurrentThread.ManagedThreadId);
                _err.AddException(ex, this, "DoLoopSubThread");
            }
        }

        public void AddValueToControlFromUpdateList_Main(object sender,EventArgs e)
        {
            try
            {
                _err.AddLog(this, "AddValueToControlFromUpdateList_Main");
                _err.AddLog("  Thread="+Thread.CurrentThread.ManagedThreadId);
                // ファイルリストを更新した後に実行する
                // コントロールにファイルリストを表示する
                if (sender.GetType().Equals(typeof(string[])))
                {
                    string buf = "";
                    buf += "" + "----------------\n";
                    string[] ary = (string[])sender;
                    if(ary.Length < 1) { buf += "\n" + "ary.Length < 1"; }
                    foreach(string val in ary)
                    {
                        buf += val + "\n";
                        //richTextBox1.AppendText(val);
                    }
                    buf = buf.Substring(1, buf.Length - 1); // 最後の1文字\nを消す
                    ToRichtextValue = buf;
                } else
                {
                    _err.AddLogAlert(this,"  sender is Invalid Type");
                }

            } catch (Exception ex)
            {
                Console.WriteLine("  Thread=" + Thread.CurrentThread.ManagedThreadId);
                _err.AddException(ex, this, "AddValueToControlFromUpdateList_Main Failed");
            }
        }

        public void JumpSource() { }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

            } catch (Exception ex)
            {
                _err.AddException(ex, this, "AddValueToControlFromUpdateList");
            }
        }
    }
}
