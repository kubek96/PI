using System;
using System.IO;

namespace Digger
{
    public static class Logger
    {
        //private static StreamWriter _logFile;
        private static Uri _fileUri;

        static Logger()
        {
            _fileUri = new Uri(@"c:\Digger\DiggerLog.txt");
            using (StreamWriter stream = new StreamWriter(_fileUri.AbsolutePath))
            {
                stream.WriteLine("Digger logger is running.\nSession from: " + DateTime.Now);
            }
        }

        public static void RedirectTo(string path)
        {
            try
            {
                _fileUri = new Uri(@path);
                Report("Digger logger was redirected.\nNew session from: " + DateTime.Now);
            }
            catch (Exception e)
            {
                Report(e.Message);
            }
        }

        public static void Report(string s)
        {
            using (StreamWriter stream = new StreamWriter(_fileUri.AbsolutePath, true))
            {
                stream.WriteLine(s);
            }
        } 
    }
}