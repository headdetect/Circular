using System;

namespace Circular {
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using ( CircularGame game = new CircularGame() )
            {
                game.Run();
            }
        }
    }
#endif
}

