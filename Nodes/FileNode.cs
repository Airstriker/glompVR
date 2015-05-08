using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Diagnostics;
using Microsoft.Win32;


namespace glomp
{
	public class FileNode : Node
	{
		//Constants
		private static readonly float[] exeColour = {0.1f, 0.7f, 0.3f};
		private static readonly float[] roColour = {0.1f, 0.1f, 0.1f};
		private static readonly float ACTIVE_SCALE = 1.3f;
		private static readonly String COLOUR_SALT = "";

		private String fileExtension;
		private String desc = "";
		private bool isReadOnly = false;
		private bool isExecutable = false;
		private bool isThumbnailed = false;
		private Bitmap thumbBmp;
		private int thumbTextureIndex;
		private int fileTextureIndex; //texture for ordinary files (just a one pixel coloured texture in repeat mode)

		public String FileExtension {
			get { return fileExtension; }
			set { fileExtension = value; }
		}

		public String Description {
			get { return desc; }
			set { 
				if(value != null) {
					desc = value; 
				} else {
					desc = "";
				}
			}
		}

		public bool IsReadOnly {
			get { return isReadOnly; }
			set { isReadOnly = value; }
		}

		public bool IsExecutable {
			get { return isExecutable; }
			set { isExecutable = value; }
		}

		public Bitmap ThumbBmp {
			get { return thumbBmp; }
			set { thumbBmp = value; }
		}


		public FileNode (String _fileName)
			: base(_fileName) {
			type = Node.NodeType.FILE_NODE;
		}


		public override void GenTexture(bool force) {
			Trace.WriteLine ("GenTexture");
			if(hasTexture) {
				if(!force) {
					return;
				}
			}
			System.Drawing.Imaging.BitmapData data;
			hasTexture = true;
			// initialise for new texture
			textureIndex = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, textureIndex);
			// set up texture filters
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

			UpdateBitmap(false);

			// upload data to openGL
			data = textLabelBmp.LockBits(new Rectangle(0, 0, textLabelBmp.Width, textLabelBmp.Height), 
				System.Drawing.Imaging.ImageLockMode.ReadOnly, 
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textLabelBmp.Width, textLabelBmp.Height, 0,
				PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0); 
			textLabelBmp.UnlockBits(data);
			textLabelBmp.Dispose();

			// right... now do the thumbnail if we have one
			if(thumbBmp != null) {

				// set up GL for the texture
				thumbTextureIndex = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, thumbTextureIndex);

				//get at the bitmap data for the thumbnail
				/*
				thumbBmp = new Bitmap(thumbFileName);
                
                Bitmap scaled = new Bitmap(128, 128);
                int x = 0;
                int y = 0;
                if(thumbBmp.Width > thumbBmp.Height) {
                	y = (thumbBmp.Width - thumbBmp.Height)/2;
                    x = 0;
                } else if(thumbBmp.Width < thumbBmp.Height) {
                    y = 0;
                    x = (thumbBmp.Height - thumbBmp.Width)/2;
                } else {
                    x = 0;
                    y = 0;
                } 
                    
                using (Graphics gfx = Graphics.FromImage(scaled)) {
                	gfx.Clear(Color.White);
                    gfx.DrawImageUnscaled(thumbBmp, x, y);
                }
                thumbBmp = scaled;
				*/

				thumbBmp = Util.ResizeBitmap (thumbBmp, 128, 128);

				// upload data to openGL
				data = thumbBmp.LockBits(new Rectangle(0, 0, thumbBmp.Width, thumbBmp.Height), 
					System.Drawing.Imaging.ImageLockMode.ReadOnly, 
					System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, thumbBmp.Width, thumbBmp.Height, 0,
					PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0); 

				thumbBmp.UnlockBits(data);

				// set texture parameters

				// Generate mipmaps
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

				// When MAGnifying the image (no bigger mipmap available), use LINEAR filtering
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				// When MINifying the image, use a LINEAR blend of two mipmaps, each filtered LINEARLY too
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);


