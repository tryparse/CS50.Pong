using System;

namespace CS50.Pong
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            using var game = new PongGame();
            game.Run();
        }
    }
}