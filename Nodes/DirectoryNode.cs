using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;

namespace glomp
{
	public class DirectoryNode : Node
	{
		//Constants
		/*  Directory Nodes Textures  */
		private static readonly String[] dirNodeTextures = {
			"..\\..\\resources\\towers2-1.png",
			"..\\..\\resources\\towers2-2.png",
			"..\\..\\resources\\towers2-3.png",
			"..\\..\\resources\\towers2-4.png",
			"..\\..\\resources\\towers2-5.png",
			"..\\..\\resources\\towers2-6.png",
			"..\\..\\resources\\towers2-7.png",
			"..\\..\\resources\\towers2-8.png"
		};

		//Static variables
		//public static int[][] nodeTextures = new int[numberOfFileNodeTypes][]; //array of arrays
		public static uint[] nodeTextures;

		protected float dirHeight = 1.0f;
		protected int numChildren = 0;
		protected int numFiles = 0;
		protected int numDirs = 0;
		protected bool isDirFaded = false; //Directories fading disabled at the moment (directories were fading when active node was being changed to file)
		private float fadeAmount = 0.1f;

		public float DirHeight {
			get { return dirHeight; }
			set { dirHeight = value; }
		}

		public int NumChildren {
			get { return numChildren; }
			set { numChildren = value; }
		}

		public int NumFiles {
			get { return numFiles; }
			set { numFiles = value; }
		}

		public int NumDirs {
			get { return numDirs; }
			set { numDirs = value; }
		}

		public bool DirFaded {
			get { return isDirFaded; }
			set { isDirFaded = value; }
		}

		public float FadeAmount {
			get { return fadeAmount; }
			set { fadeAmount = value; }
		}


		public DirectoryNode (String _fileName)
			: base(_fileName) {
			type = Node.NodeType.DIR_NODE;
			IsDirectory = true;
		}


		public static void LoadNodeTextures() { //note, that this is a static method and static array nodeTextures -> meaning, that all DirectoryNodes will have the same textures (unlike FileNodes)
			//Creates a map of textures (a couple of textures per each file node type)
			nodeTextures = new uint[dirNodeTextures.Length];
			int i = 0;
			foreach (String texture in dirNodeTextures) {
                TextureTarget textureTarget;
                TextureLoaderParameters.MagnificationFilter = TextureMagFilter.Linear;
                TextureLoaderParameters.MinificationFilter = TextureMinFilter.LinearMipmapLinear;
                ImageGDI.LoadFromDisk(texture, out nodeTextures[i], out textureTarget);
                System.Diagnostics.Debug.WriteLine("Loaded " + texture + " with handle " + nodeTextures[i] + " as " + textureTarget);
                i++;
			}
		}


		public override void DrawBox(int offset, float frameDelta) {
			PreRenderBox(frameDelta);

			MoveIntoPosition(true, ref boxModelMatrix);

			Matrix4 translationMatrix = Matrix4.CreateTranslation (0f, dirHeight-1.0f, 0f);
			Matrix4 scaleMatrix = Matrix4.CreateScale (1f, dirHeight, 1f);
			boxModelMatrix = translationMatrix * boxModelMatrix;
			boxModelMatrix = scaleMatrix * boxModelMatrix;

			/*
			if (isNodeActivated) { //Node on which ENTER was pressed (color changing disabled at the moment)
				GL.Color4 (currentColour [0], currentColour [1], currentColour [2], fadeAmount);
				GL.Enable (EnableCap.ColorMaterial);
            } else if (isDirFaded) { //Directories fading disabled at the moment (directories were fading when active node was being changed to file)
				GL.Color4 (currentColour [0], currentColour [1], currentColour [2], fadeAmount);
				GL.Enable (EnableCap.ColorMaterial);
			}
			*/

			if(isActive) {
				NodeShader.Instance.TextureEnvModeCombine = TextureEnvModeCombine.Replace;  //Makes a nice enlighted ice-cube like node (no colour mixing with texture is allowed)
				//GL.Color4(activeColour[0], activeColour[1], activeColour[2], parentSlice.Alpha);
				//GL.Enable (EnableCap.ColorMaterial);
			}

			//On changing active slice, the file nodes are "growing" to their full size
			if(parentSlice.IsScaled) {
				float scaleValue = parentSlice.Scale - (offset / (float)parentSlice.NumFiles);
				if(scaleValue > 1.0f) { scaleValue = 1.0f; }
				else if(scaleValue <= 0f) { scaleValue = 0.01f; } //Cannot be 0.0f as then we will have singular matrix to inverse, which would cause exception
				Matrix4 growingScaleMatrix = Matrix4.CreateScale (scaleValue, scaleValue, scaleValue);
				boxModelMatrix = growingScaleMatrix * boxModelMatrix;
			}


			if(isSelected) {
				GL.PushAttrib(AttribMask.EnableBit|AttribMask.PolygonBit|AttribMask.CurrentBit);
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
				GL.Color4(Color.White);

				GL.UseProgram (NodeShader.Instance.ShaderProgramID);
				NodeShader.Instance.SetShaderUniforms (boxModelMatrix);
				VBOUtil.Draw(NodeManager.vao[(int)type], NodeManager.vbo[(int)type]);
				GL.PopAttrib();

				Matrix4 selectedScaleMatrix = Matrix4.CreateScale (0.8f, 0.8f, 0.8f);
				boxModelMatrix = selectedScaleMatrix * boxModelMatrix;
			}

			GL.UseProgram (NodeShader.Instance.ShaderProgramID);
			NodeShader.Instance.SetShaderUniforms (boxModelMatrix);
			VBOUtil.Draw(NodeManager.vao[(int)type], NodeManager.vbo[(int)type]);

			PostRenderBox();    
		}


