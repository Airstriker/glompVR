
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Microsoft.Win32;

namespace glomp
{
	public class FileNodeShape : Shape
	{
		private static readonly float BOX_SCALE = 0.8f;

		//Arrays filling constructor (FileNode shape definition loading)
		public FileNodeShape()
		{
			Vertices = new Vector3[] //24 Vertices need to be defined, as every face has different texture - every face needs it's own Texture Coordinates
			{
				//bottom face
				new Vector3(-0.8f, -0.8f, -0.8f), //V0 // Top Right Of The Texture and Quad
				new Vector3( 0.8f, -0.8f, -0.8f), //V1 // Top Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f,  0.8f), //V2 // Bottom Left Of The Texture and Quad
				new Vector3(-0.8f, -0.8f,  0.8f), //V3 // Bottom Right Of The Texture and Quad

				//front face
				new Vector3(-0.8f, -0.8f,  0.8f), //V4 // Bottom Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f,  0.8f), //V5 // Bottom Right Of The Texture and Quad
				new Vector3( 0.8f,  0.8f,  0.8f), //V6 // Top Right Of The Texture and Quad
				new Vector3(-0.8f,  0.8f,  0.8f), //V7 // Top Left Of The Texture and Quad 

				//back face
				new Vector3(-0.8f, -0.8f, -0.8f), //V8 // Bottom Right Of The Texture and Quad
				new Vector3(-0.8f,  0.8f, -0.8f), //V9 // Top Right Of The Texture and Quad
				new Vector3( 0.8f,  0.8f, -0.8f), //V10 // Top Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f, -0.8f), //V11 // Bottom Left Of The Texture and Quad

				//top face
				new Vector3(-0.8f,  0.8f, -0.8f), //V12 // Top Left Of The Texture and Quad
				new Vector3(-0.8f,  0.8f,  0.8f), //V13 // Bottom Left Of The Texture and Quad
				new Vector3( 0.8f,  0.8f,  0.8f), //V14 // Bottom Right Of The Texture and Quad
				new Vector3( 0.8f,  0.8f, -0.8f), //V15 // Top Right Of The Texture and Quad

				//right face
				new Vector3( 0.8f, -0.8f, -0.8f), //V16 // Bottom Right Of The Texture and Quad
				new Vector3( 0.8f,  0.8f, -0.8f), //V17 // Top Right Of The Texture and Quad
				new Vector3( 0.8f,  0.8f,  0.8f), //V18 // Top Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f,  0.8f), //V19 // Bottom Left Of The Texture and Quad

				//left face
				new Vector3(-0.8f, -0.8f, -0.8f), //V20 // Bottom Left Of The Texture and Quad
				new Vector3(-0.8f, -0.8f,  0.8f), //V21 // Bottom Right Of The Texture and Quad
				new Vector3(-0.8f,  0.8f,  0.8f), //V22 // Top Right Of The Texture and Quad
				new Vector3(-0.8f,  0.8f, -0.8f) //V23 // Top Left Of The Texture and Quad
			};

			Indices = new int[]
			{
				//bottom face
				0, 1, 2, 2, 3, 0,
				//front face
				4, 5, 6, 6, 7, 4,
				//back face
				8, 9, 10, 10, 11, 8,
				//top face
				12, 13, 14, 14, 15, 12,
				//right face
				16, 17, 18, 18, 19, 16,
				//left face
				20, 21, 22, 22, 23, 20,
			};

			Normals = new Vector3[] //Smooth shading is disabled
			{
				// bottom face
				new Vector3( 0.0f,-1.0f, 0.0f),
				new Vector3( 0.0f,-1.0f, 0.0f),
				new Vector3( 0.0f,-1.0f, 0.0f),
				new Vector3( 0.0f,-1.0f, 0.0f),

				//front face
				new Vector3( 0.0f, 0.0f, 1.0f),
				new Vector3( 0.0f, 0.0f, 1.0f),
				new Vector3( 0.0f, 0.0f, 1.0f),
				new Vector3( 0.0f, 0.0f, 1.0f),

				//back face
				new Vector3( 0.0f, 0.0f,-1.0f),
				new Vector3( 0.0f, 0.0f,-1.0f),
				new Vector3( 0.0f, 0.0f,-1.0f),
				new Vector3( 0.0f, 0.0f,-1.0f),

				//top face
				new Vector3( 0.0f, 1.0f, 0.0f),
				new Vector3( 0.0f, 1.0f, 0.0f),
				new Vector3( 0.0f, 1.0f, 0.0f),
				new Vector3( 0.0f, 1.0f, 0.0f),

				//right face
				new Vector3( 1.0f, 0.0f, 0.0f),
				new Vector3( 1.0f, 0.0f, 0.0f),
				new Vector3( 1.0f, 0.0f, 0.0f),
				new Vector3( 1.0f, 0.0f, 0.0f),

				//left face
				new Vector3(-1.0f, 0.0f, 0.0f),
				new Vector3(-1.0f, 0.0f, 0.0f),
				new Vector3(-1.0f, 0.0f, 0.0f),
				new Vector3(-1.0f, 0.0f, 0.0f),
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
				// bottom face
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),

				//front face
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),

