using System;
using Digger.Controller;

namespace Digger
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// Wywo³uje uruchomienie gry.
        /// </summary>
        static void Main(string[] args)
        {
            using (Window game = new Window())
            {
                game.Run();
            }
        }
    }
#endif
}

