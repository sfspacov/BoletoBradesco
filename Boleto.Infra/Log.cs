using System;
using System.Configuration;
using System.IO;

namespace Boleto.Infra
{
    public class Log
    {
        public static void Write(string message, string fileName = null)
        {
            var beginMessage = DateTime.Now.ToLongTimeString() + ": ";

            var directory = ConfigurationManager.AppSettings["LogFolder"];

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string file;
            var dateString = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            if (string.IsNullOrEmpty(fileName))
                file = dateString;
            else
                file = fileName + "_" + dateString;

            var fullPath = directory + "//" + file;

            var finalMessage = string.Format("{0}{1}", beginMessage, message);

            var logWriter = !File.Exists(fullPath) ? File.CreateText(fullPath) : File.AppendText(fullPath);
            logWriter.WriteLine(finalMessage);
            logWriter.Close();
            logWriter.Dispose();
        }
    }

}