		private void PreRenderBox(float frameDelta) {

			boxModelMatrix = parentSlice.FileSliceModelMatrix;

			GL.PushAttrib (AttribMask.EnableBit); //Remembering attributes

			GL.Enable (EnableCap.Blend);     // Turn Blending On
			GL.Disable (EnableCap.CullFace); // Due to this the cubes are transparent - all walls visible

			timeSinceLastFrameChange += frameDelta * 10;
			if (timeSinceLastFrameChange > 1.6) {
				timeSinceLastFrameChange = 0;
				Random rnd = new Random ();
				currentFrame = rnd.Next (0, 7); // creates a random number in the given range
			}
			// bind texture to texture unit 0
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture (TextureTarget.Texture2D, DirectoryNode.nodeTextures[currentFrame]); //APPLY TEXTURE TO DIRECTORIES!

			if (isDirFaded || isDimmed) {
				GL.BlendFunc (BlendingFactorSrc.DstColor, BlendingFactorDest.DstAlpha); //Very transparent
				//GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One); //Use this when no texture applied.
			} else {
				GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.One); //Better visibility than GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
			}
		}


		private void PostRenderBox() {
			if (isActive) { //Active directories are drawn with TextureEnvMode = Replace - so have to restore Modulate mode after they have beeen drawn
				//GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvModeCombine.Modulate); //Restore default mode (mixing texture with colour is allowed)
				NodeShader.Instance.TextureEnvModeCombine = TextureEnvModeCombine.Modulate; //Restore default mode (mixing texture with colour is allowed)
			}

			GL.PopAttrib(); // Due to changes in attributes in PreRenderBox
		}


		public override void DestroyTexture() {
			Trace.WriteLine ("DestroyTexture");
			if(hasTexture) {
				hasTexture = false;
				uint[] textures = {labelTextureIndex};
				GL.DeleteTextures(1, textures);
			}
		}

        public static void DestroyDirectoryTextures() {
            //Called only on Application closure, because these are textures common for all directories
            GL.DeleteTextures(DirectoryNode.nodeTextures.Length, DirectoryNode.nodeTextures);
        }

		public float GetHeightForFolder(int numChildren) {
			if(numChildren > 200) {
				return 4.0f;
			} else if(numChildren > 100) {
				return (3.0f + (1.0f * ((numChildren-100)/150.0f)));
			} else if(numChildren > 50) {
				return (2.5f + (0.5f * ((numChildren -50)/50.0f)));   
			} else if(numChildren > 25) {
				return (2.0f + (0.5f * ((numChildren - 25)/25.0f)));   
			} else if(numChildren > 10) {
				return (1.6f + (0.4f * ((numChildren - 10)/15.0f)));    
			} else if (numChildren > 5) {
				return (1.23f + (0.37f * ((numChildren - 5)/5.0f)));
			} else {
				return (1.0f + (0.23f * (numChildren/5.0f)));   
			}
		}
	}
}

