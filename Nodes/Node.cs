using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using System.Diagnostics;

namespace glomp {


    public abstract class Node : SceneNode {
        
        private readonly int textureWidth = 256;
        private readonly int textureHeight = 128;
        private int textSize = 20;

		protected int currentFrame = 0;
		protected float timeSinceLastFrameChange = 0;
        
        protected Bitmap textLabelBmp = null;
        
        private static readonly float[] fileColour = {0.2f, 0.6f, 0.6f};
        private static readonly float[] activeColour = {1.0f, 0.5f, 0.5f};
		private static readonly float[] dirColour = {0.4f, 1.0f, 0.8f}; //{0.3f, 0.5f, 1.0f};
        private static readonly float[] THUMB_COLOUR = {1.0f, 1.0f, 1.0f};

		public enum NodeType { FILE_NODE, DIR_NODE, DRIVE_NODE };

		protected NodeType type;
        private String fileName;
        private String file;
        protected int textureIndex;
        private bool isVisible;
        protected bool hasTexture;
        protected bool isActive;
        private bool isDirectory = false;
		private bool isDrive = false;
		protected bool isDimmed = false; //Nodes on non-active slice are dimmed (very transparent)
		private bool isNodeActivated = false;
        private String thumbFileName = "";
		private DateTime creationTime;
		private DateTime lastAccessTime;
        private DateTime modifyTime;
        
        protected bool isSelected = false;
		private FileSlice childSlice = null;
		protected FileSlice parentSlice = null;
		public bool culled = false;

		//Shaders related stuff
		protected Matrix4 boxModelMatrix = Matrix4.Identity; //For all Node objects (files, drives, directories)
		protected Matrix4 labelModelMatrix = Matrix4.Identity; //For all labels related to Nodes
        

		public NodeType Type {
			get { return type; }
			set { type = value; }
		}

		public FileSlice ChildSlice {
			get { return childSlice; }
			set { childSlice = value; }
		}
        
        public bool Selected {
            get { return isSelected; }
            set {isSelected = value; }
        }

		public bool NodeActivated {
			get { return isNodeActivated; }
			set { isNodeActivated = value; }
		}
          
        public bool Dimmed {
            get { return isDimmed; }
            set { isDimmed = value; }
        }
        
		public DateTime CreationTime {
			get { return creationTime; }
			set { creationTime = value; }
		}

		public DateTime LastAccessTime {
			get { return lastAccessTime; }
			set { lastAccessTime = value; }
		}

        public DateTime ModifyTime {
            get { return modifyTime; }
            set { modifyTime = value; }
        }
        
        public bool IsDirectory {
            get { return isDirectory; }
            set { isDirectory = value; }
        }

		public bool IsDrive {
			get { return isDrive; }
			set { isDrive = value; }
		}
        
        public String ThumbFileName {
            get { return thumbFileName; }
            set { thumbFileName = value; }
        }
        
        public String FileName {
            get { return fileName; }
            set { fileName = value; UpdateBitmap(false); }
        }
        
        public String File {
            get { return file; }
            set { file = value; }
        }
        
        private Vector3 textOffset = new Vector3(-3.1f, 0f, 0f);
        
        public bool Visible {
            get { return isVisible; }
            set { isVisible = value; }
        }
        
        public bool Active {
            get { return isActive; }
            set { isActive = value; }
        }

        public Node() 
            : base () {
            hasTexture = isVisible = isActive = false;
            fileName = "";
        }
        
        public Node(float _scale)
            : base() {
            scale = _scale;
            hasTexture = isVisible = isActive = false;
            fileName = "";
        }
        
        public Node(Vector3 _position)
            : base () {
            position = _position;
            hasTexture = isVisible = isActive = false;
            fileName = "";
        }
        
        public Node(Vector3 _position, float _scale) 
            : base () {
            scale = _scale;
            position = _position;
            hasTexture = isVisible = isActive = false;
            fileName = "";
        }
        
        public Node(String _fileName)
            : base() {
            hasTexture = isVisible = isActive = false;
            fileName = _fileName;
        }


        public virtual void GenTexture(bool force) {
			//To be overwritten in derived Node class
        }
        
        
        public virtual void DestroyTexture() {
			//To be overwritten in derived class
        }

        
        public override void Render() {
            // push, render box, pop
            /** NOT USED           
            GL.PushMatrix();
            MoveIntoPosition(false);
            RenderBox();
            
            if(isVisible) {
                RenderLabel();
            } 
            GL.PopMatrix();
            */
        }

        
		public virtual void DrawBox(int offset, float frameDelta) {
			//To be overwritten in derived Node class
		}

