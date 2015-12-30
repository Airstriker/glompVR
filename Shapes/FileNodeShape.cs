using System;

namespace glomp
{
	public class FileNodeShape : Shape
	{
		public static readonly float BOX_SCALE = 0.8f;

		//Arrays filling constructor (FileNode shape definition loading)
		public FileNodeShape()
		{
			VertexData = new float[] //24 Vertices need to be defined, as every face has different texture - every face needs it's own Texture Coordinates; smooth shading is disabled
			{//	  X		 Y		Z			 U	   V			 N	   M	 L
				//bottom face
				-0.8f, -0.8f, -0.8f, 		1.0f, 0.0f, 		0.0f,-1.0f, 0.0f, //V0 // Top Right Of The Texture and Quad
				0.8f, -0.8f, -0.8f, 		0.0f, 0.0f, 		0.0f,-1.0f, 0.0f, //V1 // Top Left Of The Texture and Quad
				0.8f, -0.8f,  0.8f, 		0.0f, 1.0f, 		0.0f,-1.0f, 0.0f, //V2 // Bottom Left Of The Texture and Quad
				-0.8f, -0.8f,  0.8f, 		1.0f, 1.0f, 		0.0f,-1.0f, 0.0f, //V3 // Bottom Right Of The Texture and Quad

				//front face
				-0.8f, -0.8f,  0.8f, 		0.0f, 1.0f, 		0.0f, 0.0f, 1.0f, //V4 // Bottom Left Of The Texture and Quad
				0.8f, -0.8f,  0.8f, 		1.0f, 1.0f, 		0.0f, 0.0f, 1.0f, //V5 // Bottom Right Of The Texture and Quad
				0.8f,  0.8f,  0.8f, 		1.0f, 0.0f, 		0.0f, 0.0f, 1.0f, //V6 // Top Right Of The Texture and Quad
				-0.8f,  0.8f,  0.8f, 		0.0f, 0.0f, 		0.0f, 0.0f, 1.0f, //V7 // Top Left Of The Texture and Quad 

				//back face
				-0.8f, -0.8f, -0.8f, 		1.0f, 1.0f, 		0.0f, 0.0f,-1.0f, //V8 // Bottom Right Of The Texture and Quad
				-0.8f,  0.8f, -0.8f, 		1.0f, 0.0f, 		0.0f, 0.0f,-1.0f, //V9 // Top Right Of The Texture and Quad
				0.8f,  0.8f, -0.8f, 		0.0f, 0.0f, 		0.0f, 0.0f,-1.0f, //V10 // Top Left Of The Texture and Quad
				0.8f, -0.8f, -0.8f, 		0.0f, 1.0f, 		0.0f, 0.0f,-1.0f, //V11 // Bottom Left Of The Texture and Quad

				//top face
				-0.8f,  0.8f, -0.8f, 		0.0f, 0.0f, 		0.0f, 1.0f, 0.0f, //V12 // Top Left Of The Texture and Quad
				-0.8f,  0.8f,  0.8f, 		0.0f, 1.0f, 		0.0f, 1.0f, 0.0f, //V13 // Bottom Left Of The Texture and Quad
				0.8f,  0.8f,  0.8f, 		1.0f, 1.0f, 		0.0f, 1.0f, 0.0f, //V14 // Bottom Right Of The Texture and Quad
				0.8f,  0.8f, -0.8f, 		1.0f, 0.0f, 		0.0f, 1.0f, 0.0f, //V15 // Top Right Of The Texture and Quad

				//right face
				0.8f, -0.8f, -0.8f, 		1.0f, 1.0f, 		1.0f, 0.0f, 0.0f, //V16 // Bottom Right Of The Texture and Quad
				0.8f,  0.8f, -0.8f, 		1.0f, 0.0f, 		1.0f, 0.0f, 0.0f, //V17 // Top Right Of The Texture and Quad
				0.8f,  0.8f,  0.8f, 		0.0f, 0.0f, 		1.0f, 0.0f, 0.0f, //V18 // Top Left Of The Texture and Quad
				0.8f, -0.8f,  0.8f, 		0.0f, 1.0f, 		1.0f, 0.0f, 0.0f, //V19 // Bottom Left Of The Texture and Quad

				//left face
				-0.8f, -0.8f, -0.8f, 		0.0f, 1.0f,		   -1.0f, 0.0f, 0.0f, //V20 // Bottom Left Of The Texture and Quad
				-0.8f, -0.8f,  0.8f, 		1.0f, 1.0f, 	   -1.0f, 0.0f, 0.0f, //V21 // Bottom Right Of The Texture and Quad
				-0.8f,  0.8f,  0.8f, 		1.0f, 0.0f, 	   -1.0f, 0.0f, 0.0f, //V22 // Top Right Of The Texture and Quad
				-0.8f,  0.8f, -0.8f, 		0.0f, 0.0f, 	   -1.0f, 0.0f, 0.0f  //V23 // Top Left Of The Texture and Quad
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
		}
	}
}