				isThumbnailed = true;

			} else {
				// generate texture for ordinary files (just a one pixel coloured texture in repeat mode)
				fileTextureIndex = GL.GenTexture();

				// bind the texture
				GL.BindTexture(TextureTarget.Texture2D, fileTextureIndex);

				// create some image data
				int width = 1;
				int height = 1;
				byte[] image = new byte[4 * width * height];
				for(int j = 0;j<height;++j) {
					for(int i = 0;i<width;++i) {
						int index = j*width + i;

						float[] textureColour = null;
						if(isExecutable) {
							textureColour = exeColour;
						} else if(isReadOnly) {
							textureColour = roColour;
						} else {
							textureColour = GetColourForNode();
						}

						image[4 * index + 0] = Convert.ToByte (textureColour[0] * 255); // R
						image[4 * index + 1] = Convert.ToByte (textureColour[1] * 255); // G
						image[4 * index + 2] = Convert.ToByte (textureColour[2] * 255); // B
						image[4 * index + 3] = Convert.ToByte (parentSlice.Alpha * 255); // A
					}
				}

				// set texture content
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image);

				// set texture parameters
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			}
		}


		public override void DrawBox(int offset, float frameDelta) {
			PreRenderBox(frameDelta);

			MoveIntoPosition(true, ref boxModelMatrix);

			if(isThumbnailed && !isDimmed) {
				//GL.TexEnv (TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvModeCombine.Replace);
				NodeShader.Instance.TextureEnvModeCombine = TextureEnvModeCombine.Replace;  //No lightening applied to thumbnailed nodes - for better visual quality
			}

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
				Matrix4 scaleMatrix = Matrix4.CreateScale (ACTIVE_SCALE, ACTIVE_SCALE, ACTIVE_SCALE);
				Matrix4 translationMatrix = Matrix4.CreateTranslation (Vector3.UnitY * 0.5f);
				boxModelMatrix = scaleMatrix * boxModelMatrix;
				boxModelMatrix = translationMatrix * boxModelMatrix;
			}

			//On changing active slice, the file nodes are "growing" to their full size
			if(parentSlice.IsScaled) {
				float scaleValue = parentSlice.Scale - (offset / (float)parentSlice.NumFiles);
				if(scaleValue > 1.0f) { scaleValue = 1.0f; }
				else if(scaleValue <= 0f) { scaleValue = 0.01f; } //Cannot be 0.0f as then we will have singular matrix to inverse, which would cause exception
				Matrix4 scaleMatrix = Matrix4.CreateScale (scaleValue, scaleValue, scaleValue);
				boxModelMatrix = scaleMatrix * boxModelMatrix;
			}


			if(isSelected) {
				GL.PushAttrib(AttribMask.EnableBit|AttribMask.PolygonBit|AttribMask.CurrentBit);
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
				GL.Color4(Color.White);

				GL.UseProgram (NodeShader.Instance.ShaderProgramID);
				NodeShader.Instance.SetShaderUniforms(boxModelMatrix);
				VBOUtil.Draw(NodeManager.vao[(int)type], NodeManager.vbo[(int)type]);
				GL.PopAttrib();

				Matrix4 scaleMatrix = Matrix4.CreateScale (0.8f, 0.8f, 0.8f);
				boxModelMatrix = scaleMatrix * boxModelMatrix;
			}

			GL.UseProgram (NodeShader.Instance.ShaderProgramID);
			NodeShader.Instance.SetShaderUniforms (boxModelMatrix);
			VBOUtil.Draw(NodeManager.vao[(int)type], NodeManager.vbo[(int)type]);

			PostRenderBox();    
		}


		private void PreRenderBox(float frameDelta) {

			boxModelMatrix = parentSlice.FileSliceModelMatrix;

			GL.PushAttrib (AttribMask.EnableBit); //Remembering attributes

			if (isThumbnailed) {
				// bind texture to texture unit 0
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture (TextureTarget.Texture2D, thumbTextureIndex);

			} else { //ordinary files
				GL.Enable (EnableCap.Blend);     // Turn Blending On
				GL.Disable (EnableCap.CullFace); // Due to this the cubes are transparent - all walls visible

				// bind texture to texture unit 0
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture (TextureTarget.Texture2D, fileTextureIndex);
			}

			if (isDimmed) {
				GL.BlendFunc (BlendingFactorSrc.DstColor, BlendingFactorDest.DstAlpha); //Very transparent
				//GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One); //Use this when no texture applied.
			} else {
				GL.BlendFunc (BlendingFactorSrc.One, BlendingFactorDest.One); //Better visibility than GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
			}
		}


		private void PostRenderBox() {
			if (isThumbnailed && !isDimmed) { //Only Thumbnailed files and active directories are drawn with TextureEnvMode = Replace - so have to restore Modulate mode after they have beeen drawn
				//GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvModeCombine.Modulate); //Restore default mode (mixing texture with colour is allowed)
				NodeShader.Instance.TextureEnvModeCombine = TextureEnvModeCombine.Modulate; //Restore default mode (mixing texture with colour is allowed)
			}

			GL.PopAttrib(); // Due to changes in attributes in PreRenderBox
		}


		private float[] GetColourForNode() {
			if(desc.Length > 0) {
				// step 1, calculate MD5 hash from input
				System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
				byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(COLOUR_SALT + desc);
				byte[] hash = md5.ComputeHash(inputBytes);

				// step 2, extract last 3 bytes from string
				float[] returnable = new float[3];
				returnable[0] = (((int)hash[hash.Length-1])/700.0f) + 0.3f;
				returnable[1] = (((int)hash[hash.Length-2])/700.0f) + 0.4f;
				returnable[2] = (((int)hash[hash.Length-3])/700.0f) + 0.4f;
				return returnable;
			} else {
				float[] returnable = {0.2f, 0.6f, 0.6f};
				return returnable;
			}
		}


		public override void DestroyTexture() {
			Trace.WriteLine ("DestroyTexture");
			if(hasTexture) {
				hasTexture = false;
				int[] textures = {textureIndex,thumbTextureIndex};
				GL.DeleteTextures(2, textures);
			}
			//textLabelBmp.Dispose();
		}


		public string GetMIMEDescription(string fileExtension) //fileExtension with dot "."
		{
			string applicationType;
			string contentType;

			if (fileExtension == string.Empty)
				return "unknown file";

			//get the application class the extension is associated to
			using(RegistryKey rgk = Registry.ClassesRoot.OpenSubKey("\\" + fileExtension))
			{
				if (rgk != null) {
					applicationType = rgk.GetValue ("", string.Empty).ToString ();

					//get the file type description for the application associated to
					using(RegistryKey rgkey = Registry.ClassesRoot.OpenSubKey("\\" + applicationType))
					{
						if (rgkey != null) {
							contentType = rgkey.GetValue ("", string.Empty).ToString ();
							if (contentType != String.Empty)
								return contentType;
							else
								return fileExtension.Substring(1) + " file"; //dot removed
						}
					}
				}
			}

			return fileExtension.Substring(1) + " file"; //dot removed
		}
	}
}

