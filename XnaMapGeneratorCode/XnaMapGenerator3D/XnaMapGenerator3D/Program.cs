using System;

namespace XnaMapGenerator3D
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BGame game = new BGame())
            {
                game.Run();
            }
        }
    }
#endif
}

