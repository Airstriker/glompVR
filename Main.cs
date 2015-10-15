using System;
using System.Windows.Forms;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;

namespace glomp
{
    class MainClass
    {


        public static void Main (string[] args)
        {
            OpenTK.Toolkit.Init();

			using (var game = new MainWindow())
			{
				string version = GL.GetString(StringName.Version);
				Console.WriteLine(version);
				if (version.StartsWith("4.")) game.Run(0); //game.Run(60, 60);
				else Debug.WriteLine("Requested OpenGL version not available.");
			}
        }
    }
}
