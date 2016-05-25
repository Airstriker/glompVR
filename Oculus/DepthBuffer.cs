using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OculusWrap;
using System.Diagnostics;
using System.Windows.Forms;

namespace glomp
{
    public class DepthBuffer : IDisposable
    {
        private uint texId;

        int width, height;

        public DepthBuffer(OVRTypes.Sizei size, int sampleCount)
        {
            Debug.Assert(sampleCount <= 1); // The code doesn't currently handle MSAA textures.

            width = size.Width;
            height = size.Height;

            GL.GenTextures(1, out texId);
            GL.BindTexture(TextureTarget.Texture2D, texId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);

            var extensions = GL.GetString(StringName.Extensions).Split(' ');

            PixelInternalFormat internalFormat = PixelInternalFormat.DepthComponent24;
            PixelType type = PixelType.UnsignedInt;

            if (extensions.Contains("GL_ARB_depth_buffer_float"))
            {
                internalFormat = PixelInternalFormat.DepthComponent32f;
                type = PixelType.Float;
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, size.Width, size.Height, 0, PixelFormat.DepthComponent, type, IntPtr.Zero);
        }

        public uint TexId
        {
            get { return texId; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }


        #region IDisposable Members
        /// <summary>
        /// Dispose contained fields.
        /// </summary>
        public void Dispose()
        {
            if (texId != 0)
            {
                GL.DeleteTextures(1, ref texId);
                texId = 0;
            }
        }
        #endregion

    }
}
