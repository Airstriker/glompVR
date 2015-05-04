using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace glomp
{
	public static class ShadersUtil
	{
		/// <summary>
		/// This creates a new shader (using a value from the ShaderType enum), loads code for it, compiles it, and adds it to our program.
		/// It also prints any errors it found to the console, which is really nice for when you make a mistake in a shader (it will also yell at you if you use deprecated code).
		/// </summary>
		/// <param name="filename">File to load the shader from</param>
		/// <param name="type">Type of shader to load</param>
		/// <param name="program">ID of the program to use the shader with</param>
		/// <param name="address">Address of the compiled shader</param>
		public static void loadShader(String filename, ShaderType type, int program, out int address)
		{
			address = GL.CreateShader(type);
			using (StreamReader sr = new StreamReader(filename))
			{
				GL.ShaderSource(address, sr.ReadToEnd());
			}
			GL.CompileShader(address);
			GL.AttachShader(program, address);
			System.Diagnostics.Debug.WriteLine(GL.GetShaderInfoLog(address));
		}
	}
}

