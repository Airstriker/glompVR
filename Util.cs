using System;
using System.Drawing;
using OpenTK;

namespace glomp
{
	public static class Util
	{
		private static bool debug = true;
		private static string log_path = "C:\\dupa.txt";

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


		public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height ) {
			Bitmap result = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(result))
				g.DrawImage(sourceBMP, 0, 0, width, height);
			return result;
		}


		//by Kamujin - check: http://www.opentk.com/node/789
		public static void MatrixToFloatArray(Matrix4 source, ref float[] destination)
		{
			if (destination == null || destination.Length != 16)
			{
				destination = new float[16];
			}

			destination[0] = source.Column0.X;
			destination[1] = source.Column1.X;
			destination[2] = source.Column2.X;
			destination[3] = source.Column3.X;
			destination[4] = source.Column0.Y;
			destination[5] = source.Column1.Y;
			destination[6] = source.Column2.Y;
			destination[7] = source.Column3.Y;
			destination[8] = source.Column0.Z;
			destination[9] = source.Column1.Z;
			destination[10] = source.Column2.Z;
			destination[11] = source.Column3.Z;
			destination[12] = source.Column0.W;
			destination[13] = source.Column1.W;
			destination[14] = source.Column2.W;
			destination[15] = source.Column3.W;
		}


		public static DateTime ConvertFromUnixTimestamp(UInt64 timestamp) {
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return origin.AddSeconds(timestamp);
		}
	}
}

