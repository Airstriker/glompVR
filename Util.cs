using System;
using System.Drawing;

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

		/// <summary>
		/// Converts a System.Drawing.Color to a System.Int32.
		/// </summary>
		/// <param name="c">The System.Drawing.Color to convert.</param>
		/// <returns>A System.Int32 containing the R, G, B, A values of the
		/// given System.Drawing.Color in the Rbga32 format.</returns>
		public static int ColorToRgba32(Color c)
		{
			return (int)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);
		}
	}
}

