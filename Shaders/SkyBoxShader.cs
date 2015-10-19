using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace glomp
{
	public class SkyBoxShader : Shader //This is a singleton class
	{
		private static SkyBoxShader instance;

		public float SpotLightDirectionAngle { get; set; }

		/// <summary>
		/// Address of the transposed inverse of Model matrix uniform
		/// </summary>
		private int m_3x3_inv_transp_location;

		/// <summary>
		/// Address of the inverse of View matrix uniform
		/// </summary>
		private int v_inv_location;

		/// <summary>
		/// Address of the texture uniform
		/// </summary>
		private int texture_location;

		/// <summary>
		/// Address of the spotLightDirectionAngle uniform
		/// </summary>
		private static int spotLightDirectionAngle_location;


		private void InitShaderProgram()
		{
			/** In this function, we'll start with a call to the GL.CreateProgram() function,
             * which returns the ID for a new program object, which we'll store in pgmID. */
			ShaderProgramID = GL.CreateProgram();

            ShadersUtil.loadShader("..\\..\\Shaders\\GLSL Shaders\\skyBoxVertexShader.glsl", ShaderType.VertexShader, ShaderProgramID, out vertexShaderID);
            ShadersUtil.loadShader("..\\..\\Shaders\\GLSL Shaders\\skyboxFragmentShader.glsl", ShaderType.FragmentShader, ShaderProgramID, out fragmentShaderID);

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
			// obtain location of transposed inverse of Model uniform
			m_3x3_inv_transp_location = GL.GetUniformLocation(ShaderProgramID, "m_3x3_inv_transp");
			// obtain location of inverse of View uniform
			v_inv_location = GL.GetUniformLocation(ShaderProgramID, "v_inv");
			// get texture uniform location
			texture_location = GL.GetUniformLocation(ShaderProgramID, "tex");
			// get spotLightDirectionAngle uniform location
			spotLightDirectionAngle_location = GL.GetUniformLocation(ShaderProgramID, "spotLightDirectionAngle");
		}


		public void SetShaderUniforms(Matrix4 modelMatrix)
		{
			Matrix3 m_3x3_inv_transp = Matrix3.Transpose(Matrix3.Invert(new Matrix3(modelMatrix))); //the transposed inverse of Model matrix
			Matrix4 v_inv = Matrix4.Invert (ShadersCommonProperties.viewMatrix); //the inverse of View matrix uniform

			// set the uniform
			GL.UniformMatrix4(Projection_location, false, ref ShadersCommonProperties.projectionMatrix);
			GL.UniformMatrix4(View_location, false, ref ShadersCommonProperties.viewMatrix);
			GL.UniformMatrix4(Model_location, false, ref modelMatrix);
			GL.UniformMatrix3(m_3x3_inv_transp_location, false, ref m_3x3_inv_transp);
			GL.UniformMatrix4(v_inv_location, false, ref v_inv);
			GL.Uniform1(spotLightDirectionAngle_location, SpotLightDirectionAngle); //please note that the lights movement speed is faster than skyBox rotation
			GL.Uniform1 (texture_location, 0); // set texture uniform (use texture bind to texture unit 0 (search for usages of GL.ActiveTexture(TextureUnit.Texture0))
		}


		private SkyBoxShader ()
		{
			InitShaderProgram ();
		}


		public static SkyBoxShader Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new SkyBoxShader();
				}
				return instance;
			}
		}
	}
}

