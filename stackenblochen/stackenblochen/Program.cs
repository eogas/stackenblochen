using System;

namespace stackenblochen
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (StackenBlochen game = new StackenBlochen())
            {
                game.Run();
            }
        }
    }
#endif
}

