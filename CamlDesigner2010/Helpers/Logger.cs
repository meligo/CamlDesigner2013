// -----------------------------------------------------------------------
// <copyright file="Logger.cs" company="Biwug">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CamlDesigner.Helpers
{
    using System;
    using System.IO;

    /// <summary>
    /// Logger class
    /// </summary>
    public static class Logger
    {
        public static void WriteToLogFile(string logMessage, Exception ex)
        {
            string strLogMessage = string.Empty;
            string strLogFile = System.Environment.CurrentDirectory + @"\logfile.log";
            StreamWriter streamWriterLog;
            if (ex == null)
            {
                strLogMessage = string.Format("{0}: {1}", DateTime.Now, logMessage);
            }
            else
            {
                strLogMessage = string.Format("{0}: {1}", DateTime.Now, logMessage + ex.Message + " - " + ex.StackTrace.ToString());
            }

            // check if file exists or not 
            if (!File.Exists(strLogFile))
            {

                streamWriterLog = new StreamWriter(strLogFile);
            }
            else
            {
                FileInfo fileinfo = new FileInfo(strLogFile);

                // check if filesize is bigger than 1 MB,if yes, delete file to restart logging from 0
                if (fileinfo.Length >= 1048576)
                {
                    fileinfo.Delete();
                    streamWriterLog = new StreamWriter(strLogFile);
                }
                else
                {
                    streamWriterLog = File.AppendText(strLogFile);
                }
            }

            streamWriterLog.WriteLine(strLogMessage);
            streamWriterLog.WriteLine();
            streamWriterLog.Close();
        }
    }
}
