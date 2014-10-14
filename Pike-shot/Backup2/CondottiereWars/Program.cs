using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace PikeAndShot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        static List<Level> levels;
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            
            using (PikeAndShotGame game = new PikeAndShotGame())
            {
                game.Run();
            }
        }
    }
}

