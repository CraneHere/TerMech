using System;

namespace CompGraph
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create and run the OpenGLWindow (which is a subclass of GameWindow)
            using (var window = new OpenGLWindow())
            {
                window.Run();
            }
        }
    }
}
