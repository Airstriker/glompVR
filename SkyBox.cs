using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace glomp
{
	public class SkyBox
	{
		private float mSize;

		private int displayList; //DisplayList for SkyBox

		/* TEXTURES  */
		private static int[] skyBoxTexture = new int[6];

		/* TEXTURE INDEXes  */
		private static readonly int SKY_FRONT = 0;
		private static readonly int SKY_RIGHT = 1;
		private static readonly int SKY_LEFT = 2;
		private static readonly int SKY_BACK = 3;
		private static readonly int SKY_UP = 4;
		private static readonly int SKY_DOWN = 5;

		/* SKYBOX LIGHTENING */
		private float spotLightDirectionAngle = 0.0f;

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
			//GL.Disable (EnableCap.Lighting);
			GL.Disable (EnableCap.DepthTest);
			//GL.Disable (EnableCap.Blend);
			GL.Enable(EnableCap.Texture2D);
			//GL.DepthMask (false);

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
			GL.CallList(displayList); //Use pre-generated DisplayList
			///////////////////////////////////////////////////////////////////////////////////////////////////

			this.disableLights ();

			GL.PopMatrix ();

			/*
			GL.PopMatrix(); //Restoring Projection matrix

			GL.MatrixMode (MatrixMode.Modelview);
			GL.PopMatrix(); //Restoring Modelview matrix
			*/

			//GL.DepthMask(true);
			//GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Texture2D);
		}
	

		private int GenerateDisplayList() {
			displayList = GL.GenLists(1);

			GL.NewList(displayList, ListMode.Compile); // start compiling display list

			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//TO RENDER TEXTURES ON THE INNER SIDE OF THE CUBE, WE MUST DRAW THE VERTEXes COUNTER CLOCK-WISE !!!!!!
			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			float numberOfTextureRepeats = 3.0f;

			// Smooth shading enabled - every vertex has it's own normal. Looks a lot better than flat shading!
			// When a quad is being drawn, it takes a weighted average of the normals at the vertices to determine the normals at different points on the quad

			// Render the front quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_FRONT]);
			GL.Begin(BeginMode.Quads);
			GL.Normal3( -1.0f, -1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3(  mSize/2f, mSize/2f, -mSize/2f );
			GL.Normal3( 1.0f, -1.0f, 1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( -mSize/2f, mSize/2f, -mSize/2f );
			GL.Normal3( 1.0f, 1.0f, 1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( -mSize/2f,  -mSize/2f, -mSize/2f );
			GL.Normal3( -1.0f, 1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3(  mSize/2f,  -mSize/2f, -mSize/2f );
			GL.End();

			// Render the left quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_LEFT]);
			GL.Begin(BeginMode.Quads);
			GL.Normal3( 1.0f, -1.0f, -1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( -mSize/2f, mSize/2f, mSize/2f );
			GL.Normal3( 1.0f, 1.0f, -1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( -mSize/2f, -mSize/2f, mSize/2f );
			GL.Normal3( 1.0f, 1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3( -mSize/2f, -mSize/2f, -mSize/2f );
			GL.Normal3( 1.0f, -1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3( -mSize/2f, mSize/2f, -mSize/2f );
			GL.End();

			// Render the back quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_BACK]);
			GL.Begin(BeginMode.Quads);
			GL.Normal3( -1.0f, -1.0f,-1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( mSize/2f, mSize/2f,  mSize/2f );
			GL.Normal3( -1.0f, 1.0f,-1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( mSize/2f,  -mSize/2f,  mSize/2f );
			GL.Normal3( 1.0f, 1.0f,-1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3(  -mSize/2f,  -mSize/2f,  mSize/2f );
			GL.Normal3( 1.0f, -1.0f,-1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3(  -mSize/2f, mSize/2f,  mSize/2f );
			GL.End();

			// Render the right quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_RIGHT]);
			GL.Begin(BeginMode.Quads);
			GL.Normal3( -1.0f, -1.0f, 1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( mSize/2f, mSize/2f, -mSize/2f );
			GL.Normal3( -1.0f, 1.0f, 1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( mSize/2f,  -mSize/2f, -mSize/2f );
			GL.Normal3( -1.0f, 1.0f,-1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3( mSize/2f,  -mSize/2f,  mSize/2f );
			GL.Normal3( -1.0f, -1.0f,-1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3( mSize/2f, mSize/2f,  mSize/2f );
			GL.End();

			// Render the top quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_UP]);
			GL.Begin(BeginMode.Quads);
			GL.Normal3( -1.0f, -1.0f, -1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3( mSize/2f,  mSize/2f, mSize/2f );
			GL.Normal3( 1.0f, -1.0f, -1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( -mSize/2f,  mSize/2f,  mSize/2f );
			GL.Normal3( 1.0f, -1.0f, 1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3(  -mSize/2f,  mSize/2f,  -mSize/2f );
			GL.Normal3( -1.0f, -1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3(  mSize/2f,  mSize/2f, -mSize/2f );
			GL.End();

			// Render the bottom quad
			GL.BindTexture(TextureTarget.Texture2D, skyBoxTexture[SKY_DOWN]);
			GL.Begin(BeginMode.Quads);
			GL.Normal3( -1.0f, 1.0f, -1.0f);
			GL.TexCoord2(numberOfTextureRepeats, numberOfTextureRepeats); GL.Vertex3( mSize/2f, -mSize/2f, mSize/2f );
			GL.Normal3( -1.0f, 1.0f, 1.0f);
			GL.TexCoord2(numberOfTextureRepeats, 0.0f); GL.Vertex3(  mSize/2f, -mSize/2f, -mSize/2f );
			GL.Normal3( 1.0f, 1.0f, 1.0f);
			GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(  -mSize/2f, -mSize/2f,  -mSize/2f );
			GL.Normal3( 1.0f, 1.0f, -1.0f);
			GL.TexCoord2(0.0f, numberOfTextureRepeats); GL.Vertex3( -mSize/2f, -mSize/2f,  mSize/2f );
			GL.End();

			GL.EndList();                // Finish display list 

			return displayList;       
		}
	}
}

