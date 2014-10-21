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

			Vertices = new Vector3[] //24 Vertices need to be defined, as every face has different texture - every face needs it's own Texture Coordinates
			{
				//front face
				new Vector3(  mSize/2f, mSize/2f, -mSize/2f ), //V0
				new Vector3( -mSize/2f, mSize/2f, -mSize/2f ), //V1
				new Vector3( -mSize/2f, -mSize/2f, -mSize/2f ), //V2
				new Vector3(  mSize/2f, -mSize/2f, -mSize/2f ), //V3

				//left face
				new Vector3( -mSize/2f, mSize/2f, mSize/2f ), //V4
				new Vector3( -mSize/2f, -mSize/2f, mSize/2f ), //V5
				new Vector3( -mSize/2f, -mSize/2f, -mSize/2f ), //V6
				new Vector3( -mSize/2f, mSize/2f, -mSize/2f ), //V7

				//back face
				new Vector3( mSize/2f, mSize/2f,  mSize/2f ), //V8
				new Vector3( mSize/2f, -mSize/2f,  mSize/2f ), //V9
				new Vector3( -mSize/2f, -mSize/2f, mSize/2f ), //V10
				new Vector3( -mSize/2f, mSize/2f, mSize/2f ), //V11

				//right face
				new Vector3(  mSize/2f, mSize/2f, -mSize/2f ), //V12
				new Vector3(  mSize/2f, -mSize/2f, -mSize/2f ), //V13
				new Vector3( mSize/2f, -mSize/2f,  mSize/2f ), //V14
				new Vector3( mSize/2f, mSize/2f,  mSize/2f ), //V15

				//top face
				new Vector3( mSize/2f, mSize/2f,  mSize/2f ), //V16
				new Vector3( -mSize/2f, mSize/2f, mSize/2f ), //V17
				new Vector3( -mSize/2f, mSize/2f, -mSize/2f ), //V18
				new Vector3(  mSize/2f, mSize/2f, -mSize/2f ), //V19

				//bottom face
				new Vector3( mSize/2f, -mSize/2f,  mSize/2f ), //V20
				new Vector3(  mSize/2f, -mSize/2f, -mSize/2f ), //V21
				new Vector3( -mSize/2f, -mSize/2f, -mSize/2f ), //V22
				new Vector3( -mSize/2f, -mSize/2f, mSize/2f ), //V23
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

			Normals = new Vector3[] //Smooth shading is enabled
			{
				//front face
				new Vector3( -1.0f, -1.0f, 1.0f),
				new Vector3( 1.0f, -1.0f, 1.0f),
				new Vector3( 1.0f, 1.0f, 1.0f),
				new Vector3( -1.0f, 1.0f, 1.0f),

				//left face
				new Vector3( 1.0f, -1.0f, -1.0f),
				new Vector3( 1.0f, 1.0f, -1.0f),
				new Vector3( 1.0f, 1.0f, 1.0f),
				new Vector3( 1.0f, -1.0f, 1.0f),

				//back face
				new Vector3( -1.0f, -1.0f,-1.0f),
				new Vector3( -1.0f, 1.0f,-1.0f),
				new Vector3( 1.0f, 1.0f,-1.0f),
				new Vector3( 1.0f, -1.0f,-1.0f),

				//right face
				new Vector3( -1.0f, -1.0f, 1.0f),
				new Vector3( -1.0f, 1.0f, 1.0f),
				new Vector3( -1.0f, 1.0f,-1.0f),
				new Vector3( -1.0f, -1.0f,-1.0f),

				//top face
				new Vector3( -1.0f, -1.0f, -1.0f),
				new Vector3( 1.0f, -1.0f, -1.0f),
				new Vector3( 1.0f, -1.0f, 1.0f),
				new Vector3( -1.0f, -1.0f, 1.0f),

				// bottom face
				new Vector3( -1.0f, 1.0f, -1.0f),
				new Vector3( -1.0f, 1.0f, 1.0f),
				new Vector3( 1.0f, 1.0f, 1.0f),
				new Vector3( 1.0f, 1.0f, -1.0f)
			};

			/*
			Colors = new int[]
			{
				Util.ColorToRgba32(Color.DarkRed),
				Util.ColorToRgba32(Color.DarkRed),
				Util.ColorToRgba32(Color.Gold),
				Util.ColorToRgba32(Color.Gold),
				Util.ColorToRgba32(Color.DarkRed),
				Util.ColorToRgba32(Color.DarkRed),
				Util.ColorToRgba32(Color.Gold),
				Util.ColorToRgba32(Color.Gold),
			};
			*/

			Texcoords = new Vector2[]
			{
				//front face
				new Vector2(numberOfTextureRepeats, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, numberOfTextureRepeats),
				new Vector2(numberOfTextureRepeats, numberOfTextureRepeats),

				//left face
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, numberOfTextureRepeats),
				new Vector2(numberOfTextureRepeats, numberOfTextureRepeats),
				new Vector2(numberOfTextureRepeats, 0.0f),

				//back face
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, numberOfTextureRepeats),
				new Vector2(numberOfTextureRepeats, numberOfTextureRepeats),
				new Vector2(numberOfTextureRepeats, 0.0f),

				//right face
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, numberOfTextureRepeats),
				new Vector2(numberOfTextureRepeats, numberOfTextureRepeats),
				new Vector2(numberOfTextureRepeats, 0.0f),

				//top face
				new Vector2(numberOfTextureRepeats, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, numberOfTextureRepeats),
				new Vector2(numberOfTextureRepeats, numberOfTextureRepeats),

				// bottom face
				new Vector2(numberOfTextureRepeats, numberOfTextureRepeats),
				new Vector2(numberOfTextureRepeats, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, numberOfTextureRepeats)
			};
		}
	}

	public class SkyBox
	{
		private float mSize; //obsolete when using VBO

		private int displayList; //DisplayList for SkyBox - obsolete

		/* TEXTURES  */
		private static int[] skyBoxTexture = new int[6];
		private static int skyBoxSingleTexture;

		/* TEXTURE INDEXes  */
		private static readonly int SKY_FRONT = 0;
		private static readonly int SKY_RIGHT = 1;
		private static readonly int SKY_LEFT = 2;
		private static readonly int SKY_BACK = 3;
		private static readonly int SKY_UP = 4;
		private static readonly int SKY_DOWN = 5;

		/* SKYBOX LIGHTENING */
		private float spotLightDirectionAngle = 0.0f;

		//VBO related stuff
		VBOUtil.Vbo vbo; //vertex buffer object
		int vao; //array buffer object

		[Obsolete ("Use one texture instead - much lower overhead when using VBO.")]
		public SkyBox (float aSize, String[] textures)
		{
			mSize = aSize;
			if (textures !=null && textures.Length == 6) {
				skyBoxTexture[SKY_FRONT] = TextureManager.LoadTexture(textures[SKY_FRONT]);
				skyBoxTexture[SKY_RIGHT] = TextureManager.LoadTexture(textures[SKY_RIGHT]);
				skyBoxTexture[SKY_LEFT] = TextureManager.LoadTexture(textures[SKY_LEFT]);
				skyBoxTexture[SKY_BACK] = TextureManager.LoadTexture(textures[SKY_BACK]);
				skyBoxTexture[SKY_UP] = TextureManager.LoadTexture(textures[SKY_UP]);
				skyBoxTexture[SKY_DOWN] = TextureManager.LoadTexture(textures[SKY_DOWN]);
			}
			else {
				System.Console.WriteLine("Textures array needs to have exactly 6 elements!");
				return;
			}

			GenerateDisplayList();
		}

		//Constructor to be used with VBO concept. Only single texture used. DisplayList not neaded any longer.
		public SkyBox (float aSize, String texture)
		{
			mSize = aSize;
			if (texture !=null) {
				skyBoxSingleTexture = TextureManager.LoadTexture(texture);
			}

			//GenerateDisplayList();

			//Using VBO concept instead of DisplayList
			// loading Vertex Buffers
			Shape skyBoxShape = new SkyBoxShape(aSize);
			vbo = VBOUtil.LoadVBO(skyBoxShape);
			VBOUtil.ConfigureVertexArrayObject(out vao, vbo);
		}			

		//Ordinary skyboxes have lightening disabled but this is rather a box than skybox ;)
		//NOTE! We're also disabling main light (Light0) here
		private void enableLights(ref float frameDelta)
		{
			/*
			GLfloat white[] = {0.8f, 0.8f, 0.8f, 1.0f};
			GLfloat cyan[] = {0.f, .8f, .8f, 1.f};
			glMaterialfv(GL_FRONT, GL_DIFFUSE, cyan);
			glMaterialfv(GL_FRONT, GL_SPECULAR, white);
			GLfloat shininess[] = {50};
			glMaterialfv(GL_FRONT, GL_SHININESS, shininess);
			*/

			//The spot lights are moving in constant speed
			spotLightDirectionAngle += 0.0057f * frameDelta * 300.0f; //fps independent animation

			float[] lightAmbient = { 0.1f, 0.1f, 0.1f, 1.0f }; //The first three floats represent the RGB intensity of the ambient light
			float[] lightDiffuse = { 0.2f, 0.2f, 0.2f, 1.0f }; //The first three floats set the color / intensity of the positional/spot light
			float[] lightPosition = { 0.0f, 0.0f,  -1.0f, 0.0f }; //The first three elements of the array are the light position
			float[] lightDirection2 = { (float)Math.Cos(spotLightDirectionAngle), 0.0f,  1.0f }; //Three elements of the array are the spot light direction in homogeneous object coordinates
			float[] lightDirection3 = { 0.0f, (float)Math.Cos(spotLightDirectionAngle),  1.0f }; //Three elements of the array are the spot light direction in homogeneous object coordinates
			float spotCutoffAngle = 50.0f; //maximum spread angle of a light source
			float spotExponent = 10.0f; //specifies the intensity distribution of the light. Range 0-128. Higher value means more focused light source, regardless of the spot cut off angle.

			float[] white = { 0.8f, 0.8f, 0.8f, 1.0f };
			float[] cyan = { 0.0f, 0.8f, 0.8f, 0.0f };
			float[] shininess = {10.0f}; //number between 0 and 128, where 0 is the shiniest the object can be.

			float[] whiteSpecularLight = {1.0f, 1.0f, 1.0f};
			float[] whiteSpecularMaterial = {0.3f, 0.3f, 0.3f};

			//GL.LightModel (LightModelParameter.LightModelColorControl, (int)OpenTK.Graphics.ES20.All.SeparateSpecularColor);

			//GL.Disable(EnableCap.ColorMaterial);

			//GL.Enable (EnableCap.Normalize); //too slow - better to do it ourselves if changing the scale anywhere

			//GL.Material (MaterialFace.FrontAndBack, MaterialParameter.Diffuse, white);
			//GL.Material (MaterialFace.FrontAndBack, MaterialParameter.Specular, whiteSpecularMaterial);
			//GL.Material (MaterialFace.FrontAndBack, MaterialParameter.Shininess, shininess);
			//GL.Light(LightName.Light0, LightParameter.Specular, whiteSpecularLight);

			//GL.Material (MaterialFace.FrontAndBack, MaterialParameter.Shininess, shininess);
			//GL.Material (MaterialFace.FrontAndBack, MaterialParameter.Emission, cyan);

			//Disabling main light when drawing skybox
			GL.Disable (EnableCap.Light0);

			//Spot Light 1
			GL.Light(LightName.Light1, LightParameter.Diffuse, lightDiffuse);
			GL.Light (LightName.Light1, LightParameter.SpotDirection, lightDirection2);
			GL.Light (LightName.Light1, LightParameter.SpotCutoff, spotCutoffAngle);
			GL.Light (LightName.Light1, LightParameter.SpotExponent, spotExponent);
			GL.Light (LightName.Light1, LightParameter.LinearAttenuation, 0.1f);
			GL.Light (LightName.Light1, LightParameter.QuadraticAttenuation, 0.1f);
			GL.Light (LightName.Light1, LightParameter.ConstantAttenuation, 0.1f);
			GL.Enable(EnableCap.Light1);

			//Spot Light 2
			GL.Light(LightName.Light2, LightParameter.Diffuse, lightDiffuse);
			GL.Light (LightName.Light2, LightParameter.SpotDirection, lightDirection3);
			GL.Light (LightName.Light2, LightParameter.SpotCutoff, spotCutoffAngle);
			GL.Light (LightName.Light2, LightParameter.SpotExponent, spotExponent);
			GL.Light (LightName.Light2, LightParameter.LinearAttenuation, 0.1f);
			GL.Light (LightName.Light2, LightParameter.QuadraticAttenuation, 0.1f);
			GL.Light (LightName.Light2, LightParameter.ConstantAttenuation, 0.1f);
			GL.Enable(EnableCap.Light2);

			//Ambient Light
			GL.Light(LightName.Light3, LightParameter.Ambient, lightAmbient);
			GL.Light(LightName.Light3, LightParameter.Diffuse, lightDiffuse);
			GL.Light(LightName.Light3, LightParameter.ConstantAttenuation, 0.8f);
			GL.Enable(EnableCap.Light3);
		}

		//Disable all SkyBox lights and re-enable main light (Light0)
		private void disableLights()
		{
			GL.Disable (EnableCap.Light1); //Spot Light
			GL.Disable (EnableCap.Light2); //Spot Light
			GL.Disable (EnableCap.Light3); //Ambient Light
			GL.Enable (EnableCap.Light0); //Main Light
		}

		public void drawSkyBox(ref float frameDelta)
		{
			/* In some cases, you might want to disable depth testing and still allow the depth buffer updated while you are rendering your objects.
			 * It turns out that if you disable depth testing (glDisable(GL_DEPTH_TEST)​), GL also disables writes to the depth buffer.
			 * The correct solution is to tell GL to ignore the depth test results with glDepthFunc(GL_ALWAYS)​.
			 * Be careful because in this state, if you render a far away object last, the depth buffer will contain the values of that far object.
			*/
			//GL.DepthFunc(DepthFunction.Always); //GL.Disable (EnableCap.DepthTest);
			//GL.DepthMask (false);
			GL.BindTexture(TextureTarget.Texture2D, skyBoxSingleTexture); //Only one texture used


			//Changing Projection matrix to change FOV to 90 degrees - better SkyBox quality than in 22,5 degrees.
			/*
			GL.PushMatrix ();
			GL.MatrixMode(MatrixMode.Projection);
			GL.PushMatrix ();
			Matrix4 projection;
			if(glwidget1 == null) {
				GL.Viewport(0, 0, START_WIDTH, START_HEIGHT);
				projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2, START_WIDTH / (float)START_HEIGHT, 0.01f, 500f);
			} else {
				GL.Viewport(0, 0, glwidget1.Allocation.Width, glwidget1.Allocation.Height);    
				projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2, glwidget1.Allocation.Width / (float)glwidget1.Allocation.Height, 0.01f, 500f);
			}
			GL.LoadMatrix(ref projection);
			*/

			//GL.Scale(5.0f, 5.0f, 5.0f);

			//Rotates, making sure it rotates around the center of the cube

			GL.PushMatrix ();

			//GL.Translate(0.5f, 1.0f, 0.0f);
			GL.Rotate(spotLightDirectionAngle * 5, 1.0f, 1.0f, 1.0f);
			//GL.Translate(-0.5f, -1.0f, 0.0f);

			this.enableLights (ref frameDelta);

			///////////////////////////////////////////////////////////////////////////////////////////////////
			VBOUtil.Draw(vao, vbo);
			//GL.CallList(displayList); //Use pre-generated DisplayList - obsolete solution
			///////////////////////////////////////////////////////////////////////////////////////////////////

			this.disableLights ();

			GL.PopMatrix ();

			/*
			GL.PopMatrix(); //Restoring Projection matrix

			GL.MatrixMode (MatrixMode.Modelview);
			GL.PopMatrix(); //Restoring Modelview matrix
			*/

			//GL.DepthMask(true);
			//GL.Clear(ClearBufferMask.DepthBufferBit);
			//GL.Enable(EnableCap.Lighting);
			//GL.Enable(EnableCap.DepthTest);
		}
	

		[Obsolete ("Use VBO instead of DisplayList.")]
		private int GenerateDisplayList() {
			displayList = GL.GenLists(1);

			GL.NewList(displayList, ListMode.Compile); // start compiling display list

			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//TO RENDER TEXTURES ON THE INNER SIDE OF THE CUBE, WE MUST DRAW THE VERTEXes COUNTER CLOCK-WISE !!!!!!
			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			float numberOfTextureRepeats = 3.0f;

			// Smooth shading enabled - every vertex has it's own normal. Looks a lot better than flat shading!
			// When a quad is being drawn, it takes a weighted average of the normals at the vertices to determine the normals at different points on the quad

			GL.Begin(PrimitiveType.Quads);

			// Render the front quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_FRONT]);
			GL.Normal3( -1.0f, -1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3(  mSize/2f, mSize/2f, -mSize/2f );
			GL.Normal3( 1.0f, -1.0f, 1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( -mSize/2f, mSize/2f, -mSize/2f );
			GL.Normal3( 1.0f, 1.0f, 1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( -mSize/2f,  -mSize/2f, -mSize/2f );
			GL.Normal3( -1.0f, 1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3(  mSize/2f,  -mSize/2f, -mSize/2f );

			// Render the left quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_LEFT]);
			GL.Normal3( 1.0f, -1.0f, -1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( -mSize/2f, mSize/2f, mSize/2f );
			GL.Normal3( 1.0f, 1.0f, -1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( -mSize/2f, -mSize/2f, mSize/2f );
			GL.Normal3( 1.0f, 1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3( -mSize/2f, -mSize/2f, -mSize/2f );
			GL.Normal3( 1.0f, -1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3( -mSize/2f, mSize/2f, -mSize/2f );

			// Render the back quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_BACK]);
			GL.Normal3( -1.0f, -1.0f,-1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( mSize/2f, mSize/2f,  mSize/2f );
			GL.Normal3( -1.0f, 1.0f,-1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( mSize/2f,  -mSize/2f,  mSize/2f );
			GL.Normal3( 1.0f, 1.0f,-1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3(  -mSize/2f,  -mSize/2f,  mSize/2f );
			GL.Normal3( 1.0f, -1.0f,-1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3(  -mSize/2f, mSize/2f,  mSize/2f );

			// Render the right quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_RIGHT]);
			GL.Normal3( -1.0f, -1.0f, 1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( mSize/2f, mSize/2f, -mSize/2f );
			GL.Normal3( -1.0f, 1.0f, 1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( mSize/2f,  -mSize/2f, -mSize/2f );
			GL.Normal3( -1.0f, 1.0f,-1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3( mSize/2f,  -mSize/2f,  mSize/2f );
			GL.Normal3( -1.0f, -1.0f,-1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3( mSize/2f, mSize/2f,  mSize/2f );

			// Render the top quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_UP]);
			GL.Normal3( -1.0f, -1.0f, -1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3( mSize/2f,  mSize/2f, mSize/2f );
			GL.Normal3( 1.0f, -1.0f, -1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( -mSize/2f,  mSize/2f,  mSize/2f );
			GL.Normal3( 1.0f, -1.0f, 1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3(  -mSize/2f,  mSize/2f,  -mSize/2f );
			GL.Normal3( -1.0f, -1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3(  mSize/2f,  mSize/2f, -mSize/2f );

			// Render the bottom quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_DOWN]);
			GL.Normal3( -1.0f, 1.0f, -1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3( mSize/2f, -mSize/2f, mSize/2f );
			GL.Normal3( -1.0f, 1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3(  mSize/2f, -mSize/2f, -mSize/2f );
			GL.Normal3( 1.0f, 1.0f, 1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(  -mSize/2f, -mSize/2f,  -mSize/2f );
			GL.Normal3( 1.0f, 1.0f, -1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( -mSize/2f, -mSize/2f,  mSize/2f );

			GL.End();

			/*
			//Draw 3D grid in the SkyBox
			GL.Begin(PrimitiveType.Lines);
			GL.Disable (EnableCap.Texture2D);
			for (int j = -100; j <= 100;) {
				for (int i = -100; i <= 100;) {

					GL.Color4 (Color.AntiqueWhite);

					GL.Vertex3 ((float)i, -100.0f, (float)j);
					GL.Vertex3 ((float)i, 100.0f, (float)j);

					GL.Vertex3 ((float)i, (float)j, -100.0f);
					GL.Vertex3 ((float)i, (float)j, 100.0f);

					GL.Vertex3 (-100.0f, (float)j, (float)i);
					GL.Vertex3 (100.0f, (float)j, (float)i);

					i = i + 10;
				}
				j = j + 10;
			}
			GL.End();
			*/

			GL.EndList();                // Finish display list 

			return displayList;       
		}
	}
}

