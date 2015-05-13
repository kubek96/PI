using System;
using System.IO;

namespace Digger
{
    /// <summary>
    /// Klasa służąca do udostępniania metod związanych z logowaniem informacji.
    /// </summary>
    public static class Logger
    {
        //private static StreamWriter _logFile;
        private static string _fileName = "DiggerLog.txt";

        /// <summary>
        /// Konstruktor bezparametrowy.
        /// </summary>
        static Logger()
        {
            using (StreamWriter stream = new StreamWriter(_fileName))
            {
                stream.WriteLine("Digger logger is running.\nSession from: " + DateTime.Now);
            }
        }

        /// <summary>
        /// Funkcja umożliwiająca zmienę domyślnej ścieżki do pliku log.
        /// </summary>
        /// <param name="path">Ściezka do pliku.</param>
        public static void RedirectTo(string path)
        {
            try
            {
                _fileName = path;
                Report("Digger logger was redirected.\nNew session from: " + DateTime.Now);
            }
            catch (Exception e)
            {
                Report(e.Message);
            }
        }

        /// <summary>
        /// Funkcja umożliwiająca logowanie komunikatów.
        /// </summary>
        /// <param name="s">Treść komunikatu.</param>
        public static void Report(string s)
        {
            using (StreamWriter stream = new StreamWriter(_fileName, true))
            {
                stream.WriteLine(s);
            }
        } 
    }
}