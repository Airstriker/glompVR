using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace glomp
{
	public class TextureManager
	{
		public TextureManager ()
		{
		}

		public static int LoadTexture(string filename)
		{
			if (String.IsNullOrEmpty(filename))
				throw new ArgumentException(filename);

			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			Bitmap bmp = new Bitmap(filename);
			BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

			/* This uses GL_RGBA8​ for the internal format. GL_BGRA​ and GL_UNSIGNED_BYTE​ (or GL_UNSIGNED_INT_8_8_8_8​ is for the data in pixels array.
			 * The driver will likely not have to perform any CPU-based conversion and DMA this data directly to the video card.
			 * Benchmarking shows that on Windows and with nVidia and ATI/AMD, that this is the optimal format.
			 * This means you want the driver to actually store it in the R8G8B8 format.
			 * We should also state that most GPUs will up-convert GL_RGB8​ into GL_RGBA8​ internally.
			 * So it's probably best to steer clear of GL_RGB8​. We should also state that on some platforms, such as Windows, GL_BGRA​ for the pixel upload format is preferred.
			*/
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, bmp_data.Width, bmp_data.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgr/*.Bgra*/, PixelType.UnsignedByte, bmp_data.Scan0);

			bmp.UnlockBits(bmp_data);

			// We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
			// On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
			// mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.

			// Generate mipmaps
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

			// When MAGnifying the image (no bigger mipmap available), use LINEAR filtering
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			// When MINifying the image, use a LINEAR blend of two mipmaps, each filtered LINEARLY too
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);

			return id;
		}
	}
}

