using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ImageViewer.Common
{
    class FileIO
    {
        public string ExcuteRead(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(
                    path, Encoding.GetEncoding("Shift_JIS"));

                string text = sr.ReadToEnd();

                sr.Close();

                //Console.Write(text);
                return text;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".ExcuteRead Failed");
                Debug.WriteLine(ex.Message);
                return "";
            }
        }
        public int ExcuteWrite(string path, string writeData)
        {
            try
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                StreamWriter writer =
                  new StreamWriter(path, true, sjisEnc);
                writer.Write(writeData);
                writer.Close();
                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".ExcuteWrite Failed");
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        public int ExcuteOverWrite(string path, string writeData)
        {
            try
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                StreamWriter writer = new StreamWriter(path, true, sjisEnc);
                writer.Write(writeData);
                writer.Close();
                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this.ToString() + ".ExcuteWrite Failed");
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
