using System;

namespace GGJ2014
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TheyDontThinkItBeLikeItIsButItDo game = new TheyDontThinkItBeLikeItIsButItDo())
            {
                game.Run();
            }
        }
    }
#endif
}

