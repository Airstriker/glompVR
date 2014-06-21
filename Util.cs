using System;

namespace glomp
{
	public class Util
	{
		private static bool debug = true;
		private static string log_path = "C:\\dupa.txt";
		
		public Util ()
		{
		}

		public static float DegreesToRadians(float degrees) {
			return (float)(degrees * Math.PI / 180f);
		}

		public static float RadiansToDegrees(float radians) {
			return (float)(radians * 180f / Math.PI);
		}

		public static void Trace (string log_text) {
			if (debug) {
					using (System.IO.StreamWriter file = new System.IO.StreamWriter (@log_path, true)) {
						file.WriteLine (log_text);
					}
			}
		}
	}
}

