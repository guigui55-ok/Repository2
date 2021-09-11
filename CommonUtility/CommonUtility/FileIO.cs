using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class FileIO
    {
        protected ErrorManager.ErrorManager _error;
        public FileIO(ErrorManager.ErrorManager error)
        {
            _error = error;
        }

        public void WriteFile(string filePath,bool isAppend ,string writeData)
        {
            try
            {
                _error.AddLog(this, "WriteFile");
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                using (StreamWriter writer = new StreamWriter(filePath, isAppend, sjisEnc))
                {
                    writer.Write(writeData);
                }
            } catch (Exception ex)
            {                
                _error.AddException(ex,this.ToString()+".WriteFile : " + filePath);
            }
        }

        public List<string> ReadFile(string filePath)
        {
            List<string> retList = new List<string>();
            try
            {
                if (!System.IO.File.Exists(filePath)) { throw new Exception("FilePath Is Not Exists ["+filePath + "]"); }
                string line;
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                // Read the file and display it line by line.  
                System.IO.StreamReader file = new System.IO.StreamReader(filePath, sjisEnc);
                while ((line = file.ReadLine()) != null)
                {
                    retList.Add(line);
                }

                return retList;
            } catch (Exception ex)
            {
                _error.AddException(ex, this.ToString() + ".ReadFile : " + filePath);
                return retList;
            }
        }
    }
}
