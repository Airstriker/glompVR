using System;
//using System.Threading;
using System.Windows.Forms;
//using System.Security.Permissions;
using System.Diagnostics;
using Gtk;
using GLib;

namespace glomp
{
    class MainClass
    {
		static void OnException(GLib.UnhandledExceptionArgs args)
		{
			string errorMsg = "An application error occurred. Please contact the adminstrator " +
			                  "with the following information:\n\n";
			errorMsg = errorMsg + args.ExceptionObject.ToString() + "\n";
			MessageBox.Show(errorMsg, "Glib error error {0}", MessageBoxButtons.AbortRetryIgnore,
				MessageBoxIcon.Stop);


			//Debug.WriteLine("Glib error error {0}", args.ExceptionObject.ToString());
			args.ExitApplication = false;
		}

        public static void Main (string[] args)
        {
            //Console.WriteLine(OpenTK.Graphics.GraphicsMode.Default);
			UnhandledExceptionHandler h = new UnhandledExceptionHandler (OnException);
			ExceptionManager.UnhandledException += h;
            OpenTK.Toolkit.Init();
			Gtk.Application.Init ();
            MainWindow win = new MainWindow ();
            win.Show ();
			Console.WriteLine("AAAAA");
			Gtk.Application.Run ();
        }
    }
}
