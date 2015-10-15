using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System.IO;

namespace glomp
{
	public class LabelShader : Shader //This is a singleton class
	{
		private static LabelShader instance;

		public Color4 LabelColor { get; set; }


		/// <summary>
		/// Address of the texture uniform
		/// </summary>
		private int texture_location;

		/// <summary>
		/// Address of the label color uniform
		/// </summary>
		private int color_location;


		private void InitShaderProgram()
		{
			/** In this function, we'll start with a call to the GL.CreateProgram() function,
             * which returns the ID for a new program object, which we'll store in pgmID. */
			ShaderProgramID = GL.CreateProgram();

			ShadersUtil.loadShader("..\\..\\Shaders\\GLSL Shaders\\labelVertexShader.glsl", ShaderType.VertexShader, ShaderProgramID, out vertexShaderID);
			ShadersUtil.loadShader("..\\..\\Shaders\\GLSL Shaders\\labelFragmentShader.glsl", ShaderType.FragmentShader, ShaderProgramID, out fragmentShaderID);

			/** Now that the shaders are added, the program needs to be linked.
             * Like C code, the code is first compiled, then linked, so that it goes
             * from human-readable code to the machine language needed. */
			GL.LinkProgram(ShaderProgramID);
            String programInfoLog;
            GL.GetProgramInfoLog(ShaderProgramID, out programInfoLog);
            if (programInfoLog != "") System.Diagnostics.Debug.WriteLine(programInfoLog);

			// obtain location of Projection uniform
			Projection_location = GL.GetUniformLocation(ShaderProgramID, "Projection");
			// obtain location of View uniform
			View_location = GL.GetUniformLocation(ShaderProgramID, "View");
			// obtain location of Model uniform
			Model_location = GL.GetUniformLocation(ShaderProgramID, "Model");
			// get texture uniform location
			texture_location = GL.GetUniformLocation(ShaderProgramID, "tex");
			// get label color uniform location
			color_location = GL.GetUniformLocation(ShaderProgramID, "color");
		}


		public void SetShaderUniforms(Matrix4 modelMatrix)
		{
			// set the uniform
			GL.UniformMatrix4(Projection_location, false, ref ShadersCommonProperties.projectionMatrix);
			GL.UniformMatrix4(View_location, false, ref ShadersCommonProperties.viewMatrix);
			GL.UniformMatrix4(Model_location, false, ref modelMatrix);
			GL.Uniform4(color_location, LabelColor);
			GL.Uniform1(texture_location, 0); // set texture uniform (use texture bind to texture unit 0 (search for usages of GL.ActiveTexture(TextureUnit.Texture0))
		}


		private LabelShader ()
		{
			InitShaderProgram ();
		}


		public static LabelShader Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new LabelShader();
				}
				return instance;
			}
		}
	}
}

