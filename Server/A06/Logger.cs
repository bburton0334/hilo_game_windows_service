//============================================================================
/*
* FILE              : Logger.cs
* PROJECT           : PROG2121 - A06
* PROGRAMMER        : Briana Burton & Sunraj Sharma
* FIRST VERSION     : 2021-11-26
* DESCRIPTION       :
*   The function in this file are used to give the service the ability to log
*   to a txt file called log.txt. The functions here will recive a message
*   as a prameter and write to the log file the message. If a log.txt file 
*   does not exist, one will be created and the message will be logged.
*/
//============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace A06
{
    public static class Logger
    {
        //============================================================================
        // FUNCTION         : Log
        // DESCRIPTION      :
        //      Function which passes the message parameter to the LogWrite function
        //      so the message can be written to the log file.
        // PARAMETERS       : message (holds the message to be logged)
        // RETURNS          : none
        //============================================================================
        public static void Log(string message)
        {
            LogWriter(message);
        }

        //============================================================================
        // FUNCTION         : LogWriter
        // DESCRIPTION      :
        //      Function which is called so the passed logMessage can be written to 
        //      the log file. The function will get the current directory of the
        //      service and write to the log file there. If a log.txt file does
        //      not already exist, and new file will be created. The message
        //      then be written to the log file in a format which states the time
        //      and the message.
        // PARAMETERS       : logMessage (holds the message to be logged)
        // RETURNS          : none
        //============================================================================
        public static void LogWriter(string logMessage)
        {
            // getting service directory location
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            string file = "log.txt";

            // Creating a log file if one does not already exist
            if (!File.Exists(path + "\\" + file))
            {
                // Create a file to write to. Writting message to log.
                using (StreamWriter sw = File.CreateText(path + "\\" + file))
                {
                    sw.Write("\r\nLog Entry : ");
                    sw.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    sw.WriteLine("  :");
                    sw.WriteLine("  :{0}", logMessage);
                    sw.WriteLine("-------------------------------");
                }
            }
            else
            {
                try
                {
                    // log file already exists. Writting log message to file.
                    using (StreamWriter w = File.AppendText(path + "\\" + file))
                    {
                        w.Write("\r\nLog Entry : ");
                        w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                        w.WriteLine("  :");
                        w.WriteLine("  :{0}", logMessage);
                        w.WriteLine("-------------------------------");
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
