using System;

namespace CivClone
{
    /// <summary>
    /// Entry point for the Civilization I clone.  This uses MonoGame's
    /// Game class to manage the game loop.  All initialization and
    /// resource loading happens in Game1.
    /// </summary>
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Game1();
            game.Run();
        }
    }
}