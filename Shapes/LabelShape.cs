namespace glomp
{
	public class LabelShape : Shape //For 2D Text printing
	{
		//Arrays filling constructor (Label shape definition loading)
		public LabelShape()
		{
			VertexData = new float[]
			{//	  X		 Y		Z			 U	   V
				//only one quad
				-5.1f, -1.0f,  0.0f, 		1.0f, 1.0f, //V0 // Bottom Right Of The Texture and Quad
				-1.1f, -1.0f,  0.0f, 		0.0f, 1.0f, //V1 // Bottom Left Of The Texture and Quad
				-1.1f,  1.0f,  0.0f, 		0.0f, 0.0f, //V2 // Top Left Of The Texture and Quad
				-5.1f,  1.0f,  0.0f, 		1.0f, 0.0f  //V3 // Top Right Of The Texture and Quad
			};

			Indices = new int[]
			{
				// only one rectangle
				0, 1, 2, 2, 3, 0
			};
		}
	}
}

