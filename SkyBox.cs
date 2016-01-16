using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace glomp
{
	public class SkyBoxShape : Shape
	{
		private float mSize = 0;
		private float numberOfTextureRepeats = 3.0f;

		//Arrays filling constructor (SkyBox shape definition loading)
		public SkyBoxShape(float aSize)
		{
			mSize = aSize;

			VertexData = new float[] //24 Vertices need to be defined, as every face has different texture - every face needs it's own Texture Coordinates; //Smooth shading is enabled
			{//		 X		   Y		  Z				  			U		     		  V						   N	 M	   L
				//front face
				 mSize/2f,  mSize/2f, -mSize/2f, 		numberOfTextureRepeats, 0.0f,							-1.0f, -1.0f,  1.0f,	//V0
				-mSize/2f,  mSize/2f, -mSize/2f, 						  0.0f, 0.0f,							 1.0f, -1.0f,  1.0f,	//V1
				-mSize/2f, -mSize/2f, -mSize/2f, 						  0.0f, numberOfTextureRepeats,			 1.0f,  1.0f,  1.0f,  	//V2
				 mSize/2f, -mSize/2f, -mSize/2f, 		numberOfTextureRepeats, numberOfTextureRepeats,			-1.0f,  1.0f,  1.0f,  	//V3

				//left face
				-mSize/2f,  mSize/2f,  mSize/2f, 						  0.0f, 0.0f, 							 1.0f, -1.0f, -1.0f,	//V4
				-mSize/2f, -mSize/2f,  mSize/2f, 						  0.0f, numberOfTextureRepeats, 		 1.0f,  1.0f, -1.0f,	//V5
				-mSize/2f, -mSize/2f, -mSize/2f, 		numberOfTextureRepeats, numberOfTextureRepeats,			 1.0f,  1.0f,  1.0f,	//V6
				-mSize/2f,  mSize/2f, -mSize/2f, 		numberOfTextureRepeats, 0.0f, 							 1.0f, -1.0f,  1.0f,	//V7

				//back face
				 mSize/2f,  mSize/2f,  mSize/2f, 						  0.0f, 0.0f, 							-1.0f, -1.0f, -1.0f,	//V8
				 mSize/2f, -mSize/2f,  mSize/2f, 						  0.0f, numberOfTextureRepeats, 		-1.0f,  1.0f, -1.0f,	//V9
				-mSize/2f, -mSize/2f,  mSize/2f, 		numberOfTextureRepeats, numberOfTextureRepeats, 		 1.0f,  1.0f, -1.0f,	//V10
				-mSize/2f,  mSize/2f,  mSize/2f, 		numberOfTextureRepeats, 0.0f, 							 1.0f, -1.0f, -1.0f,	//V11

				//right face
				 mSize/2f,  mSize/2f, -mSize/2f, 						  0.0f, 0.0f, 							-1.0f, -1.0f,  1.0f,	//V12
				 mSize/2f, -mSize/2f, -mSize/2f, 						  0.0f, numberOfTextureRepeats,			-1.0f,  1.0f,  1.0f, 	//V13
				 mSize/2f, -mSize/2f,  mSize/2f, 		numberOfTextureRepeats, numberOfTextureRepeats, 		-1.0f,  1.0f, -1.0f,	//V14
				 mSize/2f,  mSize/2f,  mSize/2f, 		numberOfTextureRepeats, 0.0f, 							-1.0f, -1.0f, -1.0f,	//V15

				//top face
				 mSize/2f,  mSize/2f,  mSize/2f, 		numberOfTextureRepeats, 0.0f, 							-1.0f, -1.0f, -1.0f,	//V16
				-mSize/2f,  mSize/2f,  mSize/2f, 						  0.0f, 0.0f, 							 1.0f, -1.0f, -1.0f,	//V17
				-mSize/2f,  mSize/2f, -mSize/2f, 						  0.0f, numberOfTextureRepeats, 		 1.0f, -1.0f,  1.0f,	//V18
				 mSize/2f,  mSize/2f, -mSize/2f, 		numberOfTextureRepeats, numberOfTextureRepeats, 		-1.0f, -1.0f,  1.0f,	//V19

				//bottom face
				 mSize/2f, -mSize/2f,  mSize/2f, 		numberOfTextureRepeats, numberOfTextureRepeats, 		-1.0f,  1.0f, -1.0f,	//V20
				 mSize/2f, -mSize/2f, -mSize/2f, 		numberOfTextureRepeats, 0.0f, 							-1.0f,  1.0f,  1.0f,	//V21
				-mSize/2f, -mSize/2f, -mSize/2f, 						  0.0f, 0.0f,							 1.0f,  1.0f,  1.0f,	//V22
				-mSize/2f, -mSize/2f,  mSize/2f, 						  0.0f, numberOfTextureRepeats, 		 1.0f,  1.0f, -1.0f		//V23
			};

			Indices = new int[]
			{
				// front face
				0, 1, 2, 2, 3, 0,
				// left face
				4, 5, 6, 6, 7, 4,
				// back face
				8, 9, 10, 10, 11, 8,
				// right face
				12, 13, 14, 14, 15, 12,
				// top face
				16, 17, 18, 18, 19, 16,
				// bottom face
				20, 21, 22, 22, 23, 20,
			};


		}
	}

	public class SkyBox
	{
		private float mSize; //obsolete when using VBO

		/* TEXTURES  */
		/* SkyBox Texture - used with VBO (more textures would be related to much higher overhead when using VBO) */
        String skyBoxTexturePath = "..\\..\\resources\\skybox_front.dds";
		private static uint skyBoxSingleTexture;

		/* TEXTURE INDEXes  */
		private static readonly int SKY_FRONT = 0;
		private static readonly int SKY_RIGHT = 1;
		private static readonly int SKY_LEFT = 2;
		private static readonly int SKY_BACK = 3;
		private static readonly int SKY_UP = 4;
		private static readonly int SKY_DOWN = 5;

		//VBO related stuff
		VBOUtil.Vbo vbo; //vertex buffer object
		int vao; //array buffer object


		//Constructor to be used with VBO concept. Only single texture used. DisplayList not neaded any longer.
		public SkyBox (float aSize)
		{
			mSize = aSize;
			if (skyBoxTexturePath !=null) {
                TextureTarget textureTarget;
                TextureLoaderParameters.MagnificationFilter = TextureMagFilter.Linear;
                TextureLoaderParameters.MinificationFilter = TextureMinFilter.Linear;
                ImageDDS.LoadFromDisk(skyBoxTexturePath, out skyBoxSingleTexture, out textureTarget);
                System.Diagnostics.Debug.WriteLine("Loaded " + skyBoxTexturePath + " with handle " + skyBoxSingleTexture + " as " + textureTarget);
			}

			//Using VBO concept instead of DisplayList
			// loading Vertex Buffers
			Shape skyBoxShape = new SkyBoxShape(aSize);
			vbo = VBOUtil.LoadVBO(skyBoxShape);
			VBOUtil.ConfigureVertexArrayObject(out vao, vbo);
		}


		public void DrawSkyBox(float frameDelta)
		{
			/* In some cases, you might want to disable depth testing and still allow the depth buffer updated while you are rendering your objects.
			 * It turns out that if you disable depth testing (glDisable(GL_DEPTH_TEST)​), GL also disables writes to the depth buffer.
			 * The correct solution is to tell GL to ignore the depth test results with glDepthFunc(GL_ALWAYS)​.
			 * Be careful because in this state, if you render a far away object last, the depth buffer will contain the values of that far object.
			*/
			//GL.DepthFunc(DepthFunction.Always); //GL.Disable (EnableCap.DepthTest);
			//GL.DepthMask (false);

			// use the shader program (must be initiated firstly by calling InitSkyBoxShadersProgram())
			GL.UseProgram(SkyBoxShader.Instance.ShaderProgramID);

			// bind texture to texture unit 0
			GL.ActiveTexture (TextureUnit.Texture0);
			GL.BindTexture (TextureTarget.Texture2D, skyBoxSingleTexture); //Only one texture used

			//Rotates, making sure it rotates around the center of the cube; the spot lights also move around in the cube

			//The spot lights are moving in constant speed
			SkyBoxShader.Instance.SpotLightDirectionAngle += 0.0057f * frameDelta * 300; //fps independent animation

			// set up transforms which are inverse of our position
			// ie bring the world to meet the camera rather than move the camera

			Matrix4 initialTranslationMatrix = Matrix4.Identity; //Matrix4.CreateTranslation (0.5f, 1.0f, 0.0f);
			Matrix4 rotationXMatrix = Matrix4.CreateRotationX (Util.DegreesToRadians(SkyBoxShader.Instance.SpotLightDirectionAngle * 3.0f)); //please note that the lights movement speed is faster than skyBox rotation
			Matrix4 rotationYMatrix = Matrix4.CreateRotationY (Util.DegreesToRadians(SkyBoxShader.Instance.SpotLightDirectionAngle * 3.0f)); //please note that the lights movement speed is faster than skyBox rotation
			Matrix4 rotationZMatrix = Matrix4.CreateRotationZ (Util.DegreesToRadians(SkyBoxShader.Instance.SpotLightDirectionAngle * 3.0f)); //please note that the lights movement speed is faster than skyBox rotation
			Matrix4 finalTranslationMatrix = Matrix4.Identity; //Matrix4.CreateTranslation (-0.5f, -1.0f, 0.0f);
			Matrix4 skyBoxModelMatrix = finalTranslationMatrix * rotationZMatrix * rotationYMatrix * rotationXMatrix * initialTranslationMatrix;

			SkyBoxShader.Instance.SetShaderUniforms(skyBoxModelMatrix);
			VBOUtil.Draw(vao, vbo);

			//GL.DepthMask(true);
			//GL.Clear(ClearBufferMask.DepthBufferBit);
			//GL.Enable(EnableCap.DepthTest);
		}
	}
}

