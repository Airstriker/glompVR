using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace glomp
{
	public class VBOUtil
	{
		public class Vbo
		{
			public int VertexBufferID { get; set; }
            public int ElementBufferID { get; set; }
            public int NumElements { get; set; }

            public void Dispose()
            {
                if (VertexBufferID != 0) GL.DeleteBuffer(VertexBufferID);
                if (ElementBufferID != 0) GL.DeleteBuffer(ElementBufferID);
            }
        }

		private static int previouslyUsedVao = -1;

		public VBOUtil ()
		{
		}


		/// <summary>
		/// Generate a VertexBuffer for each of Color, Normal, TextureCoordinate, Vertex, and Indices
		/// </summary>
		public static Vbo LoadVBO(Shape shape)
		{
			Vbo vbo = new Vbo();

			if (shape.VertexData == null)
				return vbo;
			if (shape.Indices == null)
				return vbo;
				
			{
                // Generate Array Buffer Id
                int vertexBufferID;
                GL.GenBuffers (1, out vertexBufferID);
                vbo.VertexBufferID = vertexBufferID;

                // Bind current context to Array Buffer ID
                GL.BindBuffer (BufferTarget.ArrayBuffer, vbo.VertexBufferID);

				// Send data to buffer
				GL.BufferData (BufferTarget.ArrayBuffer, (IntPtr)(shape.VertexData.Length * sizeof(float)), shape.VertexData, BufferUsageHint.StaticDraw);

				// Clear the buffer Binding
				GL.BindBuffer (BufferTarget.ArrayBuffer, 0);
			}

			// Element Array Buffer
			{
                // Generate Array Buffer Id
                int elementBufferID;
                GL.GenBuffers (1, out elementBufferID);
                vbo.ElementBufferID = elementBufferID;

                // Bind current context to Array Buffer ID
                GL.BindBuffer (BufferTarget.ElementArrayBuffer, vbo.ElementBufferID);

				// Send data to buffer
				GL.BufferData (BufferTarget.ElementArrayBuffer, (IntPtr)(shape.Indices.Length * sizeof(int)), shape.Indices, BufferUsageHint.StaticDraw);

				// Clear the buffer Binding
				GL.BindBuffer (BufferTarget.ElementArrayBuffer, 0);
			}

			// Store the number of elements for the DrawElements call
			vbo.NumElements = shape.Indices.Length;
			
			return vbo;
		}


		/*If the app draws many different kinds of models, reconfiguring the pipeline may become a bottleneck.
		 * Instead, use a vertex array object to store a complete attribute configuration.
		 * Each configuration is independent of the other; each vertex array object can reference a different set of vertex attributes,
		 * which can be stored in the same vertex buffer object or split across several vertex buffer objects.
		 * https://developer.apple.com/library/ios/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html#//apple_ref/doc/uid/TP40008793-CH107-SW1
		*/
		public static void ConfigureVertexArrayObject(out int vao, VBOUtil.Vbo vbo)
		{
			if (vbo.VertexBufferID == 0 || vbo.ElementBufferID == 0) {
				vao = 0;
				return;
			}

			// Create and bind the vertex array object.
			GL.GenVertexArrays(1, out vao);
			GL.BindVertexArray(vao);

			// Vertex Array Buffer
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.VertexBufferID);

				GL.EnableVertexAttribArray (0); //Position
				GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), IntPtr.Zero);

				GL.EnableVertexAttribArray (1); //Texture Coordinates
				GL.VertexAttribPointer (1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (IntPtr)(IntPtr.Zero + 3 * sizeof(float)));

				GL.EnableVertexAttribArray (2); //Normal
				GL.VertexAttribPointer (2, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (IntPtr)(IntPtr.Zero + 3 * sizeof(float) + 2 * sizeof(float)));


				//TO BE REMOVED WHEN SHADERS ARE INTRODUCED
				/*
				// Set the Pointer to the current bound array describing how the data ia stored
				GL.TexCoordPointer(2, TexCoordPointerType.Float, 8 * sizeof(float), (IntPtr)(IntPtr.Zero + 3 * sizeof(float)));

				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(ArrayCap.TextureCoordArray);

				// Set the Pointer to the current bound array describing how the data ia stored
				GL.NormalPointer(NormalPointerType.Float, 8 * sizeof(float), (IntPtr)(IntPtr.Zero + 3 * sizeof(float) + 2 * sizeof(float)));

				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(ArrayCap.NormalArray);
				*/
			}

			// Element Array Buffer
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo.ElementBufferID);
			}

			// Bind back to the default state.
			GL.BindBuffer(BufferTarget.ArrayBuffer,0);
			GL.BindVertexArray(0);
		}


		public static void ConfigureVertexArrayObjectForLabels(out int vao, VBOUtil.Vbo vbo)
		{
			if (vbo.VertexBufferID == 0 || vbo.ElementBufferID == 0) {
				vao = 0;
				return;
			}

			// Create and bind the vertex array object.
			GL.GenVertexArrays(1, out vao);
			GL.BindVertexArray(vao);

			// Vertex Array Buffer
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.VertexBufferID);

				GL.EnableVertexAttribArray (0); //Position
				GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), IntPtr.Zero);

				GL.EnableVertexAttribArray (1); //Texture Coordinates
				GL.VertexAttribPointer (1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (IntPtr)(IntPtr.Zero + 3 * sizeof(float)));


				//TO BE REMOVED WHEN SHADERS ARE INTRODUCED
				/*
				// Set the Pointer to the current bound array describing how the data ia stored
				GL.TexCoordPointer(2, TexCoordPointerType.Float, 5 * sizeof(float), (IntPtr)(IntPtr.Zero + 3 * sizeof(float)));

				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(ArrayCap.TextureCoordArray);
				*/
			}

			// Element Array Buffer
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo.ElementBufferID);
			}

			// Bind back to the default state.
			GL.BindBuffer(BufferTarget.ArrayBuffer,0);
			GL.BindVertexArray(0);
		}


		//Draw using VAO and VBO, where VAO stores a complete set of attributes for the given VBO
		public static void Draw(int vao, VBOUtil.Vbo vbo)
		{
			// Push current Array Buffer state so we can restore it later
			//GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

			if (vao != previouslyUsedVao) { //To eliminate unnecessary GL calls.
				previouslyUsedVao = vao;
				GL.BindVertexArray (vao);
			}

			// Draw the elements in the element array buffer
			// Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
			//GL.DrawElements(PrimitiveType.Triangles, vbo.NumElements, DrawElementsType.UnsignedInt, IntPtr.Zero);

			//GL.DrawRangeElements is slightly better performance-wise than GL.DrawElements
			GL.DrawRangeElements(PrimitiveType.Triangles, 0, vbo.NumElements - 1, vbo.NumElements, DrawElementsType.UnsignedInt, IntPtr.Zero);

			// Could also call GL.DrawArrays which would ignore the ElementArrayBuffer and just use primitives
			// Of course we would have to reorder our data to be in the correct primitive order

			// Restore the state
			//GL.PopClientAttrib();
		}
	}
}