        public void DrawLabel() {
            if(isVisible) {
                PreRenderLabel();
         
				MoveIntoPosition(false, ref labelModelMatrix);

                if(isActive) {
					Matrix4 scaleMatrix = Matrix4.CreateScale (Vector3.One * 1.3f);
					labelModelMatrix = scaleMatrix * labelModelMatrix;
                    if(IsDirectory) {
						Matrix4 translationMatrix = Matrix4.CreateTranslation (new Vector3(0.14f, 0.35f, 0.0f));
						labelModelMatrix = translationMatrix * labelModelMatrix;
                    } else {
						Matrix4 translationMatrix = Matrix4.CreateTranslation (new Vector3(0.0f, 0.35f, 0.0f));
						labelModelMatrix = translationMatrix * labelModelMatrix;
                    }
					GL.Disable(EnableCap.DepthTest);
					LabelShader.Instance.LabelColor = new Color4( 1.0f, 0.4f, 0.4f, parentSlice.Alpha );

                } else {
					LabelShader.Instance.LabelColor = new Color4( 0.4f, 1f, 0.8f, parentSlice.Alpha );
                }

				GL.UseProgram (LabelShader.Instance.ShaderProgramID);
				LabelShader.Instance.SetShaderUniforms(labelModelMatrix);
				//Draw using VBO
				VBOUtil.Draw(NodeManager.labelVao, NodeManager.labelVbo);
                
                if(isActive) {
                    GL.Enable(EnableCap.DepthTest);
                }
                
                PostRenderLabel();
            }
        }
        
     
        private void PreRenderLabel() {
            
			labelModelMatrix = parentSlice.FileSliceModelMatrix;

			// bind texture to texture unit 0
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, textureIndex);
			// set texture uniform
			GL.Uniform1(LabelShader.Instance.Texture_location, 0);
        }
        

        private void PostRenderLabel() {
        }
        
        
        public static void SetTextState(bool dimmed) {
            GL.Enable(EnableCap.Blend);
            if(dimmed) {
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
            } else {
                GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            }
            GL.Disable(EnableCap.CullFace);
        }
        
        public static void UnsetTextState(bool dimmed) {
            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Blend);
        }
        
        public static void SetBoxState(bool dimmed) {
            if(dimmed) {
                GL.Enable(EnableCap.Blend);
				GL.BlendFunc (BlendingFactorSrc.DstColor, BlendingFactorDest.DstAlpha); //Very transparent
				//GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One); //Use this when no texture applied.
                GL.Disable(EnableCap.CullFace);
            }    
        }
        
        public static void UnsetBoxState(bool dimmed) {
            if(dimmed) {
                GL.Enable(EnableCap.CullFace);
                GL.Disable(EnableCap.Blend);   
            }
        }
          
        
        // updates the bitmap we use for our text
        protected void UpdateBitmap(bool activeTexture) {
			Trace.WriteLine ("UpdateBitmap");
            textLabelBmp = new Bitmap(textureWidth, textureHeight);
            //textSize = 20;
            using (Graphics gfx = Graphics.FromImage(textLabelBmp)) {
                String displayText;
				gfx.Clear(Color.Transparent);
				gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                RectangleF drawRect;
                if(isDirectory) {
                    drawRect = new RectangleF(0f, 0f, textLabelBmp.Width, textLabelBmp.Height);
                } else {
                    drawRect = new RectangleF(0f, 0f, textLabelBmp.Width, textLabelBmp.Height/2 + 7);
                }
                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = StringAlignment.Near;
                drawFormat.LineAlignment = StringAlignment.Center;
              
                // truncate string to fit... max 22 chars..
                if(fileName.Length > 27) {
                    displayText = fileName.Substring(0, 27);
                } else {
                    displayText = fileName;
                }
                // scale text to fit depending on length
                if(displayText.Length > 15) {
                    textSize = (int)Math.Round(20.0f / (displayText.Length / 15.0f), 0); 
                }
                
                Font TextFont = new Font(FontFamily.GenericSansSerif, textSize);
            
                gfx.DrawString(displayText, TextFont, Brushes.White, drawRect, drawFormat);    
                
                if(!isDirectory) {
                    // we are a file, draw another desriptive label
                    
                    TextFont = new Font(FontFamily.GenericSansSerif, 16);
                    drawRect = new RectangleF(5f, (textLabelBmp.Height/2) -7, textLabelBmp.Width, textLabelBmp.Height);
                    drawFormat.LineAlignment = StringAlignment.Near;
                    
					gfx.DrawString(((FileNode)this).Description, TextFont, Brushes.BlueViolet, drawRect, drawFormat);
                }
            }
        }
        
        public void SetParent(FileSlice newParent) {
            parentSlice = newParent;
        }
    }
}
