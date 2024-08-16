using ErrorUtility;
using System;

namespace ErrorManagerTest
{
    class Program
    {
        static void Main()
        {
            string folderPath = System.IO.Directory.GetCurrentDirectory();
            string errorLogFileName = "Error.Log";
            string logFileName = "Log.Log";
            //string ErrorLogPath = folderPath + "\\" + logFileName;
            //string LogPath = folderPath + logFileName;

            //ErrorManager.ErrorManager error = new ErrorManager.ErrorManager(1);
            ErrorManager error = new ErrorManager(1,folderPath, errorLogFileName,logFileName);

            error.AddException(new Exception("TestException"), "TestString");

            error.AddException(new Exception("TestException2"), "TestString2");

            error.AddException(new Exception("TestException3"), "TestString3","TestString3_UserMessage");

            string buf = error.GetErrorMessage();
            Console.WriteLine("GetErrorMessage :" + buf);

            buf = error.GetExceptionMessage();
            Console.WriteLine("GetExceptionMessage : " + buf);

            buf = error.GetMesseges();
            Console.WriteLine("GetMesseges : " + buf);

            if (error.HasException())
            {
                Console.WriteLine("HasException is true");
            } else
            {
                Console.WriteLine("HasException is false");
            }

            Console.WriteLine("\n");
            Console.WriteLine("--------------------");
            Console.WriteLine("UserMessage:");
            Console.WriteLine(error.GetUserMessageOnlyAsString());
            Console.WriteLine("\n");
            Console.WriteLine("--------------------");


            error.AddLog("log value 1", "log notes");
            error.AddLog("log value 2", "");
            error.AddLog(1, 2, "log value 3","MessagetoUser");
            //Type classType = typeof(Program);
            //error.AddLog(TestClass, "log value 3");
            //Console.WriteLine("--------------------");
            //Console.WriteLine("GetLastAlertMessages");
            //Console.WriteLine(error.GetLastAlertMessages()); // ClearErrorされる

            Console.WriteLine("\n--------------------");
            string[] msgs = error.GetLastErrorMessagesAsArray();
            if (msgs.Length > 0)
            {
                foreach(string val in msgs)
                {
                    Console.WriteLine(val);
                }
            }

            Console.WriteLine("\n");
            Console.WriteLine("--------------------");
            Console.WriteLine("error.hasAboveWarning = " + error.hasAboveWarning);


            Console.WriteLine("\n");
            bool ret;
            //string folder = System.IO.Directory.GetCurrentDirectory();
            //string filepath = folder + @"\ErrorLog.log";
            //ret = error.SetLogFilePath(filepath);
            //if (!ret){
            //    Console.WriteLine("SetLogFilePath Not Exists");
            //} else
            //{
            //    Console.WriteLine("SetLogFilePath Success");
            //}

            //ret = error.WriteLog(error.GetErrorMessage());
            ret = error.WriteErrorLog();
            if (!ret)
            {
                Console.WriteLine("WriteErrorLog Failed.");
            } else
            {
                Console.WriteLine("WriteErrorLog Success");
            }
            Console.WriteLine("ErrorLogFilePath : " + error.GetErrorLogPath);


            Console.WriteLine("-----------\n" + error.GetLogDataListAtString());

            ret = error.WriteLog();
            if (!ret)
            {
                Console.WriteLine("WriteLog Failed.");
            }
            else
            {
                Console.WriteLine("WriteLog Success");
            }
            Console.WriteLine("LogFilePath : " + error.GetLogPath);

            Console.ReadKey();

        }

        private class TestClass
        {

        }
    }
}
