using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace glomp
{
	public class VBOUtil
	{
		public struct Vbo
		{
			public int VertexBufferID;
			public int ColorBufferID;
			public int TexCoordBufferID;
			public int NormalBufferID;
			public int ElementBufferID;
			public int NumElements;
		}

		private static int previouslyUsedVao = 0;

		public VBOUtil ()
		{
		}


		/// <summary>
		/// Generate a VertexBuffer for each of Color, Normal, TextureCoordinate, Vertex, and Indices
		/// </summary>
		public static Vbo LoadVBO(Shape shape)
		{
			Vbo vbo = new Vbo();

			if (shape.Vertices == null) return vbo;
			if (shape.Indices == null) return vbo;

			int bufferSize;

			// Color Array Buffer
			if (shape.Colors != null)
			{
				// Generate Array Buffer Id
				GL.GenBuffers(1, out vbo.ColorBufferID);

				// Bind current context to Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.ColorBufferID);

				// Send data to buffer
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(shape.Colors.Length * sizeof(int)), shape.Colors, BufferUsageHint.StaticDraw);

				// Validate that the buffer is the correct size
				GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
				if (shape.Colors.Length * sizeof(int) != bufferSize)
					throw new ApplicationException("Vertex array not uploaded correctly");

				// Clear the buffer Binding
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			}

			// Normal Array Buffer
			if (shape.Normals != null)
			{
				// Generate Array Buffer Id
				GL.GenBuffers(1, out vbo.NormalBufferID);

				// Bind current context to Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.NormalBufferID);

				// Send data to buffer
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(shape.Normals.Length * Vector3.SizeInBytes), shape.Normals, BufferUsageHint.StaticDraw);

				// Validate that the buffer is the correct size
				GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
				if (shape.Normals.Length * Vector3.SizeInBytes != bufferSize)
					throw new ApplicationException("Normal array not uploaded correctly");

				// Clear the buffer Binding
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			}

			// TexCoord Array Buffer
			if (shape.Texcoords != null)
			{
				// Generate Array Buffer Id
				GL.GenBuffers(1, out vbo.TexCoordBufferID);

				// Bind current context to Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.TexCoordBufferID);

				// Send data to buffer
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(shape.Texcoords.Length * Vector2.SizeInBytes), shape.Texcoords, BufferUsageHint.StaticDraw);

				// Validate that the buffer is the correct size
				GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
				if (shape.Texcoords.Length * Vector2.SizeInBytes != bufferSize)
					throw new ApplicationException("TexCoord array not uploaded correctly");

				// Clear the buffer Binding
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			}

			// Vertex Array Buffer
			{
				// Generate Array Buffer Id
				GL.GenBuffers(1, out vbo.VertexBufferID);

				// Bind current context to Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.VertexBufferID);

				// Send data to buffer
				GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(shape.Vertices.Length * Vector3.SizeInBytes), shape.Vertices, BufferUsageHint.StaticDraw);

				// Validate that the buffer is the correct size
				GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
				if (shape.Vertices.Length * Vector3.SizeInBytes != bufferSize)
					throw new ApplicationException("Vertex array not uploaded correctly");

				// Clear the buffer Binding
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			}

			// Element Array Buffer
			{
				// Generate Array Buffer Id
				GL.GenBuffers(1, out vbo.ElementBufferID);

				// Bind current context to Array Buffer ID
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo.ElementBufferID);

				// Send data to buffer
				GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(shape.Indices.Length * sizeof(int)), shape.Indices, BufferUsageHint.StaticDraw);

				// Validate that the buffer is the correct size
				GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
				if (shape.Indices.Length * sizeof(int) != bufferSize)
					throw new ApplicationException("Element array not uploaded correctly");

				// Clear the buffer Binding
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
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

			// Normal Array Buffer
			if (vbo.NormalBufferID != 0)
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.NormalBufferID);

				// Set the Pointer to the current bound array describing how the data ia stored
				GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(ArrayCap.NormalArray);
			}

			// Texture Array Buffer
			if (vbo.TexCoordBufferID != 0)
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.TexCoordBufferID);

				// Set the Pointer to the current bound array describing how the data ia stored
				GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);

				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(ArrayCap.TextureCoordArray);
			}
				
			// Vertex Array Buffer
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.VertexBufferID);

				// Set the Pointer to the current bound array describing how the data ia stored
				GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(ArrayCap.VertexArray);
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


		//Draw using VBO only
		[Obsolete ("Use Draw(int vao, VBOUtil.Vbo vbo) instead.")]
		public static void Draw(VBOUtil.Vbo vbo)
		{
			// Push current Array Buffer state so we can restore it later
			//GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

			if (vbo.VertexBufferID == 0) return;
			if (vbo.ElementBufferID == 0) return;

			if (GL.IsEnabled(EnableCap.Lighting))
			{
				// Normal Array Buffer
				if (vbo.NormalBufferID != 0)
				{
					// Bind to the Array Buffer ID
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.NormalBufferID);

					// Set the Pointer to the current bound array describing how the data ia stored
					GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

					// Enable the client state so it will use this array buffer pointer
					GL.EnableClientState(ArrayCap.NormalArray);
				}
			}
			else
			{
				// Color Array Buffer (Colors not used when lighting is enabled)
				if (vbo.ColorBufferID != 0)
				{
					// Bind to the Array Buffer ID
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.ColorBufferID);

					// Set the Pointer to the current bound array describing how the data ia stored
					GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(int), IntPtr.Zero);

					// Enable the client state so it will use this array buffer pointer
					GL.EnableClientState(ArrayCap.ColorArray);
				}
			}

			// Texture Array Buffer
			if (GL.IsEnabled(EnableCap.Texture2D))
			{
				if (vbo.TexCoordBufferID != 0)
				{
					// Bind to the Array Buffer ID
					GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.TexCoordBufferID);

					// Set the Pointer to the current bound array describing how the data ia stored
					GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);

					// Enable the client state so it will use this array buffer pointer
					GL.EnableClientState(ArrayCap.TextureCoordArray);
				}
			}

			// Vertex Array Buffer
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo.VertexBufferID);

				// Set the Pointer to the current bound array describing how the data ia stored
				GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);

				// Enable the client state so it will use this array buffer pointer
				GL.EnableClientState(ArrayCap.VertexArray);
			}

			// Element Array Buffer
			{
				// Bind to the Array Buffer ID
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo.ElementBufferID);

				// Draw the elements in the element array buffer
				// Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
				GL.DrawElements(PrimitiveType.Triangles, vbo.NumElements, DrawElementsType.UnsignedInt, IntPtr.Zero);

				// Could also call GL.DrawArrays which would ignore the ElementArrayBuffer and just use primitives
				// Of course we would have to reorder our data to be in the correct primitive order
			}

			// Restore the state
			//GL.PopClientAttrib();
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
			GL.DrawElements(PrimitiveType.Triangles, vbo.NumElements, DrawElementsType.UnsignedInt, IntPtr.Zero);

			// Could also call GL.DrawArrays which would ignore the ElementArrayBuffer and just use primitives
			// Of course we would have to reorder our data to be in the correct primitive order

			// Restore the state
			//GL.PopClientAttrib();
		}
	}
}