				//back face
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),

				//top face
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),

				//right face
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),

				//left face
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f)
			};
		}
	}


	public class DirNodeShape : Shape
	{
		private static readonly float BOX_SCALE = 0.8f;
		private static readonly float dirHeight = 1.04f;

		//Arrays filling constructor (DirNode shape definition loading)
		public DirNodeShape()
		{
			Vertices = new Vector3[] //24 Vertices need to be defined, as every face has different texture - every face needs it's own Texture Coordinates
			{
				//bottom face
				new Vector3(-0.8f, -0.8f, -0.8f), //V0 // Top Right Of The Texture and Quad
				new Vector3( 0.8f, -0.8f, -0.8f), //V1 // Top Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f,  0.8f), //V2 // Bottom Left Of The Texture and Quad
				new Vector3(-0.8f, -0.8f,  0.8f), //V3 // Bottom Right Of The Texture and Quad

				//front face
				new Vector3(-0.8f, -0.8f,  0.8f), //V4 // Bottom Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f,  0.8f), //V5 // Bottom Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight,  0.8f), //V6 // Top Right Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight,  0.8f), //V7 // Top Left Of The Texture and Quad 

				//back face
				new Vector3(-0.8f, -0.8f, -0.8f), //V8 // Bottom Right Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight, -0.8f), //V9 // Top Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight, -0.8f), //V10 // Top Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f, -0.8f), //V11 // Bottom Left Of The Texture and Quad

				//top face
				new Vector3(-0.8f,  dirHeight, -0.8f), //V12 // Top Left Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight,  0.8f), //V13 // Bottom Left Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight,  0.8f), //V14 // Bottom Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight, -0.8f), //V15 // Top Right Of The Texture and Quad

				//right face
				new Vector3( 0.8f, -0.8f, -0.8f), //V16 // Bottom Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight, -0.8f), //V17 // Top Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight,  0.8f), //V18 // Top Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f,  0.8f), //V19 // Bottom Left Of The Texture and Quad

				//left face
				new Vector3(-0.8f, -0.8f, -0.8f), //V20 // Bottom Left Of The Texture and Quad
				new Vector3(-0.8f, -0.8f,  0.8f), //V21 // Bottom Right Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight,  0.8f), //V22 // Top Right Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight, -0.8f) //V23 // Top Left Of The Texture and Quad
			};

			Indices = new int[]
			{
				//bottom face
				0, 1, 2, 2, 3, 0,
				//front face
				4, 5, 6, 6, 7, 4,
				//back face
				8, 9, 10, 10, 11, 8,
				//top face
				12, 13, 14, 14, 15, 12,
				//right face
				16, 17, 18, 18, 19, 16,
				//left face
				20, 21, 22, 22, 23, 20,
			};
				
			//To enable smooth shading uncomment the normals that are commented now and vice versa
			//Smooth shading is enabled at the moment, because with textures it does look better than flat shading
			Normals = new Vector3[] //Smooth shading is enabled
			{
				// bottom face
				/*new Vector3( 0.0f,-1.0f, 0.0f),*/
				new Vector3( -1.0f,-1.0f, -1.0f),
				new Vector3( 1.0f,-1.0f, -1.0f),
				new Vector3( 1.0f,-1.0f, 1.0f),
				new Vector3( -1.0f,-1.0f, 1.0f),

				//front face
				/*new Vector3( 0.0f, 0.0f, 1.0f),*/
				new Vector3( -1.0f, -1.0f, 1.0f),
				new Vector3( 1.0f, -1.0f, 1.0f),
				new Vector3( 1.0f, 1.0f, 1.0f),
				new Vector3( -1.0f, 1.0f, 1.0f),

				//back face
				/*new Vector3( 0.0f, 0.0f,-1.0f),*/
				new Vector3( -1.0f, -1.0f,-1.0f),
				new Vector3( -1.0f, 1.0f,-1.0f),
				new Vector3( 1.0f, 1.0f,-1.0f),
				new Vector3( 1.0f, -1.0f,-1.0f),

				//top face
				/*new Vector3( 0.0f, 1.0f, 0.0f),*/
				new Vector3( -1.0f, 1.0f, -1.0f),
				new Vector3( -1.0f, 1.0f, 1.0f),
				new Vector3( 1.0f, 1.0f, 1.0f),
				new Vector3( 1.0f, 1.0f, -1.0f),

				//right face
				/*new Vector3( 1.0f, 0.0f, 0.0f),*/
				new Vector3( 1.0f, -1.0f, -1.0f),
				new Vector3( 1.0f, 1.0f, -1.0f),
				new Vector3( 1.0f, 1.0f, 1.0f),
				new Vector3( 1.0f, -1.0f, 1.0f),

				//left face
				/*new Vector3(-1.0f, 0.0f, 0.0f)*/
				new Vector3(-1.0f, -1.0f, -1.0f),
				new Vector3(-1.0f, -1.0f, 1.0f),
				new Vector3(-1.0f, 1.0f, 1.0f),
				new Vector3(-1.0f, 1.0f, -1.0f)
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
				// bottom face
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),

				//front face
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),

				//back face
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),

				//top face
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),

				//right face
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),

				//left face
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f)
			};
		}
	}


	public class DriveNodeShape : Shape
	{
		private static readonly float BOX_SCALE = 0.8f;
		private static readonly float dirHeight = 1.04f;

		//Arrays filling constructor (DirNode shape definition loading)
		public DriveNodeShape()
		{
			Vertices = new Vector3[] //24 Vertices need to be defined, as every face has different texture - every face needs it's own Texture Coordinates
			{
				//bottom face
				new Vector3(-0.8f, -0.8f, -0.8f), //V0 // Top Right Of The Texture and Quad
				new Vector3( 0.8f, -0.8f, -0.8f), //V1 // Top Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f,  0.8f), //V2 // Bottom Left Of The Texture and Quad
				new Vector3(-0.8f, -0.8f,  0.8f), //V3 // Bottom Right Of The Texture and Quad

				//front face
				new Vector3(-0.8f, -0.8f,  0.8f), //V4 // Bottom Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f,  0.8f), //V5 // Bottom Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight,  0.8f), //V6 // Top Right Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight,  0.8f), //V7 // Top Left Of The Texture and Quad 

				//back face
				new Vector3(-0.8f, -0.8f, -0.8f), //V8 // Bottom Right Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight, -0.8f), //V9 // Top Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight, -0.8f), //V10 // Top Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f, -0.8f), //V11 // Bottom Left Of The Texture and Quad

				//top face
				new Vector3(-0.8f,  dirHeight, -0.8f), //V12 // Top Left Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight,  0.8f), //V13 // Bottom Left Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight,  0.8f), //V14 // Bottom Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight, -0.8f), //V15 // Top Right Of The Texture and Quad

				//right face
				new Vector3( 0.8f, -0.8f, -0.8f), //V16 // Bottom Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight, -0.8f), //V17 // Top Right Of The Texture and Quad
				new Vector3( 0.8f,  dirHeight,  0.8f), //V18 // Top Left Of The Texture and Quad
				new Vector3( 0.8f, -0.8f,  0.8f), //V19 // Bottom Left Of The Texture and Quad

				//left face
				new Vector3(-0.8f, -0.8f, -0.8f), //V20 // Bottom Left Of The Texture and Quad
				new Vector3(-0.8f, -0.8f,  0.8f), //V21 // Bottom Right Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight,  0.8f), //V22 // Top Right Of The Texture and Quad
				new Vector3(-0.8f,  dirHeight, -0.8f) //V23 // Top Left Of The Texture and Quad
			};

			Indices = new int[]
			{
				//bottom face
				0, 1, 2, 2, 3, 0,
				//front face
				4, 5, 6, 6, 7, 4,
				//back face
				8, 9, 10, 10, 11, 8,
				//top face
				12, 13, 14, 14, 15, 12,
				//right face
				16, 17, 18, 18, 19, 16,
				//left face
				20, 21, 22, 22, 23, 20,
			};

			//To enable smooth shading uncomment the normals that are commented now and vice versa
			//Smooth shading is enabled at the moment, because with textures it does look better than flat shading
			Normals = new Vector3[] //Smooth shading is enabled
			{
				// bottom face
				/*new Vector3( 0.0f,-1.0f, 0.0f),*/
				new Vector3( -1.0f,-1.0f, -1.0f),
				new Vector3( 1.0f,-1.0f, -1.0f),
				new Vector3( 1.0f,-1.0f, 1.0f),
				new Vector3( -1.0f,-1.0f, 1.0f),

				//front face
				/*new Vector3( 0.0f, 0.0f, 1.0f),*/
				new Vector3( -1.0f, -1.0f, 1.0f),
				new Vector3( 1.0f, -1.0f, 1.0f),
				new Vector3( 1.0f, 1.0f, 1.0f),
				new Vector3( -1.0f, 1.0f, 1.0f),

				//back face
				/*new Vector3( 0.0f, 0.0f,-1.0f),*/
				new Vector3( -1.0f, -1.0f,-1.0f),
				new Vector3( -1.0f, 1.0f,-1.0f),
				new Vector3( 1.0f, 1.0f,-1.0f),
				new Vector3( 1.0f, -1.0f,-1.0f),

				//top face
				/*new Vector3( 0.0f, 1.0f, 0.0f),*/
				new Vector3( -1.0f, 1.0f, -1.0f),
				new Vector3( -1.0f, 1.0f, 1.0f),
				new Vector3( 1.0f, 1.0f, 1.0f),
				new Vector3( 1.0f, 1.0f, -1.0f),

				//right face
				/*new Vector3( 1.0f, 0.0f, 0.0f),*/
				new Vector3( 1.0f, -1.0f, -1.0f),
				new Vector3( 1.0f, 1.0f, -1.0f),
				new Vector3( 1.0f, 1.0f, 1.0f),
				new Vector3( 1.0f, -1.0f, 1.0f),

				//left face
				/*new Vector3(-1.0f, 0.0f, 0.0f)*/
				new Vector3(-1.0f, -1.0f, -1.0f),
				new Vector3(-1.0f, -1.0f, 1.0f),
				new Vector3(-1.0f, 1.0f, 1.0f),
				new Vector3(-1.0f, 1.0f, -1.0f)
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
				// bottom face
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),

				//front face
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),

				//back face
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),

				//top face
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),

				//right face
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),

				//left face
				new Vector2(0.0f, 1.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f)
			};
		}
	}


    public class NodeManager {
        
		public static int[] displayLists = new int[3];

		public static readonly int numberOfFileNodeTypes = 3;
		public static int[][] nodeTextures = new int[numberOfFileNodeTypes][]; //array of arrays

		public static readonly int FILE_NODE = 0;
		public static readonly int DIR_NODE = 1;
		public static readonly int DRIVE_NODE = 2;

		//VBO related stuff
		public static VBOUtil.Vbo[] vbo = new VBOUtil.Vbo[3];
		public static int[] vao = new int[3]; //vertex array objects referencing a different set of vertex attributes, which can be stored in the same vertex buffer object or split across several vertex buffer objects (like in this case).

        public NodeManager() {
        }

		public static void LoadNodesTextures(int fileNodeType, String[] textures) {
			//Creates a map of textures (a couple of textures per each file node type)
			nodeTextures [fileNodeType] = new int[textures.Length];
			int i = 0;
			foreach (String texture in textures) {
				nodeTextures[fileNodeType][i++] = TextureManager.LoadTexture(texture);
			}
		}

		[Obsolete ("Use VBO instead of DisplayLists.")]
		public static void GenerateDisplayLists() {
			displayLists[FILE_NODE] = GenerateDisplayList(FILE_NODE);
			displayLists[DIR_NODE] = GenerateDisplayList(DIR_NODE);
			displayLists[DRIVE_NODE] = GenerateDisplayList(DRIVE_NODE);
		}

		public static void LoadVBOs() {
			//Using VBO concept instead of DisplayList
			// loading Vertex Buffers
			Shape fileNodeShape = new FileNodeShape();
			Shape dirNodeShape = new DirNodeShape();
			Shape driveNodeShape = new DriveNodeShape();
			vbo[FILE_NODE] = VBOUtil.LoadVBO(fileNodeShape);
			VBOUtil.ConfigureVertexArrayObject(out vao [FILE_NODE], vbo [FILE_NODE]);
			vbo[DIR_NODE] = VBOUtil.LoadVBO(dirNodeShape);
			VBOUtil.ConfigureVertexArrayObject(out vao [DIR_NODE], vbo [DIR_NODE]);
			vbo[DRIVE_NODE] = VBOUtil.LoadVBO(driveNodeShape);
			VBOUtil.ConfigureVertexArrayObject(out vao [DRIVE_NODE], vbo [DRIVE_NODE]);
		}

		public static string GetMIMEDescription(string fileExtension) //fileExtension with dot "."
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

        
        private static int GenerateDisplayList(int nodeType) {
            System.Console.WriteLine("Genning new list");
            int displayList = GL.GenLists(1);
            if(nodeType == FILE_NODE) {
                
                GL.NewList(displayList, ListMode.Compile); // start compiling display list
                //GL.Color3(boxColour[0], boxColour[1], boxColour[2]);
                //GL.Scale(BOX_SCALE, BOX_SCALE, BOX_SCALE);

				GL.Begin(PrimitiveType.Quads);          // start drawing quads
                
				// Bottom Face
				GL.Normal3( 0.0f,-1.0f, 0.0f);      // Normal Facing Down
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Top Right Of The Texture and Quad
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Top Left Of The Texture and Quad
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                // Front Face
                GL.Normal3( 0.0f, 0.0f, 1.0f);      // Normal Facing Forward
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f,  0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f,  0.8f);  // Top Left Of The Texture and Quad
                // Back Face
                GL.Normal3( 0.0f, 0.0f,-1.0f);      // Normal Facing Away
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f, -0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f, -0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
                // Top Face
                GL.Normal3( 0.0f, 1.0f, 0.0f);      // Normal Facing Up
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f, -0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f,  0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f,  0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f, -0.8f);  // Top Right Of The Texture and Quad
                // Right face
                GL.Normal3( 1.0f, 0.0f, 0.0f);      // Normal Facing Right
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f, -0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  0.8f,  0.8f);  // Top Left Of The Texture and Quad
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
                // Left Face
                GL.Normal3(-1.0f, 0.0f, 0.0f);      // Normal Facing Left
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f,  0.8f);  // Top Right Of The Texture and Quad
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  0.8f, -0.8f);  // Top Left Of The Texture and Quad
                
                GL.End();                    // Done Drawing Quads
                
                GL.EndList();                // Finish display list
                
            } else if (nodeType == DIR_NODE) {
                
                float dirHeight = 1.04f;
                
                GL.NewList(displayList, ListMode.Compile); // start compiling display list
                //GL.Color3(boxColour[0], boxColour[1], boxColour[2]);
                //GL.Scale(BOX_SCALE, BOX_SCALE, BOX_SCALE);

				//GL.BindTexture(TextureTarget.Texture2D, nodeTextures[DIR_NODE]); //Putting textures in a display list will not make them run faster! ( see point 16.090 at http://dmi.uib.es/~josemaria/files/OpenGLFAQ/displaylist.htm )

				//To enable smooth shading uncomment the normals that are commented now and vice versa
				//Smooth shading is enabled at the moment, because with textures it does look better than flat shading

				GL.Begin(PrimitiveType.Quads);          // start drawing quads
                
				// Bottom Face
				//GL.Normal3( 0.0f,-1.0f, 0.0f);      // Normal Facing Down
				GL.Normal3( -1.0f,-1.0f, -1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3( 1.0f,-1.0f, -1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Top Left Of The Texture and Quad
				GL.Normal3( 1.0f,-1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				GL.Normal3( -1.0f,-1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
				// Front Face
				//GL.Normal3( 0.0f, 0.0f, 1.0f);      // Normal Facing Forward
				GL.Normal3( -1.0f, -1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3( 1.0f, -1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Top Left Of The Texture and Quad
				GL.Normal3( -1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Top Right Of The Texture and Quad
				// Back Face
				//GL.Normal3( 0.0f, 0.0f,-1.0f);      // Normal Facing Away
				GL.Normal3( -1.0f, -1.0f,-1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3( -1.0f, 1.0f,-1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f,-1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
				GL.Normal3( 1.0f, -1.0f,-1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
				// Top Face
				//GL.Normal3( 0.0f, 1.0f, 0.0f);      // Normal Facing Up
				GL.Normal3( -1.0f, 1.0f, -1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3( -1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Bottom Left Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, -1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
				// Right face
				//GL.Normal3( 1.0f, 0.0f, 0.0f);      // Normal Facing Right
				GL.Normal3( 1.0f, -1.0f, -1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, -1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Top Left Of The Texture and Quad
				GL.Normal3( 1.0f, -1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				// Left Face
				//GL.Normal3(-1.0f, 0.0f, 0.0f);      // Normal Facing Left
				GL.Normal3(-1.0f, -1.0f, -1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
				GL.Normal3(-1.0f, -1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3(-1.0f, 1.0f, 1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3(-1.0f, 1.0f, -1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
                
                GL.End();                    // Done Drawing Quads
                
                GL.EndList();                // Finish display list
			
			} else if (nodeType == DRIVE_NODE) {

				float dirHeight = 1.04f;

				GL.NewList(displayList, ListMode.Compile); // start compiling display list
				//GL.Color3(boxColour[0], boxColour[1], boxColour[2]);
				//GL.Scale(BOX_SCALE, BOX_SCALE, BOX_SCALE);

				//GL.BindTexture(TextureTarget.Texture2D, nodeTextures[DRIVE_NODE]); //Putting textures in a display list will not make them run faster! ( see point 16.090 at http://dmi.uib.es/~josemaria/files/OpenGLFAQ/displaylist.htm )

				GL.Begin(PrimitiveType.Quads);          // start drawing quads

				//To enable smooth shading uncomment the normals that are commented now and vice versa
				//Smooth shading is enabled at the moment, because with textures it does look better than flat shading

				// Bottom Face
				//GL.Normal3( 0.0f,-1.0f, 0.0f);      // Normal Facing Down
				GL.Normal3( -1.0f,-1.0f, -1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3( 1.0f,-1.0f, -1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Top Left Of The Texture and Quad
				GL.Normal3( 1.0f,-1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				GL.Normal3( -1.0f,-1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
				// Front Face
				//GL.Normal3( 0.0f, 0.0f, 1.0f);      // Normal Facing Forward
				GL.Normal3( -1.0f, -1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3( 1.0f, -1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Top Left Of The Texture and Quad
				GL.Normal3( -1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Top Right Of The Texture and Quad
				// Back Face
				//GL.Normal3( 0.0f, 0.0f,-1.0f);      // Normal Facing Away
				GL.Normal3( -1.0f, -1.0f,-1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3( -1.0f, 1.0f,-1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f,-1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
				GL.Normal3( 1.0f, -1.0f,-1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
				// Top Face
				//GL.Normal3( 0.0f, 1.0f, 0.0f);      // Normal Facing Up
				GL.Normal3( -1.0f, 1.0f, -1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3( -1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Bottom Left Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, -1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad
				// Right face
				//GL.Normal3( 1.0f, 0.0f, 0.0f);      // Normal Facing Right
				GL.Normal3( 1.0f, -1.0f, -1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f, -0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, -1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight, -0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3( 1.0f, 1.0f, 1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3( 0.8f,  dirHeight,  0.8f);  // Top Left Of The Texture and Quad
				GL.Normal3( 1.0f, -1.0f, 1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3( 0.8f, -0.8f,  0.8f);  // Bottom Left Of The Texture and Quad
				// Left Face
				//GL.Normal3(-1.0f, 0.0f, 0.0f);      // Normal Facing Left
				GL.Normal3(-1.0f, -1.0f, -1.0f);
				GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f, -0.8f);  // Bottom Left Of The Texture and Quad
				GL.Normal3(-1.0f, -1.0f, 1.0f);
				GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(-0.8f, -0.8f,  0.8f);  // Bottom Right Of The Texture and Quad
				GL.Normal3(-1.0f, 1.0f, 1.0f);
				GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight,  0.8f);  // Top Right Of The Texture and Quad
				GL.Normal3(-1.0f, 1.0f, -1.0f);
				GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-0.8f,  dirHeight, -0.8f);  // Top Left Of The Texture and Quad

				GL.End();                    // Done Drawing Quads

				GL.EndList();                // Finish display list 
			}
            
            return displayList;       
        }
        
        public static DateTime ConvertFromUnixTimestamp(UInt64 timestamp) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        
		public static float GetHeightForFolder(int numChildren) {
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
